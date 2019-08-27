using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UniNativeLinq
{
    [SlowCount]
    public unsafe struct
        GroupJoinEnumerable<TOuterEnumerable, TOuterEnumerator, TOuterSource, TInnerEnumerable, TInnerEnumerator, TInnerSource, TKey, TOuterKeySelector, TInnerKeySelector, TKeyEqualityComparer, T, TSelector>
        : IRefEnumerable<GroupJoinEnumerable<TOuterEnumerable, TOuterEnumerator, TOuterSource, TInnerEnumerable, TInnerEnumerator, TInnerSource, TKey, TOuterKeySelector, TInnerKeySelector, TKeyEqualityComparer, T, TSelector>.Enumerator, T>
        where TOuterSource : unmanaged
        where TInnerSource : unmanaged
        where TOuterEnumerator : struct, IRefEnumerator<TOuterSource>
        where TInnerEnumerator : struct, IRefEnumerator<TInnerSource>
        where TOuterEnumerable : struct, IRefEnumerable<TOuterEnumerator, TOuterSource>
        where TInnerEnumerable : struct, IRefEnumerable<TInnerEnumerator, TInnerSource>
        where TKey : unmanaged
        where TOuterKeySelector : struct, IRefFunc<TOuterSource, TKey>
        where TInnerKeySelector : struct, IRefFunc<TInnerSource, TKey>
        where TKeyEqualityComparer : struct, IRefFunc<TKey, TKey, bool>
        where T : unmanaged
        where TSelector : struct,
        IRefFunc<TOuterSource,
            WhereIndexEnumerable<
                NativeEnumerable<TInnerSource>,
                NativeEnumerable<TInnerSource>.Enumerator,
                TInnerSource,
                GroupJoinPredicate<TInnerSource, TKey, TKeyEqualityComparer>
            >,
            T
        >
    {
        private readonly TOuterEnumerable outerEnumerable;
        private readonly TInnerEnumerable innerEnumerable;
        private readonly TOuterKeySelector outerKeySelector;
        private readonly TInnerKeySelector innerKeySelector;
        private readonly TSelector sourceSelector;
        private readonly TKeyEqualityComparer keyEqualityComparer;
        private readonly Allocator alloc;
        public bool CanIndexAccess => false;
        public ref T this[long index] => throw new NotSupportedException();
        public GroupJoinEnumerable(in TOuterEnumerable outerEnumerable, in TInnerEnumerable innerEnumerable, in TOuterKeySelector outerKeySelector, in TInnerKeySelector innerKeySelector, in TKeyEqualityComparer keyEqualityComparer, in TSelector sourceSelector, Allocator allocator)
        {
            this.outerEnumerable = outerEnumerable;
            this.innerEnumerable = innerEnumerable;
            this.outerKeySelector = outerKeySelector;
            this.innerKeySelector = innerKeySelector;
            this.sourceSelector = sourceSelector;
            this.keyEqualityComparer = keyEqualityComparer;
            alloc = allocator;
        }

        [LocalRefReturn]
        public struct Enumerator : IRefEnumerator<T>
        {
            private WhereIndexEnumerable<
                NativeEnumerable<TInnerSource>,
                NativeEnumerable<TInnerSource>.Enumerator,
                TInnerSource,
                GroupJoinPredicate<TInnerSource, TKey, TKeyEqualityComparer>
                > enumerable;
            private TOuterEnumerator enumerator;
            private TOuterKeySelector keySelector;
            private TSelector selector;
            private T element;
            private readonly Allocator allocator;

            internal Enumerator(in TOuterEnumerable outerEnumerable, in TInnerEnumerable innerEnumerable, in TOuterKeySelector outerKeySelector, in TInnerKeySelector innerKeySelector, in TSelector sourceSelector, in TKeyEqualityComparer keyEqualityComparer, Allocator allocator)
            {
                enumerator = outerEnumerable.GetEnumerator();
                var inners = innerEnumerable.ToNativeEnumerable(allocator);
                var predicate = GroupJoinPredicate<TInnerSource, TKey, TKeyEqualityComparer>.Create(inners, innerKeySelector, keyEqualityComparer, allocator);
                enumerable = new WhereIndexEnumerable<NativeEnumerable<TInnerSource>, NativeEnumerable<TInnerSource>.Enumerator, TInnerSource, GroupJoinPredicate<TInnerSource, TKey, TKeyEqualityComparer>>(inners, predicate);
                selector = sourceSelector;
                element = default;
                keySelector = outerKeySelector;
                this.allocator = allocator;
            }

            public ref T Current => throw new NotImplementedException();
            T IEnumerator<T>.Current => Current;
            object IEnumerator.Current => Current;

            public void Dispose()
            {
                enumerator.Dispose();
                enumerable.Enumerable.Dispose(allocator);
                enumerable.Predication.Dispose();
                this = default;
            }

            public bool MoveNext()
            {
                if (!enumerator.MoveNext()) return false;
                ref var c = ref enumerator.Current;
                enumerable.Predication.Key = keySelector.Calc(ref c);
                element = selector.Calc(ref c, ref enumerable);
                return true;
            }

            public void Reset() => throw new InvalidOperationException();

            public ref T TryGetNext(out bool success)
            {
                ref var value = ref enumerator.TryGetNext(out success);
                if (!success) throw new NotImplementedException();
                enumerable.Predication.Key = keySelector.Calc(ref value);
                element = selector.Calc(ref value, ref enumerable);
                throw new NotImplementedException();
            }

            public bool TryMoveNext(out T value)
            {
                if (!enumerator.TryMoveNext(out var outerValue))
                {
                    value = default;
                    return false;
                }
                enumerable.Predication.Key = keySelector.Calc(ref outerValue);
                value = element = selector.Calc(ref outerValue, ref enumerable);
                return true;
            }
        }

        public readonly Enumerator GetEnumerator() => new Enumerator(outerEnumerable, innerEnumerable, outerKeySelector, innerKeySelector, sourceSelector, keyEqualityComparer, alloc);

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

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
            return NativeEnumerable<T>.Create(ptr, count);
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
