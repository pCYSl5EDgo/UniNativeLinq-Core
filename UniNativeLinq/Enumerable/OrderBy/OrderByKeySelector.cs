namespace UniNativeLinq
{
    public readonly struct 
        OrderByKeySelector<TSource, TKey, TKeySelector, TComparer>
        : IRefFunc<TSource, TSource, int>
        where TKeySelector : struct, IRefAction<TSource, TKey>
        where TComparer : struct, IRefFunc<TKey, TKey, int>
    {
        private readonly TKeySelector keySelector;
        private readonly TComparer comparer;
        private readonly bool descending;

        public OrderByKeySelector(TKeySelector keySelector, TComparer comparer, bool descending)
        {
            this.keySelector = keySelector;
            this.comparer = comparer;
            this.descending = descending;
        }

        public int Calc(ref TSource arg0, ref TSource arg1)
        {
            TKey key0 = default, key1 = default;
            if (descending)
            {
                keySelector.Execute(ref arg0, ref key1);
                keySelector.Execute(ref arg1, ref key0);
            }
            else
            {
                keySelector.Execute(ref arg0, ref key0);
                keySelector.Execute(ref arg1, ref key1);
            }
            return comparer.Calc(ref key0, ref key1);
        }
    }
}
