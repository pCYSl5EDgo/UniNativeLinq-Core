namespace UniNativeLinq
{
    public interface IWhereIndex<T>
    {
        bool Calc(ref T value, long index);
    }
}