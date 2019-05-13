using System;
using System.Collections.Generic;

namespace UniNativeLinq
{
    public readonly struct
        OrderByDelegateKeySelector<TSource, TKey>
        : IRefFunc<TSource, TSource, int>
    {
        private readonly Func<TSource, TKey> keySelector;
        private readonly IComparer<TKey> comparer;
        private readonly bool descending;

        public OrderByDelegateKeySelector(Func<TSource, TKey> keySelector, IComparer<TKey> comparer, bool descending)
        {
            this.keySelector = keySelector;
            this.comparer = comparer;
            this.descending = descending;
        }

        public int Calc(ref TSource arg0, ref TSource arg1) 
            => descending ? comparer.Compare(keySelector(arg1), keySelector(arg0)) : comparer.Compare(keySelector(arg0), keySelector(arg1));
    }
}
