namespace UniNativeLinq
{
    public static class FlattenEnumerable
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
