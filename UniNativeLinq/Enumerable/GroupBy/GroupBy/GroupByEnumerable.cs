using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    [SlowCount]
    public unsafe struct
        GroupByEnumerable<TEnumerable, TEnumerator, T, TKey, TKeyFunc, TElement, TElementFunc, TEqualityComparer>
        : IRefEnumerable<GroupByEnumerable<TEnumerable, TEnumerator, T, TKey, TKeyFunc, TElement, TElementFunc, TEqualityComparer>.Enumerator, GroupingEnumerable<TKey, TElement>>
        where T : unmanaged
        where TKey : unmanaged
        where TElement : unmanaged
        where TEnumerator : struct, IRefEnumerator<T>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, T>
        where TKeyFunc : struct, IRefAction<T, TKey>
        where TElementFunc : struct, IRefAction<T, TElement>
        where TEqualityComparer : struct, IRefFunc<TKey, TKey, bool>
    {
        private TEnumerable enumerable;
        private TKeyFunc keySelector;
        private TElementFunc elementSelector;
        private TEqualityComparer equalityComparer;
        private Allocator alloc;
        public GroupByDisposeOptions GroupByDisposeOption;
        public bool CanIndexAccess() => false;
        public ref GroupingEnumerable<TKey, TElement> this[long index] => throw new NotSupportedException();

        public GroupByEnumerable(in TEnumerable enumerable, Allocator allocator, GroupByDisposeOptions groupByDisposeOption)
        {
            this.enumerable = enumerable;
            keySelector = default;
            elementSelector = default;
            equalityComparer = default;
            alloc = allocator;
            GroupByDisposeOption = groupByDisposeOption;
        }

        public GroupByEnumerable(in TEnumerable enumerable, in TKeyFunc keySelector, Allocator allocator, GroupByDisposeOptions groupByDisposeOption)
        {
            this.enumerable = enumerable;
            this.keySelector = keySelector;
            elementSelector = default;
            equalityComparer = default;
            alloc = allocator;
            GroupByDisposeOption = groupByDisposeOption;
        }

        public GroupByEnumerable(in TEnumerable enumerable, in TKeyFunc keySelector, in TElementFunc elementSelector, Allocator allocator, GroupByDisposeOptions groupByDisposeOption)
        {
            this.enumerable = enumerable;
            this.keySelector = keySelector;
            this.elementSelector = elementSelector;
            equalityComparer = default;
            alloc = allocator;
            GroupByDisposeOption = groupByDisposeOption;
        }

        public GroupByEnumerable(in TEnumerable enumerable, in TKeyFunc keySelector, in TElementFunc elementSelector, in TEqualityComparer equalityComparer, Allocator allocator, GroupByDisposeOptions groupByDisposeOption)
        {
            this.enumerable = enumerable;
            this.keySelector = keySelector;
            this.elementSelector = elementSelector;
            this.equalityComparer = equalityComparer;
            alloc = allocator;
            GroupByDisposeOption = groupByDisposeOption;
        }

        public struct Enumerator
            : IRefEnumerator<GroupingEnumerable<TKey, TElement>>
        {
            internal NativeEnumerable<GroupingEnumerable<TKey, TElement>>.Enumerator enumerator;
            private Allocator allocator;
            private GroupByDisposeOptions option;

            internal Enumerator(ref TEnumerable enumerable, ref TKeyFunc keySelector, ref TElementFunc elementSelector, ref TEqualityComparer equalityComparer, Allocator allocator, GroupByDisposeOptions option)
            {
                this.allocator = allocator;
                this.option = option;
                if (enumerable.CanFastCount() && enumerable.LongCount() == 0)
                {
                    this.enumerator = default;
                    return;
                }
                this.allocator = allocator;
                var list = new NativeList<(TKey Key, NativeList<TElement> Elements)>(Allocator.Temp);
                var enumerator = enumerable.GetEnumerator();
                TKey key = default;
                TElement element = default;
                do
                {
                    ref var current = ref enumerator.TryGetNext(out var success);
                    if (!success) break;

                    keySelector.Execute(ref current, ref key);
                    elementSelector.Execute(ref current, ref element);

                    bool found = false;
                    for (var i = 0L; i < list.LongCount(); i++)
                    {
                        ref var pair = ref list[i];
                        if (!equalityComparer.Calc(ref key, ref pair.Key)) continue;
                        pair.Elements.Add(element);
                        found = true;
                        break;
                    }
                    if (found) continue;
                    list.Add((key, new NativeList<TElement>(this.allocator)
                    {
                        element
                    }));
                } while (true);
                enumerator.Dispose();

                var answer = new NativeEnumerable<GroupingEnumerable<TKey, TElement>>
                {
                    Length = list.LongCount(),
                    Ptr = UnsafeUtilityEx.Malloc<GroupingEnumerable<TKey, TElement>>(list.LongCount(), this.allocator)
                };
                for (var i = 0L; i < answer.Length; i++)
                {
                    answer[i] = new GroupingEnumerable<TKey, TElement>(list[i].Key, list[i].Elements.AsNativeEnumerable(), allocator);
                }
                list.Dispose();
                this.enumerator = answer.GetEnumerator();
            }

            public ref GroupingEnumerable<TKey, TElement> Current => ref enumerator.Current;

            GroupingEnumerable<TKey, TElement> IEnumerator<GroupingEnumerable<TKey, TElement>>.Current => Current;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                if (enumerator.Ptr == null || !UnsafeUtility.IsValidAllocator(allocator)) return;
                switch (option)
                {
                    case GroupByDisposeOptions.GroupCollectionOnly:
                        UnsafeUtility.Free(enumerator.Ptr, allocator);
                        break;
                    case GroupByDisposeOptions.None:
                        return;
                    case GroupByDisposeOptions.Recursive:
                        for (var i = 0L; i < enumerator.Length; i++)
                        {
                            enumerator.Ptr[i].Dispose();
                        }
                        UnsafeUtility.Free(enumerator.Ptr, allocator);
                        break;
                }
            }

            public bool MoveNext() => enumerator.MoveNext();

            public void Reset() => enumerator.MoveNext();

            public ref GroupingEnumerable<TKey, TElement> TryGetNext(out bool success) => ref enumerator.TryGetNext(out success);

            public bool TryMoveNext(out GroupingEnumerable<TKey, TElement> value) => enumerator.TryMoveNext(out value);
        }

        public Enumerator GetEnumerator() => new Enumerator(ref enumerable, ref keySelector, ref elementSelector, ref equalityComparer, alloc, GroupByDisposeOption);

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<GroupingEnumerable<TKey, TElement>> IEnumerable<GroupingEnumerable<TKey, TElement>>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFastCount() => false;

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
            var enumerator = GetEnumerator();
            var count = 0L;
            while (enumerator.MoveNext())
                ++count;
            enumerator.Dispose();
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long CopyTo(GroupingEnumerable<TKey, TElement>* dest)
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
        public GroupingEnumerable<TKey, TElement>[] ToArray()
        {
            var count = LongCount();
            if (count == 0) return Array.Empty<GroupingEnumerable<TKey, TElement>>();
            var answer = new GroupingEnumerable<TKey, TElement>[count];
            CopyTo(Pseudo.AsPointer(ref answer[0]));
            return answer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeEnumerable<GroupingEnumerable<TKey, TElement>> ToNativeEnumerable(Allocator allocator)
        {
            var count = LongCount();
            var ptr = UnsafeUtilityEx.Malloc<GroupingEnumerable<TKey, TElement>>(count, allocator);
            CopyTo(ptr);
            return NativeEnumerable<GroupingEnumerable<TKey, TElement>>.Create(ptr, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeArray<GroupingEnumerable<TKey, TElement>> ToNativeArray(Allocator allocator)
        {
            var count = Count();
            if (count == 0) return default;
            var answer = new NativeArray<GroupingEnumerable<TKey, TElement>>(count, allocator, NativeArrayOptions.UninitializedMemory);
            CopyTo(UnsafeUtilityEx.GetPointer(answer));
            return answer;
        }
        #endregion
    }
}