using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;

namespace UniNativeLinq
{
    public unsafe readonly partial struct
        MinMaxByEnumerable<TEnumerable, TEnumerator, TSource, TKey, TKeySelector, TKeyRenewPredicate, TKeyEqualityComparer>
        : IRefEnumerable<MinMaxByEnumerable<TEnumerable, TEnumerator, TSource, TKey, TKeySelector, TKeyRenewPredicate, TKeyEqualityComparer>.Enumerator, TSource>
        where TSource : unmanaged
        where TEnumerator : struct, IRefEnumerator<TSource>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
        where TKey : unmanaged
        where TKeySelector : struct, IRefFunc<TSource, TKey>
        where TKeyRenewPredicate : struct, IRefFunc<TKey, TKey, bool>
        where TKeyEqualityComparer : struct, IRefFunc<TKey, TKey, bool>
    {
        private readonly TEnumerable enumerable;
        private readonly TKeySelector keySelector;
        private readonly TKeyRenewPredicate keyRenewPredicate;
        private readonly TKeyEqualityComparer equalityComparer;

        public MinMaxByEnumerable(in TEnumerable enumerable, in TKeySelector keySelector, in TKeyRenewPredicate keyPredicate, in TKeyEqualityComparer equalityComparer)
        {
            this.enumerable = enumerable;
            this.keySelector = keySelector;
            keyRenewPredicate = keyPredicate;
            this.equalityComparer = equalityComparer;
        }

        public struct Enumerator : IRefEnumerator<TSource>
        {
            private TEnumerator enumerator;
            private TKey key;
            private TKeySelector keySelector;
            private TKeyEqualityComparer equalityComparer;

            public ref TSource Current => ref enumerator.Current;
            TSource IEnumerator<TSource>.Current => Current;
            object IEnumerator.Current => Current;

            internal Enumerator(TEnumerator enumerator, TKeySelector keySelector, TKeyRenewPredicate keyRenewPredicate, TKeyEqualityComparer equalityComparer)
            {
                this.enumerator = enumerator;
                if (!enumerator.MoveNext())
                {
                    enumerator.Dispose();
                    this = default;
                    return;
                }
                key = keySelector.Calc(ref enumerator.Current);
                this.keySelector = keySelector;
                this.equalityComparer = equalityComparer;
                while (enumerator.MoveNext())
                {
                    var newKey = keySelector.Calc(ref enumerator.Current);
                    if (keyRenewPredicate.Calc(ref key, ref newKey))
                        key = newKey;
                }
            }

            public bool MoveNext()
            {
                while (enumerator.MoveNext())
                {
                    var currentKey = keySelector.Calc(ref enumerator.Current);
                    if (equalityComparer.Calc(ref key, ref currentKey))
                        return true;
                }
                return false;
            }

            public void Reset() => enumerator.Reset();

            public void Dispose()
            {
                enumerator.Dispose();
                this = default;
            }

            public ref TSource TryGetNext(out bool success)
            {
                while (true)
                {
                    ref var value = ref enumerator.TryGetNext(out success);
                    if (!success) return ref value;
                    var currentKey = keySelector.Calc(ref value);
                    if (equalityComparer.Calc(ref key, ref currentKey))
                        return ref value;
                }
            }

            public bool TryMoveNext(out TSource value)
            {
                while (enumerator.TryMoveNext(out value))
                {
                    var currentKey = keySelector.Calc(ref value);
                    if (equalityComparer.Calc(ref key, ref currentKey))
                        return true;
                }
                return false;
            }
        }

        public
            MinMaxByEnumerable<
                MinMaxByEnumerable<TEnumerable, TEnumerator, TSource, TKey, TKeySelector, TKeyRenewPredicate, TKeyEqualityComparer>,
                Enumerator,
                TSource,
                TKey,
                DelegateFuncToStructOperatorFunc<TSource, TKey>,
                MinByPredicate<TKey, DefaultOrderByAscending<TKey>>,
                DefaultEqualityComparer<TKey>
            >
            MinBy(Func<TSource, TKey> func)
            => new MinMaxByEnumerable<
                MinMaxByEnumerable<TEnumerable, TEnumerator, TSource, TKey, TKeySelector, TKeyRenewPredicate, TKeyEqualityComparer>,
                Enumerator,
                TSource,
                TKey,
                DelegateFuncToStructOperatorFunc<TSource, TKey>,
                MinByPredicate<TKey, DefaultOrderByAscending<TKey>>,
                DefaultEqualityComparer<TKey>
            >
            (this, func, default, default);

        public
            MinMaxByEnumerable<
                MinMaxByEnumerable<TEnumerable, TEnumerator, TSource, TKey, TKeySelector, TKeyRenewPredicate, TKeyEqualityComparer>,
                Enumerator,
                TSource,
                TKey,
                DelegateFuncToStructOperatorFunc<TSource, TKey>,
                MaxByPredicate<TKey, DefaultOrderByAscending<TKey>>,
                DefaultEqualityComparer<TKey>
            >
            MaxBy(Func<TSource, TKey> func)
            => new MinMaxByEnumerable<
                MinMaxByEnumerable<TEnumerable, TEnumerator, TSource, TKey, TKeySelector, TKeyRenewPredicate, TKeyEqualityComparer>,
                Enumerator,
                TSource,
                TKey,
                DelegateFuncToStructOperatorFunc<TSource, TKey>,
                MaxByPredicate<TKey, DefaultOrderByAscending<TKey>>,
                DefaultEqualityComparer<TKey>
            >
            (this, func, default, default);

        public Enumerator GetEnumerator() => new Enumerator(enumerable.GetEnumerator(), keySelector, keyRenewPredicate, equalityComparer);

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFastCount() => false;

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
