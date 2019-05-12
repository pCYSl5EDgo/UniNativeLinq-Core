using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public static class BufferEnumerable
    {
        public static BufferEnumerable<TEnumerable, TEnumerator, TSource> Buffer<TEnumerable, TEnumerator, TSource>(ref this TEnumerable enumerable, long count)
            where TSource : unmanaged
            where TEnumerator : unmanaged, IRefEnumerator<TSource>
            where TEnumerable : unmanaged, IRefEnumerable<TEnumerator, TSource>
            => new BufferEnumerable<TEnumerable, TEnumerator, TSource>(enumerable, count);

        public static BufferNativeEnumerable<TSource> Buffer<TSource>(in this NativeEnumerable<TSource> enumerable, long count)
            where TSource : unmanaged
            => new BufferNativeEnumerable<TSource>(enumerable, count);

        public static BufferNativeEnumerable<TSource> Buffer<TSource>(this NativeArray<TSource> enumerable, long count)
            where TSource : unmanaged
            => new BufferNativeEnumerable<TSource>(enumerable.AsRefEnumerable(), count);

        public static BufferArrayEnumerable<TSource> Buffer<TSource>(in this ArrayEnumerable<TSource> enumerable, long count)
            where TSource : unmanaged
            => new BufferArrayEnumerable<TSource>(enumerable, count);

        public static BufferArrayEnumerable<TSource> Buffer<TSource>(this TSource[] enumerable, long count)
            where TSource : unmanaged
            => new BufferArrayEnumerable<TSource>(enumerable.AsRefEnumerable(), count);
    }
}
