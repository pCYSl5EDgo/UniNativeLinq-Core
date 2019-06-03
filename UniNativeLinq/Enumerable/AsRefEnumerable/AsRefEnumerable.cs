using System;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UniNativeLinq
{
    public static class NativeEnumerable
    {
        public static bool TryGetSingle<T>(in this NativeEnumerable<T> @this, out T value)
            where T : unmanaged
        {
            if (@this.Length != 1)
            {
                value = default;
                return false;
            }
            value = @this[0];
            return true;
        }

        public static bool TryGetSingle<TEnumerable, TEnumerator, T>(in this AppendEnumerable<TEnumerable, TEnumerator, T> @this, out T value)
            where T : unmanaged
            where TEnumerator : struct, IRefEnumerator<T>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, T>
        {
            if (@this.CanFastCount() && @this.LongCount() != 1)
            {
                value = default;
                return false;
            }
            var enumerator = @this.GetEnumerator();
            if (!enumerator.TryMoveNext(out value) || enumerator.MoveNext())
            {
                enumerator.Dispose();
                return false;
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