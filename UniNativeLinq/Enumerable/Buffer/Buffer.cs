using Unity.Collections;

namespace UniNativeLinq
{
    public static class BufferEnumerable
    {
        public static BufferEnumerable<TEnumerable, TEnumerator, T> Buffer<TEnumerable, TEnumerator, T>(ref this TEnumerable enumerable, long count)
            where T : unmanaged
            where TEnumerator : unmanaged, IRefEnumerator<T>
            where TEnumerable : unmanaged, IRefEnumerable<TEnumerator, T>
            => new BufferEnumerable<TEnumerable, TEnumerator, T>(enumerable, count);

        public static BufferNativeEnumerable<T> Buffer<T>(in this NativeEnumerable<T> enumerable, long count)
            where T : unmanaged
            => new BufferNativeEnumerable<T>(enumerable, count);

        public static BufferNativeEnumerable<T> Buffer<T>(this NativeArray<T> enumerable, long count)
            where T : unmanaged
            => new BufferNativeEnumerable<T>(enumerable.AsRefEnumerable(), count);

        public static BufferArrayEnumerable<T> Buffer<T>(in this ArrayEnumerable<T> enumerable, long count)
            where T : unmanaged
            => new BufferArrayEnumerable<T>(enumerable, count);

        public static BufferArrayEnumerable<T> Buffer<T>(this T[] enumerable, long count)
            where T : unmanaged
            => new BufferArrayEnumerable<T>(enumerable.AsRefEnumerable(), count);
    }
}
