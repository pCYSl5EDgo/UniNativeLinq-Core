namespace UniNativeLinq
{
    public interface IRangeRepeat<TSource>
    {
        void Execute(ref TSource value);
        void Back(ref TSource value);
        void Execute(ref TSource value, long count);
        void Back(ref TSource value, long count);
    }
}
