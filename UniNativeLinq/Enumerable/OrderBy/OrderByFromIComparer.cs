using System.Collections.Generic;

namespace UniNativeLinq
{
    public readonly struct OrderByFromIComparer<T>
         : IRefFunc<T, T, int>
    {
        private readonly IComparer<T> comparer;
        public OrderByFromIComparer(IComparer<T> comparer) => this.comparer = comparer;
        public int Calc(ref T arg0, ref T arg1) => comparer.Compare(arg0, arg1);
    }
}