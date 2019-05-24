using Unity.Collections;

namespace UniNativeLinq
{
    public static class Enumerable
    {
        public static RangeRepeatEnumerable<int, Int32Increment> Range(int start, long length, Allocator allocator = Allocator.Temp)
            => new RangeRepeatEnumerable<int, Int32Increment>(start, length, default);

        public static RangeRepeatEnumerable<long, Int64Increment> Range(long start, long length, Allocator allocator = Allocator.Temp)
            => new RangeRepeatEnumerable<long, Int64Increment>(start, length, default);

        public static RangeRepeatEnumerable<uint, UInt32Increment> Range(uint start, long length, Allocator allocator = Allocator.Temp)
            => new RangeRepeatEnumerable<uint, UInt32Increment>(start, length, default);

        public static RangeRepeatEnumerable<ulong, UInt64Increment> Range(ulong start, long length, Allocator allocator = Allocator.Temp)
            => new RangeRepeatEnumerable<ulong, UInt64Increment>(start, length, default);

        public static RangeRepeatEnumerable<float, SingleIncrement> Range(float start, long length, Allocator allocator = Allocator.Temp)
            => new RangeRepeatEnumerable<float, SingleIncrement>(start, length, default);

        public static RangeRepeatEnumerable<double, DoubleIncrement> Range(double start, long length, Allocator allocator = Allocator.Temp)
            => new RangeRepeatEnumerable<double, DoubleIncrement>(start, length, default);

        public static RangeRepeatEnumerable<decimal, DecimalIncrement> Range(decimal start, long length, Allocator allocator = Allocator.Temp)
            => new RangeRepeatEnumerable<decimal, DecimalIncrement>(start, length, default);

        public static RangeRepeatEnumerable<T, NoAction<T>> Repeat<T>(in T value, long length, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            => new RangeRepeatEnumerable<T, NoAction<T>>(value, length, default);
    }
}
