using System;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UniNativeLinq
{
    public static class NativeEnumerable
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Count<TEnumerable, TEnumerator, T, TPredicate>(in this AppendEnumerable<TEnumerable, TEnumerator, T> @this, ref T value, Func<T, T, bool> comparer)
            where T : unmanaged
            where TEnumerator : struct, IRefEnumerator<T>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, T>
        {
            var enumerator = @this.GetEnumerator();
            while (true)
            {
                ref var current = ref enumerator.TryGetNext(out var success);
                if (!success) break;
                if (!comparer(value, current)) continue;
                enumerator.Dispose();
                return true;
            }
            enumerator.Dispose();
            return false;
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