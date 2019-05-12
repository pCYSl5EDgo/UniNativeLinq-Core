namespace pcysl5edgo.Collections.LINQ
{
    public readonly struct ZipValueTuple<T0, T1> : IRefAction<T0, T1, (T0, T1)>
    {
        public void Execute(ref T0 arg0, ref T1 arg1, ref (T0, T1) arg2)
        {
            arg2.Item1 = arg0;
            arg2.Item2 = arg1;
        }
    }
}
