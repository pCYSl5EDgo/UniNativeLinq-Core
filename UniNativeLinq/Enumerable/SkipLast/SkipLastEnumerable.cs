using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

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
        private Allocator alloc;

        public bool CanIndexAccess() => enumerable.CanIndexAccess();
        public ref T this[long index] => ref enumerable[index];

        public SkipLastEnumerable(in TEnumerable enumerable, long skipCount, Allocator allocator)
        {
            this.enumerable = enumerable;
            this.skipCount = skipCount;
            alloc = allocator;
        }

        public struct Enumerator : IRefEnumerator<T>
        {
            private TEnumerator enumerator;
            private T* buffer;
            private long index;
            private long skipCount;
            private Allocator allocator;

            public ref T Current => ref buffer[index];
            T IEnumerator<T>.Current => Current;
            object IEnumerator.Current => Current;

            public Enumerator(in TEnumerable enumerable, long skipCount, Allocator allocator)
            {
                enumerator = enumerable.GetEnumerator();
                this.skipCount = skipCount;
                this.allocator = allocator;
                buffer = skipCount <= 0 ? null : UnsafeUtilityEx.Malloc<T>(skipCount + 1, allocator);
                index = 0;
                for (var i = 0L; i < skipCount; i++)
                {
                    if (!enumerator.MoveNext())
                    {
                        enumerator.Dispose();
                        UnsafeUtility.Free(buffer, allocator);
                        return;
                    }
                    buffer[i + 1] = enumerator.Current;
                }
            }

            public bool MoveNext()
            {
                if (!enumerator.MoveNext())
                    return false;
                buffer[index++] = enumerator.Current;
                if (index > skipCount)
                    index = 0L;
                return true;
            }

            public void Reset() => throw new InvalidOperationException();

            public void Dispose()
            {
                enumerator.Dispose();
                if (buffer == null || !UnsafeUtility.IsValidAllocator(allocator)) return;
                UnsafeUtility.Free(buffer, allocator);
            }

            public ref T TryGetNext(out bool success)
            {
                ref var value = ref enumerator.TryGetNext(out success);
                if (!success) return ref value;
                buffer[index++] = value;
                if (index > skipCount)
                    index = 0L;
                return ref buffer[index];
            }

            public bool TryMoveNext(out T value)
            {
                if (!enumerator.TryMoveNext(out value))
                    return false;
                buffer[index++] = value;
                if (index > skipCount)
                    index = 0L;
                value = buffer[index];
                return true;
            }
        }

        public Enumerator GetEnumerator() => new Enumerator(enumerable, skipCount, alloc);

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
