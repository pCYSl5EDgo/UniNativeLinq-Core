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
        JoinEnumerable<TOuterEnumerable, TOuterEnumerator, TOuterSource, TInnerEnumerable, TInnerEnumerator, TInnerSource, TKey, TOuterKeySelector, TInnerKeySelector, TKeyEqualityComparer, T, TSelector>
        : IRefEnumerable<JoinEnumerable<TOuterEnumerable, TOuterEnumerator, TOuterSource, TInnerEnumerable, TInnerEnumerator, TInnerSource, TKey, TOuterKeySelector, TInnerKeySelector, TKeyEqualityComparer, T, TSelector>.Enumerator, T>
        where TOuterSource : unmanaged
        where TInnerSource : unmanaged
        where TOuterEnumerator : struct, IRefEnumerator<TOuterSource>
        where TInnerEnumerator : struct, IRefEnumerator<TInnerSource>
        where TOuterEnumerable : struct, IRefEnumerable<TOuterEnumerator, TOuterSource>
        where TInnerEnumerable : struct, IRefEnumerable<TInnerEnumerator, TInnerSource>
        where T : unmanaged
        where TKey : unmanaged
        where TOuterKeySelector : struct, IRefFunc<TOuterSource, TKey>
        where TInnerKeySelector : struct, IRefFunc<TInnerSource, TKey>
        where TKeyEqualityComparer : struct, IRefFunc<TKey, TKey, bool>
        where TSelector : struct, IRefFunc<TOuterSource, TInnerSource, T>
    {
        private TOuterEnumerable outerEnumerable;
        private TInnerEnumerable innerEnumerable;
        private TOuterKeySelector outerKeySelector;
        private TInnerKeySelector innerKeySelector;
        private TKeyEqualityComparer equalityComparer;
        private TSelector sourceSelector;
        private Allocator alloc;
        public bool CanIndexAccess() => false;
        public ref T this[long index] => throw new NotSupportedException();
        public JoinEnumerable(in TOuterEnumerable outerEnumerable, in TInnerEnumerable innerEnumerable, in TOuterKeySelector outerKeySelector, in TInnerKeySelector innerKeySelector, in TKeyEqualityComparer equalityComparer, in TSelector sourceSelector, Allocator allocator)
        {
            this.outerEnumerable = outerEnumerable;
            this.innerEnumerable = innerEnumerable;
            this.outerKeySelector = outerKeySelector;
            this.innerKeySelector = innerKeySelector;
            this.equalityComparer = equalityComparer;
            this.sourceSelector = sourceSelector;
            alloc = allocator;
        }

        [LocalRefReturn]
        public struct Enumerator : IRefEnumerator<T>
        {
            private TOuterEnumerator outerEnumerator;
            private TOuterKeySelector outerKeySelector;
            private TKey outerKey;
            private long innerCount;
            private long innerIndex;
            private TInnerSource* innerValues;
            private TKey* innerKeys;
            private T element;
            private TKeyEqualityComparer equalityComparer;
            private TSelector sourceSelector;
            private Allocator allocator;

            public Enumerator(in TOuterEnumerable outerEnumerable, in TInnerEnumerable innerEnumerable, in TOuterKeySelector outerKeySelector, in TInnerKeySelector innerKeySelector, in TSelector sourceSelector, in TKeyEqualityComparer equalityComparer, Allocator allocator)
            {
                this = default;
                var innerCapacity = innerEnumerable.CanFastCount() ? innerEnumerable.LongCount() : 16L;
                if (innerCapacity == 0L)
                    return;
                element = default;
                this.outerKeySelector = outerKeySelector;
                outerEnumerator = outerEnumerable.GetEnumerator();
                this.equalityComparer = equalityComparer;
                this.sourceSelector = sourceSelector;
                this.allocator = allocator;
                innerCount = 0L;
                Alloc(innerCapacity, out innerValues, out innerKeys);
                foreach (ref var item in innerEnumerable)
                {
                    if (innerCount == innerCapacity)
                        ReAlloc(ref innerCapacity);
                    innerValues[innerCount] = item;
                    innerKeys[innerCount] = innerKeySelector.Calc(ref item);
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
                UnsafeUtilityEx.MemCpy(values, innerValues, capacity);
                UnsafeUtilityEx.MemCpy(keys, innerKeys, capacity);
                UnsafeUtility.Free(innerValues, allocator);
                innerValues = values;
                innerKeys = keys;
                capacity = newCapacity;
            }

            public ref T Current => throw new NotImplementedException();
            T IEnumerator<T>.Current => Current;
            object IEnumerator.Current => Current;

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
                        if (!equalityComparer.Calc(ref innerKeys[innerIndex], ref outerKey))
                            continue;
                        element = sourceSelector.Calc(ref outerEnumerator.Current, ref innerValues[innerIndex]);
                        return true;
                    }
                }
            }

            public void Reset() => throw new InvalidOperationException();

            public void Dispose()
            {
                outerEnumerator.Dispose();
                if (innerValues == null || !UnsafeUtility.IsValidAllocator(allocator)) return;
                UnsafeUtility.Free(innerValues, allocator);
            }

            public ref T TryGetNext(out bool success)
            {
                ++innerIndex;
                while (true)
                {
                    if (innerIndex >= innerCount)
                    {
                        innerIndex = 0;
                        ref var value = ref outerEnumerator.TryGetNext(out success);
                        if (!success)
                            return ref Pseudo.AsRefNull<T>();
                        outerKey = outerKeySelector.Calc(ref value);
                    }
                    for (; innerIndex < innerCount; innerIndex++)
                    {
                        if (!equalityComparer.Calc(ref innerKeys[innerIndex], ref outerKey))
                            continue;
                        success = true;
                        element = sourceSelector.Calc(ref outerEnumerator.Current, ref innerValues[innerIndex]);
                        throw new NotImplementedException();
                    }
                }
            }

            public bool TryMoveNext(out T value)
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
                        if (!equalityComparer.Calc(ref innerKeys[innerIndex], ref outerKey))
                            continue;
                        value = element = sourceSelector.Calc(ref outerEnumerator.Current, ref innerValues[innerIndex]);
                        return true;
                    }
                }
            }
        }

        public Enumerator GetEnumerator() => new Enumerator(outerEnumerable, innerEnumerable, outerKeySelector, innerKeySelector, sourceSelector, equalityComparer, alloc);

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

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
        public void CopyTo(T* dest)
        {
            var enumerator = GetEnumerator();
            while (enumerator.MoveNext())
                *dest++ = enumerator.Current;
            enumerator.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray()
        {
            var count = LongCount();
            if (count == 0) return Array.Empty<T>();
            var answer = new T[count];
            CopyTo(Pseudo.AsPointer<T>(ref answer[0]));
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
