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
        private TOuterEnumerable outerEnumerable;
        private TInnerEnumerable innerEnumerable;
        private TOuterKeySelector outerKeySelector;
        private TInnerKeySelector innerKeySelector;
        private TSelector sourceSelector;
        private TKeyEqualityComparer keyEqualityComparer;
        private Allocator alloc;
        public bool CanIndexAccess() => false;
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
            private Allocator allocator;

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

        public Enumerator GetEnumerator() => new Enumerator(outerEnumerable, innerEnumerable, outerKeySelector, innerKeySelector, sourceSelector, keyEqualityComparer, alloc);

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
        public long CopyTo(T* dest)
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
        public T[] ToArray()
        {
            var count = LongCount();
            if (count == 0) return Array.Empty<T>();
            var answer = new T[count];
            CopyTo(Pseudo.AsPointer(ref answer[0]));
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
