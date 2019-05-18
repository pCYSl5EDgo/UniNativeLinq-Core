namespace UniNativeLinq
{
    public interface ISelectIndex<TSource, TResult>
    {
        void Execute(ref TSource source, long index, ref TResult result);
    }
}