using System;
using System.Collections.Generic;

namespace UniNativeLinq
{
    public struct
        OrderByDelegateKeySelector<T, TKey>
        : IRefFunc<T, T, int>
    {
        private Func<T, TKey> keySelector;
        private IComparer<TKey> comparer;
        private bool descending;

        public OrderByDelegateKeySelector(Func<T, TKey> keySelector, IComparer<TKey> comparer, bool descending)
        {
            this.keySelector = keySelector;
            this.comparer = comparer;
            this.descending = descending;
        }

        public int Calc(ref T arg0, ref T arg1) 
            => descending ? comparer.Compare(keySelector(arg1), keySelector(arg0)) : comparer.Compare(keySelector(arg0), keySelector(arg1));
    }
}
