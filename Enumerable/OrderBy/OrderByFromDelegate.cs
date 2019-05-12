using System;

namespace pcysl5edgo.Collections.LINQ
{
    public readonly struct OrderByFromFunc<TSource>
        : IRefFunc<TSource, TSource, int>
    {
        private readonly Func<TSource, TSource, int> comparer;
        
        public OrderByFromFunc(Func<TSource, TSource, int> comparer) => this.comparer = comparer;
        public int Calc(ref TSource arg0, ref TSource arg1) => comparer(arg0, arg1);

        public static implicit operator OrderByFromFunc<TSource>(Func<TSource, TSource, int> comparer) => new OrderByFromFunc<TSource>(comparer);
    }
}