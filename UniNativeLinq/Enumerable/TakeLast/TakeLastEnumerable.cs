using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;

namespace UniNativeLinq
{
    public unsafe readonly partial struct
        TakeLastEnumerable<TEnumerable, TEnumerator, TSource>
        : IRefEnumerable<TakeLastEnumerable<TEnumerable, TEnumerator, TSource>.Enumerator, TSource>
        where TSource : unmanaged
        where TEnumerator : struct, IRefEnumerator<TSource>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
    {
        private readonly TEnumerable enumerable;
        private readonly long takeCount;
        private readonly Allocator alloc;

        public TakeLastEnumerable(in TEnumerable enumerable, long takeCount, Allocator allocator)
        {
            this.enumerable = enumerable;
            this.takeCount = takeCount;
            alloc = allocator;
        }

        public struct Enumerator : IRefEnumerator<TSource>
        {
            private readonly RingBuffer<TSource>.Enumerator enumerator;

            public readonly ref TSource Current => ref enumerator.Current;
            readonly TSource IEnumerator<TSource>.Current => Current;
            readonly object IEnumerator.Current => Current;

            public Enumerator(in TEnumerable enumerable, long takeCount, Allocator allocator)
            {
                var ringBuffer = new RingBuffer<TSource>(takeCount, allocator);
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

            public ref TSource TryGetNext(out bool success) => ref enumerator.TryGetNext(out success);

            public bool TryMoveNext(out TSource value) => enumerator.TryMoveNext(out value);
        }

        public readonly Enumerator GetEnumerator() => new Enumerator(enumerable, takeCount, alloc);

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();

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
        public readonly void CopyTo(TSource* dest)
        {
            var enumerator = GetEnumerator();
            while (enumerator.MoveNext())
                *dest++ = enumerator.Current;
            enumerator.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly TSource[] ToArray()
        {
            var count = LongCount();
            if (count == 0) return Array.Empty<TSource>();
            var answer = new TSource[count];
            CopyTo((TSource*)Unsafe.AsPointer(ref answer[0]));
            return answer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly NativeEnumerable<TSource> ToNativeEnumerable(Allocator allocator)
        {
            var count = LongCount();
            var ptr = UnsafeUtilityEx.Malloc<TSource>(count, allocator);
            CopyTo(ptr);
            return new NativeEnumerable<TSource>(ptr, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly NativeArray<TSource> ToNativeArray(Allocator allocator)
        {
            var count = Count();
            if (count == 0) return default;
            var answer = new NativeArray<TSource>(count, allocator, NativeArrayOptions.UninitializedMemory);
            CopyTo(UnsafeUtilityEx.GetPointer(answer));
            return answer;
        }
        #endregion
    }
}
