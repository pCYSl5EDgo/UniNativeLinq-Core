namespace UniNativeLinq
{
    public interface IWhereIndex<TSource>
    {
        bool Calc(ref TSource value, long index);
    }
}