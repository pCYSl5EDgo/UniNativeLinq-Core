using System.Collections.Generic;

namespace UniNativeLinq
{
    public readonly struct OrderByFromIComparer<TSource>
         : IRefFunc<TSource, TSource, int>
    {
        private readonly IComparer<TSource> comparer;
        public OrderByFromIComparer(IComparer<TSource> comparer) => this.comparer = comparer;
        public int Calc(ref TSource arg0, ref TSource arg1) => comparer.Compare(arg0, arg1);
    }
}