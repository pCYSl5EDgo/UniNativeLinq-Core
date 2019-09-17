using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;

namespace UniNativeLinq
{
    public unsafe struct
        TakeLastEnumerable<TEnumerable, TEnumerator, T>
        : IRefEnumerable<TakeLastEnumerable<TEnumerable, TEnumerator, T>.Enumerator, T>
        where T : unmanaged
        where TEnumerator : struct, IRefEnumerator<T>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, T>
    {
        private TEnumerable enumerable;
        private readonly long takeCount;

        public TakeLastEnumerable(in TEnumerable enumerable, long takeCount)
        {
            this.enumerable = enumerable;
            this.takeCount = takeCount < 0 ? 0 : takeCount;
        }

        public struct Enumerator : IRefEnumerator<T>
        {
            private TEnumerator enumerator;
            internal Enumerator(ref TEnumerable enumerable, long count)
            {
                enumerator = enumerable.GetEnumerator();
                var skipCount = -count;
                {
                    var countEnumerator = enumerator;
                    while (countEnumerator.MoveNext())
                    {
                        ++skipCount;
                    }
                }
                for (var i = 0L; i < skipCount; i++)
                {
                    if (!enumerator.MoveNext())
                        break;
                }
            }
            public bool MoveNext() => enumerator.MoveNext();

            public void Reset() => enumerator.MoveNext();

            public ref T Current => ref enumerator.Current;
            T IEnumerator<T>.Current => Current;
            object IEnumerator.Current => Current;

            public ref T TryGetNext(out bool success) => ref enumerator.TryGetNext(out success);
            public bool TryMoveNext(out T value) => enumerator.TryMoveNext(out value);
            public void Dispose() => enumerator.Dispose();
        }

        public Enumerator GetEnumerator() => new Enumerator(ref enumerable, takeCount);

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFastCount() => enumerable.CanFastCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Any() => takeCount != 0 && enumerable.Any();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count()
            => (int)LongCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long LongCount()
        {
            var count = enumerable.LongCount();
            return count > takeCount ? takeCount : count;
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

        public bool CanIndexAccess() => enumerable.CanIndexAccess();

        public ref T this[long index]
        {
            get
            {
                if (index >= takeCount) throw new ArgumentOutOfRangeException();
                return ref enumerable[enumerable.LongCount() - takeCount + index];
            }
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
