using System;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UniNativeLinq
{
    public static class NativeEnumerable
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetMin<TEnumerable, TEnumerator, T>(in this AppendEnumerable<TEnumerable, TEnumerator, T> @this, Func<T, byte> func, out byte value)
            where T : unmanaged
            where TEnumerator : struct, IRefEnumerator<T>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, T>
        {
            var enumerator = @this.GetEnumerator();
            ref var current = ref enumerator.TryGetNext(out var success);
            if (!success)
            {
                enumerator.Dispose();
                value = default;
                return false;
            }
            value = func(current);
            while (true)
            {
                current = ref enumerator.TryGetNext(out success);
                if (!success) break;
                var other = func(current);
                if (other < value)
                    value = other;
            }
            enumerator.Dispose();
            return true;
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