using System;
using System.Collections.Generic;
using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public static unsafe class FlattenEnumerable
    {
        public static
            SelectManyEnumerable<
                TEnumerableEnumerable,
                TEnumerableEnumerator,
                TEnumerable,
                T,
                TEnumerable,
                TEnumerator,
                AsIs<TEnumerable>
            >
            Flatten<
                TEnumerableEnumerable,
                TEnumerableEnumerator,
                TEnumerable,
                TEnumerator,
                T
            >(ref this TEnumerableEnumerable enumerable)
            where T : unmanaged
            where TEnumerator : unmanaged, IRefEnumerator<T>
            where TEnumerable : unmanaged, IRefEnumerable<TEnumerator, T>
            where TEnumerableEnumerator : struct, IRefEnumerator<TEnumerable>
            where TEnumerableEnumerable : struct, IRefEnumerable<TEnumerableEnumerator, TEnumerable>
            => new SelectManyEnumerable<
                TEnumerableEnumerable,
                TEnumerableEnumerator,
                TEnumerable,
                T,
                TEnumerable,
                TEnumerator,
                AsIs<TEnumerable>
            >(enumerable, default);
    }
}
