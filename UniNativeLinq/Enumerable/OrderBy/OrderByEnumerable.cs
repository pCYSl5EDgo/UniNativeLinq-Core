using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    public unsafe readonly partial struct
        OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>
        : IRefOrderedEnumerable<
            OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>.Enumerator,
            TSource,
            TEnumerable,
            TEnumerator,
            TComparer>
        where TSource : unmanaged
        where TEnumerator : struct, IRefEnumerator<TSource>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
        where TComparer : struct, IRefFunc<TSource, TSource, int>
    {
        private readonly TEnumerable enumerable;
        private readonly TComparer orderComparer;
        private readonly Allocator alloc;

        public OrderByEnumerable(in TEnumerable enumerable, TComparer comparer, Allocator allocator)
        {
            this.enumerable = enumerable;
            orderComparer = comparer;
            alloc = allocator;
        }

        public struct Enumerator : IRefEnumerator<TSource>
        {
            internal TSource* Ptr;
            private long capacity;
            internal long Count;
            private long index;
            private readonly Allocator allocator;

            internal Enumerator(in TEnumerable enumerable, TComparer orderComparer, Allocator allocator)
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
                Ptr = UnsafeUtilityEx.Malloc<TSource>(capacity, allocator);
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

            private void Insert(ref TSource current, long insertIndex)
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

            private void ReAllocAndInsert(ref TSource current, long insertIndex)
            {
                capacity += capacity >> 1;
                var tmp = UnsafeUtilityEx.Malloc<TSource>(capacity, allocator);
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

            public readonly ref TSource Current => ref Ptr[index];

            readonly TSource IEnumerator<TSource>.Current => Current;

            readonly object IEnumerator.Current => Current;

            public void Dispose()
            {
                if (Ptr != null)
                    UnsafeUtility.Free(Ptr, allocator);
                this = default;
            }

            public ref TSource TryGetNext(out bool success)
            {
                success = ++index < Count;
                if (success)
                    return ref Ptr[index];
                index = Count;
                return ref Unsafe.AsRef<TSource>(null);
            }

            public bool TryMoveNext(out TSource value)
            {
                if(++index < Count)
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

        public readonly
            OrderByEnumerable<TEnumerable, TEnumerator, TSource, CompoundOrderBy<TSource, TComparer, OrderByKeySelector<TSource, TKey0, TKeySelector0, TComparer0>>>
            CreateRefOrderedEnumerable<TKey0, TKeySelector0, TComparer0>(TKeySelector0 keySelector, TComparer0 comparer, bool descending)
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefAction<TSource, TKey0>
            where TComparer0 : struct, IRefFunc<TKey0, TKey0, int>
            => new OrderByEnumerable<
                    TEnumerable,
                    TEnumerator,
                    TSource,
                    CompoundOrderBy<
                        TSource,
                        TComparer,
                        OrderByKeySelector<
                            TSource,
                            TKey0,
                            TKeySelector0,
                            TComparer0
                        >
                    >
                >(enumerable,
                new CompoundOrderBy<TSource, TComparer, OrderByKeySelector<TSource, TKey0, TKeySelector0, TComparer0>>(
                    orderComparer,
                    new OrderByKeySelector<TSource, TKey0, TKeySelector0, TComparer0>(
                        keySelector, comparer, descending)
                    ),
                    alloc
                );

        public readonly IOrderedEnumerable<TSource> CreateOrderedEnumerable<TKey0>(Func<TSource, TKey0> keySelector, IComparer<TKey0> comparer, bool @descending)
            => new OrderByEnumerable<
                    TEnumerable,
                    TEnumerator,
                    TSource,
                    CompoundOrderBy<
                        TSource,
                        TComparer,
                        OrderByDelegateKeySelector<TSource, TKey0>
                    >
                >(
                enumerable,
                new CompoundOrderBy<TSource, TComparer, OrderByDelegateKeySelector<TSource, TKey0>>(
                    orderComparer,
                new OrderByDelegateKeySelector<TSource, TKey0>(keySelector, comparer, descending)),
                alloc);

        public readonly Enumerator GetEnumerator() => new Enumerator(enumerable, orderComparer, alloc);

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool CanFastCount() => enumerable.CanFastCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Any() => enumerable.Any();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int Count() => enumerable.Count();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly long LongCount() => enumerable.LongCount();

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