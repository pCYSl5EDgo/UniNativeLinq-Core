using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using Unity.Collections;

namespace UniNativeLinq
{
    public static unsafe class NativeArrayEnumerable
    {
        #region Interface Implementation
        public static
            NativeEnumerable<T>.Enumerator
            GetEnumerator<T>(this NativeArray<T> @this)
            where T : unmanaged
            => @this.AsRefEnumerable().GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CanFastCount<T>(this NativeArray<T> _)
            where T : unmanaged
            => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Any<T>(this NativeArray<T> @this)
            where T : unmanaged
            => @this.Length != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Count<T>(this NativeArray<T> @this)
            where T : unmanaged
            => @this.Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long LongCount<T>(this NativeArray<T> @this)
            where T : unmanaged
            => @this.Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo<T>(this NativeArray<T> @this, T* dest)
            where T : unmanaged
            => UnsafeUtilityEx.MemCpy(dest, UnsafeUtilityEx.GetPointer(@this), @this.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeEnumerable<T> ToNativeEnumerable<T>(this NativeArray<T> @this, Allocator allocator)
            where T : unmanaged
        {
            var count = LongCount(@this);
            var ptr = UnsafeUtilityEx.Malloc<T>(count, allocator);
            CopyTo(@this, ptr);
            return new NativeEnumerable<T>(ptr, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeArray<T> ToNativeArray<T>(this NativeArray<T> @this, Allocator allocator)
            where T : unmanaged
        {
            if (@this.Length == 0) return default;
            var answer = new NativeArray<T>(@this.Length, allocator, NativeArrayOptions.UninitializedMemory);
            @this.CopyTo(answer);
            return answer;
        }
        #endregion

        #region Average
        public static bool TryGetAverage(this NativeArray<byte> @this, out byte value)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                value = default;
                return false;
            }
            long sum = @this[0];
            for (var i = 1; i < @this.Length; i++)
                sum += @this[i];
            value = (byte)(sum / @this.Length);
            return true;
        }

        public static bool TryGetAverage(this NativeArray<sbyte> @this, out sbyte value)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                value = default;
                return false;
            }
            long sum = @this[0];
            for (var i = 1; i < @this.Length; i++)
                sum += @this[i];
            value = (sbyte)(sum / @this.Length);
            return true;
        }

        public static bool TryGetAverage(this NativeArray<short> @this, out short value)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                value = default;
                return false;
            }
            long sum = @this[0];
            for (var i = 1; i < @this.Length; i++)
                sum += @this[i];
            value = (short)(sum / @this.Length);
            return true;
        }

        public static bool TryGetAverage(this NativeArray<ushort> @this, out ushort value)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                value = default;
                return false;
            }
            long sum = @this[0];
            for (var i = 1; i < @this.Length; i++)
                sum += @this[i];
            value = (ushort)(sum / @this.Length);
            return true;
        }

        public static bool TryGetAverage(this NativeArray<int> @this, out int value)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                value = default;
                return false;
            }
            long sum = @this[0];
            for (var i = 1; i < @this.Length; i++)
                sum += @this[i];
            value = (int)(sum / @this.Length);
            return true;
        }

        public static bool TryGetAverage(this NativeArray<uint> @this, out uint value)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                value = default;
                return false;
            }
            long sum = @this[0];
            for (var i = 1; i < @this.Length; i++)
                sum += @this[i];
            value = (uint)(sum / @this.Length);
            return true;
        }

        public static bool TryGetAverage(this NativeArray<long> @this, out long value)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                value = default;
                return false;
            }
            long sum = @this[0];
            for (var i = 1; i < @this.Length; i++)
                sum += @this[i];
            value = sum / @this.Length;
            return true;
        }

        public static bool TryGetAverage(this NativeArray<ulong> @this, out ulong value)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                value = default;
                return false;
            }
            ulong sum = @this[0];
            for (var i = 1; i < @this.Length; i++)
                sum += @this[i];
            value = sum / (ulong)@this.Length;
            return true;
        }

        public static bool TryGetAverage(this NativeArray<float> @this, out float value)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                value = default;
                return false;
            }
            var sum = @this[0];
            for (var i = 1; i < @this.Length; i++)
                sum += @this[i];
            value = sum / @this.Length;
            return true;
        }

        public static bool TryGetAverage(this NativeArray<double> @this, out double value)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                value = default;
                return false;
            }
            var sum = @this[0];
            for (var i = 1; i < @this.Length; i++)
                sum += @this[i];
            value = sum / @this.Length;
            return true;
        }

        public static bool TryGetAverage(this NativeArray<decimal> @this, out decimal value)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                value = default;
                return false;
            }
            var sum = @this[0];
            for (var i = 1; i < @this.Length; i++)
                sum += @this[i];
            value = sum / @this.Length;
            return true;
        }
        #endregion

        #region Enumerable
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AppendEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T>
            Append<T>(this NativeArray<T> @this, T value, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            => new AppendEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T>(@this.AsRefEnumerable(), value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DefaultIfEmptyEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T>
            DefaultIfEmpty<T>(this NativeArray<T> @this, T defaultValue, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            => new DefaultIfEmptyEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T>(@this.AsRefEnumerable(), defaultValue, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DistinctEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator, T, DefaultEqualityComparer<T>, DefaultGetHashCodeFunc<T>>
            Distinct<T>(this NativeArray<T> @this, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            => new DistinctEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, DefaultEqualityComparer<T>, DefaultGetHashCodeFunc<T>>(@this.AsRefEnumerable(), default, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DistinctEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator, T, TEqualityComparer0, TGetHashCodeFunc0>
            Distinct<TEqualityComparer0, TGetHashCodeFunc0, T>(this NativeArray<T> @this, in TEqualityComparer0 comparer, TGetHashCodeFunc0 getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            => new DistinctEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TEqualityComparer0, TGetHashCodeFunc0>(@this.AsRefEnumerable(), comparer, getHashCodeFunc, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            OrderByEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TComparer0
            >
            OrderBy<TComparer0, T>(this NativeArray<T> @this, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            => new OrderByEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TComparer0>(@this.AsRefEnumerable(), comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            OrderByEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DefaultOrderByAscending<T>
            >
            OrderBy<T>(this NativeArray<T> @this, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            => new OrderByEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, DefaultOrderByAscending<T>>(@this.AsRefEnumerable(), default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            OrderByEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DelegateFuncToStructOperatorFunc<T, T, int>
            >
            OrderBy<T>(this NativeArray<T> @this, Func<T, T, int> comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            => new OrderByEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, DelegateFuncToStructOperatorFunc<T, T, int>>(@this.AsRefEnumerable(), comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            OrderByEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                OrderByDescending<T, TComparer0>
            >
            OrderByDescending<TComparer0, T>(this NativeArray<T> @this, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            => new OrderByEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, OrderByDescending<T, TComparer0>>(@this.AsRefEnumerable(), comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            OrderByEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DefaultOrderByDescending<T>
            >
            OrderByDescending<T>(this NativeArray<T> @this, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            => new OrderByEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, DefaultOrderByDescending<T>>(@this.AsRefEnumerable(), default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            OrderByEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                OrderByDescending<T, DelegateFuncToStructOperatorFunc<T, T, int>>
            >
            OrderByDescending<T>(this NativeArray<T> @this, Func<T, T, int> comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            => new OrderByEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, OrderByDescending<T, DelegateFuncToStructOperatorFunc<T, T, int>>>(@this.AsRefEnumerable(), (DelegateFuncToStructOperatorFunc<T, T, int>)comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
        (
            NativeEnumerable<T> True,
            NativeEnumerable<T> False
        )
        ToPartition<T>(this NativeArray<T> @this, Func<T, bool> predicate, Allocator allocator = Allocator.Temp)
            where T : unmanaged
        {

            var True = new NativeList<T>(allocator);
            var False = new NativeList<T>(allocator);
            var enumerator = GetEnumerator(@this);
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
        public static
        (
            NativeEnumerable<T> True,
            NativeEnumerable<T> False
        )
        ToPartition<TPredicate0, T>(this NativeArray<T> @this, in TPredicate0 predicate, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TPredicate0 : struct, IRefFunc<T, bool>
        {

            var True = new NativeList<T>(allocator);
            var False = new NativeList<T>(allocator);
            var enumerator = GetEnumerator(@this);
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
        public static
            PrependEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T
            >
            Prepend<T>(this NativeArray<T> @this, T value, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            => new PrependEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T>(@this.AsRefEnumerable(), value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ReverseEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T>
            Reverse<T>(this NativeArray<T> @this, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            => new ReverseEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T>(@this.AsRefEnumerable(), allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SelectIndexEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TNextResult0, TNextAction0>
            SelectIndex<TNextResult0, TNextAction0, T>(this NativeArray<T> @this, TNextAction0 action, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TNextResult0 : unmanaged
            where TNextAction0 : struct, ISelectIndex<T, TNextResult0>
            => new SelectIndexEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TNextResult0, TNextAction0>(@this.AsRefEnumerable(), action);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SelectIndexEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TNextResult0, DelegateFuncToSelectIndexStructOperator<T, TNextResult0>>
            SelectIndex<TNextResult0, T>(this NativeArray<T> @this, Func<T, long, TNextResult0> func, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TNextResult0 : unmanaged
            => new SelectIndexEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TNextResult0, DelegateFuncToSelectIndexStructOperator<T, TNextResult0>>(@this.AsRefEnumerable(), func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SelectEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TNextResult0, TNextAction0>
            Select<TNextResult0, TNextAction0, T>(this NativeArray<T> @this, TNextAction0 action, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TNextResult0 : unmanaged
            where TNextAction0 : struct, IRefAction<T, TNextResult0>
            => new SelectEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TNextResult0, TNextAction0>(@this.AsRefEnumerable(), action);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SelectEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TNextResult0, DelegateFuncToAction<T, TNextResult0>>
            Select<TNextResult0, T>(this NativeArray<T> @this, Func<T, TNextResult0> func, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TNextResult0 : unmanaged
            => new SelectEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TNextResult0, DelegateFuncToAction<T, TNextResult0>>(@this.AsRefEnumerable(), func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TResult0,
                TResultEnumerable0,
                TResultEnumerator0,
                TResultAction
            >
            SelectMany<TResult0, TResultEnumerable0, TResultEnumerator0, TResultAction, T>(this NativeArray<T> @this, TResultAction action)
            where T : unmanaged
            where TResult0 : unmanaged
            where TResultEnumerator0 : struct, IRefEnumerator<TResult0>
            where TResultEnumerable0 : struct, IRefEnumerable<TResultEnumerator0, TResult0>
            where TResultAction : struct, IRefAction<T, TResultEnumerable0>
            => new SelectManyEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TResult0, TResultEnumerable0, TResultEnumerator0, TResultAction>(@this.AsRefEnumerable(), action);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TResult0,
                TResultEnumerable0,
                TResultEnumerator0,
                DelegateFuncToAction<T, TResultEnumerable0>
            >
            SelectMany<TResult0, TResultEnumerable0, TResultEnumerator0, T>(this NativeArray<T> @this, Func<T, TResultEnumerable0> func)
            where T : unmanaged
            where TResult0 : unmanaged
            where TResultEnumerator0 : struct, IRefEnumerator<TResult0>
            where TResultEnumerable0 : struct, IRefEnumerable<TResultEnumerator0, TResult0>
            => new SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TResult0,
                TResultEnumerable0,
                TResultEnumerator0,
                DelegateFuncToAction<T, TResultEnumerable0>
            >(@this.AsRefEnumerable(), func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            NativeEnumerable<T>
            Skip<T>(this NativeArray<T> @this, long count)
            where T : unmanaged
            => new NativeEnumerable<T>(@this).Skip(count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            NativeEnumerable<T>
            SkipLast<T>(this NativeArray<T> @this, long count)
            where T : unmanaged
            => @this.AsRefEnumerable().SkipLast(count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SkipWhileEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TPredicate0
            >
            SkipWhile<TPredicate0, T>(this NativeArray<T> @this, in TPredicate0 predicate)
            where T : unmanaged
            where TPredicate0 : struct, IRefFunc<T, bool>
            => new SkipWhileEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TPredicate0
            >(@this.AsRefEnumerable(), predicate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SkipWhileEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DelegateFuncToStructOperatorFunc<T, bool>
            >
            SkipWhile<T>(this NativeArray<T> @this, Func<T, bool> predicate)
            where T : unmanaged
            => new SkipWhileEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DelegateFuncToStructOperatorFunc<T, bool>
            >(@this.AsRefEnumerable(), predicate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            NativeEnumerable<T>
            Take<T>(this NativeArray<T> @this, long count)
            where T : unmanaged
            => new NativeEnumerable<T>(@this).Take(count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            TakeWhileEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TPredicate0
            >
            TakeWhile<TPredicate0, T>(this NativeArray<T> @this, TPredicate0 predicate)
            where T : unmanaged
            where TPredicate0 : struct, IRefFunc<T, bool>
            => new TakeWhileEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TPredicate0
            >(@this.AsRefEnumerable(), predicate);

        public static
            TakeWhileEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DelegateFuncToStructOperatorFunc<T, bool>
            >
            TakeWhile<T>(this NativeArray<T> @this, Func<T, bool> predicate)
            where T : unmanaged
            => new TakeWhileEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DelegateFuncToStructOperatorFunc<T, bool>
            >(@this.AsRefEnumerable(), predicate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            NativeEnumerable<T>
            TakeLast<T>(this NativeArray<T> @this, long count)
            where T : unmanaged
            => @this.AsRefEnumerable().TakeLast(count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SelectIndexEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                (T, long),
                WithIndex<T>
            >
            WithIndex<T>(this NativeArray<T> @this, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            => new SelectIndexEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                (T, long),
                WithIndex<T>
            >(@this.AsRefEnumerable(), default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            WhereEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TPredicate0
            >
            Where<TPredicate0, T>(this NativeArray<T> @this, TPredicate0 predicate)
            where T : unmanaged
            where TPredicate0 : struct, IRefFunc<T, bool>
            => new WhereEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TPredicate0
            >(@this.AsRefEnumerable(), predicate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            WhereEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DelegateFuncToStructOperatorFunc<T, bool>
            >
            Where<T>(this NativeArray<T> @this, Func<T, bool> predicate)
            where T : unmanaged
            => new WhereEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DelegateFuncToStructOperatorFunc<T, bool>
            >(@this.AsRefEnumerable(), predicate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            WhereIndexEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TPredicate0
            >
            WhereIndex<TPredicate0, T>(this NativeArray<T> @this, TPredicate0 predicate)
            where T : unmanaged
            where TPredicate0 : struct, IWhereIndex<T>
            => new WhereIndexEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TPredicate0
            >(@this.AsRefEnumerable(), predicate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            WhereIndexEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DelegateFuncToWhereIndexStructOperator<T>
            >
            WhereIndex<T>(this NativeArray<T> @this, Func<T, long, bool> predicate)
            where T : unmanaged
            => new WhereIndexEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DelegateFuncToWhereIndexStructOperator<T>
            >(@this.AsRefEnumerable(), predicate);
        #endregion

        #region Concat
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                TEnumerable1,
                TEnumerator1,
                T
            >
            Concat<TEnumerable1, TEnumerator1, T>(this NativeArray<T> @this, in TEnumerable1 second)
            where T : unmanaged
            where TEnumerator1 : struct, IRefEnumerator<T>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T>
            => new ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                TEnumerable1, TEnumerator1,
                T
            >
            (@this.AsRefEnumerable(), second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, T>,
                ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, T>.Enumerator,
                T
            >
            Concat<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, T>(this NativeArray<T> @this, in ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, T> second)
            where T : unmanaged
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T>
            where TEnumerator1 : struct, IRefEnumerator<T>
            where TEnumerable2 : struct, IRefEnumerable<TEnumerator2, T>
            where TEnumerator2 : struct, IRefEnumerator<T>
            => new ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, T>,
                ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, T>.Enumerator,
                T
            >(@this.AsRefEnumerable(), second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T
            >
            Concat<T>(this NativeArray<T> @this, NativeArray<T> second)
            where T : unmanaged
            => new ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T
            >(@this.AsRefEnumerable(), NativeEnumerable.AsRefEnumerable(second));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T
            >
            Concat<T>(this NativeArray<T> @this, in NativeEnumerable<T> second)
            where T : unmanaged
            => new ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T
            >(@this.AsRefEnumerable(), second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                AppendEnumerable<TEnumerable1, TEnumerator1, T>,
                AppendEnumerable<TEnumerable1, TEnumerator1, T>.Enumerator,
                T
            >
            Concat<TEnumerable1, TEnumerator1, T>(this NativeArray<T> @this, in AppendEnumerable<TEnumerable1, TEnumerator1, T> second)
            where T : unmanaged
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T>
            where TEnumerator1 : struct, IRefEnumerator<T>
            => new ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                AppendEnumerable<TEnumerable1, TEnumerator1, T>,
                AppendEnumerable<TEnumerable1, TEnumerator1, T>.Enumerator,
                T
            >(@this.AsRefEnumerable(), second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ArrayEnumerable<T>,
                ArrayEnumerable<T>.Enumerator,
                T
            >
            Concat<T>(this NativeArray<T> @this, in ArrayEnumerable<T> second)
            where T : unmanaged
            => new ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ArrayEnumerable<T>,
                ArrayEnumerable<T>.Enumerator,
                T
            >(@this.AsRefEnumerable(), second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ArrayEnumerable<T>,
                ArrayEnumerable<T>.Enumerator,
                T
            >
            Concat<T>(this NativeArray<T> @this, in T[] second)
            where T : unmanaged
            => new ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ArrayEnumerable<T>,
                ArrayEnumerable<T>.Enumerator,
                T
            >(@this.AsRefEnumerable(), NativeEnumerable.AsRefEnumerable(second));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, T>,
                DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, T>.Enumerator,
                T
            >
            Concat<TEnumerable1, TEnumerator1, T>(this NativeArray<T> @this, in DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, T> second)
            where T : unmanaged
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T>
            where TEnumerator1 : struct, IRefEnumerator<T>
            => new ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, T>,
                DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, T>.Enumerator,
                T
            >(@this.AsRefEnumerable(), second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                DistinctEnumerable<TEnumerable1, TEnumerator1, T, TEqualityComparer0, TGetHashCodeFunc0>,
                DistinctEnumerable<TEnumerable1, TEnumerator1, T, TEqualityComparer0, TGetHashCodeFunc0>.Enumerator,
                T
            >
            Concat<TEnumerable1, TEnumerator1, TEqualityComparer0, TGetHashCodeFunc0, T>(this NativeArray<T> @this, in DistinctEnumerable<TEnumerable1, TEnumerator1, T, TEqualityComparer0, TGetHashCodeFunc0> second)
            where T : unmanaged
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T>
            where TEnumerator1 : struct, IRefEnumerator<T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            => new ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                DistinctEnumerable<TEnumerable1, TEnumerator1, T, TEqualityComparer0, TGetHashCodeFunc0>,
                DistinctEnumerable<TEnumerable1, TEnumerator1, T, TEqualityComparer0, TGetHashCodeFunc0>.Enumerator,
                T
            >(@this.AsRefEnumerable(), second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                Grouping<TKey0, T>,
                Grouping<TKey0, T>.Enumerator,
                T
            >
            Concat<TKey0, T>(this NativeArray<T> @this, in Grouping<TKey0, T> second)
            where T : unmanaged
            where TKey0 : unmanaged
            => new ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                Grouping<TKey0, T>,
                Grouping<TKey0, T>.Enumerator,
                T
            >(@this.AsRefEnumerable(), second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                GroupJoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0, TKey0, TOuterKeySelector0, TInnerKeySelector0, T, T0Selector, TKeyEqualityComparer0>,
                GroupJoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0, TKey0, TOuterKeySelector0, TInnerKeySelector0, T, T0Selector, TKeyEqualityComparer0>.Enumerator,
                T
            >
            Concat<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0, TKey0, TOuterKeySelector0, TInnerKeySelector0, T0Selector, TKeyEqualityComparer0, T>
            (this NativeArray<T> @this, in GroupJoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0, TKey0, TOuterKeySelector0, TInnerKeySelector0, T, T0Selector, TKeyEqualityComparer0>
            second)
            where T : unmanaged
            where TOuterSource0 : unmanaged
            where TInnerSource0 : unmanaged
            where TOuterEnumerator0 : struct, IRefEnumerator<TOuterSource0>
            where TInnerEnumerator0 : struct, IRefEnumerator<TInnerSource0>
            where TOuterEnumerable0 : struct, IRefEnumerable<TOuterEnumerator0, TOuterSource0>
            where TInnerEnumerable0 : struct, IRefEnumerable<TInnerEnumerator0, TInnerSource0>
            where TKey0 : unmanaged
            where TOuterKeySelector0 : struct, IRefFunc<TOuterSource0, TKey0>
            where TInnerKeySelector0 : struct, IRefFunc<TInnerSource0, TKey0>
            where T0Selector : struct,
            IRefFunc<TOuterSource0,
                WhereIndexEnumerable<
                    NativeEnumerable<TInnerSource0>,
                    NativeEnumerable<TInnerSource0>.Enumerator,
                    TInnerSource0,
                    GroupJoinPredicate<TInnerSource0, TKey0, TKeyEqualityComparer0>
                    >,
                T>
            where TKeyEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                GroupJoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0, TKey0, TOuterKeySelector0, TInnerKeySelector0, T, T0Selector, TKeyEqualityComparer0>,
                GroupJoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0, TKey0, TOuterKeySelector0, TInnerKeySelector0, T, T0Selector, TKeyEqualityComparer0>.Enumerator,
                T
            >(@this.AsRefEnumerable(), second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer0>,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer0>.Enumerator,
                T
            >
            Concat<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T0Selector, TEqualityComparer0, T>(this NativeArray<T> @this,
            in JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer0> second)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T0, TKey0>
            where TKeySelector1 : struct, IRefFunc<T1, TKey0>
            where T0Selector : struct, IRefFunc<T0, T1, T>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer0>,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer0>.Enumerator,
                T
            >(@this.AsRefEnumerable(), second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer0>,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer0>.Enumerator,
                T
            >
            Concat<TEnumerable0, TEnumerator0, TComparer0, T>(this NativeArray<T> @this,
            in OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer0> second)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TComparer0 : struct, IRefFunc<T, T, int>
            => new ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer0>,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer0>.Enumerator,
                T
            >(@this.AsRefEnumerable(), second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                PrependEnumerable<TEnumerable0, TEnumerator0, T>,
                PrependEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T
            >
            Concat<TEnumerable0, TEnumerator0, T>(this NativeArray<T> @this,
            in PrependEnumerable<TEnumerable0, TEnumerator0, T> second)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEnumerator0 : struct, IRefEnumerator<T>
            => new ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                PrependEnumerable<TEnumerable0, TEnumerator0, T>,
                PrependEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T
            >(@this.AsRefEnumerable(), second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                RangeRepeatEnumerable<T, TAction1>,
                RangeRepeatEnumerable<T, TAction1>.Enumerator,
                T
            >
            Concat<TAction1, T>(this NativeArray<T> @this,
            in RangeRepeatEnumerable<T, TAction1> second)
            where T : unmanaged
            where TAction1 : struct, IRangeRepeat<T>
            => new ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                RangeRepeatEnumerable<T, TAction1>,
                RangeRepeatEnumerable<T, TAction1>.Enumerator,
                T
            >(@this.AsRefEnumerable(), second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T>,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T
            >
            Concat<TEnumerable0, TEnumerator0, T>(this NativeArray<T> @this,
            in ReverseEnumerable<TEnumerable0, TEnumerator0, T> second)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEnumerator0 : struct, IRefEnumerator<T>
            => new ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T>,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T
            >(@this.AsRefEnumerable(), second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                SelectEnumerable<TEnumerable1, TEnumerator1, TPrev1, T, TAction1>,
                SelectEnumerable<TEnumerable1, TEnumerator1, TPrev1, T, TAction1>.Enumerator,
                T
            >
            Concat<TEnumerable1, TEnumerator1, TPrev1, TAction1, T>(this NativeArray<T> @this,
            in SelectEnumerable<TEnumerable1, TEnumerator1, TPrev1, T, TAction1> second)
            where T : unmanaged
            where TEnumerator1 : struct, IRefEnumerator<TPrev1>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TPrev1>
            where TAction1 : struct, IRefAction<TPrev1, T>
            where TPrev1 : unmanaged
            => new ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectEnumerable<TEnumerable1, TEnumerator1, TPrev1, T, TAction1>,
                    SelectEnumerable<TEnumerable1, TEnumerator1, TPrev1, T, TAction1>.Enumerator,
                    T>
                (@this.AsRefEnumerable(), second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                SelectIndexEnumerable<TEnumerable1, TEnumerator1, TPrev1, T, TAction1>,
                SelectIndexEnumerable<TEnumerable1, TEnumerator1, TPrev1, T, TAction1>.Enumerator,
                T
            >
            Concat<TEnumerable1, TEnumerator1, TPrev1, TAction1, T>(this NativeArray<T> @this,
            in SelectIndexEnumerable<TEnumerable1, TEnumerator1, TPrev1, T, TAction1> second)
            where T : unmanaged
            where TEnumerator1 : struct, IRefEnumerator<TPrev1>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TPrev1>
            where TAction1 : struct, ISelectIndex<TPrev1, T>
            where TPrev1 : unmanaged
            => new ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectIndexEnumerable<TEnumerable1, TEnumerator1, TPrev1, T, TAction1>,
                    SelectIndexEnumerable<TEnumerable1, TEnumerator1, TPrev1, T, TAction1>.Enumerator,
                    T>
                (@this.AsRefEnumerable(), second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T, TResultEnumerable0, TResultEnumerator0, TAction0>,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T, TResultEnumerable0, TResultEnumerator0, TAction0>.Enumerator,
                T
            >
            Concat<TEnumerable0, TEnumerator0, TPrev0, TResultEnumerable0, TResultEnumerator0, TAction0, T>(this NativeArray<T> @this,
            in SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T, TResultEnumerable0, TResultEnumerator0, TAction0> second)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrev0>
            where TEnumerator0 : struct, IRefEnumerator<TPrev0>
            where TPrev0 : unmanaged
            where TResultEnumerator0 : struct, IRefEnumerator<T>
            where TResultEnumerable0 : struct, IRefEnumerable<TResultEnumerator0, T>
            where TAction0 : struct, IRefAction<TPrev0, TResultEnumerable0>
            => new ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T, TResultEnumerable0, TResultEnumerator0, TAction0>,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T, TResultEnumerable0, TResultEnumerator0, TAction0>.Enumerator,
                T
            >(@this.AsRefEnumerable(), second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TSetOperation0>,
                SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TSetOperation0>.Enumerator,
                T
            >
            Concat<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSetOperation0, T>(this NativeArray<T> @this,
            in SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TSetOperation0> second)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T>
            where TEnumerator1 : struct, IRefEnumerator<T>
            where TSetOperation0 : struct, ISetOperation<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>
            => new ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TSetOperation0>,
                SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TSetOperation0>.Enumerator,
                T
            >(@this.AsRefEnumerable(), second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                SkipEnumerable<TEnumerable0, TEnumerator0, T>,
                TEnumerator0,
                T
            >
            Concat<TEnumerable0, TEnumerator0, T>(this NativeArray<T> @this,
            in SkipEnumerable<TEnumerable0, TEnumerator0, T> second)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEnumerator0 : struct, IRefEnumerator<T>
            => new ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                SkipEnumerable<TEnumerable0, TEnumerator0, T>,
                TEnumerator0,
                T
            >
            (@this.AsRefEnumerable(), second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                TakeEnumerable<TEnumerable0, TEnumerator0, T>,
                TakeEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T
            >
            Concat<TEnumerable0, TEnumerator0, T>(this NativeArray<T> @this,
            in TakeEnumerable<TEnumerable0, TEnumerator0, T> second)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEnumerator0 : struct, IRefEnumerator<T>
            => new ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                TakeEnumerable<TEnumerable0, TEnumerator0, T>,
                TakeEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T
            >
            (@this.AsRefEnumerable(), second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                T
            >
            Concat<TEnumerable0, TEnumerator0, TPredicate0, T>(this NativeArray<T> @this,
            in SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0> second)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TPredicate0 : struct, IRefFunc<T, bool>
            => new ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                T
            >
            (@this.AsRefEnumerable(), second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                T
            >
            Concat<TEnumerable0, TEnumerator0, TPredicate0, T>(this NativeArray<T> @this,
            in TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0> second)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TPredicate0 : struct, IRefFunc<T, bool>
            => new ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                T
            >
            (@this.AsRefEnumerable(), second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T>,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T
            >
            Concat<TEnumerable0, TEnumerator0, T>(this NativeArray<T> @this,
            in SkipLastEnumerable<TEnumerable0, TEnumerator0, T> second)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEnumerator0 : struct, IRefEnumerator<T>
            => new ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T>,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T
            >
            (@this.AsRefEnumerable(), second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T>,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T
            >
            Concat<TEnumerable0, TEnumerator0, T>(this NativeArray<T> @this,
            in TakeLastEnumerable<TEnumerable0, TEnumerator0, T> second)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEnumerator0 : struct, IRefEnumerator<T>
            => new ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T>,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T
            >
            (@this.AsRefEnumerable(), second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                T
            >
            Concat<TEnumerable0, TEnumerator0, TPredicate0, T>(this NativeArray<T> @this,
            in WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0> second)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TPredicate0 : struct, IRefFunc<T, bool>
            => new ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                T
            >(@this.AsRefEnumerable(), second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                WhereIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, T, TPredicate0>,
                WhereIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, T, TPredicate0>.Enumerator,
                T
            >
            Concat<TPrevEnumerable0, TPrevEnumerator0, TPredicate0, T>(this NativeArray<T> @this,
            in WhereIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, T, TPredicate0> second)
            where T : unmanaged
            where TPrevEnumerable0 : struct, IRefEnumerable<TPrevEnumerator0, T>
            where TPrevEnumerator0 : struct, IRefEnumerator<T>
            where TPredicate0 : struct, IWhereIndex<T>
            => new ConcatEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, WhereIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, T, TPredicate0>, WhereIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, T, TPredicate0>.Enumerator, T>(@this.AsRefEnumerable(), second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConcatEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>,
                ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>.Enumerator,
                T
            >
            Concat<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TAction0, T>(this NativeArray<T> @this,
            in ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0> second)
            where T : unmanaged
            where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where T1 : unmanaged
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where TAction0 : struct, IRefAction<T0, T1, T>
            => new ConcatEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>, ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>.Enumerator, T>(@this.AsRefEnumerable(), second);
        #endregion

        #region Except
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                TEnumerable0,
                TEnumerator0,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TEnumerable0,
                    TEnumerator0,
                    T,
                    TComparer0
                >
            >
            Except<TEnumerable0, TEnumerator0, TComparer0, T>(this NativeArray<T> @this,
            in TEnumerable0 second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TComparer0 : struct, IRefFunc<T, T, int>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, TEnumerable0, TEnumerator0, T, ExceptOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, TEnumerable0, TEnumerator0, T, TComparer0>>
            (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                AppendEnumerable<TEnumerable0, TEnumerator0, T>,
                AppendEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    AppendEnumerable<TEnumerable0, TEnumerator0, T>,
                    AppendEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Except<TEnumerable0, TEnumerator0, TComparer0, T>(this NativeArray<T> @this,
            in AppendEnumerable<TEnumerable0, TEnumerator0, T> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TComparer0 : struct, IRefFunc<T, T, int>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, AppendEnumerable<TEnumerable0, TEnumerator0, T>, AppendEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator, T, ExceptOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, AppendEnumerable<TEnumerable0, TEnumerator0, T>, AppendEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator, T, TComparer0>>
            (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Except<TComparer0, T>(this NativeArray<T> @this,
            in NativeEnumerable<T> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, ExceptOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TComparer0>>
                (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Except<TComparer0, T>(this NativeArray<T> @this,
            NativeArray<T> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, ExceptOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TComparer0>>
                (@this.AsRefEnumerable(), NativeEnumerable.AsRefEnumerable(second), comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ArrayEnumerable<T>,
                ArrayEnumerable<T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ArrayEnumerable<T>,
                    ArrayEnumerable<T>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Except<TComparer0, T>(this NativeArray<T> @this,
            in ArrayEnumerable<T> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, ArrayEnumerable<T>, ArrayEnumerable<T>.Enumerator, T, ExceptOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, ArrayEnumerable<T>, ArrayEnumerable<T>.Enumerator, T, TComparer0>>
                (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ArrayEnumerable<T>,
                ArrayEnumerable<T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ArrayEnumerable<T>,
                    ArrayEnumerable<T>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Except<TComparer0, T>(this NativeArray<T> @this,
            T[] second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, ArrayEnumerable<T>, ArrayEnumerable<T>.Enumerator, T, ExceptOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, ArrayEnumerable<T>, ArrayEnumerable<T>.Enumerator, T, TComparer0>>
            (@this.AsRefEnumerable(), NativeEnumerable.AsRefEnumerable(second), comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Except<TComparer0, TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>(this NativeArray<T> @this,
            in ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEnumerator1 : struct, IRefEnumerator<T>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>.Enumerator, T, ExceptOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>.Enumerator, T, TComparer0>>
                (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Except<TComparer0, TEnumerable0, TEnumerator0, T>(this NativeArray<T> @this,
            in DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>, DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator, T, ExceptOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>, DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator, T, TComparer0>>
                (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0>,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0>,
                    DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Except<TComparer0, TEnumerable0, TEnumerator0, TEqualityComparer0, TGetHashCodeFunc0, T>(this NativeArray<T> @this,
            in DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0>, DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0>.Enumerator, T, ExceptOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0>, DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0>.Enumerator, T, TComparer0>>
                (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                Grouping<TKey0, T>,
                Grouping<TKey0, T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    Grouping<TKey0, T>,
                    Grouping<TKey0, T>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Except<TKey0, TComparer0, T>(this NativeArray<T> @this,
            in Grouping<TKey0, T> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TKey0 : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                Grouping<TKey0, T>,
                Grouping<TKey0, T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    Grouping<TKey0, T>,
                    Grouping<TKey0, T>.Enumerator,
                    T,
                    TComparer0
                >
            >
            (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                    GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Except<TComparer0, TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TEqualityComparer0, TGetHashCodeFunc0, TKey0, TKeySelector0, TKeySelector1, T0Selector, TEqualityComparer1, T>(this NativeArray<T> @this,
            in GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>
            second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where T1 : unmanaged
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T0, TKey0>
            where TKeySelector1 : struct, IRefFunc<T1, TKey0>
            where T0Selector : struct,
            IRefFunc<T0,
                WhereIndexEnumerable<
                    NativeEnumerable<T1>,
                    NativeEnumerable<T1>.Enumerator,
                    T1,
                    GroupJoinPredicate<T1, TKey0, TEqualityComparer1>
                    >,
                T>
            where TEqualityComparer1 : struct, IRefFunc<TKey0, TKey0, bool>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                    GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                    T,
                    TComparer0
                >
            >(@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                    JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Except<TComparer0, TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TEqualityComparer0, TGetHashCodeFunc0, TKey0, TKeySelector0, TKeySelector1, T0Selector, TEqualityComparer1, T>(this NativeArray<T> @this,
            in JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>
            second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where T1 : unmanaged
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T0, TKey0>
            where TKeySelector1 : struct, IRefFunc<T1, TKey0>
            where T0Selector : struct, IRefFunc<T0, T1, T>
            where TEqualityComparer1 : struct, IRefFunc<TKey0, TKey0, bool>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                    JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                    T,
                    TComparer0
                >
            >(@this.AsRefEnumerable(), second, new ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                    JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                    T,
                    TComparer0
                >(comparer), allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1>,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1>,
                    OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Except<TComparer0, TEnumerable0, TEnumerator0, TComparer1, T>(this NativeArray<T> @this,
            in OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TComparer1 : struct, IRefFunc<T, T, int>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1>, OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1>.Enumerator, T, ExceptOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1>, OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1>.Enumerator, T, TComparer0>>
                (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                PrependEnumerable<TEnumerable0, TEnumerator0, T>,
                PrependEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    PrependEnumerable<TEnumerable0, TEnumerator0, T>,
                    PrependEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Except<TComparer0, TEnumerable0, TEnumerator0, T>(this NativeArray<T> @this,
            in PrependEnumerable<TEnumerable0, TEnumerator0, T> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, PrependEnumerable<TEnumerable0, TEnumerator0, T>, PrependEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator, T, ExceptOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, PrependEnumerable<TEnumerable0, TEnumerator0, T>, PrependEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator, T, TComparer0>>
                (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                RangeRepeatEnumerable<T, TAction0>,
                RangeRepeatEnumerable<T, TAction0>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    RangeRepeatEnumerable<T, TAction0>,
                    RangeRepeatEnumerable<T, TAction0>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Except<TComparer0, TAction0, T>(this NativeArray<T> @this,
            in RangeRepeatEnumerable<T, TAction0> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where TAction0 : struct, IRangeRepeat<T>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, RangeRepeatEnumerable<T, TAction0>, RangeRepeatEnumerable<T, TAction0>.Enumerator, T, ExceptOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, RangeRepeatEnumerable<T, TAction0>, RangeRepeatEnumerable<T, TAction0>.Enumerator, T, TComparer0>>
                (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T>,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ReverseEnumerable<TEnumerable0, TEnumerator0, T>,
                    ReverseEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Except<TComparer0, TEnumerable0, TEnumerator0, T>(this NativeArray<T> @this,
            in ReverseEnumerable<TEnumerable0, TEnumerator0, T> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, ReverseEnumerable<TEnumerable0, TEnumerator0, T>, ReverseEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator, T, ExceptOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, ReverseEnumerable<TEnumerable0, TEnumerator0, T>, ReverseEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator, T, TComparer0>>
                (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>,
                SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>,
                    SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Except<TComparer0, TEnumerable0, TEnumerator0, T0, TAction0, T>(this NativeArray<T> @this,
            in SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TAction0 : struct, IRefAction<T0, T>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>, SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator, T, ExceptOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>, SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator, T, TComparer0>>
                (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>,
                    SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Except<TComparer0, TEnumerable0, TEnumerator0, T0, TAction0, T>(this NativeArray<T> @this,
            in SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TAction0 : struct, ISelectIndex<T0, T>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>, SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator, T, ExceptOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>, SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator, T, TComparer0>>
                (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                SkipEnumerable<TEnumerable0, TEnumerator0, T>,
                TEnumerator0,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SkipEnumerable<TEnumerable0, TEnumerator0, T>,
                    TEnumerator0,
                    T,
                    TComparer0
                >
            >
            Except<TComparer0, TEnumerable0, TEnumerator0, T>(this NativeArray<T> @this,
            in SkipEnumerable<TEnumerable0, TEnumerator0, T> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                SkipEnumerable<TEnumerable0, TEnumerator0, T>,
                TEnumerator0,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SkipEnumerable<TEnumerable0, TEnumerator0, T>,
                    TEnumerator0,
                    T,
                    TComparer0
                >
            >
            (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                TakeEnumerable<TEnumerable0, TEnumerator0, T>,
                TakeEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TakeEnumerable<TEnumerable0, TEnumerator0, T>,
                    TakeEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Except<TComparer0, TEnumerable0, TEnumerator0, T>(this NativeArray<T> @this,
            in TakeEnumerable<TEnumerable0, TEnumerator0, T> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                TakeEnumerable<TEnumerable0, TEnumerator0, T>,
                TakeEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TakeEnumerable<TEnumerable0, TEnumerator0, T>,
                    TakeEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T,
                    TComparer0
                >
            >
            (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Except<TComparer0, TEnumerable0, TEnumerator0, TPredicate0, T>(this NativeArray<T> @this,
            in SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TPredicate0 : struct, IRefFunc<T, bool>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T,
                    TComparer0
                >
            >
            (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Except<TComparer0, TEnumerable0, TEnumerator0, TPredicate0, T>(this NativeArray<T> @this,
            in TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TPredicate0 : struct, IRefFunc<T, bool>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T,
                    TComparer0
                >
            >
            (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T>,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SkipLastEnumerable<TEnumerable0, TEnumerator0, T>,
                    SkipLastEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Except<TComparer0, TEnumerable0, TEnumerator0, T>(this NativeArray<T> @this,
            in SkipLastEnumerable<TEnumerable0, TEnumerator0, T> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T>,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SkipLastEnumerable<TEnumerable0, TEnumerator0, T>,
                    SkipLastEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T,
                    TComparer0
                >
            >
            (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T>,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TakeLastEnumerable<TEnumerable0, TEnumerator0, T>,
                    TakeLastEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Except<TComparer0, TEnumerable0, TEnumerator0, T>(this NativeArray<T> @this,
            in TakeLastEnumerable<TEnumerable0, TEnumerator0, T> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T>,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TakeLastEnumerable<TEnumerable0, TEnumerator0, T>,
                    TakeLastEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T,
                    TComparer0
                >
            >
            (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Except<TComparer0, TEnumerable0, TEnumerator0, TPredicate0, T>(this NativeArray<T> @this,
            in WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TPredicate0 : struct, IRefFunc<T, bool>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>, WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator, T, ExceptOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>, WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator, T, TComparer0>>
            (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Except<TComparer0, TEnumerable0, TEnumerator0, TPredicate0, T>(this NativeArray<T> @this,
            in WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TPredicate0 : struct, IWhereIndex<T>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>, WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator, T, ExceptOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>, WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator, T, TComparer0>>
                (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>,
                ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>,
                    ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Except<TComparer0, TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TAction0, T>(this NativeArray<T> @this,
            in ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where T1 : unmanaged
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where TAction0 : struct, IRefAction<T0, T1, T>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>, ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>.Enumerator, T, ExceptOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>, ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>.Enumerator, T, TComparer0>>
                (@this.AsRefEnumerable(), second, comparer, allocator);
        #endregion

        #region GroupJoin
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            GroupJoinEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T,
                    TEnumerable0,
                    TEnumerator0,
                    T0,
                    TKey0,
                    TKeySelector0,
                    TKeySelector1,
                    T1,
                    T0Selector,
                    TEqualityComparer0
                >
            GroupJoin<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0, T>
            (this NativeArray<T> @this,
            in TEnumerable0 inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, WhereIndexEnumerable<NativeEnumerable<T0>, NativeEnumerable<T0>.Enumerator, T0, GroupJoinPredicate<T0, TKey0, TEqualityComparer0>>, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new GroupJoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            GroupJoinEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T,
                    AppendEnumerable<TEnumerable0, TEnumerator0, T0>,
                    AppendEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                    T0,
                    TKey0,
                    TKeySelector0,
                    TKeySelector1,
                    T1,
                    T0Selector,
                    TEqualityComparer0
                >
            GroupJoin<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0, T>
            (this NativeArray<T> @this,
            in AppendEnumerable<TEnumerable0, TEnumerator0, T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, WhereIndexEnumerable<NativeEnumerable<T0>, NativeEnumerable<T0>.Enumerator, T0, GroupJoinPredicate<T0, TKey0, TEqualityComparer0>>, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new GroupJoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, AppendEnumerable<TEnumerable0, TEnumerator0, T0>, AppendEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            GroupJoinEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T,
                    NativeEnumerable<T0>,
                    NativeEnumerable<T0>.Enumerator,
                    T0,
                    TKey0,
                    TKeySelector0,
                    TKeySelector1,
                    T1,
                    T0Selector,
                    TEqualityComparer0
                >
            GroupJoin<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0, T
            >
            (this NativeArray<T> @this,
            in NativeEnumerable<T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, WhereIndexEnumerable<NativeEnumerable<T0>, NativeEnumerable<T0>.Enumerator, T0, GroupJoinPredicate<T0, TKey0, TEqualityComparer0>>, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new GroupJoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, NativeEnumerable<T0>, NativeEnumerable<T0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            GroupJoinEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T,
                    NativeEnumerable<T0>,
                    NativeEnumerable<T0>.Enumerator,
                    T0,
                    TKey0,
                    TKeySelector0,
                    TKeySelector1,
                    T1,
                    T0Selector,
                    TEqualityComparer0
                >
            GroupJoin<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0, T
            >
            (this NativeArray<T> @this,
            NativeArray<T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, WhereIndexEnumerable<NativeEnumerable<T0>, NativeEnumerable<T0>.Enumerator, T0, GroupJoinPredicate<T0, TKey0, TEqualityComparer0>>, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new GroupJoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, NativeEnumerable<T0>, NativeEnumerable<T0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner.AsRefEnumerable(), outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            GroupJoinEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T,
                    ArrayEnumerable<T0>,
                    ArrayEnumerable<T0>.Enumerator,
                    T0,
                    TKey0,
                    TKeySelector0,
                    TKeySelector1,
                    T1,
                    T0Selector,
                    TEqualityComparer0
                >
            GroupJoin<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0, T
            >
            (this NativeArray<T> @this,
            in ArrayEnumerable<T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, WhereIndexEnumerable<NativeEnumerable<T0>, NativeEnumerable<T0>.Enumerator, T0, GroupJoinPredicate<T0, TKey0, TEqualityComparer0>>, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new GroupJoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, ArrayEnumerable<T0>, ArrayEnumerable<T0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            GroupJoinEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T,
                    ArrayEnumerable<T0>,
                    ArrayEnumerable<T0>.Enumerator,
                    T0,
                    TKey0,
                    TKeySelector0,
                    TKeySelector1,
                    T1,
                    T0Selector,
                    TEqualityComparer0
                >
            GroupJoin<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0, T
            >
            (this NativeArray<T> @this,
            T0[] inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, WhereIndexEnumerable<NativeEnumerable<T0>, NativeEnumerable<T0>.Enumerator, T0, GroupJoinPredicate<T0, TKey0, TEqualityComparer0>>, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new GroupJoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, ArrayEnumerable<T0>, ArrayEnumerable<T0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner.AsRefEnumerable(), outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            GroupJoinEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>.Enumerator,
                    T0,
                    TKey0,
                    TKeySelector0,
                    TKeySelector1,
                    T1,
                    T0Selector,
                    TEqualityComparer0
                >
            GroupJoin<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0
                , TEnumerable1, TEnumerator1, T
            >
            (this NativeArray<T> @this,
            in ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, WhereIndexEnumerable<NativeEnumerable<T0>, NativeEnumerable<T0>.Enumerator, T0, GroupJoinPredicate<T0, TKey0, TEqualityComparer0>>, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            where TEnumerator1 : struct, IRefEnumerator<T0>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T0>
            => new GroupJoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            GroupJoinEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                    T0,
                    TKey0,
                    TKeySelector0,
                    TKeySelector1,
                    T1,
                    T0Selector,
                    TEqualityComparer0
                >
            GroupJoin<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0, T
            >
            (this NativeArray<T> @this,
            in DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, WhereIndexEnumerable<NativeEnumerable<T0>, NativeEnumerable<T0>.Enumerator, T0, GroupJoinPredicate<T0, TKey0, TEqualityComparer0>>, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new GroupJoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>, DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            GroupJoinEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T,
                    DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer1, TGetHashCodeFunc0>,
                    DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer1, TGetHashCodeFunc0>.Enumerator,
                    T0,
                    TKey0,
                    TKeySelector0,
                    TKeySelector1,
                    T1,
                    T0Selector,
                    TEqualityComparer0
                >
            GroupJoin<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0
                , TEqualityComparer1, TGetHashCodeFunc0, T
            >
            (this NativeArray<T> @this,
            in DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer1, TGetHashCodeFunc0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, WhereIndexEnumerable<NativeEnumerable<T0>, NativeEnumerable<T0>.Enumerator, T0, GroupJoinPredicate<T0, TKey0, TEqualityComparer0>>, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            where TEqualityComparer1 : struct, IRefFunc<T0, T0, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T0, int>
            => new GroupJoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer1, TGetHashCodeFunc0>, DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer1, TGetHashCodeFunc0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            GroupJoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                Grouping<TKey1, T0>,
                Grouping<TKey1, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            GroupJoin<
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                TKey1,
                T1,
                T0Selector,
                TEqualityComparer0,
                T
            >
            (this NativeArray<T> @this,
            in Grouping<TKey1, T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TKey1 : unmanaged
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, WhereIndexEnumerable<NativeEnumerable<T0>, NativeEnumerable<T0>.Enumerator, T0, GroupJoinPredicate<T0, TKey0, TEqualityComparer0>>, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new GroupJoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                Grouping<TKey1, T0>,
                Grouping<TKey1, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            GroupJoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SetOperationEnumerable<
                    TEnumerable0,
                    TEnumerator0,
                    TEnumerable1,
                    TEnumerator1,
                    T0,
                    TSetOperation0
                >,
                SetOperationEnumerable<
                    TEnumerable0,
                    TEnumerator0,
                    TEnumerable1,
                    TEnumerator1,
                    T0,
                    TSetOperation0
                >.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            GroupJoin<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0
                , TEnumerable1, TEnumerator1, TSetOperation0, T
            >
            (this NativeArray<T> @this,
            in SetOperationEnumerable<
                    TEnumerable0,
                    TEnumerator0,
                    TEnumerable1,
                    TEnumerator1,
                    T0,
                    TSetOperation0
                > inner,
            in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, WhereIndexEnumerable<NativeEnumerable<T0>, NativeEnumerable<T0>.Enumerator, T0, GroupJoinPredicate<T0, TKey0, TEqualityComparer0>>, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            where TEnumerator1 : struct, IRefEnumerator<T0>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T0>
            where TSetOperation0 : struct, ISetOperation<TEnumerable0,
                    TEnumerator0,
                    TEnumerable1,
                    TEnumerator1,
                    T0>
            => new GroupJoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0, TSetOperation0>, SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0, TSetOperation0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            GroupJoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                GroupJoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>,
                GroupJoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>.Enumerator,
                T0,
                TKey1,
                TOuterKeySelector1,
                TInnerKeySelector1,
                T1,
                T1Selector,
                TKeyEqualityComparer1
            >
            GroupJoin<
                TOuterEnumerable0,
                TOuterEnumerator0,
                TOuterSource0,
                TInnerEnumerable0,
                TInnerEnumerator0,
                TInnerSource0,
                TKey0,
                TOuterKeySelector0,
                TInnerKeySelector0,
                T0,
                T0Selector,
                TKeyEqualityComparer0,
                TKey1,
                TOuterKeySelector1,
                TInnerKeySelector1,
                T1,
                T1Selector,
                TKeyEqualityComparer1,
                T
            >
            (
                this NativeArray<T> @this,
                in GroupJoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>
                inner,
                in TOuterKeySelector1 outerKeySelector, in TInnerKeySelector1 innerKeySelector,
                in T1Selector resultSelector, in TKeyEqualityComparer1 comparer, Allocator allocator = Allocator.Temp
            )
            where T : unmanaged
            where TOuterSource0 : unmanaged
            where TInnerSource0 : unmanaged
            where TOuterEnumerator0 : struct, IRefEnumerator<TOuterSource0>
            where TInnerEnumerator0 : struct, IRefEnumerator<TInnerSource0>
            where TOuterEnumerable0 : struct, IRefEnumerable<TOuterEnumerator0, TOuterSource0>
            where TInnerEnumerable0 : struct, IRefEnumerable<TInnerEnumerator0, TInnerSource0>
            where T0 : unmanaged
            where TKey0 : unmanaged
            where TKey1 : unmanaged
            where TOuterKeySelector0 : struct, IRefFunc<TOuterSource0, TKey0>
            where TInnerKeySelector0 : struct, IRefFunc<TInnerSource0, TKey0>
            where TKeyEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            where T0Selector : struct,
            IRefFunc<TOuterSource0,
                WhereIndexEnumerable<
                    NativeEnumerable<TInnerSource0>,
                    NativeEnumerable<TInnerSource0>.Enumerator,
                    TInnerSource0,
                    GroupJoinPredicate<TInnerSource0, TKey0, TKeyEqualityComparer0>
                    >,
                T0>
            where T1 : unmanaged
            where TOuterKeySelector1 : struct, IRefFunc<T, TKey1>
            where TInnerKeySelector1 : struct, IRefFunc<T0, TKey1>
            where TKeyEqualityComparer1 : struct, IRefFunc<TKey1, TKey1, bool>
            where T1Selector : struct,
            IRefFunc<T,
                WhereIndexEnumerable<
                    NativeEnumerable<T0>,
                    NativeEnumerable<T0>.Enumerator,
                    T0,
                    GroupJoinPredicate<T0, TKey1, TKeyEqualityComparer1>
                    >,
                T1>
            =>
            new GroupJoinEnumerable
            <
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                GroupJoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>,
                GroupJoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>.Enumerator,
                T0,
                TKey1,
                TOuterKeySelector1,
                TInnerKeySelector1,
                T1,
                T1Selector,
                TKeyEqualityComparer1
            >(@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            GroupJoinEnumerable
            <
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                JoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>,
                JoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>.Enumerator,
                T0,
                TKey1,
                TOuterKeySelector1,
                TInnerKeySelector1,
                T1,
                T1Selector,
                TKeyEqualityComparer1
            >
            GroupJoin<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0, TKey1,
                TOuterKeySelector1,
                TInnerKeySelector1,
                T1,
                T1Selector,
                TKeyEqualityComparer1,
                T
            >
            (
                this NativeArray<T> @this,
                in JoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>
                inner,
                in TOuterKeySelector1 outerKeySelector, in TInnerKeySelector1 innerKeySelector,
                in T1Selector resultSelector, in TKeyEqualityComparer1 comparer, Allocator allocator = Allocator.Temp
            )
            where T : unmanaged
            where TOuterSource0 : unmanaged
            where TInnerSource0 : unmanaged
            where TOuterEnumerator0 : struct, IRefEnumerator<TOuterSource0>
            where TInnerEnumerator0 : struct, IRefEnumerator<TInnerSource0>
            where TOuterEnumerable0 : struct, IRefEnumerable<TOuterEnumerator0, TOuterSource0>
            where TInnerEnumerable0 : struct, IRefEnumerable<TInnerEnumerator0, TInnerSource0>
            where T0 : unmanaged
            where TKey0 : unmanaged
            where TKey1 : unmanaged
            where TOuterKeySelector0 : struct, IRefFunc<TOuterSource0, TKey0>
            where TInnerKeySelector0 : struct, IRefFunc<TInnerSource0, TKey0>
            where TKeyEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            where T0Selector : struct, IRefFunc<TOuterSource0, TInnerSource0, T0>
            where T1 : unmanaged
            where TOuterKeySelector1 : struct, IRefFunc<T, TKey1>
            where TInnerKeySelector1 : struct, IRefFunc<T0, TKey1>
            where TKeyEqualityComparer1 : struct, IRefFunc<TKey1, TKey1, bool>
            where T1Selector : struct,
            IRefFunc<T,
                WhereIndexEnumerable<
                    NativeEnumerable<T0>,
                    NativeEnumerable<T0>.Enumerator,
                    T0,
                    GroupJoinPredicate<T0, TKey1, TKeyEqualityComparer1>
                    >,
                T1>
            =>
            new GroupJoinEnumerable
            <
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                JoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>,
                JoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>.Enumerator,
                T0,
                TKey1,
                TOuterKeySelector1,
                TInnerKeySelector1,
                T1,
                T1Selector,
                TKeyEqualityComparer1
            >
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            GroupJoinEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T,
                    OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>,
                    OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>.Enumerator,
                    T0,
                    TKey0,
                    TKeySelector0,
                    TKeySelector1,
                    T1,
                    T0Selector,
                    TEqualityComparer0
                >
            GroupJoin<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0
                , TComparer0, T
            >
            (this NativeArray<T> @this,
            in OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, WhereIndexEnumerable<NativeEnumerable<T0>, NativeEnumerable<T0>.Enumerator, T0, GroupJoinPredicate<T0, TKey0, TEqualityComparer0>>, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            where TComparer0 : struct, IRefFunc<T0, T0, int>
            => new GroupJoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>, OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            GroupJoinEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T,
                    PrependEnumerable<TEnumerable0, TEnumerator0, T0>,
                    PrependEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                    T0,
                    TKey0,
                    TKeySelector0,
                    TKeySelector1,
                    T1,
                    T0Selector,
                    TEqualityComparer0
                >
            GroupJoin<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0, T
            >
            (this NativeArray<T> @this,
            in PrependEnumerable<TEnumerable0, TEnumerator0, T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, WhereIndexEnumerable<NativeEnumerable<T0>, NativeEnumerable<T0>.Enumerator, T0, GroupJoinPredicate<T0, TKey0, TEqualityComparer0>>, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new GroupJoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, PrependEnumerable<TEnumerable0, TEnumerator0, T0>, PrependEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            GroupJoinEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T,
                    RangeRepeatEnumerable<T0, TAction0>,
                    RangeRepeatEnumerable<T0, TAction0>.Enumerator,
                    T0,
                    TKey0,
                    TKeySelector0,
                    TKeySelector1,
                    T1,
                    T0Selector,
                    TEqualityComparer0
                >
            GroupJoin<
                T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0
                , TAction0, T
            >
            (this NativeArray<T> @this,
            in RangeRepeatEnumerable<T0, TAction0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, WhereIndexEnumerable<NativeEnumerable<T0>, NativeEnumerable<T0>.Enumerator, T0, GroupJoinPredicate<T0, TKey0, TEqualityComparer0>>, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            where TAction0 : struct, IRangeRepeat<T0>
            => new GroupJoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, RangeRepeatEnumerable<T0, TAction0>, RangeRepeatEnumerable<T0, TAction0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            GroupJoinEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T,
                    ReverseEnumerable<TEnumerable0, TEnumerator0, T0>,
                    ReverseEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                    T0,
                    TKey0,
                    TKeySelector0,
                    TKeySelector1,
                    T1,
                    T0Selector,
                    TEqualityComparer0
                >
            GroupJoin<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0, T
            >
            (this NativeArray<T> @this,
            in ReverseEnumerable<TEnumerable0, TEnumerator0, T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, WhereIndexEnumerable<NativeEnumerable<T0>, NativeEnumerable<T0>.Enumerator, T0, GroupJoinPredicate<T0, TKey0, TEqualityComparer0>>, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new GroupJoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, ReverseEnumerable<TEnumerable0, TEnumerator0, T0>, ReverseEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            GroupJoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            GroupJoin<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0
                , TPrev0, TAction0, T
            >
            (this NativeArray<T> @this,
            in SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TPrev0 : unmanaged
            where TAction0 : struct, IRefAction<TPrev0, T0>
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<TPrev0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrev0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, WhereIndexEnumerable<NativeEnumerable<T0>, NativeEnumerable<T0>.Enumerator, T0, GroupJoinPredicate<T0, TKey0, TEqualityComparer0>>, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new GroupJoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>, SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            GroupJoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            GroupJoin<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0
                , TPrev0, TAction0, T
            >
            (this NativeArray<T> @this,
            in SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TPrev0 : unmanaged
            where TAction0 : struct, ISelectIndex<TPrev0, T0>
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<TPrev0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrev0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, WhereIndexEnumerable<NativeEnumerable<T0>, NativeEnumerable<T0>.Enumerator, T0, GroupJoinPredicate<T0, TKey0, TEqualityComparer0>>, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new GroupJoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>, SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            GroupJoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction0>,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            GroupJoin<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0
                , TEnumerable1, TEnumerator1, TPrev0, TAction0, T
            >
            (this NativeArray<T> @this,
            in SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TPrev0 : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<TPrev0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrev0>
            where TAction0 : struct, IRefAction<TPrev0, TEnumerable1>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, WhereIndexEnumerable<NativeEnumerable<T0>, NativeEnumerable<T0>.Enumerator, T0, GroupJoinPredicate<T0, TKey0, TEqualityComparer0>>, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            where TEnumerator1 : struct, IRefEnumerator<T0>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T0>
            => new GroupJoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction0>, SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            GroupJoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipEnumerable<TEnumerable0, TEnumerator0, T0>,
                TEnumerator0,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            GroupJoin<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0, T
            >
            (this NativeArray<T> @this,
            in SkipEnumerable<TEnumerable0, TEnumerator0, T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, WhereIndexEnumerable<NativeEnumerable<T0>, NativeEnumerable<T0>.Enumerator, T0, GroupJoinPredicate<T0, TKey0, TEqualityComparer0>>, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new GroupJoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipEnumerable<TEnumerable0, TEnumerator0, T0>,
                TEnumerator0,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            GroupJoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeEnumerable<TEnumerable0, TEnumerator0, T0>,
                TakeEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            GroupJoin<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0, T
            >
            (this NativeArray<T> @this,
            in TakeEnumerable<TEnumerable0, TEnumerator0, T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, WhereIndexEnumerable<NativeEnumerable<T0>, NativeEnumerable<T0>.Enumerator, T0, GroupJoinPredicate<T0, TKey0, TEqualityComparer0>>, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new GroupJoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeEnumerable<TEnumerable0, TEnumerator0, T0>,
                TakeEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            GroupJoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            GroupJoin<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0
                , TPredicate0, T
            >
            (this NativeArray<T> @this,
            in SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, WhereIndexEnumerable<NativeEnumerable<T0>, NativeEnumerable<T0>.Enumerator, T0, GroupJoinPredicate<T0, TKey0, TEqualityComparer0>>, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            where TPredicate0 : struct, IRefFunc<T0, bool>
            => new GroupJoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            GroupJoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            GroupJoin<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0
                , TPredicate0, T
            >
            (this NativeArray<T> @this,
            in TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, WhereIndexEnumerable<NativeEnumerable<T0>, NativeEnumerable<T0>.Enumerator, T0, GroupJoinPredicate<T0, TKey0, TEqualityComparer0>>, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            where TPredicate0 : struct, IRefFunc<T0, bool>
            => new GroupJoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            GroupJoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            GroupJoin<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0, T
            >
            (this NativeArray<T> @this,
            in SkipLastEnumerable<TEnumerable0, TEnumerator0, T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, WhereIndexEnumerable<NativeEnumerable<T0>, NativeEnumerable<T0>.Enumerator, T0, GroupJoinPredicate<T0, TKey0, TEqualityComparer0>>, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new GroupJoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            GroupJoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            GroupJoin<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0, T
            >
            (this NativeArray<T> @this,
            in TakeLastEnumerable<TEnumerable0, TEnumerator0, T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, WhereIndexEnumerable<NativeEnumerable<T0>, NativeEnumerable<T0>.Enumerator, T0, GroupJoinPredicate<T0, TKey0, TEqualityComparer0>>, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new GroupJoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            GroupJoinEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T,
                    WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                    WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                    T0,
                    TKey0,
                    TKeySelector0,
                    TKeySelector1,
                    T1,
                    T0Selector,
                    TEqualityComparer0
                >
            GroupJoin<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0
                , TPredicate0, T
            >
            (this NativeArray<T> @this,
            in WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, WhereIndexEnumerable<NativeEnumerable<T0>, NativeEnumerable<T0>.Enumerator, T0, GroupJoinPredicate<T0, TKey0, TEqualityComparer0>>, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            where TPredicate0 : struct, IRefFunc<T0, bool>
            => new GroupJoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>, WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            GroupJoinEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T,
                    WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                    WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                    T0,
                    TKey0,
                    TKeySelector0,
                    TKeySelector1,
                    T1,
                    T0Selector,
                    TEqualityComparer0
                >
            GroupJoin<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0
                , TPredicate0, T
            >
            (this NativeArray<T> @this,
            in WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, WhereIndexEnumerable<NativeEnumerable<T0>, NativeEnumerable<T0>.Enumerator, T0, GroupJoinPredicate<T0, TKey0, TEqualityComparer0>>, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            where TPredicate0 : struct, IWhereIndex<T0>
            => new GroupJoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>, WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            GroupJoinEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T,
                    ZipEnumerable<TEnumerable0, TEnumerator0, T2, TEnumerable1, TEnumerator1, T3, T0, TAction0>,
                    ZipEnumerable<TEnumerable0, TEnumerator0, T2, TEnumerable1, TEnumerator1, T3, T0, TAction0>.Enumerator,
                    T0,
                    TKey0,
                    TKeySelector0,
                    TKeySelector1,
                    T1,
                    T0Selector,
                    TEqualityComparer0
                >
            GroupJoin<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0
                , TEnumerable1, TEnumerator1, T2, T3, TAction0, T
            >
            (this NativeArray<T> @this,
            in ZipEnumerable<TEnumerable0, TEnumerator0, T2, TEnumerable1, TEnumerator1, T3, T0, TAction0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where TAction0 : struct, IRefAction<T2, T3, T0>
            where TEnumerator0 : struct, IRefEnumerator<T2>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T2>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, WhereIndexEnumerable<NativeEnumerable<T0>, NativeEnumerable<T0>.Enumerator, T0, GroupJoinPredicate<T0, TKey0, TEqualityComparer0>>, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            where TEnumerator1 : struct, IRefEnumerator<T3>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T3>
            => new GroupJoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, ZipEnumerable<TEnumerable0, TEnumerator0, T2, TEnumerable1, TEnumerator1, T3, T0, TAction0>, ZipEnumerable<TEnumerable0, TEnumerator0, T2, TEnumerable1, TEnumerator1, T3, T0, TAction0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);
        #endregion

        #region Intersect
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                TEnumerable0,
                TEnumerator0,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TEnumerable0,
                    TEnumerator0,
                    T,
                    TComparer0
                >
            >
            Intersect<TEnumerable0, TEnumerator0, TComparer0, T>
            (this NativeArray<T> @this,
            in TEnumerable0 second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TComparer0 : struct, IRefFunc<T, T, int>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, TEnumerable0, TEnumerator0, T, IntersectOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, TEnumerable0, TEnumerator0, T, TComparer0>>
            (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                AppendEnumerable<TEnumerable0, TEnumerator0, T>,
                AppendEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    AppendEnumerable<TEnumerable0, TEnumerator0, T>,
                    AppendEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Intersect<TEnumerable0, TEnumerator0, TComparer0, T>
            (this NativeArray<T> @this,
            in AppendEnumerable<TEnumerable0, TEnumerator0, T> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TComparer0 : struct, IRefFunc<T, T, int>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, AppendEnumerable<TEnumerable0, TEnumerator0, T>, AppendEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator, T, IntersectOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, AppendEnumerable<TEnumerable0, TEnumerator0, T>, AppendEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator, T, TComparer0>>
            (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Intersect<TComparer0, T>
            (this NativeArray<T> @this,
            in NativeEnumerable<T> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, IntersectOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TComparer0>>
                (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Intersect<TComparer0, T>
            (this NativeArray<T> @this,
            NativeArray<T> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, IntersectOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TComparer0>>
                (@this.AsRefEnumerable(), NativeEnumerable.AsRefEnumerable(second), comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ArrayEnumerable<T>,
                ArrayEnumerable<T>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ArrayEnumerable<T>,
                    ArrayEnumerable<T>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Intersect<TComparer0, T>
            (this NativeArray<T> @this,
            in ArrayEnumerable<T> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, ArrayEnumerable<T>, ArrayEnumerable<T>.Enumerator, T, IntersectOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, ArrayEnumerable<T>, ArrayEnumerable<T>.Enumerator, T, TComparer0>>
                (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ArrayEnumerable<T>,
                ArrayEnumerable<T>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ArrayEnumerable<T>,
                    ArrayEnumerable<T>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Intersect<TComparer0, T>
            (this NativeArray<T> @this,
            T[] second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, ArrayEnumerable<T>, ArrayEnumerable<T>.Enumerator, T, IntersectOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, ArrayEnumerable<T>, ArrayEnumerable<T>.Enumerator, T, TComparer0>>
                (@this.AsRefEnumerable(), NativeEnumerable.AsRefEnumerable(second), comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Intersect<TComparer0, TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>
            (this NativeArray<T> @this,
            in ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEnumerator1 : struct, IRefEnumerator<T>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>.Enumerator, T, IntersectOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>.Enumerator, T, TComparer0>>
                (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Intersect<TComparer0, TEnumerable0, TEnumerator0, T>
            (this NativeArray<T> @this,
            in DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>, DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator, T, IntersectOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>, DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator, T, TComparer0>>
                (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0>,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0>,
                    DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Intersect<TComparer0, TEnumerable0, TEnumerator0, TEqualityComparer0, TGetHashCodeFunc0, T>
            (this NativeArray<T> @this,
            in DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0>, DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0>.Enumerator, T, IntersectOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0>, DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0>.Enumerator, T, TComparer0>>
                (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                Grouping<TKey0, T>,
                Grouping<TKey0, T>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    Grouping<TKey0, T>,
                    Grouping<TKey0, T>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Intersect<TKey0, TComparer0, T>
            (this NativeArray<T> @this,
            in Grouping<TKey0, T> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TKey0 : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                Grouping<TKey0, T>,
                Grouping<TKey0, T>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    Grouping<TKey0, T>,
                    Grouping<TKey0, T>.Enumerator,
                    T,
                    TComparer0
                >
            >
            (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                    GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Intersect<TComparer0, TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TEqualityComparer0, TGetHashCodeFunc0, TKey0, TKeySelector0, TKeySelector1, T0Selector, TEqualityComparer1, T>
            (this NativeArray<T> @this,
            in GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>
            second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where T1 : unmanaged
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T0, TKey0>
            where TKeySelector1 : struct, IRefFunc<T1, TKey0>
            where T0Selector : struct,
            IRefFunc<T0,
                WhereIndexEnumerable<
                    NativeEnumerable<T1>,
                    NativeEnumerable<T1>.Enumerator,
                    T1,
                    GroupJoinPredicate<T1, TKey0, TEqualityComparer1>
                    >,
                T>
            where TEqualityComparer1 : struct, IRefFunc<TKey0, TKey0, bool>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                    GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                    T,
                    TComparer0
                >
            >(@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                    JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Intersect<TComparer0, TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TEqualityComparer0, TGetHashCodeFunc0, TKey0, TKeySelector0, TKeySelector1, T0Selector, TEqualityComparer1, T>
            (this NativeArray<T> @this,
            in JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>
            second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where T1 : unmanaged
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T0, TKey0>
            where TKeySelector1 : struct, IRefFunc<T1, TKey0>
            where T0Selector : struct, IRefFunc<T0, T1, T>
            where TEqualityComparer1 : struct, IRefFunc<TKey0, TKey0, bool>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                    JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                    T,
                    TComparer0
                >
            >(@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1>,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1>,
                    OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Intersect<TComparer0, TEnumerable0, TEnumerator0, TComparer1, T>
            (this NativeArray<T> @this,
            in OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TComparer1 : struct, IRefFunc<T, T, int>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1>, OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1>.Enumerator, T, IntersectOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1>, OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1>.Enumerator, T, TComparer0>>
                (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                PrependEnumerable<TEnumerable0, TEnumerator0, T>,
                PrependEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    PrependEnumerable<TEnumerable0, TEnumerator0, T>,
                    PrependEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Intersect<TComparer0, TEnumerable0, TEnumerator0, T>
            (this NativeArray<T> @this,
            in PrependEnumerable<TEnumerable0, TEnumerator0, T> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, PrependEnumerable<TEnumerable0, TEnumerator0, T>, PrependEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator, T, IntersectOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, PrependEnumerable<TEnumerable0, TEnumerator0, T>, PrependEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator, T, TComparer0>>
                (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                RangeRepeatEnumerable<T, TAction0>,
                RangeRepeatEnumerable<T, TAction0>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    RangeRepeatEnumerable<T, TAction0>,
                    RangeRepeatEnumerable<T, TAction0>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Intersect<TComparer0, TAction0, T>
            (this NativeArray<T> @this,
            in RangeRepeatEnumerable<T, TAction0> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where TAction0 : struct, IRangeRepeat<T>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, RangeRepeatEnumerable<T, TAction0>, RangeRepeatEnumerable<T, TAction0>.Enumerator, T, IntersectOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, RangeRepeatEnumerable<T, TAction0>, RangeRepeatEnumerable<T, TAction0>.Enumerator, T, TComparer0>>
                (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T>,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ReverseEnumerable<TEnumerable0, TEnumerator0, T>,
                    ReverseEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Intersect<TComparer0, TEnumerable0, TEnumerator0, T>
            (this NativeArray<T> @this,
            in ReverseEnumerable<TEnumerable0, TEnumerator0, T> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, ReverseEnumerable<TEnumerable0, TEnumerator0, T>, ReverseEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator, T, IntersectOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, ReverseEnumerable<TEnumerable0, TEnumerator0, T>, ReverseEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator, T, TComparer0>>
            (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>,
                SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>,
                    SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Intersect<TComparer0, TEnumerable0, TEnumerator0, T0, TAction0, T>
            (this NativeArray<T> @this,
            in SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TAction0 : struct, IRefAction<T0, T>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>, SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator, T, IntersectOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>, SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator, T, TComparer0>>
            (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>,
                    SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Intersect<TComparer0, TEnumerable0, TEnumerator0, T0, TAction0, T>
            (this NativeArray<T> @this,
            in SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TAction0 : struct, ISelectIndex<T0, T>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>, SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator, T, IntersectOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>, SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator, T, TComparer0>>
                (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Intersect<TComparer0, TEnumerable0, TEnumerator0, TPredicate0, T>
            (this NativeArray<T> @this,
            in WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TPredicate0 : struct, IRefFunc<T, bool>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>, WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator, T, IntersectOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>, WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator, T, TComparer0>>
                (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Intersect<TComparer0, TEnumerable0, TEnumerator0, TPredicate0, T>
            (this NativeArray<T> @this,
            in WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TPredicate0 : struct, IWhereIndex<T>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>, WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator, T, IntersectOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>, WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator, T, TComparer0>>
                (@this.AsRefEnumerable(), second, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>,
                ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>,
                    ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>.Enumerator,
                    T,
                    TComparer0
                >
            >
            Intersect<TComparer0, TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TAction0, T>
            (this NativeArray<T> @this,
            in ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0> second, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where T1 : unmanaged
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where TAction0 : struct, IRefAction<T0, T1, T>
            => new SetOperationEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>, ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>.Enumerator, T, IntersectOperation<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>, ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>.Enumerator, T, TComparer0>>
                (@this.AsRefEnumerable(), second, comparer, allocator);
        #endregion

        #region Join
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TEnumerable0,
                TEnumerator0,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0, T
            >
            (this NativeArray<T> @this,
            in TEnumerable0 inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new JoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                AppendEnumerable<TEnumerable0, TEnumerator0, T0>,
                AppendEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0, T
            >
            (this NativeArray<T> @this,
            in AppendEnumerable<TEnumerable0, TEnumerator0, T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new JoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, AppendEnumerable<TEnumerable0, TEnumerator0, T0>, AppendEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                NativeEnumerable<T0>,
                NativeEnumerable<T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0, T
            >
            (this NativeArray<T> @this,
            in NativeEnumerable<T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new JoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, NativeEnumerable<T0>, NativeEnumerable<T0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                NativeEnumerable<T0>,
                NativeEnumerable<T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0, T
            >
            (this NativeArray<T> @this,
            NativeArray<T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new JoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, NativeEnumerable<T0>, NativeEnumerable<T0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner.AsRefEnumerable(), outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ArrayEnumerable<T0>,
                ArrayEnumerable<T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0, T
            >
            (this NativeArray<T> @this,
            in ArrayEnumerable<T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new JoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, ArrayEnumerable<T0>, ArrayEnumerable<T0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ArrayEnumerable<T0>,
                ArrayEnumerable<T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0, T
            >
            (this NativeArray<T> @this,
            T0[] inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new JoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, ArrayEnumerable<T0>, ArrayEnumerable<T0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner.AsRefEnumerable(), outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0
                , TEnumerable1, TEnumerator1, T
            >
            (this NativeArray<T> @this,
            in ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            where TEnumerator1 : struct, IRefEnumerator<T0>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T0>
            => new JoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0, T
            >
            (this NativeArray<T> @this,
            in DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new JoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>, DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer1, TGetHashCodeFunc0>,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer1, TGetHashCodeFunc0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0
                , TEqualityComparer1, TGetHashCodeFunc0, T
            >
            (this NativeArray<T> @this,
            in DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer1, TGetHashCodeFunc0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            where TEqualityComparer1 : struct, IRefFunc<T0, T0, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T0, int>
            => new JoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer1, TGetHashCodeFunc0>, DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer1, TGetHashCodeFunc0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                Grouping<TKey1, T0>,
                Grouping<TKey1, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0
                , TKey1, T
            >
            (this NativeArray<T> @this,
            in Grouping<TKey1, T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            where TKey1 : unmanaged
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                Grouping<TKey1, T0>,
                Grouping<TKey1, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SetOperationEnumerable<
                    TEnumerable0,
                    TEnumerator0,
                    TEnumerable1,
                    TEnumerator1,
                    T0,
                    TSetOperation0
                >,
                SetOperationEnumerable<
                    TEnumerable0,
                    TEnumerator0,
                    TEnumerable1,
                    TEnumerator1,
                    T0,
                    TSetOperation0
                >.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0
                , TEnumerable1, TEnumerator1, TSetOperation0, T
            >
            (this NativeArray<T> @this,
            in SetOperationEnumerable<
                    TEnumerable0,
                    TEnumerator0,
                    TEnumerable1,
                    TEnumerator1,
                    T0,
                    TSetOperation0
                > inner,
            in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            where TEnumerator1 : struct, IRefEnumerator<T0>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T0>
            where TSetOperation0 : struct, ISetOperation<TEnumerable0,
                    TEnumerator0,
                    TEnumerable1,
                    TEnumerator1,
                    T0>
            => new JoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0, TSetOperation0>, SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0, TSetOperation0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                GroupJoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>,
                GroupJoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>.Enumerator,
                T0,
                TKey1,
                TOuterKeySelector1,
                TInnerKeySelector1,
                T1,
                T1Selector,
                TKeyEqualityComparer1
            >
            Join<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0, TKey1,
                TOuterKeySelector1,
                TInnerKeySelector1,
                T1,
                T1Selector,
                TKeyEqualityComparer1,
                T
            >
            (
                this NativeArray<T> @this,
                in GroupJoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>
                inner,
                in TOuterKeySelector1 outerKeySelector, in TInnerKeySelector1 innerKeySelector,
                in T1Selector resultSelector, in TKeyEqualityComparer1 comparer, Allocator allocator = Allocator.Temp
            )
            where T : unmanaged
            where TOuterSource0 : unmanaged
            where TInnerSource0 : unmanaged
            where TOuterEnumerator0 : struct, IRefEnumerator<TOuterSource0>
            where TInnerEnumerator0 : struct, IRefEnumerator<TInnerSource0>
            where TOuterEnumerable0 : struct, IRefEnumerable<TOuterEnumerator0, TOuterSource0>
            where TInnerEnumerable0 : struct, IRefEnumerable<TInnerEnumerator0, TInnerSource0>
            where T0 : unmanaged
            where TKey0 : unmanaged
            where TKey1 : unmanaged
            where TOuterKeySelector0 : struct, IRefFunc<TOuterSource0, TKey0>
            where TInnerKeySelector0 : struct, IRefFunc<TInnerSource0, TKey0>
            where TKeyEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            where T0Selector : struct,
            IRefFunc<TOuterSource0,
                WhereIndexEnumerable<
                    NativeEnumerable<TInnerSource0>,
                    NativeEnumerable<TInnerSource0>.Enumerator,
                    TInnerSource0,
                    GroupJoinPredicate<TInnerSource0, TKey0, TKeyEqualityComparer0>
                    >,
                T0>
            where T1 : unmanaged
            where TOuterKeySelector1 : struct, IRefFunc<T, TKey1>
            where TInnerKeySelector1 : struct, IRefFunc<T0, TKey1>
            where TKeyEqualityComparer1 : struct, IRefFunc<TKey1, TKey1, bool>
            where T1Selector : struct, IRefFunc<T, T0, T1>
            =>
            new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                GroupJoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>,
                GroupJoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>.Enumerator,
                T0,
                TKey1,
                TOuterKeySelector1,
                TInnerKeySelector1,
                T1,
                T1Selector,
                TKeyEqualityComparer1
            >(@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                JoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>,
                JoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>.Enumerator,
                T0,
                TKey1,
                TOuterKeySelector1,
                TInnerKeySelector1,
                T1,
                T1Selector,
                TKeyEqualityComparer1
            >
            Join<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0, TKey1,
                TOuterKeySelector1,
                TInnerKeySelector1,
                T1,
                T1Selector,
                TKeyEqualityComparer1,
                T
            >
            (
                this NativeArray<T> @this,
                in JoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>
                inner,
                in TOuterKeySelector1 outerKeySelector, in TInnerKeySelector1 innerKeySelector,
                in T1Selector resultSelector, in TKeyEqualityComparer1 comparer, Allocator allocator = Allocator.Temp
            )
            where T : unmanaged
            where TOuterSource0 : unmanaged
            where TInnerSource0 : unmanaged
            where TOuterEnumerator0 : struct, IRefEnumerator<TOuterSource0>
            where TInnerEnumerator0 : struct, IRefEnumerator<TInnerSource0>
            where TOuterEnumerable0 : struct, IRefEnumerable<TOuterEnumerator0, TOuterSource0>
            where TInnerEnumerable0 : struct, IRefEnumerable<TInnerEnumerator0, TInnerSource0>
            where T0 : unmanaged
            where TKey0 : unmanaged
            where TKey1 : unmanaged
            where TOuterKeySelector0 : struct, IRefFunc<TOuterSource0, TKey0>
            where TInnerKeySelector0 : struct, IRefFunc<TInnerSource0, TKey0>
            where TKeyEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            where T0Selector : struct, IRefFunc<TOuterSource0, TInnerSource0, T0>
            where T1 : unmanaged
            where TOuterKeySelector1 : struct, IRefFunc<T, TKey1>
            where TInnerKeySelector1 : struct, IRefFunc<T0, TKey1>
            where TKeyEqualityComparer1 : struct, IRefFunc<TKey1, TKey1, bool>
            where T1Selector : struct, IRefFunc<T, T0, T1>
            =>
            new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                JoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>,
                JoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>.Enumerator,
                T0,
                TKey1,
                TOuterKeySelector1,
                TInnerKeySelector1,
                T1,
                T1Selector,
                TKeyEqualityComparer1
            >
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0
                , TComparer0, T
            >
            (this NativeArray<T> @this,
            in OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            where TComparer0 : struct, IRefFunc<T0, T0, int>
            => new JoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>, OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                PrependEnumerable<TEnumerable0, TEnumerator0, T0>,
                PrependEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0, T
            >
            (this NativeArray<T> @this,
            in PrependEnumerable<TEnumerable0, TEnumerator0, T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new JoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, PrependEnumerable<TEnumerable0, TEnumerator0, T0>, PrependEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                RangeRepeatEnumerable<T0, TAction0>,
                RangeRepeatEnumerable<T0, TAction0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            Join<
                T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0
                , TAction0, T
            >
            (this NativeArray<T> @this,
            in RangeRepeatEnumerable<T0, TAction0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            where TAction0 : struct, IRangeRepeat<T0>
            => new JoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, RangeRepeatEnumerable<T0, TAction0>, RangeRepeatEnumerable<T0, TAction0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T0>,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0, T
            >
            (this NativeArray<T> @this,
            in ReverseEnumerable<TEnumerable0, TEnumerator0, T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new JoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, ReverseEnumerable<TEnumerable0, TEnumerator0, T0>, ReverseEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0
                , TPrev0, TAction0, T
            >
            (this NativeArray<T> @this,
            in SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TPrev0 : unmanaged
            where TAction0 : struct, IRefAction<TPrev0, T0>
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<TPrev0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrev0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new JoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>, SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0
                , TPrev0, TAction0, T
            >
            (this NativeArray<T> @this,
            in SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TPrev0 : unmanaged
            where TAction0 : struct, ISelectIndex<TPrev0, T0>
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<TPrev0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrev0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new JoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>, SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction0>,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0
                , TEnumerable1, TEnumerator1, TPrev0, TAction0, T
            >
            (this NativeArray<T> @this,
            in SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TPrev0 : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<TPrev0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrev0>
            where TAction0 : struct, IRefAction<TPrev0, TEnumerable1>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            where TEnumerator1 : struct, IRefEnumerator<T0>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T0>
            => new JoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction0>, SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipEnumerable<TEnumerable0, TEnumerator0, T0>,
                TEnumerator0,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0, T
            >
            (this NativeArray<T> @this,
            in SkipEnumerable<TEnumerable0, TEnumerator0, T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipEnumerable<TEnumerable0, TEnumerator0, T0>,
                TEnumerator0,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeEnumerable<TEnumerable0, TEnumerator0, T0>,
                TakeEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0, T
            >
            (this NativeArray<T> @this,
            in TakeEnumerable<TEnumerable0, TEnumerator0, T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeEnumerable<TEnumerable0, TEnumerator0, T0>,
                TakeEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0
                , TPredicate0, T
            >
            (this NativeArray<T> @this,
            in SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            where TPredicate0 : struct, IRefFunc<T0, bool>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0
                , TPredicate0, T
            >
            (this NativeArray<T> @this,
            in TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            where TPredicate0 : struct, IRefFunc<T0, bool>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0, T
            >
            (this NativeArray<T> @this,
            in SkipLastEnumerable<TEnumerable0, TEnumerator0, T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0, T
            >
            (this NativeArray<T> @this,
            in TakeLastEnumerable<TEnumerable0, TEnumerator0, T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0
                , TPredicate0, T
            >
            (this NativeArray<T> @this,
            in WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            where TPredicate0 : struct, IRefFunc<T0, bool>
            => new JoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>, WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0
                , TPredicate0, T
            >
            (this NativeArray<T> @this,
            in WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            where TPredicate0 : struct, IWhereIndex<T0>
            => new JoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>, WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ZipEnumerable<TEnumerable0, TEnumerator0, T2, TEnumerable1, TEnumerator1, T3, T0, TAction0>,
                ZipEnumerable<TEnumerable0, TEnumerator0, T2, TEnumerable1, TEnumerator1, T3, T0, TAction0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                TEqualityComparer0
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0
                , TEnumerable1, TEnumerator1, T2, T3, TAction0, T
            >
            (this NativeArray<T> @this,
            in ZipEnumerable<TEnumerable0, TEnumerator0, T2, TEnumerable1, TEnumerator1, T3, T0, TAction0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, in TEqualityComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where TAction0 : struct, IRefAction<T2, T3, T0>
            where TEnumerator0 : struct, IRefEnumerator<T2>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T2>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            where TEnumerator1 : struct, IRefEnumerator<T3>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T3>
            => new JoinEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, ZipEnumerable<TEnumerable0, TEnumerator0, T2, TEnumerable1, TEnumerator1, T3, T0, TAction0>, ZipEnumerable<TEnumerable0, TEnumerator0, T2, TEnumerable1, TEnumerator1, T3, T0, TAction0>.Enumerator, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, TEqualityComparer0>
            (@this.AsRefEnumerable(), inner, outerKeySelector, innerKeySelector, resultSelector, comparer, allocator);
        #endregion

        #region Min Max
        public static
            bool TryGetMin(this NativeArray<byte> @this, out byte value)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                value = 0;
                return false;
            }
            value = @this[0];
            for (var i = 1; i < @this.Length; i++)
                if (@this[i] < value)
                    value = @this[i];
            return true;
        }

        public static
            bool TryGetMax(this NativeArray<byte> @this, out byte value)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                value = 0;
                return false;
            }
            value = @this[0];
            for (var i = 1; i < @this.Length; i++)
                if (@this[i] > value)
                    value = @this[i];
            return true;
        }

        public static
            bool TryGetMin(this NativeArray<sbyte> @this, out sbyte value)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                value = 0;
                return false;
            }
            value = @this[0];
            for (var i = 1; i < @this.Length; i++)
                if (@this[i] < value)
                    value = @this[i];
            return true;
        }

        public static
            bool TryGetMax(this NativeArray<sbyte> @this, out sbyte value)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                value = 0;
                return false;
            }
            value = @this[0];
            for (var i = 1; i < @this.Length; i++)
                if (@this[i] > value)
                    value = @this[i];
            return true;
        }

        public static
            bool TryGetMin(this NativeArray<short> @this, out short value)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                value = 0;
                return false;
            }
            value = @this[0];
            for (var i = 1; i < @this.Length; i++)
                if (@this[i] < value)
                    value = @this[i];
            return true;
        }

        public static
            bool TryGetMax(this NativeArray<short> @this, out short value)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                value = 0;
                return false;
            }
            value = @this[0];
            for (var i = 1; i < @this.Length; i++)
                if (@this[i] > value)
                    value = @this[i];
            return true;
        }

        public static
            bool TryGetMin(this NativeArray<ushort> @this, out ushort value)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                value = 0;
                return false;
            }
            value = @this[0];
            for (var i = 1; i < @this.Length; i++)
                if (@this[i] < value)
                    value = @this[i];
            return true;
        }

        public static
            bool TryGetMax(this NativeArray<ushort> @this, out ushort value)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                value = 0;
                return false;
            }
            value = @this[0];
            for (var i = 1; i < @this.Length; i++)
                if (@this[i] > value)
                    value = @this[i];
            return true;
        }

        public static
            bool TryGetMin(this NativeArray<int> @this, out int value)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                value = 0;
                return false;
            }
            value = @this[0];
            for (var i = 1; i < @this.Length; i++)
                if (@this[i] < value)
                    value = @this[i];
            return true;
        }

        public static
            bool TryGetMax(this NativeArray<int> @this, out int value)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                value = 0;
                return false;
            }
            value = @this[0];
            for (var i = 1; i < @this.Length; i++)
                if (@this[i] > value)
                    value = @this[i];
            return true;
        }

        public static
            bool TryGetMin(this NativeArray<uint> @this, out uint value)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                value = 0;
                return false;
            }
            value = @this[0];
            for (var i = 1; i < @this.Length; i++)
                if (@this[i] < value)
                    value = @this[i];
            return true;
        }

        public static
            bool TryGetMax(this NativeArray<uint> @this, out uint value)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                value = 0;
                return false;
            }
            value = @this[0];
            for (var i = 1; i < @this.Length; i++)
                if (@this[i] > value)
                    value = @this[i];
            return true;
        }

        public static
            bool TryGetMin(this NativeArray<long> @this, out long value)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                value = 0;
                return false;
            }
            value = @this[0];
            for (var i = 1; i < @this.Length; i++)
                if (@this[i] < value)
                    value = @this[i];
            return true;
        }

        public static
            bool TryGetMax(this NativeArray<long> @this, out long value)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                value = 0;
                return false;
            }
            value = @this[0];
            for (var i = 1; i < @this.Length; i++)
                if (@this[i] > value)
                    value = @this[i];
            return true;
        }

        public static
            bool TryGetMin(this NativeArray<ulong> @this, out ulong value)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                value = 0;
                return false;
            }
            value = @this[0];
            for (var i = 1; i < @this.Length; i++)
                if (@this[i] < value)
                    value = @this[i];
            return true;
        }

        public static
            bool TryGetMax(this NativeArray<ulong> @this, out ulong value)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                value = 0;
                return false;
            }
            value = @this[0];
            for (var i = 1; i < @this.Length; i++)
                if (@this[i] > value)
                    value = @this[i];
            return true;
        }

        public static
            bool TryGetMin(this NativeArray<float> @this, out float value)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                value = 0;
                return false;
            }
            value = @this[0];
            for (var i = 1; i < @this.Length; i++)
                if (@this[i] < value)
                    value = @this[i];
            return true;
        }

        public static
            bool TryGetMax(this NativeArray<float> @this, out float value)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                value = 0;
                return false;
            }
            value = @this[0];
            for (var i = 1; i < @this.Length; i++)
                if (@this[i] > value)
                    value = @this[i];
            return true;
        }

        public static
            bool TryGetMin(this NativeArray<double> @this, out double value)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                value = 0;
                return false;
            }
            value = @this[0];
            for (var i = 1; i < @this.Length; i++)
                if (@this[i] < value)
                    value = @this[i];
            return true;
        }

        public static
            bool TryGetMax(this NativeArray<double> @this, out double value)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                value = 0;
                return false;
            }
            value = @this[0];
            for (var i = 1; i < @this.Length; i++)
                if (@this[i] > value)
                    value = @this[i];
            return true;
        }

        public static
            bool TryGetMin(this NativeArray<decimal> @this, out decimal value)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                value = 0;
                return false;
            }
            value = @this[0];
            for (var i = 1; i < @this.Length; i++)
                if (@this[i] < value)
                    value = @this[i];
            return true;
        }

        public static
            bool TryGetMax(this NativeArray<decimal> @this, out decimal value)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                value = 0;
                return false;
            }
            value = @this[0];
            for (var i = 1; i < @this.Length; i++)
                if (@this[i] > value)
                    value = @this[i];
            return true;
        }

        public static
            MinMaxByEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                MinByPredicate<TKey0, DefaultOrderByAscending<TKey0>>,
                DefaultEqualityComparer<TKey0>
            >
            MinBy<TKey0, T>(this NativeArray<T> @this, Func<T, TKey0> func)
            where T : unmanaged
            where TKey0 : unmanaged
            => new MinMaxByEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                MinByPredicate<TKey0, DefaultOrderByAscending<TKey0>>,
                DefaultEqualityComparer<TKey0>
            >
            (@this.AsRefEnumerable(), func, default, default);

        public static
            MinMaxByEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                MaxByPredicate<TKey0, DefaultOrderByAscending<TKey0>>,
                DefaultEqualityComparer<TKey0>
            >
            MaxBy<TKey0, T>(this NativeArray<T> @this, Func<T, TKey0> func)
            where T : unmanaged
            where TKey0 : unmanaged
            => new MinMaxByEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                MaxByPredicate<TKey0, DefaultOrderByAscending<TKey0>>,
                DefaultEqualityComparer<TKey0>
            >
            (@this.AsRefEnumerable(), func, default, default);
        #endregion

        #region Union
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator, TEnumerable0, TEnumerator0, T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator, TEnumerable0, TEnumerator0, T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            Union<TEnumerable0, TEnumerator0, TEqualityComparer0, TGetHashCodeFunc0, T>
            (this NativeArray<T> @this,
            in TEnumerable0 second, TEqualityComparer0 comparer, TGetHashCodeFunc0 getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            => new DistinctEnumerable<ConcatEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, TEnumerable0, TEnumerator0, T>, ConcatEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, TEnumerable0, TEnumerator0, T>.Enumerator, T, TEqualityComparer0, TGetHashCodeFunc0>
                (Concat<TEnumerable0, TEnumerator0, T>(@this, second), comparer, getHashCodeFunc, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    AppendEnumerable<TEnumerable0, TEnumerator0, T>,
                    AppendEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    AppendEnumerable<TEnumerable0, TEnumerator0, T>,
                    AppendEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            Union<TEnumerable0, TEnumerator0, TEqualityComparer0, TGetHashCodeFunc0, T>
            (this NativeArray<T> @this,
            in AppendEnumerable<TEnumerable0, TEnumerator0, T>
                    second, TEqualityComparer0 comparer, TGetHashCodeFunc0 getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    AppendEnumerable<TEnumerable0, TEnumerator0, T>,
                    AppendEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    AppendEnumerable<TEnumerable0, TEnumerator0, T>,
                    AppendEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            (Concat(@this, second), comparer, getHashCodeFunc, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            Union<TEqualityComparer0, TGetHashCodeFunc0, T>
            (this NativeArray<T> @this,
            in NativeEnumerable<T>
                second, TEqualityComparer0 comparer, TGetHashCodeFunc0 getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            (Concat(@this, second), comparer, getHashCodeFunc, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            Union<TEqualityComparer0, TGetHashCodeFunc0, T>
            (this NativeArray<T> @this,
            NativeArray<T>
                second, TEqualityComparer0 comparer, TGetHashCodeFunc0 getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            (Concat(@this, second.AsRefEnumerable()), comparer, getHashCodeFunc, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ArrayEnumerable<T>,
                    ArrayEnumerable<T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ArrayEnumerable<T>,
                    ArrayEnumerable<T>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            Union<TEqualityComparer0, TGetHashCodeFunc0, T>
            (this NativeArray<T> @this,
            in ArrayEnumerable<T>
                second, TEqualityComparer0 comparer, TGetHashCodeFunc0 getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ArrayEnumerable<T>,
                    ArrayEnumerable<T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ArrayEnumerable<T>,
                    ArrayEnumerable<T>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            (Concat(@this, second), comparer, getHashCodeFunc, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ArrayEnumerable<T>,
                    ArrayEnumerable<T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ArrayEnumerable<T>,
                    ArrayEnumerable<T>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            Union<TEqualityComparer0, TGetHashCodeFunc0, T>
            (this NativeArray<T> @this,
            T[]
                second, TEqualityComparer0 comparer, TGetHashCodeFunc0 getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ArrayEnumerable<T>,
                    ArrayEnumerable<T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ArrayEnumerable<T>,
                    ArrayEnumerable<T>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            (Concat(@this, NativeEnumerable.AsRefEnumerable(second)), comparer, getHashCodeFunc, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            Union<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TEqualityComparer0, TGetHashCodeFunc0, T>
            (this NativeArray<T> @this,
            in ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>
                second, TEqualityComparer0 comparer, TGetHashCodeFunc0 getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEnumerator1 : struct, IRefEnumerator<T>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            (Concat(@this, second), comparer, getHashCodeFunc, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            Union<TEnumerable0, TEnumerator0, TEqualityComparer0, TGetHashCodeFunc0, T>
            (this NativeArray<T> @this,
            in DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>
                second, TEqualityComparer0 comparer, TGetHashCodeFunc0 getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            (Concat(@this, second), comparer, getHashCodeFunc, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer1, TGetHashCodeFunc1>,
                    DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer1, TGetHashCodeFunc1>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer1, TGetHashCodeFunc1>,
                    DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer1, TGetHashCodeFunc1>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            Union<TEnumerable0, TEnumerator0, TEqualityComparer0, TGetHashCodeFunc0, TEqualityComparer1, TGetHashCodeFunc1, T>
            (this NativeArray<T> @this,
            in DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer1, TGetHashCodeFunc1>
                second, TEqualityComparer0 comparer, TGetHashCodeFunc0 getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            where TEqualityComparer1 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc1 : struct, IRefFunc<T, int>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer1, TGetHashCodeFunc1>,
                    DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer1, TGetHashCodeFunc1>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer1, TGetHashCodeFunc1>,
                    DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer1, TGetHashCodeFunc1>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            (Concat(@this, second), comparer, getHashCodeFunc, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    Grouping<TKey0, T>,
                    Grouping<TKey0, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    Grouping<TKey0, T>,
                    Grouping<TKey0, T>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0
            >
            Union<TEnumerable0, TEnumerator0, TEqualityComparer0, TGetHashCodeFunc0, TKey0, T>
            (this NativeArray<T> @this,
            in Grouping<TKey0, T>
                second, TEqualityComparer0 comparer, TGetHashCodeFunc0 getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            where TKey0 : unmanaged
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    Grouping<TKey0, T>,
                    Grouping<TKey0, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    Grouping<TKey0, T>,
                    Grouping<TKey0, T>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0
            >
            (Concat(@this, second), comparer, getHashCodeFunc, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DistinctEnumerable<
                ConcatEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>, GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator, T>,
                ConcatEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>, GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator, T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            Union<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TEqualityComparer0, TGetHashCodeFunc0, TKey0, TKeySelector0, TKeySelector1, T0Selector, TEqualityComparer1, T>
            (this NativeArray<T> @this,
            in GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>
                second, TEqualityComparer0 comparer, TGetHashCodeFunc0 getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where T1 : unmanaged
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            where T0Selector : struct,
            IRefFunc<T0,
                WhereIndexEnumerable<
                    NativeEnumerable<T1>,
                    NativeEnumerable<T1>.Enumerator,
                    T1,
                    GroupJoinPredicate<T1, TKey0, TEqualityComparer1>
                    >,
                T>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T0, TKey0>
            where TKeySelector1 : struct, IRefFunc<T1, TKey0>
            where TEqualityComparer1 : struct, IRefFunc<TKey0, TKey0, bool>
            => new DistinctEnumerable<
                ConcatEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>, GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator, T>,
                ConcatEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>, GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator, T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            (Concat(@this, second), comparer, getHashCodeFunc, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DistinctEnumerable<
                ConcatEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>, JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator, T>,
                ConcatEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>, JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator, T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            Union<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TEqualityComparer0, TGetHashCodeFunc0, TKey0, TKeySelector0, TKeySelector1, T0Selector, TEqualityComparer1, T>
            (this NativeArray<T> @this,
            in JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>
                second, TEqualityComparer0 comparer, TGetHashCodeFunc0 getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where T1 : unmanaged
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            where T0Selector : struct, IRefFunc<T0, T1, T>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T0, TKey0>
            where TKeySelector1 : struct, IRefFunc<T1, TKey0>
            where TEqualityComparer1 : struct, IRefFunc<TKey0, TKey0, bool>
            => new DistinctEnumerable<
                ConcatEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>, JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator, T>,
                ConcatEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>, JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator, T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            (Concat(@this, second), comparer, getHashCodeFunc, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer0>,
                    OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer0>,
                    OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer0>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            Union<TEnumerable0, TEnumerator0, TEqualityComparer0, TGetHashCodeFunc0, TComparer0, T>
            (this NativeArray<T> @this,
            in OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer0>
                second, TEqualityComparer0 comparer, TGetHashCodeFunc0 getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            where TComparer0 : struct, IRefFunc<T, T, int>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer0>,
                    OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer0>,
                    OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer0>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            (Concat(@this, second), comparer, getHashCodeFunc, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    PrependEnumerable<TEnumerable0, TEnumerator0, T>,
                    PrependEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    PrependEnumerable<TEnumerable0, TEnumerator0, T>,
                    PrependEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            Union<TEnumerable0, TEnumerator0, TEqualityComparer0, TGetHashCodeFunc0, T>
            (this NativeArray<T> @this,
            in PrependEnumerable<TEnumerable0, TEnumerator0, T>
                second, TEqualityComparer0 comparer, TGetHashCodeFunc0 getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    PrependEnumerable<TEnumerable0, TEnumerator0, T>,
                    PrependEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    PrependEnumerable<TEnumerable0, TEnumerator0, T>,
                    PrependEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            (Concat(@this, second), comparer, getHashCodeFunc, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    RangeRepeatEnumerable<T, TAction0>,
                    RangeRepeatEnumerable<T, TAction0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    RangeRepeatEnumerable<T, TAction0>,
                    RangeRepeatEnumerable<T, TAction0>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            Union<TAction0, TEqualityComparer0, TGetHashCodeFunc0, T>
            (this NativeArray<T> @this,
            in RangeRepeatEnumerable<T, TAction0>
                second, TEqualityComparer0 comparer, TGetHashCodeFunc0 getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TAction0 : struct, IRangeRepeat<T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    RangeRepeatEnumerable<T, TAction0>,
                    RangeRepeatEnumerable<T, TAction0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    RangeRepeatEnumerable<T, TAction0>,
                    RangeRepeatEnumerable<T, TAction0>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            (Concat(@this, second), comparer, getHashCodeFunc, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ReverseEnumerable<TEnumerable0, TEnumerator0, T>,
                    ReverseEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ReverseEnumerable<TEnumerable0, TEnumerator0, T>,
                    ReverseEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            Union<TEnumerable0, TEnumerator0, TEqualityComparer0, TGetHashCodeFunc0, T>
            (this NativeArray<T> @this,
            in ReverseEnumerable<TEnumerable0, TEnumerator0, T>
                second, TEqualityComparer0 comparer, TGetHashCodeFunc0 getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ReverseEnumerable<TEnumerable0, TEnumerator0, T>,
                    ReverseEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ReverseEnumerable<TEnumerable0, TEnumerator0, T>,
                    ReverseEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            (Concat(@this, second), comparer, getHashCodeFunc, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>,
                    SelectEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>,
                    SelectEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            Union<TPrevEnumerable0, TPrevEnumerator0, TEqualityComparer0, TGetHashCodeFunc0, TPrev0, TAction0, T>
            (this NativeArray<T> @this,
            in SelectEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>
                second, TEqualityComparer0 comparer, TGetHashCodeFunc0 getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TPrev0 : unmanaged
            where TPrevEnumerator0 : struct, IRefEnumerator<TPrev0>
            where TPrevEnumerable0 : struct, IRefEnumerable<TPrevEnumerator0, TPrev0>
            where TAction0 : struct, IRefAction<TPrev0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>,
                    SelectEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>,
                    SelectEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            (Concat(@this, second), comparer, getHashCodeFunc, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>,
                    SelectIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>,
                    SelectIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            Union<TPrevEnumerable0, TPrevEnumerator0, TEqualityComparer0, TGetHashCodeFunc0, TPrev0, TAction0, T>
            (this NativeArray<T> @this,
            in SelectIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>
                second, TEqualityComparer0 comparer, TGetHashCodeFunc0 getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TPrev0 : unmanaged
            where TPrevEnumerator0 : struct, IRefEnumerator<TPrev0>
            where TPrevEnumerable0 : struct, IRefEnumerable<TPrevEnumerator0, TPrev0>
            where TAction0 : struct, ISelectIndex<TPrev0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>,
                    SelectIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>,
                    SelectIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            (Concat(@this, second), comparer, getHashCodeFunc, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectManyEnumerable<TEnumerable0, TEnumerator0, T0, T, TEnumerable1, TEnumerator1, TAction0>,
                    SelectManyEnumerable<TEnumerable0, TEnumerator0, T0, T, TEnumerable1, TEnumerator1, TAction0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectManyEnumerable<TEnumerable0, TEnumerator0, T0, T, TEnumerable1, TEnumerator1, TAction0>,
                    SelectManyEnumerable<TEnumerable0, TEnumerator0, T0, T, TEnumerable1, TEnumerator1, TAction0>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            Union<TEnumerable0, TEnumerator0, TEqualityComparer0, TGetHashCodeFunc0, T0, TEnumerable1, TEnumerator1, TAction0, T>
            (this NativeArray<T> @this,
            in SelectManyEnumerable<TEnumerable0, TEnumerator0, T0, T, TEnumerable1, TEnumerator1, TAction0>
                second, TEqualityComparer0 comparer, TGetHashCodeFunc0 getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            where TEnumerator1 : struct, IRefEnumerator<T>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T>
            where TAction0 : struct, IRefAction<T0, TEnumerable1>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectManyEnumerable<TEnumerable0, TEnumerator0, T0, T, TEnumerable1, TEnumerator1, TAction0>,
                    SelectManyEnumerable<TEnumerable0, TEnumerator0, T0, T, TEnumerable1, TEnumerator1, TAction0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectManyEnumerable<TEnumerable0, TEnumerator0, T0, T, TEnumerable1, TEnumerator1, TAction0>,
                    SelectManyEnumerable<TEnumerable0, TEnumerator0, T0, T, TEnumerable1, TEnumerator1, TAction0>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            (Concat(@this, second), comparer, getHashCodeFunc, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TSetOperation0>,
                    SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TSetOperation0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TSetOperation0>,
                    SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TSetOperation0>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            Union<TEnumerable0, TEnumerator0, TEqualityComparer0, TGetHashCodeFunc0, TEnumerable1, TEnumerator1, TSetOperation0, T>
            (this NativeArray<T> @this,
            in SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TSetOperation0>
                second, TEqualityComparer0 comparer, TGetHashCodeFunc0 getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            where TEnumerator1 : struct, IRefEnumerator<T>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T>
            where TSetOperation0 : struct, ISetOperation<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>
            => new DistinctEnumerable<ConcatEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TSetOperation0>, SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TSetOperation0>.Enumerator, T>, ConcatEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TSetOperation0>, SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TSetOperation0>.Enumerator, T>.Enumerator, T, TEqualityComparer0, TGetHashCodeFunc0>
            (Concat(@this, second), comparer, getHashCodeFunc, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SkipEnumerable<TEnumerable0, TEnumerator0, T>,
                    TEnumerator0,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SkipEnumerable<TEnumerable0, TEnumerator0, T>,
                    TEnumerator0,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0
            >
            Union<TEnumerable0, TEnumerator0, TEqualityComparer0, TGetHashCodeFunc0, T>
            (this NativeArray<T> @this,
            in SkipEnumerable<TEnumerable0, TEnumerator0, T>
                second, TEqualityComparer0 comparer, TGetHashCodeFunc0 getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SkipEnumerable<TEnumerable0, TEnumerator0, T>,
                    TEnumerator0,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SkipEnumerable<TEnumerable0, TEnumerator0, T>,
                    TEnumerator0,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0
            >
            (Concat(@this, second), comparer, getHashCodeFunc, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TakeEnumerable<TEnumerable0, TEnumerator0, T>,
                    TakeEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TakeEnumerable<TEnumerable0, TEnumerator0, T>,
                    TakeEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0
            >
            Union<TEnumerable0, TEnumerator0, TEqualityComparer0, TGetHashCodeFunc0, T>
            (this NativeArray<T> @this,
            in TakeEnumerable<TEnumerable0, TEnumerator0, T>
                second, TEqualityComparer0 comparer, TGetHashCodeFunc0 getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TakeEnumerable<TEnumerable0, TEnumerator0, T>,
                    TakeEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TakeEnumerable<TEnumerable0, TEnumerator0, T>,
                    TakeEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0
            >
            (Concat(@this, second), comparer, getHashCodeFunc, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0
            >
            Union<TEnumerable0, TEnumerator0, TEqualityComparer0, TGetHashCodeFunc0, TPredicate0, T>
            (this NativeArray<T> @this,
            in SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>
                second, TEqualityComparer0 comparer, TGetHashCodeFunc0 getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            where TPredicate0 : struct, IRefFunc<T, bool>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0
            >
            (Concat(@this, second), comparer, getHashCodeFunc, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0
            >
            Union<TEnumerable0, TEnumerator0, TEqualityComparer0, TGetHashCodeFunc0, TPredicate0, T>
            (this NativeArray<T> @this,
            in TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>
                second, TEqualityComparer0 comparer, TGetHashCodeFunc0 getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            where TPredicate0 : struct, IRefFunc<T, bool>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0
            >
            (Concat(@this, second), comparer, getHashCodeFunc, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SkipLastEnumerable<TEnumerable0, TEnumerator0, T>,
                    SkipLastEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SkipLastEnumerable<TEnumerable0, TEnumerator0, T>,
                    SkipLastEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0
            >
            Union<TEnumerable0, TEnumerator0, TEqualityComparer0, TGetHashCodeFunc0, T>
            (this NativeArray<T> @this,
            in SkipLastEnumerable<TEnumerable0, TEnumerator0, T>
                second, TEqualityComparer0 comparer, TGetHashCodeFunc0 getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SkipLastEnumerable<TEnumerable0, TEnumerator0, T>,
                    SkipLastEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SkipLastEnumerable<TEnumerable0, TEnumerator0, T>,
                    SkipLastEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0
            >
            (Concat(@this, second), comparer, getHashCodeFunc, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TakeLastEnumerable<TEnumerable0, TEnumerator0, T>,
                    TakeLastEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TakeLastEnumerable<TEnumerable0, TEnumerator0, T>,
                    TakeLastEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0
            >
            Union<TEnumerable0, TEnumerator0, TEqualityComparer0, TGetHashCodeFunc0, T>
            (this NativeArray<T> @this,
            in TakeLastEnumerable<TEnumerable0, TEnumerator0, T>
                second, TEqualityComparer0 comparer, TGetHashCodeFunc0 getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TakeLastEnumerable<TEnumerable0, TEnumerator0, T>,
                    TakeLastEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TakeLastEnumerable<TEnumerable0, TEnumerator0, T>,
                    TakeLastEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0
            >
            (Concat(@this, second), comparer, getHashCodeFunc, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0
            >
            Union<TEnumerable0, TEnumerator0, TEqualityComparer0, TGetHashCodeFunc0, TPredicate0, T>
            (this NativeArray<T> @this,
            in WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>
                second, TEqualityComparer0 comparer, TGetHashCodeFunc0 getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TPredicate0 : struct, IRefFunc<T, bool>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            (Concat(@this, second), comparer, getHashCodeFunc, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            Union<TEnumerable0, TEnumerator0, TEqualityComparer0, TGetHashCodeFunc0, TPredicate0, T>
            (this NativeArray<T> @this,
            in WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>
                second, TEqualityComparer0 comparer, TGetHashCodeFunc0 getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TPredicate0 : struct, IWhereIndex<T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            (Concat(@this, second), comparer, getHashCodeFunc, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>,
                    ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>,
                    ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            Union<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TEqualityComparer0, TGetHashCodeFunc0, TAction0, T>
            (this NativeArray<T> @this,
            in ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>
                second, TEqualityComparer0 comparer, TGetHashCodeFunc0 getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where T1 : unmanaged
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            where TAction0 : struct, IRefAction<T0, T1, T>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>,
                    ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>,
                    ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>.Enumerator,
                    T>.Enumerator,
                T,
                TEqualityComparer0,
                TGetHashCodeFunc0>
            (Concat(@this, second), comparer, getHashCodeFunc, allocator);
        #endregion

        #region SequenceEqual
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, TEqualityComparer0, T>
            (this NativeArray<T> @this,
            in TEnumerable0 second, in TEqualityComparer0 comparer)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
        {
            if (second.CanFastCount() && @this.Length != second.LongCount())
                return false;
            var enumerable = @this.AsRefEnumerable();
            var enumerator1 = second.GetEnumerator();
            for (var i = 0L; i < enumerable.Length; i++)
            {
                if (!enumerator1.MoveNext() || !comparer.Calc(ref enumerable[i], ref enumerator1.Current))
                {
                    enumerator1.Dispose();
                    return false;
                }
            }
            if (enumerator1.MoveNext())
            {
                enumerator1.Dispose();
                return false;
            }
            enumerator1.Dispose();
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, T>
            (this NativeArray<T> @this,
            in TEnumerable0 second)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => SequenceEqual<
                TEnumerable0, TEnumerator0,
                DefaultEqualityComparer<T>
            , T>(@this, second, default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, TEqualityComparer0, T>
            (this NativeArray<T> @this,
            in AppendEnumerable<TEnumerable0, TEnumerator0, T> second, in TEqualityComparer0 comparer)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            => SequenceEqual<
                AppendEnumerable<TEnumerable0, TEnumerator0, T>,
                AppendEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                TEqualityComparer0
            , T>(@this, second, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, T>
            (this NativeArray<T> @this,
            in AppendEnumerable<TEnumerable0, TEnumerator0, T> second)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => SequenceEqual<
                AppendEnumerable<TEnumerable0, TEnumerator0, T>,
                AppendEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                DefaultEqualityComparer<T>
            , T>(@this, second, default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEqualityComparer0, T>
            (this NativeArray<T> @this,
            in NativeEnumerable<T> second, in TEqualityComparer0 comparer)
            where T : unmanaged
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            => SequenceEqual<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                TEqualityComparer0
            , T>(@this, second, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<T>
            (this NativeArray<T> @this,
            in NativeEnumerable<T> second)
            where T : unmanaged
            => SequenceEqual<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                DefaultEqualityComparer<T>
            , T>(@this, second, default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEqualityComparer0, T>
            (this NativeArray<T> @this,
            in NativeArray<T> second, in TEqualityComparer0 comparer)
            where T : unmanaged
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            => SequenceEqual<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                TEqualityComparer0,
                T
            >(@this, second.AsRefEnumerable(), comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<T>
            (this NativeArray<T> @this,
            in NativeArray<T> second)
            where T : unmanaged
            => SequenceEqual<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                DefaultEqualityComparer<T>
                , T
            >(@this, second.AsRefEnumerable(), default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEqualityComparer0, T>
            (this NativeArray<T> @this,
            in ArrayEnumerable<T> second, in TEqualityComparer0 comparer)
            where T : unmanaged
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            => SequenceEqual<
                ArrayEnumerable<T>,
                ArrayEnumerable<T>.Enumerator,
                TEqualityComparer0
            , T>(@this, second, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<T>
            (this NativeArray<T> @this,
            in ArrayEnumerable<T> second)
            where T : unmanaged
            => SequenceEqual<
                ArrayEnumerable<T>,
                ArrayEnumerable<T>.Enumerator,
                DefaultEqualityComparer<T>
            , T>(@this, second, default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEqualityComparer0, T>
            (this NativeArray<T> @this,
            T[] second, in TEqualityComparer0 comparer)
            where T : unmanaged
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            => SequenceEqual<
                ArrayEnumerable<T>,
                ArrayEnumerable<T>.Enumerator,
                TEqualityComparer0,
                T
            >(@this, NativeEnumerable.AsRefEnumerable(second), comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<T>
            (this NativeArray<T> @this,
            T[] second)
            where T : unmanaged
            => SequenceEqual<
                ArrayEnumerable<T>,
                ArrayEnumerable<T>.Enumerator,
                DefaultEqualityComparer<T>
            , T>(@this, NativeEnumerable.AsRefEnumerable(second), default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TEqualityComparer0, T>
            (this NativeArray<T> @this,
            in ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T> second, in TEqualityComparer0 comparer)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEnumerator1 : struct, IRefEnumerator<T>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            => SequenceEqual<
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>.Enumerator,
                TEqualityComparer0
            , T>(@this, second, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>
            (this NativeArray<T> @this,
            in ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T> second)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEnumerator1 : struct, IRefEnumerator<T>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T>
            => SequenceEqual<
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>.Enumerator,
                DefaultEqualityComparer<T>
            , T>(@this, second, default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, TEqualityComparer0, T>
            (this NativeArray<T> @this,
            in DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T> second, in TEqualityComparer0 comparer)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            => SequenceEqual<
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                TEqualityComparer0
            , T>(@this, second, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, T>
            (this NativeArray<T> @this,
            in DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T> second)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => SequenceEqual<
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                DefaultEqualityComparer<T>
            , T>(@this, second, default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, TEqualityComparer0, TEqualityComparer1, TGetHashCodeFunc0, T>
            (this NativeArray<T> @this,
            in DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer1, TGetHashCodeFunc0> second, in TEqualityComparer0 comparer)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TEqualityComparer1 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            => SequenceEqual<
                DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer1, TGetHashCodeFunc0>,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer1, TGetHashCodeFunc0>.Enumerator,
                TEqualityComparer0
            , T>(@this, second, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEqualityComparer0, TKey0, T>
            (this NativeArray<T> @this,
            in Grouping<TKey0, T> second, in TEqualityComparer0 comparer)
            where T : unmanaged
            where TKey0 : unmanaged
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            => SequenceEqual<
                Grouping<TKey0, T>,
                Grouping<TKey0, T>.Enumerator,
                TEqualityComparer0,
                T
            >
            (@this, second, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, TEqualityComparer1, TGetHashCodeFunc0, T>
            (this NativeArray<T> @this,
            in DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer1, TGetHashCodeFunc0> second)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer1 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            => SequenceEqual<
                DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer1, TGetHashCodeFunc0>,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer1, TGetHashCodeFunc0>.Enumerator,
                DefaultEqualityComparer<T>
            , T>(@this, second, default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TEqualityComparer0, TComparer0, TKey0, TKeySelector0, TKeySelector1, T0Selector, TEqualityComparer1, T>
            (this NativeArray<T> @this,
            in JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1> second, in TEqualityComparer0 comparer)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T0, TKey0>
            where TKeySelector1 : struct, IRefFunc<T1, TKey0>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TEqualityComparer1 : struct, IRefFunc<TKey0, TKey0, bool>
            where T0Selector : struct, IRefFunc<T0, T1, T>
            => SequenceEqual<
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                TEqualityComparer0
            , T>(@this, second, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TEqualityComparer0, TComparer0, TKey0, TKeySelector0, TKeySelector1, T0Selector, TEqualityComparer1, T>
            (this NativeArray<T> @this,
            in GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1> second, in TEqualityComparer0 comparer)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T0, TKey0>
            where TKeySelector1 : struct, IRefFunc<T1, TKey0>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TEqualityComparer1 : struct, IRefFunc<TKey0, TKey0, bool>
            where T0Selector : struct,
            IRefFunc<T0,
                WhereIndexEnumerable<
                    NativeEnumerable<T1>,
                    NativeEnumerable<T1>.Enumerator,
                    T1,
                    GroupJoinPredicate<T1, TKey0, TEqualityComparer1>
                    >,
                T>
            => SequenceEqual<
                GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                TEqualityComparer0
            , T>(@this, second, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TComparer0, TKey0, TKeySelector0, TKeySelector1, T0Selector, TEqualityComparer1, T>
            (this NativeArray<T> @this,
            in JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1> second)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T0, TKey0>
            where TKeySelector1 : struct, IRefFunc<T1, TKey0>
            where TEqualityComparer1 : struct, IRefFunc<TKey0, TKey0, bool>
            where T0Selector : struct, IRefFunc<T0, T1, T>
            => SequenceEqual<
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                DefaultEqualityComparer<T>
            , T>(@this, second, default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, TEqualityComparer0, TComparer0, T>
            (this NativeArray<T> @this,
            in OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer0> second, in TEqualityComparer0 comparer)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TComparer0 : struct, IRefFunc<T, T, int>
            => SequenceEqual<
                OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer0>,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer0>.Enumerator,
                TEqualityComparer0
            , T>(@this, second, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, TComparer0, T>
            (this NativeArray<T> @this,
            in OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer0> second)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TComparer0 : struct, IRefFunc<T, T, int>
            => SequenceEqual<
                OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer0>,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer0>.Enumerator,
                DefaultEqualityComparer<T>
            , T>(@this, second, default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, TEqualityComparer0, T>
            (this NativeArray<T> @this,
            in PrependEnumerable<TEnumerable0, TEnumerator0, T> second, in TEqualityComparer0 comparer)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            => SequenceEqual<
                PrependEnumerable<TEnumerable0, TEnumerator0, T>,
                PrependEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                TEqualityComparer0
            , T>(@this, second, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, T>
            (this NativeArray<T> @this,
            in PrependEnumerable<TEnumerable0, TEnumerator0, T> second)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => SequenceEqual<
                PrependEnumerable<TEnumerable0, TEnumerator0, T>,
                PrependEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                DefaultEqualityComparer<T>
            , T>(@this, second, default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEqualityComparer0, TAction0, T>
            (this NativeArray<T> @this,
            in RangeRepeatEnumerable<T, TAction0> second, in TEqualityComparer0 comparer)
            where T : unmanaged
            where TAction0 : struct, IRangeRepeat<T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            => SequenceEqual<
                RangeRepeatEnumerable<T, TAction0>,
                RangeRepeatEnumerable<T, TAction0>.Enumerator,
                TEqualityComparer0
            , T>(@this, second, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TAction0, T>
            (this NativeArray<T> @this,
            in RangeRepeatEnumerable<T, TAction0> second)
            where T : unmanaged
            where TAction0 : struct, IRangeRepeat<T>
            => SequenceEqual<

                RangeRepeatEnumerable<T, TAction0>,
                RangeRepeatEnumerable<T, TAction0>.Enumerator,

                DefaultEqualityComparer<T>
            , T>(@this, second, default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, TEqualityComparer0, T>
            (this NativeArray<T> @this,
            in ReverseEnumerable<TEnumerable0, TEnumerator0, T> second, in TEqualityComparer0 comparer)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            => SequenceEqual<
                ReverseEnumerable<TEnumerable0, TEnumerator0, T>,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                TEqualityComparer0
            , T>(@this, second, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, T>
            (this NativeArray<T> @this,
            in ReverseEnumerable<TEnumerable0, TEnumerator0, T> second)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => SequenceEqual<
                ReverseEnumerable<TEnumerable0, TEnumerator0, T>,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                DefaultEqualityComparer<T>
            , T>(@this, second, default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, TEqualityComparer0, T0, TAction0, T>
            (this NativeArray<T> @this,
            in SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0> second, in TEqualityComparer0 comparer)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where T0 : unmanaged
            where TAction0 : struct, IRefAction<T0, T>
            => SequenceEqual<
                SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>,
                SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator,
                TEqualityComparer0
            , T>(@this, second, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, T0, TAction0, T>
            (this NativeArray<T> @this,
            in SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0> second)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where T0 : unmanaged
            where TAction0 : struct, IRefAction<T0, T>
            => SequenceEqual<
                SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>,
                SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator,
                DefaultEqualityComparer<T>
            , T>(@this, second, default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, TEqualityComparer0, T0, TAction0, T>
            (this NativeArray<T> @this,
            in SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0> second, in TEqualityComparer0 comparer)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where T0 : unmanaged
            where TAction0 : struct, ISelectIndex<T0, T>
            => SequenceEqual<
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator,
                TEqualityComparer0
            , T>(@this, second, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, T0, TAction0, T>
            (this NativeArray<T> @this,
            in SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0> second)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where T0 : unmanaged
            where TAction0 : struct, ISelectIndex<T0, T>
            => SequenceEqual<
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator,
                DefaultEqualityComparer<T>
            , T>(@this, second, default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TEqualityComparer0, T0, TAction0, T>
            (this NativeArray<T> @this,
            in SelectManyEnumerable<TEnumerable0, TEnumerator0, T0, T, TEnumerable1, TEnumerator1, TAction0> second, in TEqualityComparer0 comparer)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator1 : struct, IRefEnumerator<T>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where T0 : unmanaged
            where TAction0 : struct, IRefAction<T0, TEnumerable1>
            => SequenceEqual<
                SelectManyEnumerable<TEnumerable0, TEnumerator0, T0, T, TEnumerable1, TEnumerator1, TAction0>,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, T0, T, TEnumerable1, TEnumerator1, TAction0>.Enumerator,
                TEqualityComparer0
            , T>(@this, second, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0, TAction0, T>
            (this NativeArray<T> @this,
            in SelectManyEnumerable<TEnumerable0, TEnumerator0, T0, T, TEnumerable1, TEnumerator1, TAction0> second)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator1 : struct, IRefEnumerator<T>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T>
            where T0 : unmanaged
            where TAction0 : struct, IRefAction<T0, TEnumerable1>
            => SequenceEqual<
                SelectManyEnumerable<TEnumerable0, TEnumerator0, T0, T, TEnumerable1, TEnumerator1, TAction0>,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, T0, T, TEnumerable1, TEnumerator1, TAction0>.Enumerator,
                DefaultEqualityComparer<T>
            , T>(@this, second, default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TEqualityComparer0, TSetOperation0, T>
            (this NativeArray<T> @this,
            in SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TSetOperation0> second, in TEqualityComparer0 comparer)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEnumerator1 : struct, IRefEnumerator<T>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TSetOperation0 : struct, ISetOperation<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>
            => SequenceEqual<
                SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TSetOperation0>,
                SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TSetOperation0>.Enumerator,
                TEqualityComparer0
            , T>(@this, second, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSetOperation0, T>
            (this NativeArray<T> @this,
            in SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TSetOperation0> second)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEnumerator1 : struct, IRefEnumerator<T>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T>
            where TSetOperation0 : struct, ISetOperation<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>
            => SequenceEqual<
                SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TSetOperation0>,
                SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TSetOperation0>.Enumerator,
                DefaultEqualityComparer<T>
            , T>(@this, second, default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, TEqualityComparer0, T>
            (this NativeArray<T> @this,
            in SkipEnumerable<TEnumerable0, TEnumerator0, T> second, in TEqualityComparer0 comparer)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            => SequenceEqual<
                SkipEnumerable<TEnumerable0, TEnumerator0, T>,
                TEnumerator0,
                TEqualityComparer0
            , T>(@this, second, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, TEqualityComparer0, T>
            (this NativeArray<T> @this,
            in TakeEnumerable<TEnumerable0, TEnumerator0, T> second, in TEqualityComparer0 comparer)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            => SequenceEqual<
                TakeEnumerable<TEnumerable0, TEnumerator0, T>,
                TakeEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                TEqualityComparer0
            , T>(@this, second, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, TEqualityComparer0, TPredicate0, T>
            (this NativeArray<T> @this,
            in SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0> second, in TEqualityComparer0 comparer)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TPredicate0 : struct, IRefFunc<T, bool>
            => SequenceEqual<
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                TEqualityComparer0
            , T>(@this, second, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, TEqualityComparer0, TPredicate0, T>
            (this NativeArray<T> @this,
            in TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0> second, in TEqualityComparer0 comparer)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TPredicate0 : struct, IRefFunc<T, bool>
            => SequenceEqual<
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                TEqualityComparer0
            , T>(@this, second, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, TEqualityComparer0, T>
            (this NativeArray<T> @this,
            in SkipLastEnumerable<TEnumerable0, TEnumerator0, T> second, in TEqualityComparer0 comparer)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            => SequenceEqual<
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T>,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                TEqualityComparer0
            , T>(@this, second, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, TEqualityComparer0, T>
            (this NativeArray<T> @this,
            in TakeLastEnumerable<TEnumerable0, TEnumerator0, T> second, in TEqualityComparer0 comparer)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            => SequenceEqual<
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T>,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                TEqualityComparer0
            , T>(@this, second, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, TEqualityComparer0, TPredicate0, T>
            (this NativeArray<T> @this,
            in WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0> second, in TEqualityComparer0 comparer)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TPredicate0 : struct, IRefFunc<T, bool>
            => SequenceEqual<
                WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                TEqualityComparer0
            , T>(@this, second, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, TPredicate0, T>
            (this NativeArray<T> @this,
            in WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0> second)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TPredicate0 : struct, IRefFunc<T, bool>
            => SequenceEqual<
                WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                DefaultEqualityComparer<T>
            , T>(@this, second, default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, TEqualityComparer0, TPredicate0, T>
            (this NativeArray<T> @this,
            in WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0> second, in TEqualityComparer0 comparer)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TPredicate0 : struct, IWhereIndex<T>
            => SequenceEqual<
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                TEqualityComparer0
            , T>(@this, second, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, TPredicate0, T>
            (this NativeArray<T> @this,
            in WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0> second)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TPredicate0 : struct, IWhereIndex<T>
            => SequenceEqual<
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                DefaultEqualityComparer<T>
            , T>(@this, second, default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TEqualityComparer0, T0, T1, TAction0, T>
            (this NativeArray<T> @this,
            in ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0> second, in TEqualityComparer0 comparer)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TAction0 : struct, IRefAction<T0, T1, T>
            => SequenceEqual<
                ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>,
                ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>.Enumerator,
                TEqualityComparer0
            , T>(@this, second, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool
            SequenceEqual<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0, T1, TAction0, T>
            (this NativeArray<T> @this,
            in ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0> second)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where TAction0 : struct, IRefAction<T0, T1, T>
            => SequenceEqual<
                ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>,
                ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>.Enumerator,
                DefaultEqualityComparer<T>
            , T>(@this, second, default);
        #endregion

        #region Sum
        public static
            byte Sum(this NativeArray<byte> @this)
        {
            var sum = 0;
            for (int i = 0; i < @this.Length; i++)
                sum += @this[i];
            return (byte)sum;
        }

        public static
            sbyte Sum(this NativeArray<sbyte> @this)
        {
            var sum = 0;
            for (int i = 0; i < @this.Length; i++)
                sum += @this[i];
            return (sbyte)sum;
        }

        public static
            short Sum(this NativeArray<short> @this)
        {
            var sum = 0;
            for (int i = 0; i < @this.Length; i++)
                sum += @this[i];
            return (short)sum;
        }

        public static
            ushort Sum(this NativeArray<ushort> @this)
        {
            var sum = 0;
            for (int i = 0; i < @this.Length; i++)
                sum += @this[i];
            return (ushort)sum;
        }

        public static
            int Sum(this NativeArray<int> @this)
        {
            var sum = 0;
            for (int i = 0; i < @this.Length; i++)
                sum += @this[i];
            return sum;
        }

        public static
            uint Sum(this NativeArray<uint> @this)
        {
            var sum = 0U;
            for (int i = 0; i < @this.Length; i++)
                sum += @this[i];
            return sum;
        }

        public static
            long Sum(this NativeArray<long> @this)
        {
            var sum = 0L;
            for (int i = 0; i < @this.Length; i++)
                sum += @this[i];
            return sum;
        }

        public static
            ulong Sum(this NativeArray<ulong> @this)
        {
            var sum = 0UL;
            for (int i = 0; i < @this.Length; i++)
                sum += @this[i];
            return sum;
        }

        public static
            float Sum(this NativeArray<float> @this)
        {
            float sum = default;
            for (int i = 0; i < @this.Length; i++)
                sum += @this[i];
            return sum;
        }

        public static
            double Sum(this NativeArray<double> @this)
        {
            double sum = default;
            for (int i = 0; i < @this.Length; i++)
                sum += @this[i];
            return sum;
        }

        public static
            decimal Sum(this NativeArray<decimal> @this)
        {
            decimal sum = default;
            for (int i = 0; i < @this.Length; i++)
                sum += @this[i];
            return sum;
        }
        #endregion

        #region Zip
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TEnumerable0,
                TEnumerator0,
                T0,
                TResult0,
                TAction0
            >
            Zip<TEnumerable0, TEnumerator0, T0, TResult0, TAction0, T>
            (this NativeArray<T> @this,
            in TEnumerable0 second, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TAction0 : struct, IRefAction<T, T0, TResult0>
            where TResult0 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TEnumerable0,
                TEnumerator0,
                T0,
                TResult0,
                TAction0
            >(@this.AsRefEnumerable(), second, default, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                AppendEnumerable<TEnumerable0, TEnumerator0, T0>,
                AppendEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TResult0,
                TAction0
            >
            Zip<TEnumerable0, TEnumerator0, T0, TResult0, TAction0, T>
            (this NativeArray<T> @this,
            in AppendEnumerable<TEnumerable0, TEnumerator0, T0>
            second, TAction0 action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TAction0 : struct, IRefAction<T, T0, TResult0>
            where TResult0 : unmanaged
            => new ZipEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, AppendEnumerable<TEnumerable0, TEnumerator0, T0>, AppendEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator, T0, TResult0, TAction0>
            (@this.AsRefEnumerable(), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>.Enumerator,
                T0,
                TResult0,
                TAction0
            >
            Zip<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0, TResult0, TAction0, T>
            (this NativeArray<T> @this,
            in ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>
            second, TAction0 action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T0>
            where TEnumerator1 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TAction0 : struct, IRefAction<T, T0, TResult0>
            where TResult0 : unmanaged
            => new ZipEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>.Enumerator, T0, TResult0, TAction0>
            (@this.AsRefEnumerable(), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TResult0,
                TAction0
            >
            Zip<TEnumerable0, TEnumerator0, T0, TResult0, TAction0, T>
            (this NativeArray<T> @this,
            in DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>
            second, TAction0 action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TAction0 : struct, IRefAction<T, T0, TResult0>
            where TResult0 : unmanaged
            => new ZipEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>, DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator, T0, TResult0, TAction0>
            (@this.AsRefEnumerable(), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer0, TGetHashCodeFunc0>,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer0, TGetHashCodeFunc0>.Enumerator,
                T0,
                TResult0,
                TAction0
            >
            Zip<TEnumerable0, TEnumerator0, T0, TResult0, TAction0, TEqualityComparer0, TGetHashCodeFunc0, T>
            (this NativeArray<T> @this,
            in DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer0, TGetHashCodeFunc0>
            second, TAction0 action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TAction0 : struct, IRefAction<T, T0, TResult0>
            where TResult0 : unmanaged
            where TEqualityComparer0 : struct, IRefFunc<T0, T0, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T0, int>
            => new ZipEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer0, TGetHashCodeFunc0>, DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer0, TGetHashCodeFunc0>.Enumerator, T0, TResult0, TAction0>
            (@this.AsRefEnumerable(), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                Grouping<TKey0, T0>,
                Grouping<TKey0, T0>.Enumerator,
                T0,
                TResult0,
                TAction0
            >
            Zip<TKey0, T0, TResult0, TAction0, T>
            (this NativeArray<T> @this,
            in Grouping<TKey0, T0>
            second, TAction0 action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TKey0 : unmanaged
            where T0 : unmanaged
            where TAction0 : struct, IRefAction<T, T0, TResult0>
            where TResult0 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                Grouping<TKey0, T0>,
                Grouping<TKey0, T0>.Enumerator,
                T0,
                TResult0,
                TAction0
            >
            (@this.AsRefEnumerable(), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T2, T0Selector, TEqualityComparer1>,
                GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T2, T0Selector, TEqualityComparer1>.Enumerator,
                T2,
                TResult0,
                TAction0
            >
            Zip<T2, TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TComparer0, TKey0, TKeySelector0, TKeySelector1, T0Selector, TEqualityComparer1, TResult0, TAction0, T>
            (this NativeArray<T> @this,
            in GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T2, T0Selector, TEqualityComparer1>
            second, TAction0 action, T firstDefaultValue = default, T2 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where T2 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T0, TKey0>
            where TKeySelector1 : struct, IRefFunc<T1, TKey0>
            where TEqualityComparer1 : struct, IRefFunc<TKey0, TKey0, bool>
            where T0Selector : struct, IRefFunc<T0, WhereIndexEnumerable<NativeEnumerable<T1>, NativeEnumerable<T1>.Enumerator, T1, GroupJoinPredicate<T1, TKey0, TEqualityComparer1>>, T2>
            where TResult0 : unmanaged
            where TAction0 : struct, IRefAction<T, T2, TResult0>
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T2, T0Selector, TEqualityComparer1>,
                GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T2, T0Selector, TEqualityComparer1>.Enumerator,
                T2,
                TResult0,
                TAction0
            >
            (@this.AsRefEnumerable(), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T2, T0Selector, TEqualityComparer1>,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T2, T0Selector, TEqualityComparer1>.Enumerator,
                T2,
                TResult0,
                TAction0
            >
            Zip<T2, TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TComparer0, TKey0, TKeySelector0, TKeySelector1, T0Selector, TEqualityComparer1, TResult0, TAction0, T>
            (this NativeArray<T> @this,
            in JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T2, T0Selector, TEqualityComparer1>
            second, TAction0 action, T firstDefaultValue = default, T2 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where T2 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T0, TKey0>
            where TKeySelector1 : struct, IRefFunc<T1, TKey0>
            where TEqualityComparer1 : struct, IRefFunc<TKey0, TKey0, bool>
            where T0Selector : struct, IRefFunc<T0, T1, T2>
            where TResult0 : unmanaged
            where TAction0 : struct, IRefAction<T, T2, TResult0>
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T2, T0Selector, TEqualityComparer1>,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T2, T0Selector, TEqualityComparer1>.Enumerator,
                T2,
                TResult0,
                TAction0
            >
            (@this.AsRefEnumerable(), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>.Enumerator,
                T0,
                TResult0,
                TAction0
            >
            Zip<TEnumerable0, TEnumerator0, T0, TResult0, TAction0, TComparer0, T>
            (this NativeArray<T> @this,
            in OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>
            second, TAction0 action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TAction0 : struct, IRefAction<T, T0, TResult0>
            where TResult0 : unmanaged
            where TComparer0 : struct, IRefFunc<T0, T0, int>
            => new ZipEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>, OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>.Enumerator, T0, TResult0, TAction0>
            (@this.AsRefEnumerable(), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                PrependEnumerable<TEnumerable0, TEnumerator0, T0>,
                PrependEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TResult0,
                TAction0
            >
            Zip<TEnumerable0, TEnumerator0, T0, TResult0, TAction0, T>
            (this NativeArray<T> @this,
            in PrependEnumerable<TEnumerable0, TEnumerator0, T0>
                second, TAction0 action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TAction0 : struct, IRefAction<T, T0, TResult0>
            where TResult0 : unmanaged
            => new ZipEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, PrependEnumerable<TEnumerable0, TEnumerator0, T0>, PrependEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator, T0, TResult0, TAction0>(@this.AsRefEnumerable(), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                RangeRepeatEnumerable<T0, TAction1>,
                RangeRepeatEnumerable<T0, TAction1>.Enumerator,
                T0,
                TResult0,
                TAction0
            >
            Zip<T0, TResult0, TAction0, TAction1, T>
            (this NativeArray<T> @this,
            in RangeRepeatEnumerable<T0, TAction1>
            second, TAction0 action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where TAction0 : struct, IRefAction<T, T0, TResult0>
            where TResult0 : unmanaged
            where TAction1 : struct, IRangeRepeat<T0>
            => new ZipEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, RangeRepeatEnumerable<T0, TAction1>, RangeRepeatEnumerable<T0, TAction1>.Enumerator, T0, TResult0, TAction0>
            (@this.AsRefEnumerable(), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T0>,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TResult0,
                TAction0
            >
            Zip<TEnumerable0, TEnumerator0, T0, TResult0, TAction0, T>
            (this NativeArray<T> @this,
            in ReverseEnumerable<TEnumerable0, TEnumerator0, T0>
                second, TAction0 action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TAction0 : struct, IRefAction<T, T0, TResult0>
            where TResult0 : unmanaged
            => new ZipEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, ReverseEnumerable<TEnumerable0, TEnumerator0, T0>, ReverseEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator, T0, TResult0, TAction0>(@this.AsRefEnumerable(), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction1>,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction1>.Enumerator,
                T0,
                TResult0,
                TAction0
            >
            Zip<TEnumerable0, TEnumerator0, T0, TResult0, TAction0, TPrev0, TAction1, T>
            (this NativeArray<T> @this,
            in SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction1>
            second, TAction0 action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrev0>
            where TEnumerator0 : struct, IRefEnumerator<TPrev0>
            where T0 : unmanaged
            where TAction0 : struct, IRefAction<T, T0, TResult0>
            where TResult0 : unmanaged
            where TAction1 : struct, IRefAction<TPrev0, T0>
            where TPrev0 : unmanaged
            => new ZipEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction1>, SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction1>.Enumerator, T0, TResult0, TAction0>
            (@this.AsRefEnumerable(), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction1>,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction1>.Enumerator,
                T0,
                TResult0,
                TAction0
            >
            Zip<TEnumerable0, TEnumerator0, T0, TResult0, TAction0, TPrev0, TAction1, T>
            (this NativeArray<T> @this,
            in SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction1>
            second, TAction0 action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrev0>
            where TEnumerator0 : struct, IRefEnumerator<TPrev0>
            where T0 : unmanaged
            where TAction0 : struct, IRefAction<T, T0, TResult0>
            where TResult0 : unmanaged
            where TAction1 : struct, ISelectIndex<TPrev0, T0>
            where TPrev0 : unmanaged
            => new ZipEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction1>, SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction1>.Enumerator, T0, TResult0, TAction0>
            (@this.AsRefEnumerable(), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction1>,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction1>.Enumerator,
                T0,
                TResult0,
                TAction0
            >
            Zip<TEnumerable0, TEnumerator0, T0, TResult0, TAction0, TPrev0, TEnumerable1, TEnumerator1, TAction1, T>
            (this NativeArray<T> @this,
            in SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction1>
            second, TAction0 action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrev0>
            where TEnumerator0 : struct, IRefEnumerator<TPrev0>
            where T0 : unmanaged
            where TAction0 : struct, IRefAction<T, T0, TResult0>
            where TResult0 : unmanaged
            where TAction1 : struct, IRefAction<TPrev0, TEnumerable1>
            where TPrev0 : unmanaged
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T0>
            where TEnumerator1 : struct, IRefEnumerator<T0>
            => new ZipEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction1>, SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction1>.Enumerator, T0, TResult0, TAction0>
            (@this.AsRefEnumerable(), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0, TSetOperation0>,
                SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0, TSetOperation0>.Enumerator,
                T0,
                TResult0,
                TAction0
            >
            Zip<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSetOperation0, T0, TResult0, TAction0, T>
            (this NativeArray<T> @this,
            in SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0, TSetOperation0>
            second, TAction0 action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where TAction0 : struct, IRefAction<T, T0, TResult0>
            where TResult0 : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T0>
            where TEnumerator1 : struct, IRefEnumerator<T0>
            where TSetOperation0 : struct, ISetOperation<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>
            => new ZipEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0, TSetOperation0>, SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0, TSetOperation0>.Enumerator, T0, TResult0, TAction0>
            (@this.AsRefEnumerable(), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipEnumerable<TEnumerable0, TEnumerator0, T0>,
                TEnumerator0,
                T0,
                TResult0,
                TAction0
            >
            Zip<TEnumerable0, TEnumerator0, T0, TResult0, TAction0, T>
            (this NativeArray<T> @this,
            in SkipEnumerable<TEnumerable0, TEnumerator0, T0>
            second, TAction0 action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TAction0 : struct, IRefAction<T, T0, TResult0>
            where TResult0 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipEnumerable<TEnumerable0, TEnumerator0, T0>,
                TEnumerator0,
                T0,
                TResult0,
                TAction0
            >
            (@this.AsRefEnumerable(), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeEnumerable<TEnumerable0, TEnumerator0, T0>,
                TakeEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TResult0,
                TAction0
            >
            Zip<TEnumerable0, TEnumerator0, T0, TResult0, TAction0, T>
            (this NativeArray<T> @this,
            in TakeEnumerable<TEnumerable0, TEnumerator0, T0>
            second, TAction0 action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TAction0 : struct, IRefAction<T, T0, TResult0>
            where TResult0 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeEnumerable<TEnumerable0, TEnumerator0, T0>,
                TakeEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TResult0,
                TAction0
            >
            (@this.AsRefEnumerable(), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TResult0,
                TAction0
            >
            Zip<TEnumerable0, TEnumerator0, T0, TResult0, TAction0, TPredicate0, T>
            (this NativeArray<T> @this,
            in SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>
            second, TAction0 action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TAction0 : struct, IRefAction<T, T0, TResult0>
            where TResult0 : unmanaged
            where TPredicate0 : struct, IRefFunc<T0, bool>
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TResult0,
                TAction0
            >
            (@this.AsRefEnumerable(), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TResult0,
                TAction0
            >
            Zip<TEnumerable0, TEnumerator0, T0, TResult0, TAction0, TPredicate0, T>
            (this NativeArray<T> @this,
            in TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>
            second, TAction0 action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TAction0 : struct, IRefAction<T, T0, TResult0>
            where TResult0 : unmanaged
            where TPredicate0 : struct, IRefFunc<T0, bool>
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TResult0,
                TAction0
            >
            (@this.AsRefEnumerable(), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TResult0,
                TAction0
            >
            Zip<TEnumerable0, TEnumerator0, T0, TResult0, TAction0, T>
            (this NativeArray<T> @this,
            in SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>
            second, TAction0 action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TAction0 : struct, IRefAction<T, T0, TResult0>
            where TResult0 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TResult0,
                TAction0
            >
            (@this.AsRefEnumerable(), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TResult0,
                TAction0
            >
            Zip<TEnumerable0, TEnumerator0, T0, TResult0, TAction0, T>
            (this NativeArray<T> @this,
            in TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>
            second, TAction0 action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TAction0 : struct, IRefAction<T, T0, TResult0>
            where TResult0 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TResult0,
                TAction0
            >
            (@this.AsRefEnumerable(), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TResult0,
                TAction0
            >
            Zip<TEnumerable0, TEnumerator0, T0, TResult0, TAction0, TPredicate0, T>
            (this NativeArray<T> @this,
            in WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>
            second, TAction0 action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TAction0 : struct, IRefAction<T, T0, TResult0>
            where TResult0 : unmanaged
            where TPredicate0 : struct, IRefFunc<T0, bool>
            => new ZipEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>, WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator, T0, TResult0, TAction0>
            (@this.AsRefEnumerable(), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TResult0,
                TAction0
            >
            Zip<TEnumerable0, TEnumerator0, T0, TResult0, TAction0, TPredicate0, T>
            (this NativeArray<T> @this,
            in WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>
            second, TAction0 action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TAction0 : struct, IRefAction<T, T0, TResult0>
            where TResult0 : unmanaged
            where TPredicate0 : struct, IWhereIndex<T0>
            => new ZipEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>, WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator, T0, TResult0, TAction0>
            (@this.AsRefEnumerable(), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TResult1, TAction1>,
                ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TResult1, TAction1>.Enumerator,
                TResult1,
                TResult0,
                TAction0
            >
            Zip<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TResult0, TResult1, TAction0, TAction1, T>
            (this NativeArray<T> @this,
            in ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TResult1, TAction1>
            second, TAction0 action, T firstDefaultValue = default, TResult1 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where T1 : unmanaged
            where TAction1 : struct, IRefAction<T0, T1, TResult1>
            where TResult0 : unmanaged
            where TResult1 : unmanaged
            where TAction0 : struct, IRefAction<T, TResult1, TResult0>
            => new ZipEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TResult1, TAction1>, ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TResult1, TAction1>.Enumerator, TResult1, TResult0, TAction0>
            (@this.AsRefEnumerable(), second, action, firstDefaultValue, secondDefaultValue);
        #endregion

        #region Zip ValueTuple
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TEnumerable0,
                TEnumerator0,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            Zip<TEnumerable0, TEnumerator0, T0, T>
            (this NativeArray<T> @this,
            in TEnumerable0 second, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TEnumerable0,
                TEnumerator0,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            (@this.AsRefEnumerable(), second, default, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                AppendEnumerable<TEnumerable0, TEnumerator0, T0>,
                AppendEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            Zip<TEnumerable0, TEnumerator0, T0, T>
            (this NativeArray<T> @this,
            in AppendEnumerable<TEnumerable0, TEnumerator0, T0>
            second, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                AppendEnumerable<TEnumerable0, TEnumerator0, T0>,
                AppendEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            (@this.AsRefEnumerable(), second, default, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            Zip<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0, T>
            (this NativeArray<T> @this,
            in ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>
            second, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T0>
            where TEnumerator1 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            (@this.AsRefEnumerable(), second, default, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            Zip<TEnumerable0, TEnumerator0, T0, T>
            (this NativeArray<T> @this,
            in DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>
            second, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            (@this.AsRefEnumerable(), second, default, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer0, TGetHashCodeFunc0>,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer0, TGetHashCodeFunc0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            Zip<TEnumerable0, TEnumerator0, T0, TEqualityComparer0, TGetHashCodeFunc0, T>
            (this NativeArray<T> @this,
            in DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer0, TGetHashCodeFunc0>
            second, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TEqualityComparer0 : struct, IRefFunc<T0, T0, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T0, int>
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer0, TGetHashCodeFunc0>,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer0, TGetHashCodeFunc0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            (@this.AsRefEnumerable(), second, default, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                Grouping<TKey0, T0>,
                Grouping<TKey0, T0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            Zip<TKey0, T0, T>
            (this NativeArray<T> @this,
            in Grouping<TKey0, T0>
            second, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TKey0 : unmanaged
            where T0 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                Grouping<TKey0, T0>,
                Grouping<TKey0, T0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            (@this.AsRefEnumerable(), second, default, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T2, T0Selector, TEqualityComparer1>,
                GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T2, T0Selector, TEqualityComparer1>.Enumerator,
                T2,
                (T, T2),
                ZipValueTuple<T, T2>
            >
            Zip<T2, TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TComparer0, TKey0, TKeySelector0, TKeySelector1, T0Selector, TEqualityComparer1, T>
            (this NativeArray<T> @this,
            in GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T2, T0Selector, TEqualityComparer1>
            second, T firstDefaultValue = default, T2 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where T2 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T0, TKey0>
            where TKeySelector1 : struct, IRefFunc<T1, TKey0>
            where TEqualityComparer1 : struct, IRefFunc<TKey0, TKey0, bool>
            where T0Selector : struct, IRefFunc<T0, WhereIndexEnumerable<NativeEnumerable<T1>, NativeEnumerable<T1>.Enumerator, T1, GroupJoinPredicate<T1, TKey0, TEqualityComparer1>>, T2>
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T2, T0Selector, TEqualityComparer1>,
                GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T2, T0Selector, TEqualityComparer1>.Enumerator,
                T2,
                (T, T2),
                ZipValueTuple<T, T2>
            >
            (@this.AsRefEnumerable(), second, default, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T2, T0Selector, TEqualityComparer1>,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T2, T0Selector, TEqualityComparer1>.Enumerator,
                T2,
                (T, T2),
                ZipValueTuple<T, T2>
            >
            Zip<T2, TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TComparer0, TKey0, TKeySelector0, TKeySelector1, T0Selector, TEqualityComparer1, T>
            (this NativeArray<T> @this,
            in JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T2, T0Selector, TEqualityComparer1>
            second, T firstDefaultValue = default, T2 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where T2 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T0, TKey0>
            where TKeySelector1 : struct, IRefFunc<T1, TKey0>
            where TEqualityComparer1 : struct, IRefFunc<TKey0, TKey0, bool>
            where T0Selector : struct, IRefFunc<T0, T1, T2>
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T2, T0Selector, TEqualityComparer1>,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T2, T0Selector, TEqualityComparer1>.Enumerator,
                T2,
                (T, T2),
                ZipValueTuple<T, T2>
            >
            (@this.AsRefEnumerable(), second, default, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            Zip<TEnumerable0, TEnumerator0, T0, TComparer0, T>
            (this NativeArray<T> @this,
            in OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>
            second, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TComparer0 : struct, IRefFunc<T0, T0, int>
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            (@this.AsRefEnumerable(), second, default, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                PrependEnumerable<TEnumerable0, TEnumerator0, T0>,
                PrependEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            Zip<TEnumerable0, TEnumerator0, T0, T>
            (this NativeArray<T> @this,
            in PrependEnumerable<TEnumerable0, TEnumerator0, T0>
                second, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                PrependEnumerable<TEnumerable0, TEnumerator0, T0>,
                PrependEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >(@this.AsRefEnumerable(), second, default, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                RangeRepeatEnumerable<T0, TAction0>,
                RangeRepeatEnumerable<T0, TAction0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            Zip<T0, TAction0, T>
            (this NativeArray<T> @this,
            in RangeRepeatEnumerable<T0, TAction0>
            second, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where TAction0 : struct, IRangeRepeat<T0>
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                RangeRepeatEnumerable<T0, TAction0>,
                RangeRepeatEnumerable<T0, TAction0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            (@this.AsRefEnumerable(), second, default, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T0>,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            Zip<TEnumerable0, TEnumerator0, T0, T>
            (this NativeArray<T> @this,
            in ReverseEnumerable<TEnumerable0, TEnumerator0, T0>
                second, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T0>,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            (@this.AsRefEnumerable(), second, default, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            Zip<TEnumerable0, TEnumerator0, T0, TPrev0, TAction0, T>
            (this NativeArray<T> @this,
            in SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>
            second, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrev0>
            where TEnumerator0 : struct, IRefEnumerator<TPrev0>
            where T0 : unmanaged
            where TAction0 : struct, IRefAction<TPrev0, T0>
            where TPrev0 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            (@this.AsRefEnumerable(), second, default, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            Zip<TEnumerable0, TEnumerator0, T0, TPrev0, TAction0, T>
            (this NativeArray<T> @this,
            in SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>
            second, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrev0>
            where TEnumerator0 : struct, IRefEnumerator<TPrev0>
            where T0 : unmanaged
            where TAction0 : struct, ISelectIndex<TPrev0, T0>
            where TPrev0 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            (@this.AsRefEnumerable(), second, default, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction0>,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            Zip<TEnumerable0, TEnumerator0, T0, TPrev0, TEnumerable1, TEnumerator1, TAction0, T>
            (this NativeArray<T> @this,
            in SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction0>
            second, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrev0>
            where TEnumerator0 : struct, IRefEnumerator<TPrev0>
            where T0 : unmanaged
            where TAction0 : struct, IRefAction<TPrev0, TEnumerable1>
            where TPrev0 : unmanaged
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T0>
            where TEnumerator1 : struct, IRefEnumerator<T0>
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction0>,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            (@this.AsRefEnumerable(), second, default, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0, TSetOperation0>,
                SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0, TSetOperation0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            Zip<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSetOperation0, T0, T>
            (this NativeArray<T> @this,
            in SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0, TSetOperation0>
            second, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T0>
            where TEnumerator1 : struct, IRefEnumerator<T0>
            where TSetOperation0 : struct, ISetOperation<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0, TSetOperation0>,
                SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0, TSetOperation0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            (@this.AsRefEnumerable(), second, default, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipEnumerable<TEnumerable0, TEnumerator0, T0>,
                TEnumerator0,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            Zip<TEnumerable0, TEnumerator0, T0, T>
            (this NativeArray<T> @this,
            in SkipEnumerable<TEnumerable0, TEnumerator0, T0>
            second, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipEnumerable<TEnumerable0, TEnumerator0, T0>,
                TEnumerator0,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            (@this.AsRefEnumerable(), second, default, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeEnumerable<TEnumerable0, TEnumerator0, T0>,
                TakeEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            Zip<TEnumerable0, TEnumerator0, T0, T>
            (this NativeArray<T> @this,
            in TakeEnumerable<TEnumerable0, TEnumerator0, T0>
            second, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeEnumerable<TEnumerable0, TEnumerator0, T0>,
                TakeEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            (@this.AsRefEnumerable(), second, default, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            Zip<TEnumerable0, TEnumerator0, T0, TPredicate0, T>
            (this NativeArray<T> @this,
            in SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>
            second, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TPredicate0 : struct, IRefFunc<T0, bool>
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            (@this.AsRefEnumerable(), second, default, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            Zip<TEnumerable0, TEnumerator0, T0, TPredicate0, T>
            (this NativeArray<T> @this,
            in TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>
            second, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TPredicate0 : struct, IRefFunc<T0, bool>
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            (@this.AsRefEnumerable(), second, default, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            Zip<TEnumerable0, TEnumerator0, T0, T>
            (this NativeArray<T> @this,
            in SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>
            second, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            (@this.AsRefEnumerable(), second, default, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            Zip<TEnumerable0, TEnumerator0, T0, T>
            (this NativeArray<T> @this,
            in TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>
            second, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            (@this.AsRefEnumerable(), second, default, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            Zip<TEnumerable0, TEnumerator0, T0, TPredicate0, T>
            (this NativeArray<T> @this,
            in WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>
            second, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TPredicate0 : struct, IRefFunc<T0, bool>
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            (@this.AsRefEnumerable(), second, default, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            Zip<TEnumerable0, TEnumerator0, T0, TPredicate0, T>
            (this NativeArray<T> @this,
            in WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>
            second, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TPredicate0 : struct, IWhereIndex<T0>
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                (T, T0),
                ZipValueTuple<T, T0>
            >
            (@this.AsRefEnumerable(), second, default, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TResult0, TAction0>,
                ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TResult0, TAction0>.Enumerator,
                TResult0,
                (T, TResult0),
                ZipValueTuple<T, TResult0>
            >
            Zip<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TResult0, TAction0, T>
            (this NativeArray<T> @this,
            in ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TResult0, TAction0>
            second, T firstDefaultValue = default, TResult0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where T1 : unmanaged
            where TResult0 : unmanaged
            where TAction0 : struct, IRefAction<T0, T1, TResult0>
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TResult0, TAction0>,
                ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TResult0, TAction0>.Enumerator,
                TResult0,
                (T, TResult0),
                ZipValueTuple<T, TResult0>
            >
            (@this.AsRefEnumerable(), second, default, firstDefaultValue, secondDefaultValue);
        #endregion

        #region Function
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty<T>(this NativeArray<T> @this)
            where T : unmanaged
            => !Any(@this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Any<TPredicate0, T>
            (this NativeArray<T> @this, TPredicate0 predicate)
            where T : unmanaged
            where TPredicate0 : struct, IRefFunc<T, bool>
        {
            var enumerable = @this.AsRefEnumerable();
            for (var i = 0L; i < enumerable.Length; i++)
                if (predicate.Calc(ref enumerable[i]))
                    return true;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Any<T>
            (this NativeArray<T> @this, Func<T, bool> predicate)
            where T : unmanaged
        {
            for (var i = 0; i < @this.Length; i++)
                if (predicate(@this[i]))
                    return true;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool All<TPredicate0, T>
            (this NativeArray<T> @this, TPredicate0 predicate)
            where T : unmanaged
            where TPredicate0 : struct, IRefFunc<T, bool>
        {
            var enumerable = @this.AsRefEnumerable();
            for (var i = 0L; i < enumerable.Length; i++)
                if (!predicate.Calc(ref enumerable[i]))
                    return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool All<T>
            (this NativeArray<T> @this, Func<T, bool> predicate)
            where T : unmanaged
        {
            for (var i = 0; i < @this.Length; i++)
                if (!predicate(@this[i]))
                    return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Aggregate<TAccumulate0, TFunc0, T>
            (this NativeArray<T> @this, ref TAccumulate0 seed, TFunc0 func)
            where T : unmanaged
            where TFunc0 : struct, IRefAction<TAccumulate0, T>
        {
            var enumerator = GetEnumerator(@this);
            while (enumerator.MoveNext())
                func.Execute(ref seed, ref enumerator.Current);
            enumerator.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TNextResult0 Aggregate<TAccumulate0, TNextResult0, TFunc0, TResultFunc0, T>
            (this NativeArray<T> @this, ref TAccumulate0 seed, TFunc0 func, TResultFunc0 resultFunc)
            where T : unmanaged
            where TFunc0 : struct, IRefAction<TAccumulate0, T>
            where TResultFunc0 : struct, IRefFunc<TAccumulate0, TNextResult0>
        {
            var enumerator = GetEnumerator(@this);
            while (enumerator.MoveNext())
                func.Execute(ref seed, ref enumerator.Current);
            enumerator.Dispose();
            return resultFunc.Calc(ref seed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Aggregate<T>
            (this NativeArray<T> @this, Func<T, T, T> func)
            where T : unmanaged
        {
            var enumerator = GetEnumerator(@this);
            if (!enumerator.MoveNext())
            {
                enumerator.Dispose();
                throw new InvalidOperationException();
            }
            var seed = enumerator.Current;
            while (enumerator.MoveNext())
                seed = func(seed, enumerator.Current);
            enumerator.Dispose();
            return seed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TAccumulate0 Aggregate<TAccumulate0, T>
            (this NativeArray<T> @this, TAccumulate0 seed, Func<TAccumulate0, T, TAccumulate0> func)
            where T : unmanaged
        {
            var enumerator = GetEnumerator(@this);
            while (enumerator.MoveNext())
                seed = func(seed, enumerator.Current);
            enumerator.Dispose();
            return seed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TNextResult0 Aggregate<TAccumulate0, TNextResult0, T>
            (this NativeArray<T> @this, TAccumulate0 seed, Func<TAccumulate0, T, TAccumulate0> func, Func<TAccumulate0, TNextResult0> resultFunc)
            where T : unmanaged
        {
            var enumerator = GetEnumerator(@this);
            while (enumerator.MoveNext())
                seed = func(seed, enumerator.Current);
            enumerator.Dispose();
            return resultFunc(seed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<T>
            (this NativeArray<T> @this, in T value)
            where T : unmanaged, IEquatable<T>
        {
            for (var i = 0; i < @this.Length; i++)
                if (value.Equals(@this[i]))
                    return true;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<T>
            (this NativeArray<T> @this, T value)
            where T : unmanaged, IEquatable<T>
        {
            for (var i = 0; i < @this.Length; i++)
                if (value.Equals(@this[i]))
                    return true;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<T>
            (this NativeArray<T> @this, in T value, IEqualityComparer<T> comparer)
            where T : unmanaged
        {
            var enumerator = GetEnumerator(@this);
            while (enumerator.MoveNext())
            {
                if (!comparer.Equals(enumerator.Current, value)) continue;
                enumerator.Dispose();
                return true;
            }
            enumerator.Dispose();
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<T>
            (this NativeArray<T> @this, T value, IEqualityComparer<T> comparer)
            where T : unmanaged
            => Contains(@this, in value, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<TComparer0, T>
            (this NativeArray<T> @this, ref T value, ref TComparer0 comparer)
            where TComparer0 : struct, IRefFunc<T, T, bool>
            where T : unmanaged
        {
            var enumerator = GetEnumerator(@this);
            while (enumerator.MoveNext())
            {
                if (!comparer.Calc(ref enumerator.Current, ref value)) continue;
                enumerator.Dispose();
                return true;
            }
            enumerator.Dispose();
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<TComparer0, T>
            (this NativeArray<T> @this, T value, TComparer0 comparer)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, bool>
            => Contains(@this, ref value, ref comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Count<T>
            (this NativeArray<T> @this, Func<T, bool> predicate)
            where T : unmanaged
            => (int)LongCount(@this, predicate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Count<TPredicate0, T>
            (this NativeArray<T> @this, TPredicate0 predicate)
            where T : unmanaged
            where TPredicate0 : struct, IRefFunc<T, bool>
            => (int)LongCount(@this, predicate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long LongCount<T>
            (this NativeArray<T> @this,
            Func<T, bool> predicate)
            where T : unmanaged
        {
            var count = 0L;
            for (var i = 0; i < @this.Length; i++)
                if (predicate(@this[i]))
                    ++count;
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long LongCount<TPredicate0, T>
            (this NativeArray<T> @this,
            TPredicate0 predicate)
            where TPredicate0 : struct, IRefFunc<T, bool>
            where T : unmanaged
        {
            var count = 0L;
            var enumerable = NativeEnumerable.AsRefEnumerable(@this);
            for (int i = 0; i < @this.Length; i++)
                if (predicate.Calc(ref enumerable[i]))
                    ++count;
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetElementAt<T>
            (this NativeArray<T> @this,
            long index, out T value)
            where T : unmanaged
        {
            if (index < 0 || index >= @this.Length)
            {
                value = default;
                return false;
            }
            value = @this[(int)index];
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetFirst<T>
            (this NativeArray<T> @this,
            out T first)
            where T : unmanaged
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                first = default;
                return false;
            }
            first = @this[0];
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetLast<T>
            (this NativeArray<T> @this,
            out T last)
            where T : unmanaged
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                last = default;
                return false;
            }
            last = @this[@this.Length - 1];
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetSingle<T>
            (this NativeArray<T> @this,
            out T value)
            where T : unmanaged
        {
            if (@this.Length != 1)
            {
                value = default;
                return false;
            }
            value = @this[0];
            return true; ;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetSingle<TPredicate0, T>
            (this NativeArray<T> @this,
            out T value, TPredicate0 predicate)
            where T : unmanaged
            where TPredicate0 : struct, IRefFunc<T, bool>
        {
            value = default;
            var enumerator = GetEnumerator(@this);
            var count = 0;
            while (enumerator.MoveNext())
            {
                if (!predicate.Calc(ref enumerator.Current)) continue;
                value = enumerator.Current;
                if (++count <= 1) continue;
                enumerator.Dispose();
                return false;
            }
            enumerator.Dispose();
            return count == 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetSingle<T>
            (this NativeArray<T> @this,
            out T value, Func<T, bool> predicate)
            where T : unmanaged
        {
            value = default;
            var enumerator = GetEnumerator(@this);
            var count = 0;
            while (enumerator.MoveNext())
            {
                if (!predicate(enumerator.Current)) continue;
                value = enumerator.Current;
                if (++count <= 1) continue;
                enumerator.Dispose();
                return false;
            }
            enumerator.Dispose();
            return count == 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Dictionary<TKey0, TElement0> ToDictionary<TKey0, TElement0, T>
            (this NativeArray<T> @this,
            Func<T, TKey0> keySelector, Func<T, TElement0> elementSelector)
            where T : unmanaged
        {
            var answer = new Dictionary<TKey0, TElement0>();
            var enumerator = GetEnumerator(@this);
            while (enumerator.MoveNext())
            {
                ref var current = ref enumerator.Current;
                answer.Add(keySelector(current), elementSelector(current));
            }
            enumerator.Dispose();
            return answer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Dictionary<TKey0, TElement0> ToDictionary<TKey0, TElement0, TKey0Func, TElement0Func, T>
            (this NativeArray<T> @this,
            TKey0Func keySelector, TElement0Func elementSelector)
            where T : unmanaged
            where TKey0Func : struct, IRefFunc<T, TKey0>
            where TElement0Func : struct, IRefFunc<T, TElement0>
        {
            var answer = new Dictionary<TKey0, TElement0>();
            var enumerator = GetEnumerator(@this);
            while (enumerator.MoveNext())
            {
                ref var current = ref enumerator.Current;
                answer.Add(keySelector.Calc(ref current), elementSelector.Calc(ref current));
            }
            enumerator.Dispose();
            return answer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HashSet<T> ToHashSet<T>
            (this NativeArray<T> @this)
            where T : unmanaged
        {
            var answer = new HashSet<T>();
            var enumerator = GetEnumerator(@this);
            while (enumerator.MoveNext())
                answer.Add(enumerator.Current);
            enumerator.Dispose();
            return answer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HashSet<T> ToHashSet<T>
            (this NativeArray<T> @this,
            IEqualityComparer<T> comparer)
            where T : unmanaged
        {
            var answer = new HashSet<T>(comparer);
            var enumerator = GetEnumerator(@this);
            while (enumerator.MoveNext())
                answer.Add(enumerator.Current);
            enumerator.Dispose();
            return answer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> ToList<T>
            (this NativeArray<T> @this)
            where T : unmanaged
        {
            var answer = new List<T>();
            var enumerator = GetEnumerator(@this);
            while (enumerator.MoveNext())
                answer.Add(enumerator.Current);
            enumerator.Dispose();
            return answer;
        }
        #endregion

        #region Union Default
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator, TEnumerable0, TEnumerator0, T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator, TEnumerable0, TEnumerator0, T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>
            >
            Union<TEnumerable0, TEnumerator0, T>
            (this NativeArray<T> @this, in TEnumerable0 second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator, TEnumerable0, TEnumerator0, T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator, TEnumerable0, TEnumerator0, T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>
            >
            (Concat<TEnumerable0, TEnumerator0, T>(@this, second), default, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    AppendEnumerable<TEnumerable0, TEnumerator0, T>,
                    AppendEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    AppendEnumerable<TEnumerable0, TEnumerator0, T>,
                    AppendEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            Union<TEnumerable0, TEnumerator0, T>
            (this NativeArray<T> @this, in AppendEnumerable<TEnumerable0, TEnumerator0, T>
                    second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => new DistinctEnumerable<
                    ConcatEnumerable<
                        NativeEnumerable<T>,
                        NativeEnumerable<T>.Enumerator,
                        AppendEnumerable<TEnumerable0, TEnumerator0, T>,
                        AppendEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                        T>,
                    ConcatEnumerable<
                        NativeEnumerable<T>,
                        NativeEnumerable<T>.Enumerator,
                        AppendEnumerable<TEnumerable0, TEnumerator0, T>,
                        AppendEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                        T>.Enumerator,
                    T,
                    DefaultEqualityComparer<T>,
                    DefaultGetHashCodeFunc<T>
                >
                (Concat(@this, second), default, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>
            >
            Union<T>
            (this NativeArray<T> @this, in NativeEnumerable<T>
                second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            (Concat(@this, second), default, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            Union<T>
            (this NativeArray<T> @this, NativeArray<T>
                second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            (Concat(@this, NativeEnumerable.AsRefEnumerable(second)), default, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ArrayEnumerable<T>,
                    ArrayEnumerable<T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ArrayEnumerable<T>,
                    ArrayEnumerable<T>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>
            >
            Union<T>
            (this NativeArray<T> @this, in ArrayEnumerable<T>
                second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ArrayEnumerable<T>,
                    ArrayEnumerable<T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ArrayEnumerable<T>,
                    ArrayEnumerable<T>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            (Concat(@this, second), default, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ArrayEnumerable<T>,
                    ArrayEnumerable<T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ArrayEnumerable<T>,
                    ArrayEnumerable<T>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>
            >
            Union<T>
            (this NativeArray<T> @this, T[]
                second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ArrayEnumerable<T>,
                    ArrayEnumerable<T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ArrayEnumerable<T>,
                    ArrayEnumerable<T>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            (Concat(@this, NativeEnumerable.AsRefEnumerable(second)), default, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>
            >
            Union<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>
            (this NativeArray<T> @this, in ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>
                second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEnumerator1 : struct, IRefEnumerator<T>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            (Concat(@this, second), default, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            Union<TEnumerable0, TEnumerator0, T>
            (this NativeArray<T> @this, in DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>
                second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            (Concat(@this, second), default, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer1, TGetHashCodeFunc1>,
                    DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer1, TGetHashCodeFunc1>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer1, TGetHashCodeFunc1>,
                    DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer1, TGetHashCodeFunc1>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            Union<TEnumerable0, TEnumerator0, TEqualityComparer1, TGetHashCodeFunc1, T>
            (this NativeArray<T> @this, in DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer1, TGetHashCodeFunc1>
                second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer1 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc1 : struct, IRefFunc<T, int>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer1, TGetHashCodeFunc1>,
                    DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer1, TGetHashCodeFunc1>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer1, TGetHashCodeFunc1>,
                    DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer1, TGetHashCodeFunc1>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            (Concat(@this, second), default, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    Grouping<TKey0, T>,
                    Grouping<TKey0, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    Grouping<TKey0, T>,
                    Grouping<TKey0, T>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>
            >
            Union<TEnumerable0, TEnumerator0, TKey0, T>
            (this NativeArray<T> @this, in Grouping<TKey0, T>
                second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TKey0 : unmanaged
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    Grouping<TKey0, T>,
                    Grouping<TKey0, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    Grouping<TKey0, T>,
                    Grouping<TKey0, T>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>
            >
            (Concat(@this, second), default, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DistinctEnumerable<
                ConcatEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>, GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator, T>,
                ConcatEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>, GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator, T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            Union<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T0Selector, TEqualityComparer1, T>
            (this NativeArray<T> @this, in GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>
                second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where T1 : unmanaged
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where T0Selector : struct,
            IRefFunc<T0,
                WhereIndexEnumerable<
                    NativeEnumerable<T1>,
                    NativeEnumerable<T1>.Enumerator,
                    T1,
                    GroupJoinPredicate<T1, TKey0, TEqualityComparer1>
                    >,
                T>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T0, TKey0>
            where TKeySelector1 : struct, IRefFunc<T1, TKey0>
            where TEqualityComparer1 : struct, IRefFunc<TKey0, TKey0, bool>
            => new DistinctEnumerable<
                ConcatEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>, GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator, T>,
                ConcatEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>, GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator, T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            (Concat(@this, second), default, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DistinctEnumerable<
                ConcatEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>, JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator, T>,
                ConcatEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>, JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator, T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            Union<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T0Selector, TEqualityComparer1, T>
            (this NativeArray<T> @this, in JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>
                second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where T1 : unmanaged
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where T0Selector : struct, IRefFunc<T0, T1, T>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T0, TKey0>
            where TKeySelector1 : struct, IRefFunc<T1, TKey0>
            where TEqualityComparer1 : struct, IRefFunc<TKey0, TKey0, bool>
            => new DistinctEnumerable<
                ConcatEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>, JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator, T>,
                ConcatEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>, JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator, T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            (Concat(@this, second), default, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer0>,
                    OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer0>,
                    OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer0>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            Union<TEnumerable0, TEnumerator0, TComparer0, T>
            (this NativeArray<T> @this, in OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer0>
                second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TComparer0 : struct, IRefFunc<T, T, int>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer0>,
                    OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer0>,
                    OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer0>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            (Concat(@this, second), default, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    PrependEnumerable<TEnumerable0, TEnumerator0, T>,
                    PrependEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    PrependEnumerable<TEnumerable0, TEnumerator0, T>,
                    PrependEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            Union<TEnumerable0, TEnumerator0, T>
            (this NativeArray<T> @this, in PrependEnumerable<TEnumerable0, TEnumerator0, T>
                second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    PrependEnumerable<TEnumerable0, TEnumerator0, T>,
                    PrependEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    PrependEnumerable<TEnumerable0, TEnumerator0, T>,
                    PrependEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            (Concat(@this, second), default, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    RangeRepeatEnumerable<T, TAction0>,
                    RangeRepeatEnumerable<T, TAction0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    RangeRepeatEnumerable<T, TAction0>,
                    RangeRepeatEnumerable<T, TAction0>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            Union<TAction0, T>
            (this NativeArray<T> @this, in RangeRepeatEnumerable<T, TAction0>
                second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TAction0 : struct, IRangeRepeat<T>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    RangeRepeatEnumerable<T, TAction0>,
                    RangeRepeatEnumerable<T, TAction0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    RangeRepeatEnumerable<T, TAction0>,
                    RangeRepeatEnumerable<T, TAction0>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            (Concat(@this, second), default, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ReverseEnumerable<TEnumerable0, TEnumerator0, T>,
                    ReverseEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ReverseEnumerable<TEnumerable0, TEnumerator0, T>,
                    ReverseEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            Union<TEnumerable0, TEnumerator0, T>
            (this NativeArray<T> @this, in ReverseEnumerable<TEnumerable0, TEnumerator0, T>
                second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ReverseEnumerable<TEnumerable0, TEnumerator0, T>,
                    ReverseEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ReverseEnumerable<TEnumerable0, TEnumerator0, T>,
                    ReverseEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            (Concat(@this, second), default, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>,
                    SelectEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>,
                    SelectEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            Union<TPrevEnumerable0, TPrevEnumerator0, TPrev0, TAction0, T>
            (this NativeArray<T> @this, in SelectEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>
                second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TPrev0 : unmanaged
            where TPrevEnumerator0 : struct, IRefEnumerator<TPrev0>
            where TPrevEnumerable0 : struct, IRefEnumerable<TPrevEnumerator0, TPrev0>
            where TAction0 : struct, IRefAction<TPrev0, T>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>,
                    SelectEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>,
                    SelectEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            (Concat(@this, second), default, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>,
                    SelectIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>,
                    SelectIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            Union<TPrevEnumerable0, TPrevEnumerator0, TPrev0, TAction0, T>
            (this NativeArray<T> @this, in SelectIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>
                second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TPrev0 : unmanaged
            where TPrevEnumerator0 : struct, IRefEnumerator<TPrev0>
            where TPrevEnumerable0 : struct, IRefEnumerable<TPrevEnumerator0, TPrev0>
            where TAction0 : struct, ISelectIndex<TPrev0, T>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>,
                    SelectIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>,
                    SelectIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T, TAction0>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            (Concat(@this, second), default, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectManyEnumerable<TEnumerable0, TEnumerator0, T0, T, TEnumerable1, TEnumerator1, TAction0>,
                    SelectManyEnumerable<TEnumerable0, TEnumerator0, T0, T, TEnumerable1, TEnumerator1, TAction0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectManyEnumerable<TEnumerable0, TEnumerator0, T0, T, TEnumerable1, TEnumerator1, TAction0>,
                    SelectManyEnumerable<TEnumerable0, TEnumerator0, T0, T, TEnumerable1, TEnumerator1, TAction0>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            Union<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, TAction0, T>
            (this NativeArray<T> @this, in SelectManyEnumerable<TEnumerable0, TEnumerator0, T0, T, TEnumerable1, TEnumerator1, TAction0>
                second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator1 : struct, IRefEnumerator<T>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T>
            where TAction0 : struct, IRefAction<T0, TEnumerable1>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectManyEnumerable<TEnumerable0, TEnumerator0, T0, T, TEnumerable1, TEnumerator1, TAction0>,
                    SelectManyEnumerable<TEnumerable0, TEnumerator0, T0, T, TEnumerable1, TEnumerator1, TAction0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectManyEnumerable<TEnumerable0, TEnumerator0, T0, T, TEnumerable1, TEnumerator1, TAction0>,
                    SelectManyEnumerable<TEnumerable0, TEnumerator0, T0, T, TEnumerable1, TEnumerator1, TAction0>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            (Concat(@this, second), default, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TSetOperation0>,
                    SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TSetOperation0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TSetOperation0>,
                    SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TSetOperation0>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>
            >
            Union<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSetOperation0, T>
            (this NativeArray<T> @this, in SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TSetOperation0>
                second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEnumerator1 : struct, IRefEnumerator<T>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T>
            where TSetOperation0 : struct, ISetOperation<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TSetOperation0>,
                    SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TSetOperation0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TSetOperation0>,
                    SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TSetOperation0>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>
            >
            (Concat(@this, second), default, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SkipEnumerable<TEnumerable0, TEnumerator0, T>,
                    TEnumerator0,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SkipEnumerable<TEnumerable0, TEnumerator0, T>,
                    TEnumerator0,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>
            >
            Union<TEnumerable0, TEnumerator0, T>
            (this NativeArray<T> @this, in SkipEnumerable<TEnumerable0, TEnumerator0, T>
                second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SkipEnumerable<TEnumerable0, TEnumerator0, T>,
                    TEnumerator0,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SkipEnumerable<TEnumerable0, TEnumerator0, T>,
                    TEnumerator0,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>
            >
            (Concat(@this, second), default, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TakeEnumerable<TEnumerable0, TEnumerator0, T>,
                    TakeEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TakeEnumerable<TEnumerable0, TEnumerator0, T>,
                    TakeEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>
            >
            Union<TEnumerable0, TEnumerator0, T>
            (this NativeArray<T> @this, in TakeEnumerable<TEnumerable0, TEnumerator0, T>
                second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TakeEnumerable<TEnumerable0, TEnumerator0, T>,
                    TakeEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TakeEnumerable<TEnumerable0, TEnumerator0, T>,
                    TakeEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>
            >
            (Concat(@this, second), default, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>
            >
            Union<TEnumerable0, TEnumerator0, TPredicate0, T>
            (this NativeArray<T> @this, in SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>
                second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TPredicate0 : struct, IRefFunc<T, bool>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    SkipWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>
            >
            (Concat(@this, second), default, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>
            >
            Union<TEnumerable0, TEnumerator0, TPredicate0, T>
            (this NativeArray<T> @this, in TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>
                second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TPredicate0 : struct, IRefFunc<T, bool>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    TakeWhileEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>
            >
            (Concat(@this, second), default, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SkipLastEnumerable<TEnumerable0, TEnumerator0, T>,
                    SkipLastEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SkipLastEnumerable<TEnumerable0, TEnumerator0, T>,
                    SkipLastEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>
            >
            Union<TEnumerable0, TEnumerator0, T>
            (this NativeArray<T> @this, in SkipLastEnumerable<TEnumerable0, TEnumerator0, T>
                second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SkipLastEnumerable<TEnumerable0, TEnumerator0, T>,
                    SkipLastEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SkipLastEnumerable<TEnumerable0, TEnumerator0, T>,
                    SkipLastEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>
            >
            (Concat(@this, second), default, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TakeLastEnumerable<TEnumerable0, TEnumerator0, T>,
                    TakeLastEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TakeLastEnumerable<TEnumerable0, TEnumerator0, T>,
                    TakeLastEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>
            >
            Union<TEnumerable0, TEnumerator0, T>
            (this NativeArray<T> @this, in TakeLastEnumerable<TEnumerable0, TEnumerator0, T>
                second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TakeLastEnumerable<TEnumerable0, TEnumerator0, T>,
                    TakeLastEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TakeLastEnumerable<TEnumerable0, TEnumerator0, T>,
                    TakeLastEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>
            >
            (Concat(@this, second), default, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>
            >
            Union<TEnumerable0, TEnumerator0, TPredicate0, T>
            (this NativeArray<T> @this, in WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>
                second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TPredicate0 : struct, IRefFunc<T, bool>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            (Concat(@this, second), default, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            Union<TEnumerable0, TEnumerator0, TPredicate0, T>
            (this NativeArray<T> @this, in WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>
                second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TPredicate0 : struct, IWhereIndex<T>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            (Concat(@this, second), default, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>,
                    ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>,
                    ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            Union<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TAction0, T>
            (this NativeArray<T> @this, in ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>
                second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where T1 : unmanaged
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where TAction0 : struct, IRefAction<T0, T1, T>
            => new DistinctEnumerable<
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>,
                    ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>.Enumerator,
                    T>,
                ConcatEnumerable<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>,
                    ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>.Enumerator,
                    T>.Enumerator,
                T,
                DefaultEqualityComparer<T>,
                DefaultGetHashCodeFunc<T>>
            (Concat(@this, second), default, default, allocator);
        #endregion

        #region Intersect Default
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                TEnumerable0,
                TEnumerator0,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TEnumerable0,
                    TEnumerator0,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Intersect<TEnumerable0, TEnumerator0, T>
            (this NativeArray<T> @this, in TEnumerable0 second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                TEnumerable0,
                TEnumerator0,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TEnumerable0,
                    TEnumerator0,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                AppendEnumerable<TEnumerable0, TEnumerator0, T>,
                AppendEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    AppendEnumerable<TEnumerable0, TEnumerator0, T>,
                    AppendEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Intersect<TEnumerable0, TEnumerator0, T>
            (this NativeArray<T> @this, in AppendEnumerable<TEnumerable0, TEnumerator0, T> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                AppendEnumerable<TEnumerable0, TEnumerator0, T>,
                AppendEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    AppendEnumerable<TEnumerable0, TEnumerator0, T>,
                    AppendEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Intersect<T>
            (this NativeArray<T> @this, in NativeEnumerable<T> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
           (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Intersect<T>
            (this NativeArray<T> @this, NativeArray<T> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), NativeEnumerable.AsRefEnumerable(second), default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ArrayEnumerable<T>,
                ArrayEnumerable<T>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ArrayEnumerable<T>,
                    ArrayEnumerable<T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Intersect<T>
            (this NativeArray<T> @this, in ArrayEnumerable<T> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ArrayEnumerable<T>,
                ArrayEnumerable<T>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ArrayEnumerable<T>,
                    ArrayEnumerable<T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ArrayEnumerable<T>,
                ArrayEnumerable<T>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ArrayEnumerable<T>,
                    ArrayEnumerable<T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Intersect<T>
            (this NativeArray<T> @this, T[] second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ArrayEnumerable<T>,
                ArrayEnumerable<T>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ArrayEnumerable<T>,
                    ArrayEnumerable<T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
           (NativeEnumerable.AsRefEnumerable(@this), NativeEnumerable.AsRefEnumerable(second), default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Intersect<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>
            (this NativeArray<T> @this, in ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEnumerator1 : struct, IRefEnumerator<T>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Intersect<TEnumerable0, TEnumerator0, T>
            (this NativeArray<T> @this, in DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0>,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0>,
                    DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Intersect<TEnumerable0, TEnumerator0, TEqualityComparer0, TGetHashCodeFunc0, T>
            (this NativeArray<T> @this, in DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0>,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0>,
                    DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                Grouping<TKey0, T>,
                Grouping<TKey0, T>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    Grouping<TKey0, T>,
                    Grouping<TKey0, T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Intersect<TKey0, T>
            (this NativeArray<T> @this, in Grouping<TKey0, T> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TKey0 : unmanaged
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                Grouping<TKey0, T>,
                Grouping<TKey0, T>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    Grouping<TKey0, T>,
                    Grouping<TKey0, T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                    GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Intersect<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TEqualityComparer0, TGetHashCodeFunc0, TKey0, TKeySelector0, TKeySelector1, T0Selector, TEqualityComparer1, T>
            (this NativeArray<T> @this, in GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>
            second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where T1 : unmanaged
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T0, TKey0>
            where TKeySelector1 : struct, IRefFunc<T1, TKey0>
            where T0Selector : struct,
            IRefFunc<T0,
                WhereIndexEnumerable<
                    NativeEnumerable<T1>,
                    NativeEnumerable<T1>.Enumerator,
                    T1,
                    GroupJoinPredicate<T1, TKey0, TEqualityComparer1>
                    >,
                T>
            where TEqualityComparer1 : struct, IRefFunc<TKey0, TKey0, bool>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                    GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >(NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                    JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Intersect<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TEqualityComparer0, TGetHashCodeFunc0, TKey0, TKeySelector0, TKeySelector1, T0Selector, TEqualityComparer1, T>
            (this NativeArray<T> @this, in JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>
            second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where T1 : unmanaged
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T0, TKey0>
            where TKeySelector1 : struct, IRefFunc<T1, TKey0>
            where T0Selector : struct, IRefFunc<T0, T1, T>
            where TEqualityComparer1 : struct, IRefFunc<TKey0, TKey0, bool>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                    JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >(NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1>,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1>,
                    OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Intersect<TEnumerable0, TEnumerator0, TComparer1, T>
            (this NativeArray<T> @this, in OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TComparer1 : struct, IRefFunc<T, T, int>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1>,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1>,
                    OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                PrependEnumerable<TEnumerable0, TEnumerator0, T>,
                PrependEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    PrependEnumerable<TEnumerable0, TEnumerator0, T>,
                    PrependEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Intersect<TEnumerable0, TEnumerator0, T>
            (this NativeArray<T> @this, in PrependEnumerable<TEnumerable0, TEnumerator0, T> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                PrependEnumerable<TEnumerable0, TEnumerator0, T>,
                PrependEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    PrependEnumerable<TEnumerable0, TEnumerator0, T>,
                    PrependEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                RangeRepeatEnumerable<T, TAction0>,
                RangeRepeatEnumerable<T, TAction0>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    RangeRepeatEnumerable<T, TAction0>,
                    RangeRepeatEnumerable<T, TAction0>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Intersect<TAction0, T>
            (this NativeArray<T> @this, in RangeRepeatEnumerable<T, TAction0> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TAction0 : struct, IRangeRepeat<T>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                RangeRepeatEnumerable<T, TAction0>,
                RangeRepeatEnumerable<T, TAction0>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    RangeRepeatEnumerable<T, TAction0>,
                    RangeRepeatEnumerable<T, TAction0>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T>,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ReverseEnumerable<TEnumerable0, TEnumerator0, T>,
                    ReverseEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Intersect<TEnumerable0, TEnumerator0, T>
            (this NativeArray<T> @this, in ReverseEnumerable<TEnumerable0, TEnumerator0, T> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T>,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ReverseEnumerable<TEnumerable0, TEnumerator0, T>,
                    ReverseEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>,
                SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>,
                    SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Intersect<TEnumerable0, TEnumerator0, T0, TAction0, T>
            (this NativeArray<T> @this, in SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TAction0 : struct, IRefAction<T0, T>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>,
                SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>,
                    SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>,
                    SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Intersect<TEnumerable0, TEnumerator0, T0, TAction0, T>
            (this NativeArray<T> @this, in SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TAction0 : struct, ISelectIndex<T0, T>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>,
                    SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Intersect<TEnumerable0, TEnumerator0, TPredicate0, T>
            (this NativeArray<T> @this, in WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TPredicate0 : struct, IRefFunc<T, bool>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Intersect<TEnumerable0, TEnumerator0, TPredicate0, T>
            (this NativeArray<T> @this, in WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TPredicate0 : struct, IWhereIndex<T>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>,
                ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>,
                    ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Intersect<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TAction0, T>
            (this NativeArray<T> @this, in ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where T1 : unmanaged
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where TAction0 : struct, IRefAction<T0, T1, T>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>,
                ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>.Enumerator,
                T,
                IntersectOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>,
                    ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);
        #endregion

        #region Except Default
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                TEnumerable0,
                TEnumerator0,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TEnumerable0,
                    TEnumerator0,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Except<TEnumerable0, TEnumerator0, T>
            (this NativeArray<T> @this, in TEnumerable0 second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                TEnumerable0,
                TEnumerator0,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    TEnumerable0,
                    TEnumerator0,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                AppendEnumerable<TEnumerable0, TEnumerator0, T>,
                AppendEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    AppendEnumerable<TEnumerable0, TEnumerator0, T>,
                    AppendEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Except<TEnumerable0, TEnumerator0, T>
            (this NativeArray<T> @this, in AppendEnumerable<TEnumerable0, TEnumerator0, T> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                AppendEnumerable<TEnumerable0, TEnumerator0, T>,
                AppendEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    AppendEnumerable<TEnumerable0, TEnumerator0, T>,
                    AppendEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Except<T>
            (this NativeArray<T> @this, in NativeEnumerable<T> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
           (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Except<T>
            (this NativeArray<T> @this, NativeArray<T> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), NativeEnumerable.AsRefEnumerable(second), default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ArrayEnumerable<T>,
                ArrayEnumerable<T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ArrayEnumerable<T>,
                    ArrayEnumerable<T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Except<T>
            (this NativeArray<T> @this, in ArrayEnumerable<T> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ArrayEnumerable<T>,
                ArrayEnumerable<T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ArrayEnumerable<T>,
                    ArrayEnumerable<T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ArrayEnumerable<T>,
                ArrayEnumerable<T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ArrayEnumerable<T>,
                    ArrayEnumerable<T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Except<T>
            (this NativeArray<T> @this, T[] second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ArrayEnumerable<T>,
                ArrayEnumerable<T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ArrayEnumerable<T>,
                    ArrayEnumerable<T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
           (NativeEnumerable.AsRefEnumerable(@this), NativeEnumerable.AsRefEnumerable(second), default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Except<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>
            (this NativeArray<T> @this, in ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEnumerator1 : struct, IRefEnumerator<T>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Except<TEnumerable0, TEnumerator0, T>
            (this NativeArray<T> @this, in DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0>,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0>,
                    DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Except<TEnumerable0, TEnumerator0, TEqualityComparer0, TGetHashCodeFunc0, T>
            (this NativeArray<T> @this, in DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0>,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0>,
                    DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer0, TGetHashCodeFunc0>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                Grouping<TKey0, T>,
                Grouping<TKey0, T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    Grouping<TKey0, T>,
                    Grouping<TKey0, T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Except<TKey0, T>
            (this NativeArray<T> @this, in Grouping<TKey0, T> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TKey0 : unmanaged
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                Grouping<TKey0, T>,
                Grouping<TKey0, T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    Grouping<TKey0, T>,
                    Grouping<TKey0, T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                    GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Except<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TEqualityComparer0, TGetHashCodeFunc0, TKey0, TKeySelector0, TKeySelector1, T0Selector, TEqualityComparer1, T>
            (this NativeArray<T> @this, in GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>
            second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where T1 : unmanaged
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T0, TKey0>
            where TKeySelector1 : struct, IRefFunc<T1, TKey0>
            where T0Selector : struct,
            IRefFunc<T0,
                WhereIndexEnumerable<
                    NativeEnumerable<T1>,
                    NativeEnumerable<T1>.Enumerator,
                    T1,
                    GroupJoinPredicate<T1, TKey0, TEqualityComparer1>
                    >,
                T>
            where TEqualityComparer1 : struct, IRefFunc<TKey0, TKey0, bool>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                    GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >(NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                    JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Except<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TEqualityComparer0, TGetHashCodeFunc0, TKey0, TKeySelector0, TKeySelector1, T0Selector, TEqualityComparer1, T>
            (this NativeArray<T> @this, in JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>
            second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where T1 : unmanaged
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where TEqualityComparer0 : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T, int>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T0, TKey0>
            where TKeySelector1 : struct, IRefFunc<T1, TKey0>
            where T0Selector : struct, IRefFunc<T0, T1, T>
            where TEqualityComparer1 : struct, IRefFunc<TKey0, TKey0, bool>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>,
                    JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T, T0Selector, TEqualityComparer1>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >(NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1>,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1>,
                    OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Except<TEnumerable0, TEnumerator0, TComparer1, T>
            (this NativeArray<T> @this, in OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TComparer1 : struct, IRefFunc<T, T, int>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1>,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1>,
                    OrderByEnumerable<TEnumerable0, TEnumerator0, T, TComparer1>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                PrependEnumerable<TEnumerable0, TEnumerator0, T>,
                PrependEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    PrependEnumerable<TEnumerable0, TEnumerator0, T>,
                    PrependEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Except<TEnumerable0, TEnumerator0, T>
            (this NativeArray<T> @this, in PrependEnumerable<TEnumerable0, TEnumerator0, T> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                PrependEnumerable<TEnumerable0, TEnumerator0, T>,
                PrependEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    PrependEnumerable<TEnumerable0, TEnumerator0, T>,
                    PrependEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                RangeRepeatEnumerable<T, TAction0>,
                RangeRepeatEnumerable<T, TAction0>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    RangeRepeatEnumerable<T, TAction0>,
                    RangeRepeatEnumerable<T, TAction0>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Except<TAction0, T>
            (this NativeArray<T> @this, in RangeRepeatEnumerable<T, TAction0> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TAction0 : struct, IRangeRepeat<T>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                RangeRepeatEnumerable<T, TAction0>,
                RangeRepeatEnumerable<T, TAction0>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    RangeRepeatEnumerable<T, TAction0>,
                    RangeRepeatEnumerable<T, TAction0>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T>,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ReverseEnumerable<TEnumerable0, TEnumerator0, T>,
                    ReverseEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Except<TEnumerable0, TEnumerator0, T>
            (this NativeArray<T> @this, in ReverseEnumerable<TEnumerable0, TEnumerator0, T> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T>,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ReverseEnumerable<TEnumerable0, TEnumerator0, T>,
                    ReverseEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>,
                SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>,
                    SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Except<TEnumerable0, TEnumerator0, T0, TAction0, T>
            (this NativeArray<T> @this, in SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TAction0 : struct, IRefAction<T0, T>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>,
                SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>,
                    SelectEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>,
                    SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Except<TEnumerable0, TEnumerator0, T0, TAction0, T>
            (this NativeArray<T> @this, in SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TAction0 : struct, ISelectIndex<T0, T>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>,
                    SelectIndexEnumerable<TEnumerable0, TEnumerator0, T0, T, TAction0>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Except<TEnumerable0, TEnumerator0, TPredicate0, T>
            (this NativeArray<T> @this, in WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TPredicate0 : struct, IRefFunc<T, bool>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Except<TEnumerable0, TEnumerator0, TPredicate0, T>
            (this NativeArray<T> @this, in WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TPredicate0 : struct, IWhereIndex<T>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>,
                    WhereIndexEnumerable<TEnumerable0, TEnumerator0, T, TPredicate0>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>,
                ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>,
                    ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            Except<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TAction0, T>
            (this NativeArray<T> @this, in ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0> second, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where T1 : unmanaged
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where TAction0 : struct, IRefAction<T0, T1, T>
            => new SetOperationEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>,
                ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>.Enumerator,
                T,
                ExceptOperation<
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>,
                    ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, T, TAction0>.Enumerator,
                    T,
                    DefaultOrderByAscending<T>
                >
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, default, allocator);
        #endregion

        #region Join Default
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TEnumerable0,
                TEnumerator0,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, T
            >
            (this NativeArray<T> @this, in TEnumerable0 inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TEnumerable0,
                TEnumerator0,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                AppendEnumerable<TEnumerable0, TEnumerator0, T0>,
                AppendEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, T
            >
            (this NativeArray<T> @this, in AppendEnumerable<TEnumerable0, TEnumerator0, T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                AppendEnumerable<TEnumerable0, TEnumerator0, T0>,
                AppendEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                NativeEnumerable<T0>,
                NativeEnumerable<T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, T
            >
            (this NativeArray<T> @this, in NativeEnumerable<T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                NativeEnumerable<T0>,
                NativeEnumerable<T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                NativeEnumerable<T0>,
                NativeEnumerable<T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, T
            >
            (this NativeArray<T> @this, NativeArray<T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                NativeEnumerable<T0>,
                NativeEnumerable<T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner.AsRefEnumerable(), outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ArrayEnumerable<T0>,
                ArrayEnumerable<T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, T
            >
            (this NativeArray<T> @this, in ArrayEnumerable<T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ArrayEnumerable<T0>,
                ArrayEnumerable<T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ArrayEnumerable<T0>,
                ArrayEnumerable<T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, T
            >
            (this NativeArray<T> @this, T0[] inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ArrayEnumerable<T0>,
                ArrayEnumerable<T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner.AsRefEnumerable(), outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector
                , TEnumerable1, TEnumerator1, T
            >
            (this NativeArray<T> @this, in ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TEnumerator1 : struct, IRefEnumerator<T0>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T0>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, T
            >
            (this NativeArray<T> @this, in DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer1, TGetHashCodeFunc0>,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer1, TGetHashCodeFunc0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector
                , TEqualityComparer1, TGetHashCodeFunc0, T
            >
            (this NativeArray<T> @this, in DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer1, TGetHashCodeFunc0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TEqualityComparer1 : struct, IRefFunc<T0, T0, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T0, int>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer1, TGetHashCodeFunc0>,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer1, TGetHashCodeFunc0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                Grouping<TKey1, T0>,
                Grouping<TKey1, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector
                , TKey1, T
            >
            (this NativeArray<T> @this, in Grouping<TKey1, T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TKey1 : unmanaged
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                Grouping<TKey1, T0>,
                Grouping<TKey1, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SetOperationEnumerable<
                    TEnumerable0,
                    TEnumerator0,
                    TEnumerable1,
                    TEnumerator1,
                    T0,
                    TSetOperation0
                >,
                SetOperationEnumerable<
                    TEnumerable0,
                    TEnumerator0,
                    TEnumerable1,
                    TEnumerator1,
                    T0,
                    TSetOperation0
                >.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector
                , TEnumerable1, TEnumerator1, TSetOperation0, T
            >
            (this NativeArray<T> @this, in SetOperationEnumerable<
                    TEnumerable0,
                    TEnumerator0,
                    TEnumerable1,
                    TEnumerator1,
                    T0,
                    TSetOperation0
                > inner,
            in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TEnumerator1 : struct, IRefEnumerator<T0>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T0>
            where TSetOperation0 : struct, ISetOperation<TEnumerable0,
                    TEnumerator0,
                    TEnumerable1,
                    TEnumerator1,
                    T0>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SetOperationEnumerable<
                    TEnumerable0,
                    TEnumerator0,
                    TEnumerable1,
                    TEnumerator1,
                    T0,
                    TSetOperation0
                >,
                SetOperationEnumerable<
                    TEnumerable0,
                    TEnumerator0,
                    TEnumerable1,
                    TEnumerator1,
                    T0,
                    TSetOperation0
                >.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                GroupJoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey2, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TEqualityComparer1>,
                GroupJoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey2, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TEqualityComparer1>.Enumerator,
                T0,
                TKey0,
                TOuterKeySelector1,
                TInnerKeySelector1,
                T1,
                T1Selector,
                DefaultEqualityComparer<TKey0>
            >
            Join<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                TKey2, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TEqualityComparer1, TKey0,
                TOuterKeySelector1,
                TInnerKeySelector1,
                T1,
                T1Selector,
                T
            >
            (
                this NativeArray<T> @this,
                in GroupJoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey2, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TEqualityComparer1>
                inner,
                in TOuterKeySelector1 outerKeySelector, in TInnerKeySelector1 innerKeySelector,
                in T1Selector resultSelector, Allocator allocator = Allocator.Temp
            )
            where T : unmanaged
            where TOuterSource0 : unmanaged
            where TInnerSource0 : unmanaged
            where TOuterEnumerator0 : struct, IRefEnumerator<TOuterSource0>
            where TInnerEnumerator0 : struct, IRefEnumerator<TInnerSource0>
            where TOuterEnumerable0 : struct, IRefEnumerable<TOuterEnumerator0, TOuterSource0>
            where TInnerEnumerable0 : struct, IRefEnumerable<TInnerEnumerator0, TInnerSource0>
            where T0 : unmanaged
            where TKey2 : unmanaged
            where TKey0 : unmanaged
            where TOuterKeySelector0 : struct, IRefFunc<TOuterSource0, TKey2>
            where TInnerKeySelector0 : struct, IRefFunc<TInnerSource0, TKey2>
            where TEqualityComparer1 : struct, IRefFunc<TKey2, TKey2, bool>
            where T0Selector : struct,
            IRefFunc<TOuterSource0,
                WhereIndexEnumerable<
                    NativeEnumerable<TInnerSource0>,
                    NativeEnumerable<TInnerSource0>.Enumerator,
                    TInnerSource0,
                    GroupJoinPredicate<TInnerSource0, TKey2, TEqualityComparer1>
                    >,
                T0>
            where T1 : unmanaged
            where TOuterKeySelector1 : struct, IRefFunc<T, TKey0>
            where TInnerKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T1Selector : struct, IRefFunc<T, T0, T1>
            =>
            new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                GroupJoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey2, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TEqualityComparer1>,
                GroupJoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey2, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TEqualityComparer1>.Enumerator,
                T0,
                TKey0,
                TOuterKeySelector1,
                TInnerKeySelector1,
                T1,
                T1Selector,
                DefaultEqualityComparer<TKey0>
            >(NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                JoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey2, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>,
                JoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey2, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>.Enumerator,
                T0,
                TKey0,
                TOuterKeySelector1,
                TInnerKeySelector1,
                T1,
                T1Selector,
                DefaultEqualityComparer<TKey0>
            >
            Join<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey2, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0, TKey0,
                TOuterKeySelector1,
                TInnerKeySelector1,
                T1,
                T1Selector,
                T
            >
            (
                this NativeArray<T> @this,
                in JoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey2, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>
                inner,
                in TOuterKeySelector1 outerKeySelector, in TInnerKeySelector1 innerKeySelector,
                in T1Selector resultSelector, Allocator allocator = Allocator.Temp
            )
            where T : unmanaged
            where TOuterSource0 : unmanaged
            where TInnerSource0 : unmanaged
            where TOuterEnumerator0 : struct, IRefEnumerator<TOuterSource0>
            where TInnerEnumerator0 : struct, IRefEnumerator<TInnerSource0>
            where TOuterEnumerable0 : struct, IRefEnumerable<TOuterEnumerator0, TOuterSource0>
            where TInnerEnumerable0 : struct, IRefEnumerable<TInnerEnumerator0, TInnerSource0>
            where T0 : unmanaged
            where TKey2 : unmanaged
            where TKey0 : unmanaged
            where TOuterKeySelector0 : struct, IRefFunc<TOuterSource0, TKey2>
            where TInnerKeySelector0 : struct, IRefFunc<TInnerSource0, TKey2>
            where TKeyEqualityComparer0 : struct, IRefFunc<TKey2, TKey2, bool>
            where T0Selector : struct, IRefFunc<TOuterSource0, TInnerSource0, T0>
            where T1 : unmanaged
            where TOuterKeySelector1 : struct, IRefFunc<T, TKey0>
            where TInnerKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T1Selector : struct, IRefFunc<T, T0, T1>
            =>
            new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                JoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey2, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>,
                JoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey2, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>.Enumerator,
                T0,
                TKey0,
                TOuterKeySelector1,
                TInnerKeySelector1,
                T1,
                T1Selector,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector
                , TComparer0, T
            >
            (this NativeArray<T> @this, in OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TComparer0 : struct, IRefFunc<T0, T0, int>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                PrependEnumerable<TEnumerable0, TEnumerator0, T0>,
                PrependEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, T
            >
            (this NativeArray<T> @this, in PrependEnumerable<TEnumerable0, TEnumerator0, T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                PrependEnumerable<TEnumerable0, TEnumerator0, T0>,
                PrependEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                RangeRepeatEnumerable<T0, TAction0>,
                RangeRepeatEnumerable<T0, TAction0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector
                , TAction0, T
            >
            (this NativeArray<T> @this, in RangeRepeatEnumerable<T0, TAction0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TAction0 : struct, IRangeRepeat<T0>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                RangeRepeatEnumerable<T0, TAction0>,
                RangeRepeatEnumerable<T0, TAction0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T0>,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, T
            >
            (this NativeArray<T> @this, in ReverseEnumerable<TEnumerable0, TEnumerator0, T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T0>,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector
                , TPrev0, TAction0, T
            >
            (this NativeArray<T> @this, in SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TPrev0 : unmanaged
            where TAction0 : struct, IRefAction<TPrev0, T0>
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<TPrev0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrev0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector
                , TPrev0, TAction0, T
            >
            (this NativeArray<T> @this, in SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TPrev0 : unmanaged
            where TAction0 : struct, ISelectIndex<TPrev0, T0>
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<TPrev0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrev0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction0>,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector
                , TEnumerable1, TEnumerator1, TPrev0, TAction0, T
            >
            (this NativeArray<T> @this, in SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TPrev0 : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<TPrev0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrev0>
            where TAction0 : struct, IRefAction<TPrev0, TEnumerable1>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TEnumerator1 : struct, IRefEnumerator<T0>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T0>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction0>,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipEnumerable<TEnumerable0, TEnumerator0, T0>,
                TEnumerator0,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, T
            >
            (this NativeArray<T> @this, in SkipEnumerable<TEnumerable0, TEnumerator0, T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipEnumerable<TEnumerable0, TEnumerator0, T0>,
                TEnumerator0,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeEnumerable<TEnumerable0, TEnumerator0, T0>,
                TakeEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, T
            >
            (this NativeArray<T> @this, in TakeEnumerable<TEnumerable0, TEnumerator0, T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeEnumerable<TEnumerable0, TEnumerator0, T0>,
                TakeEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector
                , TPredicate0, T
            >
            (this NativeArray<T> @this, in SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TPredicate0 : struct, IRefFunc<T0, bool>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector
                , TPredicate0, T
            >
            (this NativeArray<T> @this, in TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TPredicate0 : struct, IRefFunc<T0, bool>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, T
            >
            (this NativeArray<T> @this, in SkipLastEnumerable<TEnumerable0, TEnumerator0, T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector, T
            >
            (this NativeArray<T> @this, in TakeLastEnumerable<TEnumerable0, TEnumerator0, T0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector
                , TPredicate0, T
            >
            (this NativeArray<T> @this, in WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TPredicate0 : struct, IRefFunc<T0, bool>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector
                , TPredicate0, T
            >
            (this NativeArray<T> @this, in WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TPredicate0 : struct, IWhereIndex<T0>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ZipEnumerable<TEnumerable0, TEnumerator0, T2, TEnumerable1, TEnumerator1, T3, T0, TAction0>,
                ZipEnumerable<TEnumerable0, TEnumerator0, T2, TEnumerable1, TEnumerator1, T3, T0, TAction0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeySelector1, T1, T0Selector
                , TEnumerable1, TEnumerator1, T2, T3, TAction0, T
            >
            (this NativeArray<T> @this, in ZipEnumerable<TEnumerable0, TEnumerator0, T2, TEnumerable1, TEnumerator1, T3, T0, TAction0> inner, in TKeySelector0 outerKeySelector, in TKeySelector1 innerKeySelector, in T0Selector resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where TAction0 : struct, IRefAction<T2, T3, T0>
            where TEnumerator0 : struct, IRefEnumerator<T2>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T2>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T, TKey0>
            where TKeySelector1 : struct, IRefFunc<T0, TKey0>
            where T0Selector : struct, IRefFunc<T, T0, T1>
            where TEnumerator1 : struct, IRefEnumerator<T3>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T3>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ZipEnumerable<TEnumerable0, TEnumerator0, T2, TEnumerable1, TEnumerator1, T3, T0, TAction0>,
                ZipEnumerable<TEnumerable0, TEnumerator0, T2, TEnumerable1, TEnumerator1, T3, T0, TAction0>.Enumerator,
                T0,
                TKey0,
                TKeySelector0,
                TKeySelector1,
                T1,
                T0Selector,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);
        #endregion

        #region Join Default Func
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TEnumerable0,
                TEnumerator0,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, T1, T
            >
            (this NativeArray<T> @this, in TEnumerable0 inner, in Func<T, TKey0> outerKeySelector, in Func<T0, TKey0> innerKeySelector, in Func<T, T0, T1> resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TEnumerable0,
                TEnumerator0,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                AppendEnumerable<TEnumerable0, TEnumerator0, T0>,
                AppendEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, T1, T
            >
            (this NativeArray<T> @this, in AppendEnumerable<TEnumerable0, TEnumerator0, T0> inner, in DelegateFuncToStructOperatorFunc<T, TKey0> outerKeySelector, in DelegateFuncToStructOperatorFunc<T0, TKey0> innerKeySelector, in Func<T, T0, T1> resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                AppendEnumerable<TEnumerable0, TEnumerator0, T0>,
                AppendEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                NativeEnumerable<T0>,
                NativeEnumerable<T0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, T1, T
            >
            (this NativeArray<T> @this, in NativeEnumerable<T0> inner, in DelegateFuncToStructOperatorFunc<T, TKey0> outerKeySelector, in DelegateFuncToStructOperatorFunc<T0, TKey0> innerKeySelector, in Func<T, T0, T1> resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                NativeEnumerable<T0>,
                NativeEnumerable<T0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                NativeEnumerable<T0>,
                NativeEnumerable<T0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, T1, T
            >
            (this NativeArray<T> @this, NativeArray<T0> inner, in DelegateFuncToStructOperatorFunc<T, TKey0> outerKeySelector, in DelegateFuncToStructOperatorFunc<T0, TKey0> innerKeySelector, in Func<T, T0, T1> resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                NativeEnumerable<T0>,
                NativeEnumerable<T0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner.AsRefEnumerable(), outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ArrayEnumerable<T0>,
                ArrayEnumerable<T0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, T1, T
            >
            (this NativeArray<T> @this, in ArrayEnumerable<T0> inner, in DelegateFuncToStructOperatorFunc<T, TKey0> outerKeySelector, in DelegateFuncToStructOperatorFunc<T0, TKey0> innerKeySelector, in Func<T, T0, T1> resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ArrayEnumerable<T0>,
                ArrayEnumerable<T0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ArrayEnumerable<T0>,
                ArrayEnumerable<T0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, T1, T
            >
            (this NativeArray<T> @this, T0[] inner, in DelegateFuncToStructOperatorFunc<T, TKey0> outerKeySelector, in DelegateFuncToStructOperatorFunc<T0, TKey0> innerKeySelector, in Func<T, T0, T1> resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ArrayEnumerable<T0>,
                ArrayEnumerable<T0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner.AsRefEnumerable(), outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, T1
                , TEnumerable1, TEnumerator1, T
            >
            (this NativeArray<T> @this, in ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0> inner, in DelegateFuncToStructOperatorFunc<T, TKey0> outerKeySelector, in DelegateFuncToStructOperatorFunc<T0, TKey0> innerKeySelector, in Func<T, T0, T1> resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TEnumerator1 : struct, IRefEnumerator<T0>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T0>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, T1, T
            >
            (this NativeArray<T> @this, in DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0> inner, in DelegateFuncToStructOperatorFunc<T, TKey0> outerKeySelector, in DelegateFuncToStructOperatorFunc<T0, TKey0> innerKeySelector, in Func<T, T0, T1> resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer1, TGetHashCodeFunc0>,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer1, TGetHashCodeFunc0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, T1
                , TEqualityComparer1, TGetHashCodeFunc0, T
            >
            (this NativeArray<T> @this, in DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer1, TGetHashCodeFunc0> inner, in DelegateFuncToStructOperatorFunc<T, TKey0> outerKeySelector, in DelegateFuncToStructOperatorFunc<T0, TKey0> innerKeySelector, in Func<T, T0, T1> resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TEqualityComparer1 : struct, IRefFunc<T0, T0, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T0, int>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer1, TGetHashCodeFunc0>,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer1, TGetHashCodeFunc0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                Grouping<TKey1, T0>,
                Grouping<TKey1, T0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, T1
                , TKey1, T
            >
            (this NativeArray<T> @this, in Grouping<TKey1, T0> inner, in DelegateFuncToStructOperatorFunc<T, TKey0> outerKeySelector, in DelegateFuncToStructOperatorFunc<T0, TKey0> innerKeySelector, in Func<T, T0, T1> resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKey1 : unmanaged
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                Grouping<TKey1, T0>,
                Grouping<TKey1, T0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SetOperationEnumerable<
                    TEnumerable0,
                    TEnumerator0,
                    TEnumerable1,
                    TEnumerator1,
                    T0,
                    TSetOperation0
                >,
                SetOperationEnumerable<
                    TEnumerable0,
                    TEnumerator0,
                    TEnumerable1,
                    TEnumerator1,
                    T0,
                    TSetOperation0
                >.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, T1
                , TEnumerable1, TEnumerator1, TSetOperation0, T
            >
            (this NativeArray<T> @this, in SetOperationEnumerable<
                    TEnumerable0,
                    TEnumerator0,
                    TEnumerable1,
                    TEnumerator1,
                    T0,
                    TSetOperation0
                > inner,
            in DelegateFuncToStructOperatorFunc<T, TKey0> outerKeySelector, in DelegateFuncToStructOperatorFunc<T0, TKey0> innerKeySelector, in Func<T, T0, T1> resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TEnumerator1 : struct, IRefEnumerator<T0>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T0>
            where TSetOperation0 : struct, ISetOperation<TEnumerable0,
                    TEnumerator0,
                    TEnumerable1,
                    TEnumerator1,
                    T0>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SetOperationEnumerable<
                    TEnumerable0,
                    TEnumerator0,
                    TEnumerable1,
                    TEnumerator1,
                    T0,
                    TSetOperation0
                >,
                SetOperationEnumerable<
                    TEnumerable0,
                    TEnumerator0,
                    TEnumerable1,
                    TEnumerator1,
                    T0,
                    TSetOperation0
                >.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                GroupJoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey2, TOuterKeySelector0, TInnerKeySelector0, T0, T2Selector, TEqualityComparer1>,
                GroupJoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey2, TOuterKeySelector0, TInnerKeySelector0, T0, T2Selector, TEqualityComparer1>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            Join<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                TKey2, TOuterKeySelector0, TInnerKeySelector0, T0, T2Selector, TEqualityComparer1, TKey0,
                T1, T
            >
            (
                this NativeArray<T> @this,
                in GroupJoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey2, TOuterKeySelector0, TInnerKeySelector0, T0, T2Selector, TEqualityComparer1>
                inner,
                in Func<T, TKey0> outerKeySelector,
                in Func<T0, TKey0> innerKeySelector,
                in Func<T, T0, T1> resultSelector,
                Allocator allocator = Allocator.Temp
            )
            where T : unmanaged
            where TOuterSource0 : unmanaged
            where TInnerSource0 : unmanaged
            where TOuterEnumerator0 : struct, IRefEnumerator<TOuterSource0>
            where TInnerEnumerator0 : struct, IRefEnumerator<TInnerSource0>
            where TOuterEnumerable0 : struct, IRefEnumerable<TOuterEnumerator0, TOuterSource0>
            where TInnerEnumerable0 : struct, IRefEnumerable<TInnerEnumerator0, TInnerSource0>
            where T0 : unmanaged
            where TKey2 : unmanaged
            where TKey0 : unmanaged
            where TOuterKeySelector0 : struct, IRefFunc<TOuterSource0, TKey2>
            where TInnerKeySelector0 : struct, IRefFunc<TInnerSource0, TKey2>
            where TEqualityComparer1 : struct, IRefFunc<TKey2, TKey2, bool>
            where T2Selector : struct,
            IRefFunc<TOuterSource0,
                WhereIndexEnumerable<
                    NativeEnumerable<TInnerSource0>,
                    NativeEnumerable<TInnerSource0>.Enumerator,
                    TInnerSource0,
                    GroupJoinPredicate<TInnerSource0, TKey2, TEqualityComparer1>
                    >,
                T0>
            where T1 : unmanaged
            =>
            new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                GroupJoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey2, TOuterKeySelector0, TInnerKeySelector0, T0, T2Selector, TEqualityComparer1>,
                GroupJoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey2, TOuterKeySelector0, TInnerKeySelector0, T0, T2Selector, TEqualityComparer1>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >(NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                JoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey2, TOuterKeySelector0, TInnerKeySelector0, T0, T2Selector, TKeyEqualityComparer0>,
                JoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey2, TOuterKeySelector0, TInnerKeySelector0, T0, T2Selector, TKeyEqualityComparer0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TOuterEnumerable0,
                TOuterEnumerator0,
                TOuterSource0,
                TInnerEnumerable0,
                TInnerEnumerator0,
                TInnerSource0,
                TKey2,
                TOuterKeySelector0,
                TInnerKeySelector0,
                T0,
                T2Selector,
                TKeyEqualityComparer0,
                TKey0,
                T1,
                T
            >
            (
                this NativeArray<T> @this,
                in JoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey2, TOuterKeySelector0, TInnerKeySelector0, T0, T2Selector, TKeyEqualityComparer0>
                inner,
                in Func<T, TKey0> outerKeySelector,
                in Func<T0, TKey0> innerKeySelector,
                in Func<T, T0, T1> resultSelector,
                Allocator allocator = Allocator.Temp
            )
            where T : unmanaged
            where TOuterSource0 : unmanaged
            where TInnerSource0 : unmanaged
            where TOuterEnumerator0 : struct, IRefEnumerator<TOuterSource0>
            where TInnerEnumerator0 : struct, IRefEnumerator<TInnerSource0>
            where TOuterEnumerable0 : struct, IRefEnumerable<TOuterEnumerator0, TOuterSource0>
            where TInnerEnumerable0 : struct, IRefEnumerable<TInnerEnumerator0, TInnerSource0>
            where T0 : unmanaged
            where TKey2 : unmanaged
            where TKey0 : unmanaged
            where TOuterKeySelector0 : struct, IRefFunc<TOuterSource0, TKey2>
            where TInnerKeySelector0 : struct, IRefFunc<TInnerSource0, TKey2>
            where TKeyEqualityComparer0 : struct, IRefFunc<TKey2, TKey2, bool>
            where T2Selector : struct, IRefFunc<TOuterSource0, TInnerSource0, T0>
            where T1 : unmanaged
            =>
            new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                JoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey2, TOuterKeySelector0, TInnerKeySelector0, T0, T2Selector, TKeyEqualityComparer0>,
                JoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0,
                    TKey2, TOuterKeySelector0, TInnerKeySelector0, T0, T2Selector, TKeyEqualityComparer0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, T1
                , TComparer0, T
            >
            (this NativeArray<T> @this, in OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0> inner, in DelegateFuncToStructOperatorFunc<T, TKey0> outerKeySelector, in DelegateFuncToStructOperatorFunc<T0, TKey0> innerKeySelector, in Func<T, T0, T1> resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TComparer0 : struct, IRefFunc<T0, T0, int>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                PrependEnumerable<TEnumerable0, TEnumerator0, T0>,
                PrependEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, T1, T
            >
            (this NativeArray<T> @this, in PrependEnumerable<TEnumerable0, TEnumerator0, T0> inner, in DelegateFuncToStructOperatorFunc<T, TKey0> outerKeySelector, in DelegateFuncToStructOperatorFunc<T0, TKey0> innerKeySelector, in Func<T, T0, T1> resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                PrependEnumerable<TEnumerable0, TEnumerator0, T0>,
                PrependEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                RangeRepeatEnumerable<T0, TAction0>,
                RangeRepeatEnumerable<T0, TAction0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                T0, TKey0, T1
                , TAction0, T
            >
            (this NativeArray<T> @this, in RangeRepeatEnumerable<T0, TAction0> inner, in DelegateFuncToStructOperatorFunc<T, TKey0> outerKeySelector, in DelegateFuncToStructOperatorFunc<T0, TKey0> innerKeySelector, in Func<T, T0, T1> resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TKey0 : unmanaged
            where TAction0 : struct, IRangeRepeat<T0>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                RangeRepeatEnumerable<T0, TAction0>,
                RangeRepeatEnumerable<T0, TAction0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T0>,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, T1, T
            >
            (this NativeArray<T> @this, in ReverseEnumerable<TEnumerable0, TEnumerator0, T0> inner, in DelegateFuncToStructOperatorFunc<T, TKey0> outerKeySelector, in DelegateFuncToStructOperatorFunc<T0, TKey0> innerKeySelector, in Func<T, T0, T1> resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T0>,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, T1
                , TPrev0, TAction0, T
            >
            (this NativeArray<T> @this, in SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0> inner, in DelegateFuncToStructOperatorFunc<T, TKey0> outerKeySelector, in DelegateFuncToStructOperatorFunc<T0, TKey0> innerKeySelector, in Func<T, T0, T1> resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TPrev0 : unmanaged
            where TAction0 : struct, IRefAction<TPrev0, T0>
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<TPrev0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrev0>
            where TKey0 : unmanaged
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, T1
                , TPrev0, TAction0, T
            >
            (this NativeArray<T> @this, in SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0> inner, in DelegateFuncToStructOperatorFunc<T, TKey0> outerKeySelector, in DelegateFuncToStructOperatorFunc<T0, TKey0> innerKeySelector, in Func<T, T0, T1> resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TPrev0 : unmanaged
            where TAction0 : struct, ISelectIndex<TPrev0, T0>
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<TPrev0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrev0>
            where TKey0 : unmanaged
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction0>,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, T1
                , TEnumerable1, TEnumerator1, TPrev0, TAction0, T
            >
            (this NativeArray<T> @this, in SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction0> inner, in DelegateFuncToStructOperatorFunc<T, TKey0> outerKeySelector, in DelegateFuncToStructOperatorFunc<T0, TKey0> innerKeySelector, in Func<T, T0, T1> resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TPrev0 : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<TPrev0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrev0>
            where TAction0 : struct, IRefAction<TPrev0, TEnumerable1>
            where TKey0 : unmanaged
            where TEnumerator1 : struct, IRefEnumerator<T0>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T0>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction0>,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipEnumerable<TEnumerable0, TEnumerator0, T0>,
                TEnumerator0,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, T1, T
            >
            (this NativeArray<T> @this, in SkipEnumerable<TEnumerable0, TEnumerator0, T0> inner, in DelegateFuncToStructOperatorFunc<T, TKey0> outerKeySelector, in DelegateFuncToStructOperatorFunc<T0, TKey0> innerKeySelector, in Func<T, T0, T1> resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipEnumerable<TEnumerable0, TEnumerator0, T0>,
                TEnumerator0,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeEnumerable<TEnumerable0, TEnumerator0, T0>,
                TakeEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, T1, T
            >
            (this NativeArray<T> @this, in TakeEnumerable<TEnumerable0, TEnumerator0, T0> inner, in DelegateFuncToStructOperatorFunc<T, TKey0> outerKeySelector, in DelegateFuncToStructOperatorFunc<T0, TKey0> innerKeySelector, in Func<T, T0, T1> resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeEnumerable<TEnumerable0, TEnumerator0, T0>,
                TakeEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, T1
                , TPredicate0, T
            >
            (this NativeArray<T> @this, in SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0> inner, in DelegateFuncToStructOperatorFunc<T, TKey0> outerKeySelector, in DelegateFuncToStructOperatorFunc<T0, TKey0> innerKeySelector, in Func<T, T0, T1> resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TPredicate0 : struct, IRefFunc<T0, bool>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, T1
                , TPredicate0, T
            >
            (this NativeArray<T> @this, in TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0> inner, in DelegateFuncToStructOperatorFunc<T, TKey0> outerKeySelector, in DelegateFuncToStructOperatorFunc<T0, TKey0> innerKeySelector, in Func<T, T0, T1> resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TPredicate0 : struct, IRefFunc<T0, bool>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, T1, T
            >
            (this NativeArray<T> @this, in SkipLastEnumerable<TEnumerable0, TEnumerator0, T0> inner, in DelegateFuncToStructOperatorFunc<T, TKey0> outerKeySelector, in DelegateFuncToStructOperatorFunc<T0, TKey0> innerKeySelector, in Func<T, T0, T1> resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, T1, T
            >
            (this NativeArray<T> @this, in TakeLastEnumerable<TEnumerable0, TEnumerator0, T0> inner, in DelegateFuncToStructOperatorFunc<T, TKey0> outerKeySelector, in DelegateFuncToStructOperatorFunc<T0, TKey0> innerKeySelector, in Func<T, T0, T1> resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, T1
                , TPredicate0, T
            >
            (this NativeArray<T> @this, in WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0> inner, in DelegateFuncToStructOperatorFunc<T, TKey0> outerKeySelector, in DelegateFuncToStructOperatorFunc<T0, TKey0> innerKeySelector, in Func<T, T0, T1> resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TPredicate0 : struct, IRefFunc<T0, bool>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, T1
                , TPredicate0, T
            >
            (this NativeArray<T> @this, in WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0> inner, in DelegateFuncToStructOperatorFunc<T, TKey0> outerKeySelector, in DelegateFuncToStructOperatorFunc<T0, TKey0> innerKeySelector, in Func<T, T0, T1> resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TPredicate0 : struct, IWhereIndex<T0>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ZipEnumerable<TEnumerable0, TEnumerator0, T2, TEnumerable1, TEnumerator1, T3, T0, TAction0>,
                ZipEnumerable<TEnumerable0, TEnumerator0, T2, TEnumerable1, TEnumerator1, T3, T0, TAction0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            Join<
                TEnumerable0, TEnumerator0, T0, TKey0, T1
                , TEnumerable1, TEnumerator1, T2, T3, TAction0, T
            >
            (this NativeArray<T> @this, in ZipEnumerable<TEnumerable0, TEnumerator0, T2, TEnumerable1, TEnumerator1, T3, T0, TAction0> inner, in DelegateFuncToStructOperatorFunc<T, TKey0> outerKeySelector, in DelegateFuncToStructOperatorFunc<T0, TKey0> innerKeySelector, in Func<T, T0, T1> resultSelector, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
            where TAction0 : struct, IRefAction<T2, T3, T0>
            where TEnumerator0 : struct, IRefEnumerator<T2>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T2>
            where TKey0 : unmanaged
            where TEnumerator1 : struct, IRefEnumerator<T3>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T3>
            => new JoinEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ZipEnumerable<TEnumerable0, TEnumerator0, T2, TEnumerable1, TEnumerator1, T3, T0, TAction0>,
                ZipEnumerable<TEnumerable0, TEnumerator0, T2, TEnumerable1, TEnumerator1, T3, T0, TAction0>.Enumerator,
                T0,
                TKey0,
                DelegateFuncToStructOperatorFunc<T, TKey0>,
                DelegateFuncToStructOperatorFunc<T0, TKey0>,
                T1,
                DelegateFuncToStructOperatorFunc<T, T0, T1>,
                DefaultEqualityComparer<TKey0>
            >
            (NativeEnumerable.AsRefEnumerable(@this), inner, outerKeySelector, innerKeySelector, resultSelector, default, allocator);
        #endregion

        #region Zip Function
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TEnumerable0,
                TEnumerator0,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            Zip<TEnumerable0, TEnumerator0, T0, TResult1, T>
            (this NativeArray<T> @this, in TEnumerable0 second, Func<T, T0, TResult1> action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TResult1 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TEnumerable0,
                TEnumerator0,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                AppendEnumerable<TEnumerable0, TEnumerator0, T0>,
                AppendEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            Zip<TEnumerable0, TEnumerator0, T0, TResult1, T>
            (this NativeArray<T> @this, in AppendEnumerable<TEnumerable0, TEnumerator0, T0>
            second, Func<T, T0, TResult1> action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TResult1 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                AppendEnumerable<TEnumerable0, TEnumerator0, T0>,
                AppendEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            Zip<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0, TResult1, T>
            (this NativeArray<T> @this, in ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>
            second, Func<T, T0, TResult1> action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T0>
            where TEnumerator1 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TResult1 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            Zip<TEnumerable0, TEnumerator0, T0, TResult1, T>
            (this NativeArray<T> @this, in DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>
            second, Func<T, T0, TResult1> action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TResult1 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer0, TGetHashCodeFunc0>,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer0, TGetHashCodeFunc0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            Zip<TEnumerable0, TEnumerator0, T0, TEqualityComparer0, TGetHashCodeFunc0, TResult1, T>
            (this NativeArray<T> @this, in DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer0, TGetHashCodeFunc0>
            second, Func<T, T0, TResult1> action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TEqualityComparer0 : struct, IRefFunc<T0, T0, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T0, int>
            where TResult1 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer0, TGetHashCodeFunc0>,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer0, TGetHashCodeFunc0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                Grouping<TKey0, T0>,
                Grouping<TKey0, T0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            Zip<TKey0, T0, TResult1, T>
            (this NativeArray<T> @this, in Grouping<TKey0, T0>
            second, Func<T, T0, TResult1> action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TKey0 : unmanaged
            where T0 : unmanaged
            where TResult1 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                Grouping<TKey0, T0>,
                Grouping<TKey0, T0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T2, T0Selector, TEqualityComparer1>,
                GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T2, T0Selector, TEqualityComparer1>.Enumerator,
                T2,
                TResult1,
                DelegateFuncToAction<T, T2, TResult1>
            >
            Zip<T2, TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TComparer0, TKey0, TKeySelector0, TKeySelector1, T0Selector, TEqualityComparer1, TResult1, T>
            (this NativeArray<T> @this, in GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T2, T0Selector, TEqualityComparer1>
            second, Func<T, T2, TResult1> action, T firstDefaultValue = default, T2 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where T2 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T0, TKey0>
            where TKeySelector1 : struct, IRefFunc<T1, TKey0>
            where TEqualityComparer1 : struct, IRefFunc<TKey0, TKey0, bool>
            where T0Selector : struct, IRefFunc<T0, WhereIndexEnumerable<NativeEnumerable<T1>, NativeEnumerable<T1>.Enumerator, T1, GroupJoinPredicate<T1, TKey0, TEqualityComparer1>>, T2>
            where TResult1 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T2, T0Selector, TEqualityComparer1>,
                GroupJoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T2, T0Selector, TEqualityComparer1>.Enumerator,
                T2,
                TResult1,
                DelegateFuncToAction<T, T2, TResult1>
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T2, T0Selector, TEqualityComparer1>,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T2, T0Selector, TEqualityComparer1>.Enumerator,
                T2,
                TResult1,
                DelegateFuncToAction<T, T2, TResult1>
            >
            Zip<T2, TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TComparer0, TKey0, TKeySelector0, TKeySelector1, T0Selector, TEqualityComparer1, TResult1, T>
            (this NativeArray<T> @this, in JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T2, T0Selector, TEqualityComparer1>
            second, Func<T, T2, TResult1> action, T firstDefaultValue = default, T2 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where T1 : unmanaged
            where T2 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T0, TKey0>
            where TKeySelector1 : struct, IRefFunc<T1, TKey0>
            where TEqualityComparer1 : struct, IRefFunc<TKey0, TKey0, bool>
            where T0Selector : struct, IRefFunc<T0, T1, T2>
            where TResult1 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T2, T0Selector, TEqualityComparer1>,
                JoinEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TKey0, TKeySelector0, TKeySelector1, T2, T0Selector, TEqualityComparer1>.Enumerator,
                T2,
                TResult1,
                DelegateFuncToAction<T, T2, TResult1>
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            Zip<TEnumerable0, TEnumerator0, T0, TComparer0, TResult1, T>
            (this NativeArray<T> @this, in OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>
            second, Func<T, T0, TResult1> action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TComparer0 : struct, IRefFunc<T0, T0, int>
            where TResult1 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                PrependEnumerable<TEnumerable0, TEnumerator0, T0>,
                PrependEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            Zip<TEnumerable0, TEnumerator0, T0, TResult1, T>
            (this NativeArray<T> @this, in PrependEnumerable<TEnumerable0, TEnumerator0, T0>
                second, Func<T, T0, TResult1> action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TResult1 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                PrependEnumerable<TEnumerable0, TEnumerator0, T0>,
                PrependEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >(NativeEnumerable.AsRefEnumerable(@this), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                RangeRepeatEnumerable<T0, TAction0>,
                RangeRepeatEnumerable<T0, TAction0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            Zip<T0, TAction0, TResult1, T>
            (this NativeArray<T> @this, in RangeRepeatEnumerable<T0, TAction0>
            second, Func<T, T0, TResult1> action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where TAction0 : struct, IRangeRepeat<T0>
            where TResult1 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                RangeRepeatEnumerable<T0, TAction0>,
                RangeRepeatEnumerable<T0, TAction0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T0>,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            Zip<TEnumerable0, TEnumerator0, T0, TResult1, T>
            (this NativeArray<T> @this, in ReverseEnumerable<TEnumerable0, TEnumerator0, T0>
                second, Func<T, T0, TResult1> action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TResult1 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T0>,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            Zip<TEnumerable0, TEnumerator0, T0, TPrev0, TAction0, TResult1, T>
            (this NativeArray<T> @this, in SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>
            second, Func<T, T0, TResult1> action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrev0>
            where TEnumerator0 : struct, IRefEnumerator<TPrev0>
            where T0 : unmanaged
            where TAction0 : struct, IRefAction<TPrev0, T0>
            where TPrev0 : unmanaged
            where TResult1 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            Zip<TEnumerable0, TEnumerator0, T0, TPrev0, TAction0, TResult1, T>
            (this NativeArray<T> @this, in SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>
            second, Func<T, T0, TResult1> action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrev0>
            where TEnumerator0 : struct, IRefEnumerator<TPrev0>
            where T0 : unmanaged
            where TAction0 : struct, ISelectIndex<TPrev0, T0>
            where TPrev0 : unmanaged
            where TResult1 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TAction0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction0>,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            Zip<TEnumerable0, TEnumerator0, T0, TPrev0, TEnumerable1, TEnumerator1, TAction0, TResult1, T>
            (this NativeArray<T> @this, in SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction0>
            second, Func<T, T0, TResult1> action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrev0>
            where TEnumerator0 : struct, IRefEnumerator<TPrev0>
            where T0 : unmanaged
            where TAction0 : struct, IRefAction<TPrev0, TEnumerable1>
            where TPrev0 : unmanaged
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T0>
            where TEnumerator1 : struct, IRefEnumerator<T0>
            where TResult1 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction0>,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, TEnumerable1, TEnumerator1, TAction0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0, TSetOperation0>,
                SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0, TSetOperation0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            Zip<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSetOperation0, T0, TResult1, T>
            (this NativeArray<T> @this, in SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0, TSetOperation0>
            second, Func<T, T0, TResult1> action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where T0 : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T0>
            where TEnumerator1 : struct, IRefEnumerator<T0>
            where TSetOperation0 : struct, ISetOperation<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>
            where TResult1 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0, TSetOperation0>,
                SetOperationEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0, TSetOperation0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipEnumerable<TEnumerable0, TEnumerator0, T0>,
                TEnumerator0,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            Zip<TEnumerable0, TEnumerator0, T0, TResult1, T>
            (this NativeArray<T> @this, in SkipEnumerable<TEnumerable0, TEnumerator0, T0>
            second, Func<T, T0, TResult1> action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TResult1 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipEnumerable<TEnumerable0, TEnumerator0, T0>,
                TEnumerator0,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeEnumerable<TEnumerable0, TEnumerator0, T0>,
                TakeEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            Zip<TEnumerable0, TEnumerator0, T0, TResult1, T>
            (this NativeArray<T> @this, in TakeEnumerable<TEnumerable0, TEnumerator0, T0>
            second, Func<T, T0, TResult1> action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TResult1 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeEnumerable<TEnumerable0, TEnumerator0, T0>,
                TakeEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            Zip<TEnumerable0, TEnumerator0, T0, TPredicate0, TResult1, T>
            (this NativeArray<T> @this, in SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>
            second, Func<T, T0, TResult1> action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TPredicate0 : struct, IRefFunc<T0, bool>
            where TResult1 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            Zip<TEnumerable0, TEnumerator0, T0, TPredicate0, TResult1, T>
            (this NativeArray<T> @this, in TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>
            second, Func<T, T0, TResult1> action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TPredicate0 : struct, IRefFunc<T0, bool>
            where TResult1 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            Zip<TEnumerable0, TEnumerator0, T0, TResult1, T>
            (this NativeArray<T> @this, in SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>
            second, Func<T, T0, TResult1> action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TResult1 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            Zip<TEnumerable0, TEnumerator0, T0, TResult1, T>
            (this NativeArray<T> @this, in TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>
            second, Func<T, T0, TResult1> action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TResult1 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            Zip<TEnumerable0, TEnumerator0, T0, TPredicate0, TResult1, T>
            (this NativeArray<T> @this, in WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>
            second, Func<T, T0, TResult1> action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TPredicate0 : struct, IRefFunc<T0, bool>
            where TResult1 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                WhereEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            Zip<TEnumerable0, TEnumerator0, T0, TPredicate0, TResult1, T>
            (this NativeArray<T> @this, in WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>
            second, Func<T, T0, TResult1> action, T firstDefaultValue = default, T0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TPredicate0 : struct, IWhereIndex<T0>
            where TResult1 : unmanaged
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                WhereIndexEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                T0,
                TResult1,
                DelegateFuncToAction<T, T0, TResult1>
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, action, firstDefaultValue, secondDefaultValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TResult0, TAction0>,
                ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TResult0, TAction0>.Enumerator,
                TResult0,
                TResult1,
                DelegateFuncToAction<T, TResult0, TResult1>
            >
            Zip<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TResult0, TAction0, TResult1, T>
            (this NativeArray<T> @this, in ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TResult0, TAction0>
            second, Func<T, TResult0, TResult1> action, T firstDefaultValue = default, TResult0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where T0 : unmanaged
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T1>
            where TEnumerator1 : struct, IRefEnumerator<T1>
            where T1 : unmanaged
            where TResult0 : unmanaged
            where TResult1 : unmanaged
            where TAction0 : struct, IRefAction<T0, T1, TResult0>
            => new ZipEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TResult0, TAction0>,
                ZipEnumerable<TEnumerable0, TEnumerator0, T0, TEnumerable1, TEnumerator1, T1, TResult0, TAction0>.Enumerator,
                TResult0,
                TResult1,
                DelegateFuncToAction<T, TResult0, TResult1>
            >
            (NativeEnumerable.AsRefEnumerable(@this), second, action, firstDefaultValue, secondDefaultValue);
        #endregion

        #region SelectMany
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                NativeEnumerable<T0>,
                NativeEnumerable<T0>.Enumerator,
                DelegateFuncToAction<T, NativeEnumerable<T0>>
            >
            SelectMany<T0, T>(this NativeArray<T> @this, Func<T, NativeEnumerable<T0>> func)
            where T : unmanaged
            where T0 : unmanaged
            => new SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                NativeEnumerable<T0>,
                NativeEnumerable<T0>.Enumerator,
                DelegateFuncToAction<T, NativeEnumerable<T0>>
            >
            (@this.AsRefEnumerable(), func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                NativeEnumerable<T0>,
                NativeEnumerable<T0>.Enumerator,
                NativeArrayToNativeEnumerableActionWrapper<T, T0>
            >
            SelectMany<T0, T>(this NativeArray<T> @this, Func<T, NativeArray<T0>> func)
            where T : unmanaged
            where T0 : unmanaged
            => new SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                NativeEnumerable<T0>,
                NativeEnumerable<T0>.Enumerator,
                NativeArrayToNativeEnumerableActionWrapper<T, T0>
            >
            (@this.AsRefEnumerable(), func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                ArrayEnumerable<T0>,
                ArrayEnumerable<T0>.Enumerator,
                DelegateFuncToAction<T, ArrayEnumerable<T0>>
            >
            SelectMany<T0, T>(this NativeArray<T> @this, Func<T, ArrayEnumerable<T0>> func)
            where T : unmanaged
            where T0 : unmanaged
            => new SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                ArrayEnumerable<T0>,
                ArrayEnumerable<T0>.Enumerator,
                DelegateFuncToAction<T, ArrayEnumerable<T0>>
            >
            (@this.AsRefEnumerable(), func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                ArrayEnumerable<T0>,
                ArrayEnumerable<T0>.Enumerator,
                ArrayToArrayEnumerableActionWrapper<T, T0>
            >
            SelectMany<T0, T>(this NativeArray<T> @this, Func<T, T0[]> func)
            where T : unmanaged where T0 : unmanaged
            => new SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                ArrayEnumerable<T0>,
                ArrayEnumerable<T0>.Enumerator,
                ArrayToArrayEnumerableActionWrapper<T, T0>
            >
            (@this.AsRefEnumerable(), func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                AppendEnumerable<TEnumerable0, TEnumerator0, T0>,
                AppendEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                DelegateFuncToAction<T, AppendEnumerable<TEnumerable0, TEnumerator0, T0>>
            >
            SelectMany<T0, TEnumerable0, TEnumerator0, T>(this NativeArray<T> @this, Func<T, AppendEnumerable<TEnumerable0, TEnumerator0, T0>> func)
            where T : unmanaged where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            => new SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                AppendEnumerable<TEnumerable0, TEnumerator0, T0>,
                AppendEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                DelegateFuncToAction<T, AppendEnumerable<TEnumerable0, TEnumerator0, T0>>
            >
            (@this.AsRefEnumerable(), func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>.Enumerator,
                DelegateFuncToAction<T, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>>
            >
            SelectMany<T0, TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>(this NativeArray<T> @this, Func<T, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>> func)
            where T : unmanaged where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEnumerator1 : struct, IRefEnumerator<T0>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T0>
            => new SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>.Enumerator,
                DelegateFuncToAction<T, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T0>>
            >
            (@this.AsRefEnumerable(), func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                DelegateFuncToAction<T, DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>>
            >
            SelectMany<T0, TEnumerable0, TEnumerator0, T>(this NativeArray<T> @this, Func<T, DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>> func)
            where T : unmanaged where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            => new SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                DelegateFuncToAction<T, DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T0>>
            >
            (@this.AsRefEnumerable(), func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer0, TGetHashCodeFunc0>,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer0, TGetHashCodeFunc0>.Enumerator,
                DelegateFuncToAction<T, DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer0, TGetHashCodeFunc0>>
            >
            SelectMany<T0, TEnumerable0, TEnumerator0, TEqualityComparer0, TGetHashCodeFunc0, T>(this NativeArray<T> @this, Func<T, DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer0, TGetHashCodeFunc0>> func)
            where T : unmanaged where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TEqualityComparer0 : struct, IRefFunc<T0, T0, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<T0, int>
            => new SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer0, TGetHashCodeFunc0>,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer0, TGetHashCodeFunc0>.Enumerator,
                DelegateFuncToAction<T, DistinctEnumerable<TEnumerable0, TEnumerator0, T0, TEqualityComparer0, TGetHashCodeFunc0>>
            >
            (@this.AsRefEnumerable(), func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                Grouping<TKey0, TElement0>,
                GroupByEnumerable<TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TElement0, TElementSelector0, TEqualityComparer0>,
                GroupByEnumerable<TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TElement0, TElementSelector0, TEqualityComparer0>.Enumerator,
                DelegateFuncToAction<T, GroupByEnumerable<TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TElement0, TElementSelector0, TEqualityComparer0>>
            >
            SelectMany<T0, TEnumerable0, TEnumerator0, TKey0, TKeySelector0, TElement0, TElementSelector0, TEqualityComparer0, T>(this NativeArray<T> @this, Func<T, GroupByEnumerable<TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TElement0, TElementSelector0, TEqualityComparer0>> func)
            where T : unmanaged where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TElement0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T0, TKey0>
            where TElementSelector0 : struct, IRefFunc<T0, TElement0>
            where TEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                Grouping<TKey0, TElement0>,
                GroupByEnumerable<TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TElement0, TElementSelector0, TEqualityComparer0>,
                GroupByEnumerable<TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TElement0, TElementSelector0, TEqualityComparer0>.Enumerator,
                DelegateFuncToAction<T, GroupByEnumerable<TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TElement0, TElementSelector0, TEqualityComparer0>>
            >
            (@this.AsRefEnumerable(), func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                Grouping<TKey0, T0>,
                Grouping<TKey0, T0>.Enumerator,
                DelegateFuncToAction<T, Grouping<TKey0, T0>>
            >
            SelectMany<T0, TKey0, T>(this NativeArray<T> @this, Func<T, Grouping<TKey0, T0>> func)
            where T : unmanaged where T0 : unmanaged
            where TKey0 : unmanaged
            => new SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                Grouping<TKey0, T0>,
                Grouping<TKey0, T0>.Enumerator,
                DelegateFuncToAction<T, Grouping<TKey0, T0>>
            >
            (@this.AsRefEnumerable(), func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                GroupJoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0, TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>,
                GroupJoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0, TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>.Enumerator,
                DelegateFuncToAction<T, GroupJoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0, TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>>
            >
            SelectMany<T0, TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0, TKey0, TOuterKeySelector0, TInnerKeySelector0, T0Selector, TKeyEqualityComparer0, T>(this NativeArray<T> @this, Func<T, GroupJoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0, TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>> func)
            where T : unmanaged where T0 : unmanaged
            where TOuterSource0 : unmanaged
            where TInnerSource0 : unmanaged
            where TOuterEnumerator0 : struct, IRefEnumerator<TOuterSource0>
            where TInnerEnumerator0 : struct, IRefEnumerator<TInnerSource0>
            where TOuterEnumerable0 : struct, IRefEnumerable<TOuterEnumerator0, TOuterSource0>
            where TInnerEnumerable0 : struct, IRefEnumerable<TInnerEnumerator0, TInnerSource0>
            where TKey0 : unmanaged
            where TOuterKeySelector0 : struct, IRefFunc<TOuterSource0, TKey0>
            where TInnerKeySelector0 : struct, IRefFunc<TInnerSource0, TKey0>
            where T0Selector : struct,
            IRefFunc<TOuterSource0,
                WhereIndexEnumerable<
                    NativeEnumerable<TInnerSource0>,
                    NativeEnumerable<TInnerSource0>.Enumerator,
                    TInnerSource0,
                    GroupJoinPredicate<TInnerSource0, TKey0, TKeyEqualityComparer0>
                    >,
                T0>
            where TKeyEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                GroupJoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0, TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>,
                GroupJoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0, TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>.Enumerator,
                DelegateFuncToAction<T, GroupJoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0, TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>>
            >
            (@this.AsRefEnumerable(), func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                JoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0, TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>,
                JoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0, TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>.Enumerator,
                DelegateFuncToAction<T, JoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0, TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>>
            >
            SelectMany<T0, TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0, TKey0, TOuterKeySelector0, TInnerKeySelector0, T0Selector, TKeyEqualityComparer0, T>
            (this NativeArray<T> @this, 
            Func<
                T, 
                JoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0, TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>
            > 
            func)
            where T : unmanaged where T0 : unmanaged
            where TOuterSource0 : unmanaged
            where TInnerSource0 : unmanaged
            where TOuterEnumerator0 : struct, IRefEnumerator<TOuterSource0>
            where TInnerEnumerator0 : struct, IRefEnumerator<TInnerSource0>
            where TOuterEnumerable0 : struct, IRefEnumerable<TOuterEnumerator0, TOuterSource0>
            where TInnerEnumerable0 : struct, IRefEnumerable<TInnerEnumerator0, TInnerSource0>
            where TKey0 : unmanaged
            where TOuterKeySelector0 : struct, IRefFunc<TOuterSource0, TKey0>
            where TInnerKeySelector0 : struct, IRefFunc<TInnerSource0, TKey0>
            where TKeyEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            where T0Selector : struct, IRefFunc<TOuterSource0, TInnerSource0, T0>
            => new SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                JoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0, TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>,
                JoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0, TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>.Enumerator,
                DelegateFuncToAction<T, JoinEnumerable<TOuterEnumerable0, TOuterEnumerator0, TOuterSource0, TInnerEnumerable0, TInnerEnumerator0, TInnerSource0, TKey0, TOuterKeySelector0, TInnerKeySelector0, T0, T0Selector, TKeyEqualityComparer0>>
            >
            (@this.AsRefEnumerable(), func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                MinMaxByEnumerable<TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeyRenewPredicate0, TKeyEqualityComparer0>,
                MinMaxByEnumerable<TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeyRenewPredicate0, TKeyEqualityComparer0>.Enumerator,
                DelegateFuncToAction<T, MinMaxByEnumerable<TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeyRenewPredicate0, TKeyEqualityComparer0>>
            >
            SelectMany<T0, TEnumerable0, TEnumerator0, TKey0, TKeySelector0, TKeyRenewPredicate0, TKeyEqualityComparer0, T>(this NativeArray<T> @this, Func<T, MinMaxByEnumerable<TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeyRenewPredicate0, TKeyEqualityComparer0>> func)
            where T : unmanaged where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefFunc<T0, TKey0>
            where TKeyRenewPredicate0 : struct, IRefFunc<TKey0, TKey0, bool>
            where TKeyEqualityComparer0 : struct, IRefFunc<TKey0, TKey0, bool>
            => new SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                MinMaxByEnumerable<TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeyRenewPredicate0, TKeyEqualityComparer0>,
                MinMaxByEnumerable<TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeyRenewPredicate0, TKeyEqualityComparer0>.Enumerator,
                DelegateFuncToAction<T, MinMaxByEnumerable<TEnumerable0, TEnumerator0, T0, TKey0, TKeySelector0, TKeyRenewPredicate0, TKeyEqualityComparer0>>
            >
            (@this.AsRefEnumerable(), func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>.Enumerator,
                DelegateFuncToAction<T, OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>>
            >
            SelectMany<T0, TEnumerable0, TEnumerator0, TComparer0, T>(this NativeArray<T> @this, Func<T, OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>> func)
            where T : unmanaged where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TComparer0 : struct, IRefFunc<T0, T0, int>
            => new SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>,
                OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>.Enumerator,
                DelegateFuncToAction<T, OrderByEnumerable<TEnumerable0, TEnumerator0, T0, TComparer0>>
            >
            (@this.AsRefEnumerable(), func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                PrependEnumerable<TEnumerable0, TEnumerator0, T0>,
                PrependEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                DelegateFuncToAction<T, PrependEnumerable<TEnumerable0, TEnumerator0, T0>>
            >
            SelectMany<T0, TEnumerable0, TEnumerator0, T>(this NativeArray<T> @this, Func<T, PrependEnumerable<TEnumerable0, TEnumerator0, T0>> func)
            where T : unmanaged where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            => new SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                PrependEnumerable<TEnumerable0, TEnumerator0, T0>,
                PrependEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                DelegateFuncToAction<T, PrependEnumerable<TEnumerable0, TEnumerator0, T0>>
            >
            (@this.AsRefEnumerable(), func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                RangeRepeatEnumerable<T0, TAction0>,
                RangeRepeatEnumerable<T0, TAction0>.Enumerator,
                DelegateFuncToAction<T, RangeRepeatEnumerable<T0, TAction0>>
            >
            SelectMany<T0, TAction0, T>(this NativeArray<T> @this, Func<T, RangeRepeatEnumerable<T0, TAction0>> func)
            where T : unmanaged where T0 : unmanaged
            where TAction0 : struct, IRangeRepeat<T0>
            => new SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                RangeRepeatEnumerable<T0, TAction0>,
                RangeRepeatEnumerable<T0, TAction0>.Enumerator,
                DelegateFuncToAction<T, RangeRepeatEnumerable<T0, TAction0>>
            >
            (@this.AsRefEnumerable(), func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T0>,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                DelegateFuncToAction<T, ReverseEnumerable<TEnumerable0, TEnumerator0, T0>>
            >
            SelectMany<T0, TEnumerable0, TEnumerator0, T>(this NativeArray<T> @this, Func<T, ReverseEnumerable<TEnumerable0, TEnumerator0, T0>> func)
            where T : unmanaged where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            => new SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T0>,
                ReverseEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                DelegateFuncToAction<T, ReverseEnumerable<TEnumerable0, TEnumerator0, T0>>
            >
            (@this.AsRefEnumerable(), func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                SelectEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T0, TAction0>,
                SelectEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T0, TAction0>.Enumerator,
                DelegateFuncToAction<T, SelectEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T0, TAction0>>
            >
            SelectMany<T0, TPrevEnumerable0, TPrevEnumerator0, TPrev0, TAction0, T>(this NativeArray<T> @this, Func<T, SelectEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T0, TAction0>> func)
            where T : unmanaged where T0 : unmanaged
            where TPrev0 : unmanaged
            where TPrevEnumerable0 : struct, IRefEnumerable<TPrevEnumerator0, TPrev0>
            where TPrevEnumerator0 : struct, IRefEnumerator<TPrev0>
            where TAction0 : struct, IRefAction<TPrev0, T0>
            => new SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                SelectEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T0, TAction0>,
                SelectEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T0, TAction0>.Enumerator,
                DelegateFuncToAction<T, SelectEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T0, TAction0>>
            >
            (@this.AsRefEnumerable(), func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                SelectIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T0, TAction0>,
                SelectIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T0, TAction0>.Enumerator,
                DelegateFuncToAction<T, SelectIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T0, TAction0>>
            >
            SelectMany<T0, TPrevEnumerable0, TPrevEnumerator0, TPrev0, TAction0, T>(this NativeArray<T> @this, Func<T, SelectIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T0, TAction0>> func)
            where T : unmanaged where T0 : unmanaged
            where TPrev0 : unmanaged
            where TAction0 : struct, ISelectIndex<TPrev0, T0>
            where TPrevEnumerator0 : struct, IRefEnumerator<TPrev0>
            where TPrevEnumerable0 : struct, IRefEnumerable<TPrevEnumerator0, TPrev0>
            => new SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                SelectIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T0, TAction0>,
                SelectIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T0, TAction0>.Enumerator,
                DelegateFuncToAction<T, SelectIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrev0, T0, TAction0>>
            >
            (@this.AsRefEnumerable(), func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, T0Enumerable, T0Enumerator, TAction0>,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, T0Enumerable, T0Enumerator, TAction0>.Enumerator,
                DelegateFuncToAction<T, SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, T0Enumerable, T0Enumerator, TAction0>>
            >
            SelectMany<T0, TEnumerable0, TEnumerator0, TPrev0, T0Enumerable, T0Enumerator, TAction0, T>(this NativeArray<T> @this, Func<T, SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, T0Enumerable, T0Enumerator, TAction0>> func)
            where T : unmanaged where T0 : unmanaged
            where TPrev0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<TPrev0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrev0>
            where T0Enumerator : struct, IRefEnumerator<T0>
            where T0Enumerable : struct, IRefEnumerable<T0Enumerator, T0>
            where TAction0 : struct, IRefAction<TPrev0, T0Enumerable>
            => new SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, T0Enumerable, T0Enumerator, TAction0>,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, T0Enumerable, T0Enumerator, TAction0>.Enumerator,
                DelegateFuncToAction<T, SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrev0, T0, T0Enumerable, T0Enumerator, TAction0>>
            >
            (@this.AsRefEnumerable(), func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                SkipEnumerable<TEnumerable0, TEnumerator0, T0>,
                TEnumerator0,
                DelegateFuncToAction<T, SkipEnumerable<TEnumerable0, TEnumerator0, T0>>
            >
            SelectMany<T0, TEnumerable0, TEnumerator0, T>(this NativeArray<T> @this, Func<T, SkipEnumerable<TEnumerable0, TEnumerator0, T0>> func)
            where T : unmanaged where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            => new SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                SkipEnumerable<TEnumerable0, TEnumerator0, T0>,
                TEnumerator0,
                DelegateFuncToAction<T, SkipEnumerable<TEnumerable0, TEnumerator0, T0>>
            >
            (@this.AsRefEnumerable(), func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                DelegateFuncToAction<T, SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>>
            >
            SelectMany<T0, TEnumerable0, TEnumerator0, T>(this NativeArray<T> @this, Func<T, SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>> func)
            where T : unmanaged where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            => new SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>,
                SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                DelegateFuncToAction<T, SkipLastEnumerable<TEnumerable0, TEnumerator0, T0>>
            >
            (@this.AsRefEnumerable(), func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                DelegateFuncToAction<T, SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>>
            >
            SelectMany<T0, TEnumerable0, TEnumerator0, TPredicate0, T>(this NativeArray<T> @this, Func<T, SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>> func)
            where T : unmanaged where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TPredicate0 : struct, IRefFunc<T0, bool>
            => new SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                DelegateFuncToAction<T, SkipWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>>
            >
            (@this.AsRefEnumerable(), func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                TakeEnumerable<TEnumerable0, TEnumerator0, T0>,
                TakeEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                DelegateFuncToAction<T, TakeEnumerable<TEnumerable0, TEnumerator0, T0>>
            >
            SelectMany<T0, TEnumerable0, TEnumerator0, T>(this NativeArray<T> @this, Func<T, TakeEnumerable<TEnumerable0, TEnumerator0, T0>> func)
            where T : unmanaged where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            => new SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                TakeEnumerable<TEnumerable0, TEnumerator0, T0>,
                TakeEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                DelegateFuncToAction<T, TakeEnumerable<TEnumerable0, TEnumerator0, T0>>
            >
            (@this.AsRefEnumerable(), func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                DelegateFuncToAction<T, TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>>
            >
            SelectMany<T0, TEnumerable0, TEnumerator0, T>(this NativeArray<T> @this, Func<T, TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>> func)
            where T : unmanaged where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            => new SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>,
                TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>.Enumerator,
                DelegateFuncToAction<T, TakeLastEnumerable<TEnumerable0, TEnumerator0, T0>>
            >
            (@this.AsRefEnumerable(), func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                DelegateFuncToAction<T, TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>>
            >
            SelectMany<T0, TEnumerable0, TEnumerator0, TPredicate0, T>(this NativeArray<T> @this, Func<T, TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>> func)
            where T : unmanaged where T0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<T0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T0>
            where TPredicate0 : struct, IRefFunc<T0, bool>
            => new SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>,
                TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>.Enumerator,
                DelegateFuncToAction<T, TakeWhileEnumerable<TEnumerable0, TEnumerator0, T0, TPredicate0>>
            >
            (@this.AsRefEnumerable(), func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                WhereEnumerable<TPrevEnumerable0, TPrevEnumerator0, T0, TPredicate0>,
                WhereEnumerable<TPrevEnumerable0, TPrevEnumerator0, T0, TPredicate0>.Enumerator,
                DelegateFuncToAction<T, WhereEnumerable<TPrevEnumerable0, TPrevEnumerator0, T0, TPredicate0>>
            >
            SelectMany<T0, TPrevEnumerable0, TPrevEnumerator0, TPredicate0, T>(this NativeArray<T> @this, Func<T, WhereEnumerable<TPrevEnumerable0, TPrevEnumerator0, T0, TPredicate0>> func)
            where T : unmanaged where T0 : unmanaged
            where TPredicate0 : struct, IRefFunc<T0, bool>
            where TPrevEnumerator0 : struct, IRefEnumerator<T0>
            where TPrevEnumerable0 : struct, IRefEnumerable<TPrevEnumerator0, T0>
            => new SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                WhereEnumerable<TPrevEnumerable0, TPrevEnumerator0, T0, TPredicate0>,
                WhereEnumerable<TPrevEnumerable0, TPrevEnumerator0, T0, TPredicate0>.Enumerator,
                DelegateFuncToAction<T, WhereEnumerable<TPrevEnumerable0, TPrevEnumerator0, T0, TPredicate0>>
            >
            (@this.AsRefEnumerable(), func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                WhereIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, T0, TPredicate0>,
                WhereIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, T0, TPredicate0>.Enumerator,
                DelegateFuncToAction<T, WhereIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, T0, TPredicate0>>
            >
            SelectMany<T0, TPrevEnumerable0, TPrevEnumerator0, TPredicate0, T>(this NativeArray<T> @this, Func<T, WhereIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, T0, TPredicate0>> func)
            where T : unmanaged where T0 : unmanaged
            where TPredicate0 : struct, IWhereIndex<T0>
            where TPrevEnumerator0 : struct, IRefEnumerator<T0>
            where TPrevEnumerable0 : struct, IRefEnumerable<TPrevEnumerator0, T0>
            => new SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                WhereIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, T0, TPredicate0>,
                WhereIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, T0, TPredicate0>.Enumerator,
                DelegateFuncToAction<T, WhereIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, T0, TPredicate0>>
            >
            (@this.AsRefEnumerable(), func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                ZipEnumerable<TFirstEnumerable0, TFirstEnumerator0, TFirstSource0, TSecondEnumerable0, TSecondEnumerator0, TSecondSource0, T0, TAction0>,
                ZipEnumerable<TFirstEnumerable0, TFirstEnumerator0, TFirstSource0, TSecondEnumerable0, TSecondEnumerator0, TSecondSource0, T0, TAction0>.Enumerator,
                DelegateFuncToAction<T, ZipEnumerable<TFirstEnumerable0, TFirstEnumerator0, TFirstSource0, TSecondEnumerable0, TSecondEnumerator0, TSecondSource0, T0, TAction0>>
            >
            SelectMany<T0, TFirstEnumerable0, TFirstEnumerator0, TFirstSource0, TSecondEnumerable0, TSecondEnumerator0, TSecondSource0, TAction0, T>(this NativeArray<T> @this, Func<T, ZipEnumerable<TFirstEnumerable0, TFirstEnumerator0, TFirstSource0, TSecondEnumerable0, TSecondEnumerator0, TSecondSource0, T0, TAction0>> func)
            where T : unmanaged where T0 : unmanaged
            where TFirstSource0 : unmanaged
            where TSecondSource0 : unmanaged
            where TFirstEnumerator0 : struct, IRefEnumerator<TFirstSource0>
            where TFirstEnumerable0 : struct, IRefEnumerable<TFirstEnumerator0, TFirstSource0>
            where TSecondEnumerator0 : struct, IRefEnumerator<TSecondSource0>
            where TSecondEnumerable0 : struct, IRefEnumerable<TSecondEnumerator0, TSecondSource0>
            where TAction0 : struct, IRefAction<TFirstSource0, TSecondSource0, T0>
            => new SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                ZipEnumerable<TFirstEnumerable0, TFirstEnumerator0, TFirstSource0, TSecondEnumerable0, TSecondEnumerator0, TSecondSource0, T0, TAction0>,
                ZipEnumerable<TFirstEnumerable0, TFirstEnumerator0, TFirstSource0, TSecondEnumerable0, TSecondEnumerator0, TSecondSource0, T0, TAction0>.Enumerator,
                DelegateFuncToAction<T, ZipEnumerable<TFirstEnumerable0, TFirstEnumerator0, TFirstSource0, TSecondEnumerable0, TSecondEnumerator0, TSecondSource0, T0, TAction0>>
            >
            (@this.AsRefEnumerable(), func);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static
            SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                SetOperationEnumerable<TFirstEnumerable0, TFirstEnumerator0, TSecondEnumerable0, TSecondEnumerator0, T0, TSetOperation0>,
                SetOperationEnumerable<TFirstEnumerable0, TFirstEnumerator0, TSecondEnumerable0, TSecondEnumerator0, T0, TSetOperation0>.Enumerator,
                DelegateFuncToAction<T, SetOperationEnumerable<TFirstEnumerable0, TFirstEnumerator0, TSecondEnumerable0, TSecondEnumerator0, T0, TSetOperation0>>
            >
            SelectMany<T0, TFirstEnumerable0, TFirstEnumerator0, TSecondEnumerable0, TSecondEnumerator0, TSetOperation0, T>(this NativeArray<T> @this, Func<T, SetOperationEnumerable<TFirstEnumerable0, TFirstEnumerator0, TSecondEnumerable0, TSecondEnumerator0, T0, TSetOperation0>> func)
            where T : unmanaged where T0 : unmanaged
            where TFirstEnumerable0 : struct, IRefEnumerable<TFirstEnumerator0, T0>
            where TFirstEnumerator0 : struct, IRefEnumerator<T0>
            where TSecondEnumerable0 : struct, IRefEnumerable<TSecondEnumerator0, T0>
            where TSecondEnumerator0 : struct, IRefEnumerator<T0>
            where TSetOperation0 : struct, ISetOperation<TFirstEnumerable0, TFirstEnumerator0, TSecondEnumerable0, TSecondEnumerator0, T0>
            => new SelectManyEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                T0,
                SetOperationEnumerable<TFirstEnumerable0, TFirstEnumerator0, TSecondEnumerable0, TSecondEnumerator0, T0, TSetOperation0>,
                SetOperationEnumerable<TFirstEnumerable0, TFirstEnumerator0, TSecondEnumerable0, TSecondEnumerator0, T0, TSetOperation0>.Enumerator,
                DelegateFuncToAction<T, SetOperationEnumerable<TFirstEnumerable0, TFirstEnumerator0, TSecondEnumerable0, TSecondEnumerator0, T0, TSetOperation0>>
            >
            (@this.AsRefEnumerable(), func);
        #endregion
    }
}
