using System;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UniNativeLinq
{
    public static class NativeEnumerable
    {
        public static bool TryGetElementAt<T>(in this NativeEnumerable<T> @this, long index, out T value)
            where T : unmanaged
        {
            if (index < 0 || @this.LongCount() <= index)
            {
                value = default;
                return false;
            }
            var enumerator = @this.GetEnumerator();
            for (var i = 0L; i < index; i++)
                enumerator.MoveNext();
            value = enumerator.TryGetNext(out var success);
            enumerator.Dispose();
            return success;
        }

        public static bool TryGetElementAt<TEnumerable, TEnumerator, T>(in this AppendEnumerable<TEnumerable, TEnumerator, T> @this, long index, out T value)
            where T : unmanaged
            where TEnumerator : struct, IRefEnumerator<T>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, T>
        {
            if (index < 0)
            {
                value = default;
                return false;
            }
            AppendEnumerable<TEnumerable, TEnumerator, T>.Enumerator enumerator;
            if (@this.CanFastCount())
            {
                if (index >= @this.LongCount())
                {
                    value = default;
                    return false;
                }
                enumerator = @this.GetEnumerator();
                for (var i = 0L; i < index; i++)
                    enumerator.MoveNext();
            }
            else
            {
                enumerator = @this.GetEnumerator();
                for (var i = 0L; i < index; i++)
                {
                    if (!enumerator.MoveNext())
                    {
                        value = default;
                        enumerator.Dispose();
                        return false;
                    }
                }
            }
            value = enumerator.TryGetNext(out var success);
            enumerator.Dispose();
            return success;
        }

        public static bool TryGetElementAt<TEnumerable, TEnumerator, T, TPredicate0>(in this WhereEnumerable<TEnumerable, TEnumerator, T, TPredicate0> @this, long index, out T value)
            where T : unmanaged
            where TEnumerator : struct, IRefEnumerator<T>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, T>
            where TPredicate0 : struct, IRefFunc<T, bool>
        {
            if (index < 0)
            {
                value = default;
                return false;
            }
            var enumerator = @this.GetEnumerator();
            for (var i = 0L; i < index; i++)
            {
                if (enumerator.MoveNext()) continue;
                value = default;
                enumerator.Dispose();
                return false;
            }
            value = enumerator.TryGetNext(out var success);
            enumerator.Dispose();
            return success;
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