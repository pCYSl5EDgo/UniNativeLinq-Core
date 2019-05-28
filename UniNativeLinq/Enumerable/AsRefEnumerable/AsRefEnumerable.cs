using System;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UniNativeLinq
{
    public static class NativeEnumerable
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Any<TPredicate0, TEnumerable, TEnumerator, T>(in this AppendEnumerable<TEnumerable, TEnumerator, T> @this, in TPredicate0 predicate)
            where TPredicate0 : struct, IRefFunc<T, bool>
            where T : unmanaged
            where TEnumerator : struct, IRefEnumerator<T>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, T>
        {
            ref var _predicate = ref Unsafe.AsRef(predicate);
            var enumerator = @this.GetEnumerator();
            while (true)
            {
                ref var current = ref enumerator.TryGetNext(out var success);
                if (!success)
                {
                    enumerator.Dispose();
                    return false;
                }
                if (_predicate.Calc(ref current))
                {
                    enumerator.Dispose();
                    return true;
                }
            }
        }

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