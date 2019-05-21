using System;
using System.Collections.Generic;

namespace UniNativeLinq
{
    public readonly struct
        OrderByDelegateKeySelector<T, TKey>
        : IRefFunc<T, T, int>
    {
        private readonly Func<T, TKey> keySelector;
        private readonly IComparer<TKey> comparer;
        private readonly bool descending;

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
