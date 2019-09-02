namespace UniNativeLinq.Average
{
    public interface IAverageOperator<T, TResult> : IRefAction<TResult, T>
    {
        bool TryCalculateResult(TResult accumulate, out TResult result);
    }
}