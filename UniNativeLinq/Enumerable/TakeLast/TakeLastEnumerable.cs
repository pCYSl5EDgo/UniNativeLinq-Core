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
            private RingBuffer<T>.Enumerator enumerator;

            public ref T Current => ref enumerator.Current;
            T IEnumerator<T>.Current => Current;
            object IEnumerator.Current => Current;

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
        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        public bool CanFastCount() => enumerable.CanFastCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        public bool Any() => takeCount != 0 && enumerable.Any();

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        public int Count()
            => (int)LongCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        public long LongCount()
        {
            var count = enumerable.LongCount();
            return count > takeCount ? takeCount : count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        public void CopyTo(T* dest)
        {
            var enumerator = GetEnumerator();
            while (enumerator.MoveNext())
                *dest++ = enumerator.Current;
            enumerator.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        public T[] ToArray()
        {
            var count = LongCount();
            if (count == 0) return Array.Empty<T>();
            var answer = new T[count];
            CopyTo(Pseudo.AsPointer(ref answer[0]));
            return answer;
        }

        public bool CanIndexAccess => enumerable.CanIndexAccess;

        public ref T this[long index]
        {
            get
            {
                if (index >= takeCount) throw new ArgumentOutOfRangeException();
                return ref enumerable[enumerable.LongCount() - takeCount + index];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        public NativeEnumerable<T> ToNativeEnumerable(Allocator allocator)
        {
            var count = LongCount();
            var ptr = UnsafeUtilityEx.Malloc<T>(count, allocator);
            CopyTo(ptr);
            return NativeEnumerable<T>.Create(ptr, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
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
