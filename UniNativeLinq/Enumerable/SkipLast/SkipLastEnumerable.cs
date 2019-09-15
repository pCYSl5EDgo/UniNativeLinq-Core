using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;

namespace UniNativeLinq
{
    public unsafe struct
        SkipLastEnumerable<TEnumerable, TEnumerator, T>
        : IRefEnumerable<SkipLastEnumerable<TEnumerable, TEnumerator, T>.Enumerator, T>
        where T : unmanaged
        where TEnumerator : struct, IRefEnumerator<T>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, T>
    {
        private TEnumerable enumerable;
        private long skipCount;

        public bool CanIndexAccess() => enumerable.CanIndexAccess();
        public ref T this[long index] => ref enumerable[index];

        public SkipLastEnumerable(in TEnumerable enumerable, long skipCount)
        {
            this.enumerable = enumerable;
            this.skipCount = skipCount < 0 ? 0 : skipCount;
        }

        public struct Enumerator : IRefEnumerator<T>
        {
            private TEnumerator enumerator;
            private long index;
            private long length;

            public ref T Current => ref enumerator.Current;
            T IEnumerator<T>.Current => Current;
            object IEnumerator.Current => Current;

            public Enumerator(ref TEnumerable enumerable, long skipCount)
            {
                length = enumerable.LongCount() - skipCount;
                if (length < 0)
                    length = 0;
                index = -1;
                enumerator = length > 0 ? enumerable.GetEnumerator() : default;
            }

            public bool MoveNext()
            {
                if (++index >= length)
                {
                    return false;
                }
                return enumerator.MoveNext();
            }

            public void Reset() => throw new InvalidOperationException();

            public void Dispose() => enumerator.Dispose();

            public ref T TryGetNext(out bool success)
            {
                if (++index >= length)
                {
                    success = false;
                    return ref Pseudo.AsRefNull<T>();
                }
                return ref enumerator.TryGetNext(out success);
            }

            public bool TryMoveNext(out T value)
            {
                if (++index >= length)
                {
                    value = default;
                    return false;
                }
                return enumerator.TryMoveNext(out value);
            }
        }

        public Enumerator GetEnumerator() => new Enumerator(ref enumerable, skipCount);

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFastCount() => enumerable.CanFastCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Any()
        {
            var enumerator = GetEnumerator();
            if (enumerator.MoveNext())
            {
                enumerator.Dispose();
                return true;
            }
            enumerator.Dispose();
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count()
            => (int)LongCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long LongCount()
        {
            var count = enumerable.LongCount() - skipCount;
            return count < 0 ? 0 : count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long CopyTo(T* dest)
        {
            var enumerator = GetEnumerator();
            long answer = 0;
            while (enumerator.MoveNext())
            {
                *dest++ = enumerator.Current;
                answer++;
            }
            enumerator.Dispose();
            return answer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray()
        {
            var count = LongCount();
            if (count == 0) return Array.Empty<T>();
            var answer = new T[count];
            CopyTo(Pseudo.AsPointer(ref answer[0]));
            return answer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeEnumerable<T> ToNativeEnumerable(Allocator allocator)
        {
            var count = LongCount();
            var ptr = UnsafeUtilityEx.Malloc<T>(count, allocator);
            CopyTo(ptr);
            return NativeEnumerable<T>.Create(ptr, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeArray<T> ToNativeArray(Allocator allocator)
        {
            var count = Count();
            if (count == 0) return default;
            var answer = new NativeArray<T>(count, allocator, NativeArrayOptions.UninitializedMemory);
            CopyTo(answer.GetPointer());
            return answer;
        }
        #endregion
    }
}
