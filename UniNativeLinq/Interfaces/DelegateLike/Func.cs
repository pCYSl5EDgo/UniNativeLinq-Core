namespace UniNativeLinq
{
    public interface IFunc<out T>
    {
        T Calc();
    }
    public interface IRefFunc<T0, out TResult>
    {
        TResult Calc(ref T0 arg0);
    }
    public interface IRefFunc<T0, T1, out TResult>
    {
        TResult Calc(ref T0 arg0, ref T1 arg1);
    }
}
