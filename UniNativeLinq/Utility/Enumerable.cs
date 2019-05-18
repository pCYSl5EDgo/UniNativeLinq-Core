using Unity.Collections;

namespace UniNativeLinq
{
    public static class Enumerable
    {
        public static RangeRepeatEnumerable<int, Int32Increment> Range(int start, long length, Allocator allocator = Allocator.Temp)
            => new RangeRepeatEnumerable<int, Int32Increment>(start, length, default, allocator);

        public static RangeRepeatEnumerable<long, Int64Increment> Range(long start, long length, Allocator allocator = Allocator.Temp)
            => new RangeRepeatEnumerable<long, Int64Increment>(start, length, default, allocator);

        public static RangeRepeatEnumerable<uint, UInt32Increment> Range(uint start, long length, Allocator allocator = Allocator.Temp)
            => new RangeRepeatEnumerable<uint, UInt32Increment>(start, length, default, allocator);

        public static RangeRepeatEnumerable<ulong, UInt64Increment> Range(ulong start, long length, Allocator allocator = Allocator.Temp)
            => new RangeRepeatEnumerable<ulong, UInt64Increment>(start, length, default, allocator);

        public static RangeRepeatEnumerable<float, SingleIncrement> Range(float start, long length, Allocator allocator = Allocator.Temp)
            => new RangeRepeatEnumerable<float, SingleIncrement>(start, length, default, allocator);

        public static RangeRepeatEnumerable<double, DoubleIncrement> Range(double start, long length, Allocator allocator = Allocator.Temp)
            => new RangeRepeatEnumerable<double, DoubleIncrement>(start, length, default, allocator);

        public static RangeRepeatEnumerable<decimal, DecimalIncrement> Range(decimal start, long length, Allocator allocator = Allocator.Temp)
            => new RangeRepeatEnumerable<decimal, DecimalIncrement>(start, length, default, allocator);

        public static RangeRepeatEnumerable<TSource, NoAction<TSource>> Repeat<TSource>(in TSource value, long length, Allocator allocator = Allocator.Temp)
            where TSource : unmanaged
            => new RangeRepeatEnumerable<TSource, NoAction<TSource>>(value, length, default, allocator);
    }
}
