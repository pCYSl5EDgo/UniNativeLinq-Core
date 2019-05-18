using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    internal unsafe struct
        NativeList<T>
        : IRefEnumerable<NativeList<T>.Enumerator, T>
        where T : unmanaged
    {
        private T* Ptr;
        private long Capacity;
        private long Length;
        private readonly Allocator Allocator;

        public NativeList(Allocator allocator)
        {
            Capacity = 16L;
            Ptr = UnsafeUtilityEx.Malloc<T>(Capacity, allocator);
            Length = 0L;
            Allocator = allocator;
        }

        public readonly Enumerator GetEnumerator() => new Enumerator(this);
        readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public readonly bool IsEmpty => Length == 0;
        public readonly bool IsFull => Length == Capacity;

        public void Add(in T value)
        {
            if (Length == Capacity)
            {
                var newCapa = Capacity + (Capacity >> 1);
                UnsafeUtilityEx.ReAlloc(ref Ptr, Capacity, newCapa, Allocator);
                Capacity = newCapa;
            }
            Ptr[Length++] = value;
        }

        public void Clear() => Length = 0;

        public readonly bool CanFastCount() => true;

        public readonly bool Any() => Length != 0;

        public readonly int Count() => (int)Length;

        public readonly long LongCount() => Length;

        public readonly NativeEnumerable<T> AsNativeEnumerable() => new NativeEnumerable<T>(Ptr, Length);

        public readonly void CopyTo(T* dest) => UnsafeUtilityEx.MemCpy(dest, Ptr, Length);

        public readonly NativeEnumerable<T> ToNativeEnumerable(Allocator allocator)
        {
            var ptr = UnsafeUtilityEx.Malloc<T>(Length, allocator);
            CopyTo(ptr);
            return new NativeEnumerable<T>(ptr, Length);
        }

        public readonly NativeArray<T> ToNativeArray(Allocator allocator)
        {
            var answer = new NativeArray<T>(Count(), allocator, NativeArrayOptions.UninitializedMemory);
            CopyTo(answer.GetPointer());
            return answer;
        }

        public readonly T[] ToArray()
        {
            var answer = new T[Length];
            CopyTo((T*)Unsafe.AsPointer(ref answer[0]));
            return answer;
        }

        public struct Enumerator : IRefEnumerator<T>
        {
            private readonly T* ptr;
            private readonly long count;
            private long index;

            internal Enumerator(in NativeList<T> @this)
            {
                ptr = @this.Ptr;
                count = @this.Length;
                index = -1;
            }

            public ref T Current => ref ptr[index];
            T IEnumerator<T>.Current => Current;
            object IEnumerator.Current => Current;
            public void Dispose() => this = default;
            public void Dispose(Allocator allocator)
            {
                if (UnsafeUtility.IsValidAllocator(allocator) && ptr != null)
                    UnsafeUtility.Free(ptr, allocator);
                this = default;
            }
            public bool MoveNext() => ++index < count;
            public void Reset() => index = -1;

            public ref T TryGetNext(out bool success)
            {
                if (index >= count)
                    success = false;
                else
                    success = ++index < count;
                return ref ptr[index];
            }
        }

        #region Enumerable
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly AppendEnumerable<NativeList<T>, Enumerator, T>
            Append(T value, Allocator allocator = Allocator.Temp)
            => new AppendEnumerable<NativeList<T>, Enumerator, T>(this, value, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly NativeList<T> AsRefEnumerable() => this;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly BufferEnumerable<NativeList<T>, Enumerator, T> Buffer(long count)
            => new BufferEnumerable<NativeList<T>, Enumerator, T>(this, count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly DefaultIfEmptyEnumerable<NativeList<T>, Enumerator, T>
            DefaultIfEmpty(T defaultValue, Allocator allocator = Allocator.Temp)
            => new DefaultIfEmptyEnumerable<NativeList<T>, Enumerator, T>(this, defaultValue, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly DistinctEnumerable<
                NativeList<T>,
                Enumerator, T, DefaultEqualityComparer<T>, DefaultGetHashCodeFunc<T>>
            Distinct(Allocator allocator = Allocator.Temp)
            => new DistinctEnumerable<NativeList<T>, Enumerator, T, DefaultEqualityComparer<T>, DefaultGetHashCodeFunc<T>>(this, default, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly DistinctEnumerable<
                NativeList<T>,
                Enumerator, T, TEqualityComparer0, TGetHashCodeFunc0>
            Distinct<TEqualityComparer0, TGetHashCodeFunc0>(in TEqualityComparer0 comparer, TGetHashCodeFunc0 getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            => new DistinctEnumerable<NativeList<T>, Enumerator, T, TEqualityComparer0, TGetHashCodeFunc0>(this, comparer, getHashCodeFunc, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly
            OrderByEnumerable<
                NativeList<T>,
                Enumerator,
                T,
                TComparer0
            >
            OrderBy<TComparer0>(in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where TComparer0 : struct, IRefFunc<T, T, int>
            => new OrderByEnumerable<NativeList<T>, Enumerator, T, TComparer0>(this, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly
            OrderByEnumerable<
                NativeList<T>,
                Enumerator,
                T,
                OrderByFromFunc<T>
            >
            OrderBy(Func<T, T, int> comparer, Allocator allocator = Allocator.Temp)
            => new OrderByEnumerable<NativeList<T>, Enumerator, T, OrderByFromFunc<T>>(this, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly
            OrderByEnumerable<
                NativeList<T>,
                Enumerator,
                T,
                OrderByDescending<T, TComparer0>
            >
            OrderByDescending<TComparer0>(in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where TComparer0 : struct, IRefFunc<T, T, int>
            => new OrderByEnumerable<NativeList<T>, Enumerator, T, OrderByDescending<T, TComparer0>>(this, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly
            OrderByEnumerable<
                NativeList<T>,
                Enumerator,
                T,
                OrderByDescending<T, OrderByFromFunc<T>>
            >
            OrderByDescending(Func<T, T, int> comparer, Allocator allocator = Allocator.Temp)
            => new OrderByEnumerable<NativeList<T>, Enumerator, T, OrderByDescending<T, OrderByFromFunc<T>>>(this, (OrderByFromFunc<T>)comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly
        (
            NativeEnumerable<T> True,
            NativeEnumerable<T> False
        )
        ToPartition(Func<T, bool> predicate, Allocator allocator = Allocator.Temp)
        {

            var True = new NativeList<T>(allocator);
            var False = new NativeList<T>(allocator);
            var enumerator = GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (predicate(enumerator.Current))
                    True.Add(enumerator.Current);
                else
                    False.Add(enumerator.Current);
            }
            enumerator.Dispose();
            return (True.AsNativeEnumerable(), False.AsNativeEnumerable());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly
        (
            NativeEnumerable<T> True,
            NativeEnumerable<T> False
        )
        ToPartition<TPredicate0>(in TPredicate0 predicate, Allocator allocator = Allocator.Temp)
            where TPredicate0 : struct, IRefFunc<T, bool>
        {

            var True = new NativeList<T>(allocator);
            var False = new NativeList<T>(allocator);
            var enumerator = GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (predicate.Calc(ref enumerator.Current))
                    True.Add(enumerator.Current);
                else
                    False.Add(enumerator.Current);
            }
            enumerator.Dispose();
            return (True.AsNativeEnumerable(), False.AsNativeEnumerable());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly
            PrependEnumerable<
                NativeList<T>,
                Enumerator,
                T
            >
            Prepend(T value, Allocator allocator = Allocator.Temp)
            => new PrependEnumerable<NativeList<T>, Enumerator, T>(this, value, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly
            ReverseEnumerable<NativeList<T>, Enumerator, T>
            Reverse(Allocator allocator = Allocator.Temp)
            => new ReverseEnumerable<NativeList<T>, Enumerator, T>(this, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly SelectIndexEnumerable<NativeList<T>, Enumerator, T, TNextResult0, TNextAction0>
            SelectIndex<TNextResult0, TNextAction0>(TNextAction0 action, Allocator allocator = Allocator.Temp)
            where TNextResult0 : unmanaged
            where TNextAction0 : struct, ISelectIndex<T, TNextResult0>
            => new SelectIndexEnumerable<NativeList<T>, Enumerator, T, TNextResult0, TNextAction0>(this, action, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly SelectIndexEnumerable<NativeList<T>, Enumerator, T, TNextResult0, DelegateFuncToSelectIndexStructOperator<T, TNextResult0>>
            SelectIndex<TNextResult0>(Func<T, long, TNextResult0> func, Allocator allocator = Allocator.Temp)
            where TNextResult0 : unmanaged
            => new SelectIndexEnumerable<NativeList<T>, Enumerator, T, TNextResult0, DelegateFuncToSelectIndexStructOperator<T, TNextResult0>>(this, func, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly SelectEnumerable<NativeList<T>, Enumerator, T, TNextResult0, TNextAction0>
            Select<TNextResult0, TNextAction0>(TNextAction0 action, Allocator allocator = Allocator.Temp)
            where TNextResult0 : unmanaged
            where TNextAction0 : struct, IRefAction<T, TNextResult0>
            => new SelectEnumerable<NativeList<T>, Enumerator, T, TNextResult0, TNextAction0>(this, action, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly SelectEnumerable<NativeList<T>, Enumerator, T, TNextResult0, DelegateFuncToAction<T, TNextResult0>>
            Select<TNextResult0>(Func<T, TNextResult0> func, Allocator allocator = Allocator.Temp)
            where TNextResult0 : unmanaged
            => new SelectEnumerable<NativeList<T>, Enumerator, T, TNextResult0, DelegateFuncToAction<T, TNextResult0>>(this, func, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly
            SelectManyEnumerable<
                NativeList<T>,
                Enumerator,
                T,
                TResult0,
                TResultEnumerable0,
                TResultEnumerator0,
                TResultAction
            >
            SelectMany<TResult0, TResultEnumerable0, TResultEnumerator0, TResultAction>(TResultAction action)
            where TResult0 : unmanaged
            where TResultEnumerator0 : struct, IRefEnumerator<TResult0>
            where TResultEnumerable0 : struct, IRefEnumerable<TResultEnumerator0, TResult0>
            where TResultAction : struct, IRefAction<T, TResultEnumerable0>
            => new SelectManyEnumerable<NativeList<T>, Enumerator, T, TResult0, TResultEnumerable0, TResultEnumerator0, TResultAction>(this, action);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly
            SelectManyEnumerable<
                NativeList<T>,
                Enumerator,
                T,
                TResult0,
                TResultEnumerable0,
                TResultEnumerator0,
                DelegateFuncToAction<T, TResultEnumerable0>
            >
            SelectMany<TResult0, TResultEnumerable0, TResultEnumerator0>(Func<T, TResultEnumerable0> func)
            where TResult0 : unmanaged
            where TResultEnumerator0 : struct, IRefEnumerator<TResult0>
            where TResultEnumerable0 : struct, IRefEnumerable<TResultEnumerator0, TResult0>
            => new SelectManyEnumerable<
                NativeList<T>,
                Enumerator,
                T,
                TResult0,
                TResultEnumerable0,
                TResultEnumerator0,
                DelegateFuncToAction<T, TResultEnumerable0>
            >(this, func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly
            SkipEnumerable<
                NativeList<T>,
                Enumerator,
                T
            >
            Skip(long count)
            => new SkipEnumerable<
                NativeList<T>,
                Enumerator,
                T
            >(this, count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly
            SkipLastEnumerable<
                NativeList<T>,
                Enumerator,
                T
            >
            SkipLast(long count, Allocator allocator = Allocator.Temp)
            => new SkipLastEnumerable<
                NativeList<T>,
                Enumerator,
                T
            >(this, count, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly
            SkipWhileEnumerable<
                NativeList<T>,
                Enumerator,
                T,
                TPredicate0
            >
            SkipWhileIndex<TPredicate0>(in TPredicate0 predicate)
            where TPredicate0 : struct, IRefFunc<T, bool>
            => new SkipWhileEnumerable<
                NativeList<T>,
                Enumerator,
                T,
                TPredicate0
            >(this, predicate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly
            TakeEnumerable<
                NativeList<T>,
                Enumerator,
                T
            >
            Take(long count)
            => new TakeEnumerable<
                NativeList<T>,
                Enumerator,
                T
            >(this, count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly
            TakeWhileEnumerable<
                NativeList<T>,
                Enumerator,
                T,
                TPredicate0
            >
            TakeWhileIndex<TPredicate0>(TPredicate0 predicate)
            where TPredicate0 : struct, IRefFunc<T, bool>
            => new TakeWhileEnumerable<
                NativeList<T>,
                Enumerator,
                T,
                TPredicate0
            >(this, predicate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly
            TakeLastEnumerable<
                NativeList<T>,
                Enumerator,
                T
            >
            TakeLast(long count, Allocator allocator = Allocator.Temp)
            => new TakeLastEnumerable<
                NativeList<T>,
                Enumerator,
                T
            >(this, count, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly
            WhereEnumerable<
                NativeList<T>,
                Enumerator,
                T,
                TPredicate0
            >
            Where<TPredicate0>(TPredicate0 predicate)
            where TPredicate0 : struct, IRefFunc<T, bool>
            => new WhereEnumerable<
                NativeList<T>,
                Enumerator,
                T,
                TPredicate0
            >(this, predicate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly
            SelectIndexEnumerable<
                NativeList<T>,
                Enumerator,
                T,
                (T, long),
                WithIndex<T>
            >
            WithIndex(Allocator allocator = Allocator.Temp)
            => new SelectIndexEnumerable<
                NativeList<T>,
                Enumerator,
                T,
                (T, long),
                WithIndex<T>
            >(this, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly
            WhereEnumerable<
                NativeList<T>,
                Enumerator,
                T,
                DelegateFuncToStructOperatorFunc<T, bool>
            >
            Where(Func<T, bool> predicate)
            => new WhereEnumerable<
                NativeList<T>,
                Enumerator,
                T,
                DelegateFuncToStructOperatorFunc<T, bool>
            >(this, predicate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly
            WhereIndexEnumerable<
                NativeList<T>,
                Enumerator,
                T,
                TPredicate0
            >
            WhereIndex<TPredicate0>(TPredicate0 predicate)
            where TPredicate0 : struct, IWhereIndex<T>
            => new WhereIndexEnumerable<
                NativeList<T>,
                Enumerator,
                T,
                TPredicate0
            >(this, predicate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly
            WhereIndexEnumerable<
                NativeList<T>,
                Enumerator,
                T,
                DelegateFuncToWhereIndexStructOperator<T>
            >
            WhereIndex(Func<T, long, bool> predicate)
            => new WhereIndexEnumerable<
                NativeList<T>,
                Enumerator,
                T,
                DelegateFuncToWhereIndexStructOperator<T>
            >(this, predicate);
        #endregion
    }
}
