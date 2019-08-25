namespace UniNativeLinq
{
    public struct AsIs<T0> : IRefAction<T0, T0>
    {
        public void Execute(ref T0 arg0, ref T0 arg1) => arg1 = arg0;
    }
}
