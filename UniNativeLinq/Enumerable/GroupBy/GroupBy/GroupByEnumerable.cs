using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    [SlowCount]
    public readonly unsafe struct
        GroupByEnumerable<TEnumerable, TEnumerator, T, TKey, TKeySelector, TElement, TElementSelector, TEqualityComparer>
        : IRefEnumerable<GroupByEnumerable<TEnumerable, TEnumerator, T, TKey, TKeySelector, TElement, TElementSelector, TEqualityComparer>.Enumerator, Grouping<TKey, TElement>>
        where T : unmanaged
        where TKey : unmanaged
        where TElement : unmanaged
        where TEnumerator : struct, IRefEnumerator<T>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, T>
        where TKeySelector : struct, IRefFunc<T, TKey>
        where TElementSelector : struct, IRefFunc<T, TElement>
        where TEqualityComparer : struct, IRefFunc<TKey, TKey, bool>
    {
        private readonly TEnumerable enumerable;
        private readonly TKeySelector keySelector;
        private readonly TElementSelector elementSelector;
        private readonly TEqualityComparer equalityComparer;
        private readonly Allocator alloc;
        public readonly GroupByDisposeOptions GroupByDisposeOption;

        public GroupByEnumerable(in TEnumerable enumerable, in TKeySelector keySelector, in TElementSelector elementSelector, in TEqualityComparer equalityComparer, Allocator allocator, GroupByDisposeOptions groupByDisposeOption)
        {
            this.enumerable = enumerable;
            this.keySelector = keySelector;
            this.elementSelector = elementSelector;
            this.equalityComparer = equalityComparer;
            alloc = allocator;
            GroupByDisposeOption = groupByDisposeOption;
        }

        public struct Enumerator
            : IRefEnumerator<Grouping<TKey, TElement>>
        {
            internal Grouping<TKey, TElement>* Groups;
            internal long Count;
            private long index;
            private readonly Allocator allocator;
            private readonly GroupByDisposeOptions option;

            private const long INITIAL_CAPACITY = 16L;

            internal Enumerator(in TEnumerable enumerable, TKeySelector keySelector, TElementSelector elementSelector, TEqualityComparer equalityComparer, Allocator allocator, GroupByDisposeOptions option)
            {
                this.option = option;
                index = -1;
                Count = 0;
                this.allocator = allocator;
                // ReSharper disable once InconsistentNaming
                ref var _enumerable = ref Unsafe.AsRef(enumerable);
                if (_enumerable.CanFastCount() && _enumerable.LongCount() == 0)
                {
                    Groups = null;
                    return;
                }
                var capacity = INITIAL_CAPACITY;
                Groups = UnsafeUtilityEx.Malloc<Grouping<TKey, TElement>>(capacity, allocator);
                var capacities = UnsafeUtilityEx.Malloc<long>(capacity, Allocator.Temp);
                var enumerator = _enumerable.GetEnumerator();
                EnumerateAndSort(ref capacity, ref enumerator, ref capacities, ref keySelector, ref elementSelector, ref equalityComparer);
                UnsafeUtility.Free(capacities, Allocator.Temp);
                enumerator.Dispose();
            }

            private void EnumerateAndSort(ref long capacity, ref TEnumerator enumerator, ref long* capacities, ref TKeySelector keySelector, ref TElementSelector elementSelector, ref TEqualityComparer equalityComparer)
            {
                TKey key;
                while (enumerator.MoveNext())
                {
                    ref var current = ref enumerator.Current;
                    key = keySelector.Calc(ref current);
                    long insertIndex = LinearSearchInsertIndex(ref key, ref equalityComparer);
                    if (capacity == insertIndex)
                        ReAllocGroups(ref capacity, ref capacities, in key);
                    ref var group = ref Groups[insertIndex];
                    ref var capa = ref capacities[insertIndex];
                    if (capa == 0)
                        AllocGroup(key, ref group, ref capa);
                    InsertTo(ref group, ref capa, ref current, ref elementSelector);
                }
            }

            private void InsertTo(ref Grouping<TKey, TElement> group, ref long capa, ref T current, ref TElementSelector elementSelector)
            {
                if (capa == group.Length)
                {
                    UnsafeUtilityEx.ReAlloc(ref group.Elements, capa, capa << 1, allocator);
                    capa <<= 1;
                }
                group.Elements[group.Length++] = elementSelector.Calc(ref current);
            }

            private void AllocGroup(in TKey key, ref Grouping<TKey, TElement> group, ref long capacity)
            {
                group.Key = key;
                group.Allocator = allocator;
                capacity = INITIAL_CAPACITY;
                group.Elements = UnsafeUtilityEx.Malloc<TElement>(capacity, allocator);
                group.Length = 0;
            }

            private long LinearSearchInsertIndex(ref TKey key, ref TEqualityComparer equalityComparer)
            {
                for (var i = 0L; i < Count; i++)
                    if (equalityComparer.Calc(ref key, ref Groups[i].Key))
                        return i;
                return Count++;
            }

            private void ReAllocGroups(ref long capacity, ref long* capacities, in TKey key)
            {
                UnsafeUtilityEx.ReAlloc(ref capacities, capacity, capacity << 1, Allocator.Temp);
                UnsafeUtilityEx.ReAlloc(ref Groups, capacity, capacity << 1, allocator);
                UnsafeUtility.MemClear(capacities + capacity, sizeof(long) * capacity);
                capacity <<= 1;
            }

            public ref Grouping<TKey, TElement> Current => ref Groups[index];
            Grouping<TKey, TElement> IEnumerator<Grouping<TKey, TElement>>.Current => Current;
            object IEnumerator.Current => Current;

            public void Dispose()
            {
                switch (option)
                {
                    case GroupByDisposeOptions.Recursive:
                        if (Groups != null)
                            for (var i = 0L; i < Count; i++)
                                Groups[i].Dispose();
                        goto case GroupByDisposeOptions.GroupCollectionOnly;
                    case GroupByDisposeOptions.GroupCollectionOnly:
                        if (Groups != null && UnsafeUtility.IsValidAllocator(allocator))
                            UnsafeUtility.Free(Groups, allocator);
                        break;
                    case GroupByDisposeOptions.None:
                    default:
                        break;
                }
                this = default;
            }

            public bool MoveNext() => ++index < Count;

            public void Reset() => index = -1;

            public ref Grouping<TKey, TElement> TryGetNext(out bool success)
            {
                success = ++index < Count;
                if (success)
                    return ref Groups[index];
                index = Count;
                return ref Unsafe.AsRef<Grouping<TKey, TElement>>(null);
            }

            public bool TryMoveNext(out Grouping<TKey, TElement> value)
            {
                if(++index < Count)
                {
                    value = Groups[index];
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

        public readonly Enumerator GetEnumerator() => new Enumerator(in enumerable, keySelector, elementSelector, equalityComparer, alloc, GroupByDisposeOption);
        public readonly Enumerator GetEnumerator(Allocator allocator, GroupByDisposeOptions option) => new Enumerator(in enumerable, keySelector, elementSelector, equalityComparer, allocator, option);

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator<Grouping<TKey, TElement>> IEnumerable<Grouping<TKey, TElement>>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool CanFastCount() => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Any()
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
        public readonly int Count()
            => (int)LongCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly long LongCount()
        {
            var enumerator = GetEnumerator();
            var count = 0L;
            while (enumerator.MoveNext())
                ++count;
            enumerator.Dispose();
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void CopyTo(Grouping<TKey, TElement>* dest)
        {
            var enumerator = GetEnumerator();
            while (enumerator.MoveNext())
                *dest++ = enumerator.Current;
            enumerator.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Grouping<TKey, TElement>[] ToArray()
        {
            var count = LongCount();
            if (count == 0) return Array.Empty<Grouping<TKey, TElement>>();
            var answer = new Grouping<TKey, TElement>[count];
            CopyTo((Grouping<TKey, TElement>*)Unsafe.AsPointer(ref answer[0]));
            return answer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly NativeEnumerable<Grouping<TKey, TElement>> ToNativeEnumerable(Allocator allocator)
        {
            var count = LongCount();
            var ptr = UnsafeUtilityEx.Malloc<Grouping<TKey, TElement>>(count, allocator);
            CopyTo(ptr);
            return new NativeEnumerable<Grouping<TKey, TElement>>(ptr, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly NativeArray<Grouping<TKey, TElement>> ToNativeArray(Allocator allocator)
        {
            var count = Count();
            if (count == 0) return default;
            var answer = new NativeArray<Grouping<TKey, TElement>>(count, allocator, NativeArrayOptions.UninitializedMemory);
            CopyTo(UnsafeUtilityEx.GetPointer(answer));
            return answer;
        }
        #endregion
    }
}