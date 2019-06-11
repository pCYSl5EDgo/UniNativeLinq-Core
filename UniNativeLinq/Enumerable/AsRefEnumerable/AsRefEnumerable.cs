using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;

namespace UniNativeLinq
{
    public static class NativeEnumerable
    {
        public static SkipWhileEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, DelegateFuncToStructOperatorFunc<T, bool>>
            SkipWhile<T>(in NativeEnumerable<T> @this, Func<T, bool> predicate)
            where T : unmanaged
            => new SkipWhileEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, DelegateFuncToStructOperatorFunc<T, bool>>(@this, new DelegateFuncToStructOperatorFunc<T, bool>(predicate));

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