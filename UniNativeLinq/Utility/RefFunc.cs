namespace UniNativeLinq
{
    public delegate TResult RefFunc<T0, TResult>(ref T0 arg0);
    public delegate TResult RefFunc<T0, T1, TResult>(ref T0 arg0, ref T1 arg1);

    public delegate bool RefWhereIndex<T0>(ref T0 arg0, long index);
}
