using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    [PseudoIsReadOnly]
    public unsafe struct
        OrderByEnumerable<TEnumerable, TEnumerator, T, TComparer>
        : IRefOrderedEnumerable<
            OrderByEnumerable<TEnumerable, TEnumerator, T, TComparer>.Enumerator,
            T,
            TEnumerable,
            TEnumerator,
            TComparer>
        where T : unmanaged
        where TEnumerator : struct, IRefEnumerator<T>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, T>
        where TComparer : struct, IRefFunc<T, T, int>
    {
        [PseudoIsReadOnly] private TEnumerable enumerable;
        private readonly TComparer orderComparer;
        private readonly Allocator alloc;

        public OrderByEnumerable(in TEnumerable enumerable, in TComparer comparer, Allocator allocator)
        {
            this.enumerable = enumerable;
            orderComparer = comparer;
            alloc = allocator;
        }

        public struct Enumerator : IRefEnumerator<T>
        {
            internal T* Ptr;
            private long capacity;
            internal long Count;
            private long index;
            private readonly Allocator allocator;

            internal Enumerator([PseudoIsReadOnly]ref TEnumerable enumerable, TComparer orderComparer, Allocator allocator)
            {
                this.allocator = allocator;
                index = -1;
                Count = 0;
                capacity = 16;
                if (enumerable.CanFastCount() && (capacity = enumerable.Count()) == 0)
                {
                    this = default;
                    return;
                }
                Ptr = UnsafeUtilityEx.Malloc<T>(capacity, allocator);
                var enumerator = enumerable.GetEnumerator();
                if (!enumerator.MoveNext())
                {
                    enumerator.Dispose();
                    UnsafeUtility.Free(Ptr, allocator);
                    this = default;
                    return;
                }
                Ptr[0] = enumerator.Current;
                Count = 1;
                Sort(ref enumerator, ref orderComparer);
                enumerator.Dispose();
            }

            private void Sort(ref TEnumerator enumerator, ref TComparer comparer)
            {
                while (enumerator.MoveNext())
                {
                    var minInclusive = 0L;
                    var maxInclusive = Count - 1;
                    ref var current = ref enumerator.Current;
                    while (minInclusive < maxInclusive)
                    {
                        var insertIndex = (minInclusive + maxInclusive) >> 1;
                        var comp = comparer.Calc(ref current, ref Ptr[insertIndex]);
                        if (comp == 0)
                        {
                            minInclusive = insertIndex + 1;
                            break;
                        }
                        if (comp > 0)
                            minInclusive = insertIndex + 1;
                        else
                            maxInclusive = insertIndex - 1;
                    }
                    Insert(ref current, minInclusive);
                }
            }

            private void Insert(ref T current, long insertIndex)
            {
                if (Count == capacity)
                {
                    ReAllocAndInsert(ref current, insertIndex);
                    return;
                }
                if (insertIndex == 0)
                    UnsafeUtilityEx.MemMove(Ptr + 1, Ptr, Count);
                else if (insertIndex != Count)
                    UnsafeUtilityEx.MemMove(Ptr + insertIndex + 1, Ptr + insertIndex, Count - insertIndex);
                Ptr[insertIndex] = current;
                ++Count;
            }

            private void ReAllocAndInsert(ref T current, long insertIndex)
            {
                capacity += capacity >> 1;
                var tmp = UnsafeUtilityEx.Malloc<T>(capacity, allocator);
                if (insertIndex == 0)
                {
                    UnsafeUtilityEx.MemCpy(tmp + 1, Ptr, Count);
                }
                else if (insertIndex == Count)
                {
                    UnsafeUtilityEx.MemCpy(tmp, Ptr, Count);
                }
                else
                {
                    UnsafeUtilityEx.MemCpy(tmp, Ptr, insertIndex);
                    UnsafeUtilityEx.MemCpy(tmp + insertIndex + 1, Ptr + insertIndex, Count - insertIndex);
                }
                tmp[insertIndex] = current;
                UnsafeUtility.Free(Ptr, allocator);
                Ptr = tmp;
                ++Count;
            }

            public bool MoveNext()
            {
                if (++index < Count)
                    return true;
                index = Count;
                return false;
            }

            public void Reset() => index = -1;

            public ref T Current => ref Ptr[index];

            T IEnumerator<T>.Current => Current;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                if (Ptr != null)
                    UnsafeUtility.Free(Ptr, allocator);
                this = default;
            }

            public ref T TryGetNext(out bool success)
            {
                success = ++index < Count;
                if (success)
                    return ref Ptr[index];
                index = Count;
                return ref Pseudo.AsRefNull<T>();
            }

            public bool TryMoveNext(out T value)
            {
                if (++index < Count)
                {
                    value = Ptr[index];
                    return true;
                }
                else
                {
                    value = default;
                    index = Count;
                    return false;
                }
            }
        }

        public
            OrderByEnumerable<TEnumerable, TEnumerator, T, CompoundOrderBy<T, TComparer, OrderByKeySelector<T, TKey0, TKeySelector0, TComparer0>>>
            CreateRefOrderedEnumerable<TKey0, TKeySelector0, TComparer0>(TKeySelector0 keySelector, TComparer0 comparer, bool descending)
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefAction<T, TKey0>
            where TComparer0 : struct, IRefFunc<TKey0, TKey0, int>
            => new OrderByEnumerable<
                    TEnumerable,
                    TEnumerator,
                    T,
                    CompoundOrderBy<
                        T,
                        TComparer,
                        OrderByKeySelector<
                            T,
                            TKey0,
                            TKeySelector0,
                            TComparer0
                        >
                    >
                >(enumerable,
                new CompoundOrderBy<T, TComparer, OrderByKeySelector<T, TKey0, TKeySelector0, TComparer0>>(
                    orderComparer,
                    new OrderByKeySelector<T, TKey0, TKeySelector0, TComparer0>(
                        keySelector, comparer, descending)
                    ),
                    alloc
                );

        public IOrderedEnumerable<T> CreateOrderedEnumerable<TKey0>(Func<T, TKey0> keySelector, IComparer<TKey0> comparer, bool @descending)
            => new OrderByEnumerable<
                    TEnumerable,
                    TEnumerator,
                    T,
                    CompoundOrderBy<
                        T,
                        TComparer,
                        OrderByDelegateKeySelector<T, TKey0>
                    >
                >(
                enumerable,
                new CompoundOrderBy<T, TComparer, OrderByDelegateKeySelector<T, TKey0>>(
                    orderComparer,
                new OrderByDelegateKeySelector<T, TKey0>(keySelector, comparer, descending)),
                alloc);

        [PseudoIsReadOnly] public Enumerator GetEnumerator() => new Enumerator(ref enumerable, orderComparer, alloc);

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        public bool CanFastCount() => enumerable.CanFastCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        public bool Any() => enumerable.Any();

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        public int Count() => enumerable.Count();

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        public long LongCount() => enumerable.LongCount();

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
            CopyTo(Pseudo.AsPointer<T>(ref answer[0]));
            return answer;
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
            CopyTo(UnsafeUtilityEx.GetPointer(answer));
            return answer;
        }
        #endregion
    }
}