using System;
using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public static unsafe class NativeEnumerable
    {
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