namespace UniNativeLinq
{
    public interface ISelectIndex<T, TResult>
    {
        void Execute(ref T source, long index, ref TResult result);
    }
}