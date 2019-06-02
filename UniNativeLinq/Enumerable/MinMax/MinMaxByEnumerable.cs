using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;

namespace UniNativeLinq
{
    [SlowCount]
    public readonly unsafe struct
        MinMaxByEnumerable<TEnumerable, TEnumerator, T, TKey, TKeySelector, TKeyRenewPredicate, TKeyEqualityComparer>
        : IRefEnumerable<MinMaxByEnumerable<TEnumerable, TEnumerator, T, TKey, TKeySelector, TKeyRenewPredicate, TKeyEqualityComparer>.Enumerator, T>
        where T : unmanaged
        where TEnumerator : struct, IRefEnumerator<T>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, T>
        where TKey : unmanaged
        where TKeySelector : struct, IRefFunc<T, TKey>
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

        public struct Enumerator : IRefEnumerator<T>
        {
            private TEnumerator enumerator;
            private TKey key;
            private TKeySelector keySelector;
            private TKeyEqualityComparer equalityComparer;

            public ref T Current => ref enumerator.Current;
            T IEnumerator<T>.Current => Current;
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

            public ref T TryGetNext(out bool success)
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

            public bool TryMoveNext(out T value)
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
                MinMaxByEnumerable<TEnumerable, TEnumerator, T, TKey, TKeySelector, TKeyRenewPredicate, TKeyEqualityComparer>,
                Enumerator,
                T,
                TKey,
                DelegateFuncToStructOperatorFunc<T, TKey>,
                MinByPredicate<TKey, DefaultOrderByAscending<TKey>>,
                DefaultEqualityComparer<TKey>
            >
            MinBy(Func<T, TKey> func)
            => new MinMaxByEnumerable<
                MinMaxByEnumerable<TEnumerable, TEnumerator, T, TKey, TKeySelector, TKeyRenewPredicate, TKeyEqualityComparer>,
                Enumerator,
                T,
                TKey,
                DelegateFuncToStructOperatorFunc<T, TKey>,
                MinByPredicate<TKey, DefaultOrderByAscending<TKey>>,
                DefaultEqualityComparer<TKey>
            >
            (this, func, default, default);

        public
            MinMaxByEnumerable<
                MinMaxByEnumerable<TEnumerable, TEnumerator, T, TKey, TKeySelector, TKeyRenewPredicate, TKeyEqualityComparer>,
                Enumerator,
                T,
                TKey,
                DelegateFuncToStructOperatorFunc<T, TKey>,
                MaxByPredicate<TKey, DefaultOrderByAscending<TKey>>,
                DefaultEqualityComparer<TKey>
            >
            MaxBy(Func<T, TKey> func)
            => new MinMaxByEnumerable<
                MinMaxByEnumerable<TEnumerable, TEnumerator, T, TKey, TKeySelector, TKeyRenewPredicate, TKeyEqualityComparer>,
                Enumerator,
                T,
                TKey,
                DelegateFuncToStructOperatorFunc<T, TKey>,
                MaxByPredicate<TKey, DefaultOrderByAscending<TKey>>,
                DefaultEqualityComparer<TKey>
            >
            (this, func, default, default);

        public Enumerator GetEnumerator() => new Enumerator(enumerable.GetEnumerator(), keySelector, keyRenewPredicate, equalityComparer);

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

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
            CopyTo((T*)Unsafe.AsPointer(ref answer[0]));
            return answer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly NativeEnumerable<T> ToNativeEnumerable(Allocator allocator)
        {
            var count = LongCount();
            var ptr = UnsafeUtilityEx.Malloc<T>(count, allocator);
            CopyTo(ptr);
            return new NativeEnumerable<T>(ptr, count);
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
