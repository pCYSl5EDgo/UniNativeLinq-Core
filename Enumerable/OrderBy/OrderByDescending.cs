namespace pcysl5edgo.Collections.LINQ
{
    public readonly struct OrderByDescending<TSource, TComparer>
        : IRefFunc<TSource, TSource, int>
        where TComparer : struct, IRefFunc<TSource, TSource, int>
    {
        private readonly TComparer comparer;
        public OrderByDescending(in TComparer comparer) => this.comparer = comparer;
        public int Calc(ref TSource arg0, ref TSource arg1) => comparer.Calc(ref arg1, ref arg0);

        public static  implicit  operator OrderByDescending<TSource, TComparer>(in TComparer comparer)
            => new OrderByDescending<TSource, TComparer>(comparer);
    }
}