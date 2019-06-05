using System;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UniNativeLinq
{
    public static class NativeEnumerable
    {
        public static byte Sum(in this NativeEnumerable<byte> @this)
        {
            var enumerator = @this.GetEnumerator();
            byte x = default;
            while (enumerator.TryMoveNext(out var value))
            {
                x += value;
            }
            enumerator.Dispose();
            return x;
        }

        public static sbyte Sum(in this NativeEnumerable<sbyte> @this)
        {
            var enumerator = @this.GetEnumerator();
            sbyte x = default;
            while (enumerator.TryMoveNext(out var value))
            {
                x += value;
            }
            enumerator.Dispose();
            return x;
        }

        public static short Sum(in this NativeEnumerable<short> @this)
        {
            var enumerator = @this.GetEnumerator();
            short x = default;
            while (enumerator.TryMoveNext(out var value))
            {
                x += value;
            }
            enumerator.Dispose();
            return x;
        }

        public static ushort Sum(in this NativeEnumerable<ushort> @this)
        {
            var enumerator = @this.GetEnumerator();
            ushort x = default;
            while (enumerator.TryMoveNext(out var value))
            {
                x += value;
            }
            enumerator.Dispose();
            return x;
        }

        public static int Sum(in this NativeEnumerable<int> @this)
        {
            var enumerator = @this.GetEnumerator();
            int x = default;
            while (enumerator.TryMoveNext(out var value))
            {
                x += value;
            }
            enumerator.Dispose();
            return x;
        }

        public static uint Sum(in this NativeEnumerable<uint> @this)
        {
            var enumerator = @this.GetEnumerator();
            uint x = default;
            while (enumerator.TryMoveNext(out var value))
            {
                x += value;
            }
            enumerator.Dispose();
            return x;
        }

        public static long Sum(in this NativeEnumerable<long> @this)
        {
            var enumerator = @this.GetEnumerator();
            long x = default;
            while (enumerator.TryMoveNext(out var value))
            {
                x += value;
            }
            enumerator.Dispose();
            return x;
        }

        public static ulong Sum(in this NativeEnumerable<ulong> @this)
        {
            var enumerator = @this.GetEnumerator();
            ulong x = default;
            while (enumerator.TryMoveNext(out var value))
            {
                x += value;
            }
            enumerator.Dispose();
            return x;
        }

        public static float Sum(in this NativeEnumerable<float> @this)
        {
            var enumerator = @this.GetEnumerator();
            float x = default;
            while (enumerator.TryMoveNext(out var value))
            {
                x += value;
            }
            enumerator.Dispose();
            return x;
        }

        public static double Sum(in this NativeEnumerable<double> @this)
        {
            var enumerator = @this.GetEnumerator();
            double x = default;
            while (enumerator.TryMoveNext(out var value))
            {
                x += value;
            }
            enumerator.Dispose();
            return x;
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