namespace UniNativeLinq
{
    delegate TResult RefFunc<T0, out TResult>(ref T0 arg0);
    
    delegate bool RefWhereIndex<T0>(ref T0 arg0, long index);
}
