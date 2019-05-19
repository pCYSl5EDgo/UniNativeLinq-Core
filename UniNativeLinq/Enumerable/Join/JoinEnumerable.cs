using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    public unsafe readonly struct
        JoinEnumerable<TOuterEnumerable, TOuterEnumerator, TOuterSource, TInnerEnumerable, TInnerEnumerator, TInnerSource, TKey, TOuterKeySelector, TInnerKeySelector, TSource, TSourceSelector, TKeyEqualityComparer>
        : IRefEnumerable<JoinEnumerable<TOuterEnumerable, TOuterEnumerator, TOuterSource, TInnerEnumerable, TInnerEnumerator, TInnerSource, TKey, TOuterKeySelector, TInnerKeySelector, TSource, TSourceSelector, TKeyEqualityComparer>.Enumerator, TSource>
        where TOuterSource : unmanaged
        where TInnerSource : unmanaged
        where TOuterEnumerator : struct, IRefEnumerator<TOuterSource>
        where TInnerEnumerator : struct, IRefEnumerator<TInnerSource>
        where TOuterEnumerable : struct, IRefEnumerable<TOuterEnumerator, TOuterSource>
        where TInnerEnumerable : struct, IRefEnumerable<TInnerEnumerator, TInnerSource>
        where TSource : unmanaged
        where TKey : unmanaged
        where TOuterKeySelector : struct, IRefFunc<TOuterSource, TKey>
        where TInnerKeySelector : struct, IRefFunc<TInnerSource, TKey>
        where TKeyEqualityComparer : struct, IRefFunc<TKey, TKey, bool>
        where TSourceSelector : struct, IRefFunc<TOuterSource, TInnerSource, TSource>
    {
        private readonly TOuterEnumerable outerEnumerable;
        private readonly TInnerEnumerable innerEnumerable;
        private readonly TOuterKeySelector outerKeySelector;
        private readonly TInnerKeySelector innerKeySelector;
        private readonly TKeyEqualityComparer equalityComparer;
        private readonly TSourceSelector sourceSelector;
        private readonly Allocator alloc;

        public JoinEnumerable(in TOuterEnumerable outerEnumerable, in TInnerEnumerable innerEnumerable, in TOuterKeySelector outerKeySelector, in TInnerKeySelector innerKeySelector, in TSourceSelector sourceSelector, in TKeyEqualityComparer equalityComparer, Allocator allocator)
        {
            this.outerEnumerable = outerEnumerable;
            this.innerEnumerable = innerEnumerable;
            this.outerKeySelector = outerKeySelector;
            this.innerKeySelector = innerKeySelector;
            this.equalityComparer = equalityComparer;
            this.sourceSelector = sourceSelector;
            alloc = allocator;
        }

        public struct Enumerator : IRefEnumerator<TSource>
        {
            private TOuterEnumerator outerEnumerator;
            private TOuterKeySelector outerKeySelector;
            private TKey outerKey;
            private long innerCount;
            private long innerIndex;
            private TInnerSource* innerValue;
            private TKey* innerKey;
            private TSource* element;
            private TKeyEqualityComparer equalityComparer;
            private TSourceSelector sourceSelector;
            private readonly Allocator allocator;

            public Enumerator(in TOuterEnumerable outerEnumerable, in TInnerEnumerable innerEnumerable, in TOuterKeySelector outerKeySelector, in TInnerKeySelector innerKeySelector, in TSourceSelector sourceSelector, in TKeyEqualityComparer equalityComparer, Allocator allocator)
            {
                this = default;
                var innerCapacity = innerEnumerable.CanFastCount() ? innerEnumerable.LongCount() : 16L;
                if (innerCapacity == 0L)
                    return;
                element = UnsafeUtilityEx.Malloc<TSource>(1, allocator);
                this.outerKeySelector = outerKeySelector;
                outerEnumerator = outerEnumerable.GetEnumerator();
                this.equalityComparer = equalityComparer;
                this.sourceSelector = sourceSelector;
                this.allocator = allocator;
                innerCount = 0L;
                Alloc(innerCapacity, out innerValue, out innerKey);
                foreach (ref var item in innerEnumerable)
                {
                    if (innerCount == innerCapacity)
                        ReAlloc(ref innerCapacity);
                    innerValue[innerCount] = item;
                    innerKey[innerCount] = innerKeySelector.Calc(ref item);
                    ++innerCount;
                }
                innerIndex = innerCount;
            }

            private void Alloc(long count, out TInnerSource* values, out TKey* keys)
            {
                values = (TInnerSource*)UnsafeUtility.Malloc((sizeof(TInnerSource) * sizeof(TKey)) * count, 4, allocator);
                keys = (TKey*)(values + count);
            }

            private void ReAlloc(ref long capacity)
            {
                var newCapacity = capacity + (capacity >> 1);
                Alloc(newCapacity, out var values, out var keys);
                UnsafeUtilityEx.MemCpy(values, innerValue, capacity);
                UnsafeUtilityEx.MemCpy(keys, innerKey, capacity);
                UnsafeUtility.Free(innerValue, allocator);
                innerValue = values;
                innerKey = keys;
                capacity = newCapacity;
            }

            public readonly ref TSource Current => ref *element;
            readonly TSource IEnumerator<TSource>.Current => Current;
            readonly object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                ++innerIndex;
                while (true)
                {
                    if (innerIndex >= innerCount)
                    {
                        innerIndex = 0;
                        if (!outerEnumerator.MoveNext())
                            return false;
                        outerKey = outerKeySelector.Calc(ref outerEnumerator.Current);
                    }
                    for (; innerIndex < innerCount; innerIndex++)
                    {
                        if (!equalityComparer.Calc(ref innerKey[innerIndex], ref outerKey))
                            continue;
                        *element = sourceSelector.Calc(ref outerEnumerator.Current, ref innerValue[innerIndex]);
                        return true;
                    }
                }
            }

            public readonly void Reset() => throw new InvalidOperationException();

            public void Dispose()
            {
                outerEnumerator.Dispose();
                if (innerValue != null)
                    UnsafeUtility.Free(innerValue, allocator);
                if (element != null)
                    UnsafeUtility.Free(element, allocator);
                this = default;
            }

            public ref TSource TryGetNext(out bool success)
            {
                ++innerIndex;
                while (true)
                {
                    if (innerIndex >= innerCount)
                    {
                        innerIndex = 0;
                        ref var value = ref outerEnumerator.TryGetNext(out success);
                        if (!success)
                            return ref Unsafe.AsRef<TSource>(null);
                        outerKey = outerKeySelector.Calc(ref value);
                    }
                    for (; innerIndex < innerCount; innerIndex++)
                    {
                        if (!equalityComparer.Calc(ref innerKey[innerIndex], ref outerKey))
                            continue;
                        success = true;
                        *element = sourceSelector.Calc(ref outerEnumerator.Current, ref innerValue[innerIndex]);
                        return ref *element;
                    }
                }
            }

            public bool TryMoveNext(out TSource value)
            {
                ++innerIndex;
                while (true)
                {
                    if (innerIndex >= innerCount)
                    {
                        innerIndex = 0;
                        if (!outerEnumerator.TryMoveNext(out var _value))
                        {
                            value = default;
                            return false;
                        }
                        outerKey = outerKeySelector.Calc(ref _value);
                    }
                    for (; innerIndex < innerCount; innerIndex++)
                    {
                        if (!equalityComparer.Calc(ref innerKey[innerIndex], ref outerKey))
                            continue;
                        value = *element = sourceSelector.Calc(ref outerEnumerator.Current, ref innerValue[innerIndex]);
                        return true;
                    }
                }
            }
        }

        public readonly Enumerator GetEnumerator() => new Enumerator(outerEnumerable, innerEnumerable, outerKeySelector, innerKeySelector, sourceSelector, equalityComparer, alloc);

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();

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
