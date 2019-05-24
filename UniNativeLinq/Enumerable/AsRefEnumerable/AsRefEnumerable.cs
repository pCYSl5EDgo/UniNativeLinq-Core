using System;
using Unity.Collections;

namespace UniNativeLinq
{
    public static class NativeEnumerable
    {
        public static bool TryGetMin<TEnumerable, TEnumerator>(in this AppendEnumerable<TEnumerable, TEnumerator, byte> @this, out byte value)
            where TEnumerator : struct, IRefEnumerator<byte>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, byte>
        {
            var enumerator = @this.GetEnumerator();
            if (!enumerator.TryMoveNext(out value))
            {
                enumerator.Dispose();
                return false;
            }
            while (enumerator.TryMoveNext(out var other))
                if (other < value)
                    value = other;
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