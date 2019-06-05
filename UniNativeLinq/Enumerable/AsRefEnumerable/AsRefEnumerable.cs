using System;
using Unity.Collections;

namespace UniNativeLinq
{
    public static class NativeEnumerable
    {
        public static byte Average(in this NativeEnumerable<byte> @this)
        {
            var enumerator = @this.GetEnumerator();
            ulong x = default;
            ulong count = default;
            while (enumerator.TryMoveNext(out var value))
            {
                ++count;
                x += value;
            }
            enumerator.Dispose();
            return (byte)(x / count);
        }

        public static sbyte Average(in this NativeEnumerable<sbyte> @this)
        {
            var enumerator = @this.GetEnumerator();
            long x = default;
            long count = default;
            while (enumerator.TryMoveNext(out var value))
            {
                ++count;
                x += value;
            }
            enumerator.Dispose();
            return (sbyte)(x / count);
        }

        public static short Average(in this NativeEnumerable<short> @this)
        {
            var enumerator = @this.GetEnumerator();
            long x = default;
            long count = default;
            while (enumerator.TryMoveNext(out var value))
            {
                ++count;
                x += value;
            }
            enumerator.Dispose();
            return (short)(x / count);
        }

        public static ushort Average(in this NativeEnumerable<ushort> @this)
        {
            var enumerator = @this.GetEnumerator();
            ulong x = default;
            ulong count = default;
            while (enumerator.TryMoveNext(out var value))
            {
                ++count;
                x += value;
            }
            enumerator.Dispose();
            return (ushort)(x / count);
        }

        public static int Average(in this NativeEnumerable<int> @this)
        {
            var enumerator = @this.GetEnumerator();
            long x = default;
            long count = default;
            while (enumerator.TryMoveNext(out var value))
            {
                x += value;
            }
            enumerator.Dispose();
            return (int)(x / count);
        }

        public static uint Average(in this NativeEnumerable<uint> @this)
        {
            var enumerator = @this.GetEnumerator();
            ulong x = default;
            ulong count = default;
            while (enumerator.TryMoveNext(out var value))
            {
                ++count;
                x += value;
            }
            enumerator.Dispose();
            return (uint)(x / count);
        }

        public static long Average(in this NativeEnumerable<long> @this)
        {
            var enumerator = @this.GetEnumerator();
            long x = default;
            long count = default;
            while (enumerator.TryMoveNext(out var value))
            {
                ++count;
                x += value;
            }
            enumerator.Dispose();
            return x / count;
        }

        public static ulong Average(in this NativeEnumerable<ulong> @this)
        {
            var enumerator = @this.GetEnumerator();
            ulong x = default;
            ulong count = default;
            while (enumerator.TryMoveNext(out var value))
            {
                ++count;
                x += value;
            }
            enumerator.Dispose();
            return x / count;
        }

        public static float Average(in this NativeEnumerable<float> @this)
        {
            var enumerator = @this.GetEnumerator();
            float x = default;
            float count = default;
            while (enumerator.TryMoveNext(out var value))
            {
                ++count;
                x += value;
            }
            enumerator.Dispose();
            return x / count;
        }

        public static double Average(in this NativeEnumerable<double> @this)
        {
            var enumerator = @this.GetEnumerator();
            double x = default;
            double count = default;
            while (enumerator.TryMoveNext(out var value))
            {
                ++count;
                x += value;
            }
            enumerator.Dispose();
            return x / count;
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