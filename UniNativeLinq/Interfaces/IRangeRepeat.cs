namespace UniNativeLinq
{
    public interface IRangeRepeat<T>
    {
        void Execute(ref T value);
        void Back(ref T value);
        void Execute(ref T value, long count);
        void Back(ref T value, long count);
    }
}
