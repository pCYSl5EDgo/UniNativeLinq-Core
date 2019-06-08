using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;

namespace UniNativeLinq
{
    public static class NativeEnumerable
    {
        public static
            OrderByEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TComparer0
            >
            OrderBy<T, TComparer0>
            (in this NativeEnumerable<T> @this, in TComparer0 comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TComparer0 : struct, IRefFunc<T, T, int>
            => new OrderByEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                TComparer0
            >(@this, comparer, allocator);

        public static
            OrderByEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                OrderByFromIComparer<T>
            >
            OrderBy<T>
            (in this NativeEnumerable<T> @this, IComparer<T> comparer, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            => new OrderByEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                OrderByFromIComparer<T>
            >(@this, new OrderByFromIComparer<T>(comparer), allocator);


        public static
            OrderByEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DefaultOrderByAscending<T>
            >
            OrderBy<T>
            (in this NativeEnumerable<T> @this, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            => new OrderByEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DefaultOrderByAscending<T>
            >(@this, default, allocator);

        public static
            OrderByEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DefaultOrderByDescending<T>
            >
            OrderByDescending<T>
            (in this NativeEnumerable<T> @this, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            => new OrderByEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DefaultOrderByDescending<T>
            >(@this, default, allocator);

        public static NativeEnumerable<T> AsRefEnumerable<T>(this NativeArray<T> array)
            where T : unmanaged
            => new NativeEnumerable<T>(array);

        public static ArrayEnumerable<T> AsRefEnumerable<T>(this T[] array)
            where T : unmanaged
            => new ArrayEnumerable<T>(array);

        public static ArrayEnumerable<T> AsRefEnumerable<T>(this T[] array, long offset, long count)
            where T : unmanaged
            => new ArrayEnumerable<T>(array, offset, count);

        public static ArrayEnumerable<T> AsRefEnumerable<T>(this ArraySegment<T> arraySegment)
            where T : unmanaged
            => new ArrayEnumerable<T>(arraySegment);
    }
}