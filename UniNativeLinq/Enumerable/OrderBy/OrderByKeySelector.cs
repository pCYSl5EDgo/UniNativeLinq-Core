namespace UniNativeLinq
{
    public struct 
        OrderByKeySelector<T, TKey, TKeySelector, TComparer>
        : IRefFunc<T, T, int>
        where TKeySelector : struct, IRefAction<T, TKey>
        where TComparer : struct, IRefFunc<TKey, TKey, int>
    {
        private TKeySelector keySelector;
        private TComparer comparer;
        private bool descending;

        public OrderByKeySelector(TKeySelector keySelector, TComparer comparer, bool descending)
        {
            this.keySelector = keySelector;
            this.comparer = comparer;
            this.descending = descending;
        }

        public int Calc(ref T arg0, ref T arg1)
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
