namespace UniNativeLinq
{
    public readonly struct MaxByPredicate<T, TComparer>
        : IRefFunc<T, T, bool>
        where T : unmanaged
        where TComparer : struct, IRefFunc<T, T, int>
    {
        public readonly TComparer Comparer;
        public MaxByPredicate(in TComparer comparer) => Comparer = comparer;
        public bool Calc(ref T arg0, ref T arg1) => Comparer.Calc(ref arg0, ref arg1) < 0;
    }
}
