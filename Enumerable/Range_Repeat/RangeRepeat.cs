namespace UniNativeLinq
{
    public readonly struct SingleIncrement : IRangeRepeat<float>
    {
        public void Execute(ref float value) => ++value;
        public void Back(ref float value) => --value;
        public void Execute(ref float value, long count) => value += count;
        public void Back(ref float value, long count) => value -= count;
    }

    public readonly struct DoubleIncrement : IRangeRepeat<double>
    {
        public void Execute(ref double value) => ++value;
        public void Back(ref double value) => --value;
        public void Execute(ref double value, long count) => value += count;
        public void Back(ref double value, long count) => value -= count;
    }

    public readonly struct DecimalIncrement : IRangeRepeat<decimal>
    {
        public void Execute(ref decimal value) => ++value;
        public void Back(ref decimal value) => --value;
        public void Execute(ref decimal value, long count) => value += count;
        public void Back(ref decimal value, long count) => value -= count;
    }

    public readonly struct Int32Increment : IRangeRepeat<int>
    {
        public void Execute(ref int value) => ++value;
        public void Back(ref int value) => --value;
        public void Execute(ref int value, long count) => value = (int)(value + count);
        public void Back(ref int value, long count) => value = (int)(value - count);
    }

    public readonly struct Int64Increment : IRangeRepeat<long>
    {
        public void Execute(ref long value) => ++value;
        public void Back(ref long value) => --value;
        public void Execute(ref long value, long count) => value += count;
        public void Back(ref long value, long count) => value -= count;
    }

    public readonly struct UInt64Increment : IRangeRepeat<ulong>
    {
        public void Execute(ref ulong value) => ++value;
        public void Back(ref ulong value) => --value;
        public void Execute(ref ulong value, long count) => value = (ulong)((long)value + count);
        public void Back(ref ulong value, long count) => value = (ulong)((long)value - count);
    }

    public readonly struct UInt32Increment : IRangeRepeat<uint>
    {
        public void Execute(ref uint value) => ++value;
        public void Back(ref uint value) => --value;
        public void Execute(ref uint value, long count) => value = (uint)(value + count);
        public void Back(ref uint value, long count) => value = (uint)(value - count);
    }

    public readonly struct NoAction<T> : IRangeRepeat<T>
        where T : unmanaged
    {
        public void Execute(ref T value) { }
        public void Back(ref T value) { }
        public void Execute(ref T value, long count) { }
        public void Back(ref T value, long count) { }
    }
}