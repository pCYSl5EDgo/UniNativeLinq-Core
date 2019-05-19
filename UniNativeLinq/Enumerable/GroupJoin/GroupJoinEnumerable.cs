using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    public unsafe partial struct
        GroupJoinEnumerable<TOuterEnumerable, TOuterEnumerator, TOuterSource, TInnerEnumerable, TInnerEnumerator, TInnerSource, TKey, TOuterKeySelector, TInnerKeySelector, TSource, TSourceSelector, TKeyEqualityComparer>
        : IRefEnumerable<GroupJoinEnumerable<TOuterEnumerable, TOuterEnumerator, TOuterSource, TInnerEnumerable, TInnerEnumerator, TInnerSource, TKey, TOuterKeySelector, TInnerKeySelector, TSource, TSourceSelector, TKeyEqualityComparer>.Enumerator, TSource>
        where TOuterSource : unmanaged
        where TInnerSource : unmanaged
        where TOuterEnumerator : struct, IRefEnumerator<TOuterSource>
        where TInnerEnumerator : struct, IRefEnumerator<TInnerSource>
        where TOuterEnumerable : struct, IRefEnumerable<TOuterEnumerator, TOuterSource>
        where TInnerEnumerable : struct, IRefEnumerable<TInnerEnumerator, TInnerSource>
        where TKey : unmanaged
        where TOuterKeySelector : struct, IRefFunc<TOuterSource, TKey>
        where TInnerKeySelector : struct, IRefFunc<TInnerSource, TKey>
        where TSource : unmanaged
        where TSourceSelector : struct,
        IRefFunc<TOuterSource,
            WhereIndexEnumerable<
                NativeEnumerable<TInnerSource>,
                NativeEnumerable<TInnerSource>.Enumerator,
                TInnerSource,
                GroupJoinPredicate<TInnerSource, TKey, TKeyEqualityComparer>
                >,
            TSource>
        where TKeyEqualityComparer : struct, IRefFunc<TKey, TKey, bool>
    {
        private TOuterEnumerable outerEnumerable;
        private TInnerEnumerable innerEnumerable;
        private TOuterKeySelector outerKeySelector;
        private TInnerKeySelector InnerKeySelector;
        private readonly TSourceSelector sourceSelector;
        private readonly TKeyEqualityComparer keyEqualityComparer;
        private readonly Allocator alloc;

        public GroupJoinEnumerable(in TOuterEnumerable outerEnumerable, in TInnerEnumerable innerEnumerable, in TOuterKeySelector outerKeySelector, in TInnerKeySelector innerKeySelector, in TSourceSelector sourceSelector, in TKeyEqualityComparer keyEqualityComparer, Allocator allocator)
        {
            this.outerEnumerable = outerEnumerable;
            this.innerEnumerable = innerEnumerable;
            this.outerKeySelector = outerKeySelector;
            InnerKeySelector = innerKeySelector;
            this.sourceSelector = sourceSelector;
            this.keyEqualityComparer = keyEqualityComparer;
            alloc = allocator;
        }

        public struct Enumerator : IRefEnumerator<TSource>
        {
            private WhereIndexEnumerable<
                NativeEnumerable<TInnerSource>,
                NativeEnumerable<TInnerSource>.Enumerator,
                TInnerSource,
                GroupJoinPredicate<TInnerSource, TKey, TKeyEqualityComparer>
                > enumerable;
            private TOuterEnumerator enumerator;
            private TOuterKeySelector keySelector;
            private TSourceSelector selector;
            private TSource* element;
            private Allocator allocator;

            internal Enumerator(in TOuterEnumerable outerEnumerable, in TInnerEnumerable innerEnumerable, in TOuterKeySelector outerKeySelector, in TInnerKeySelector innerKeySelector, in TSourceSelector sourceSelector, in TKeyEqualityComparer keyEqualityComparer, Allocator allocator)
            {
                enumerator = outerEnumerable.GetEnumerator();
                var inners = innerEnumerable.ToNativeEnumerable(allocator);
                var predicate = GroupJoinPredicate<TInnerSource, TKey, TKeyEqualityComparer>.Create(inners, innerKeySelector, keyEqualityComparer, allocator);
                enumerable = new WhereIndexEnumerable<NativeEnumerable<TInnerSource>, NativeEnumerable<TInnerSource>.Enumerator, TInnerSource, GroupJoinPredicate<TInnerSource, TKey, TKeyEqualityComparer>>(inners, predicate);
                selector = sourceSelector;
                element = UnsafeUtilityEx.Malloc<TSource>(1, allocator);
                keySelector = outerKeySelector;
                this.allocator = allocator;
            }

            public ref TSource Current => ref *element;
            TSource IEnumerator<TSource>.Current => Current;
            object IEnumerator.Current => Current;

            public void Dispose()
            {
                enumerator.Dispose();
                enumerable.Enumerable.Dispose(allocator);
                enumerable.Predication.Dispose();
                if (element != null)
                    UnsafeUtility.Free(element, allocator);
                this = default;
            }

            public bool MoveNext()
            {
                if (!enumerator.MoveNext()) return false;
                ref var c = ref enumerator.Current;
                enumerable.Predication.Key = keySelector.Calc(ref c);
                *element = selector.Calc(ref c, ref enumerable);
                return true;
            }

            public void Reset() => throw new InvalidOperationException();

            public ref TSource TryGetNext(out bool success)
            {
                ref var value = ref enumerator.TryGetNext(out success);
                if (!success) return ref *element;
                enumerable.Predication.Key = keySelector.Calc(ref value);
                *element = selector.Calc(ref value, ref enumerable);
                return ref *element;
            }

            public bool TryMoveNext(out TSource value)
            {
                if (!enumerator.TryMoveNext(out var outerValue))
                {
                    value = default;
                    return false;
                }
                enumerable.Predication.Key = keySelector.Calc(ref outerValue);
                value = *element = selector.Calc(ref outerValue, ref enumerable);
                return true;
            }
        }

        public readonly Enumerator GetEnumerator() => new Enumerator(outerEnumerable, innerEnumerable, outerKeySelector, InnerKeySelector, sourceSelector, keyEqualityComparer, alloc);

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
