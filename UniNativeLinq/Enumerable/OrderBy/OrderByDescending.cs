namespace UniNativeLinq
{
    public readonly struct OrderByDescending<T, TComparer>
        : IRefFunc<T, T, int>
        where TComparer : struct, IRefFunc<T, T, int>
    {
        private readonly TComparer comparer;
        public OrderByDescending(in TComparer comparer) => this.comparer = comparer;
        public int Calc(ref T arg0, ref T arg1) => comparer.Calc(ref arg1, ref arg0);

        public static  implicit  operator OrderByDescending<T, TComparer>(in TComparer comparer)
            => new OrderByDescending<T, TComparer>(comparer);
    }
}