using System.Collections.Generic;

namespace UniNativeLinq
{
    public struct OrderByFromIComparer<T>
         : IRefFunc<T, T, int>
    {
        public IComparer<T> Func;
        public OrderByFromIComparer(IComparer<T> comparer) => this.Func = comparer;
        public int Calc(ref T arg0, ref T arg1) => Func.Compare(arg0, arg1);
    }
}