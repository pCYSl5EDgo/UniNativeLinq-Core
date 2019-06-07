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
        private readonly Allocator alloc;

        public TakeLastEnumerable(in TEnumerable enumerable, long takeCount, Allocator allocator)
        {
            this.enumerable = enumerable;
            this.takeCount = takeCount;
            alloc = allocator;
        }

        public struct Enumerator : IRefEnumerator<T>
        {
            private readonly RingBuffer<T>.Enumerator enumerator;

            public readonly ref T Current => ref enumerator.Current;
            readonly T IEnumerator<T>.Current => Current;
            readonly object IEnumerator.Current => Current;

            public Enumerator([PseudoIsReadOnly]ref TEnumerable enumerable, long takeCount, Allocator allocator)
            {
                var ringBuffer = new RingBuffer<T>(takeCount, allocator);
                var baseEnumerator = enumerable.GetEnumerator();
                while (baseEnumerator.MoveNext())
                {
                    if (ringBuffer.IsFull)
                        ringBuffer.RemoveFirst();
                    ringBuffer.Add(baseEnumerator.Current);
                }
                baseEnumerator.Dispose();
                enumerator = ringBuffer.GetEnumerator();
            }

            public bool MoveNext() => enumerator.MoveNext();

            public void Reset() => throw new InvalidOperationException();

            public void Dispose()
            {
                enumerator.Parent.Dispose();
                this = default;
            }

            public ref T TryGetNext(out bool success) => ref enumerator.TryGetNext(out success);

            public bool TryMoveNext(out T value) => enumerator.TryMoveNext(out value);
        }

        [PseudoIsReadOnly] public Enumerator GetEnumerator() => new Enumerator(ref enumerable, takeCount, alloc);

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool CanFastCount() => enumerable.CanFastCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Any() => takeCount != 0 && enumerable.Any();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int Count()
            => (int)LongCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly long LongCount()
        {
            var count = enumerable.LongCount();
            return count > takeCount ? takeCount : count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void CopyTo(T* dest)
        {
            var enumerator = GetEnumerator();
            while (enumerator.MoveNext())
                *dest++ = enumerator.Current;
            enumerator.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T[] ToArray()
        {
            var count = LongCount();
            if (count == 0) return Array.Empty<T>();
            var answer = new T[count];
            CopyTo(Pseudo.AsPointer<T>(ref answer[0]));
            return answer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly NativeEnumerable<T> ToNativeEnumerable(Allocator allocator)
        {
            var count = LongCount();
            var ptr = UnsafeUtilityEx.Malloc<T>(count, allocator);
            CopyTo(ptr);
            return new NativeEnumerable<T>(ptr, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly NativeArray<T> ToNativeArray(Allocator allocator)
        {
            var count = Count();
            if (count == 0) return default;
            var answer = new NativeArray<T>(count, allocator, NativeArrayOptions.UninitializedMemory);
            CopyTo(UnsafeUtilityEx.GetPointer(answer));
            return answer;
        }
        #endregion
    }
}
