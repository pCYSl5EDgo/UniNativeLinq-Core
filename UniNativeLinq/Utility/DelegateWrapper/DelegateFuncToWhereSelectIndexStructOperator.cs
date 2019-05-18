using System;

namespace UniNativeLinq
{
    public struct
        DelegateFuncToWhereIndexStructOperator<TSource>
        : IWhereIndex<TSource>
        where TSource : unmanaged
    {
        public Func<TSource, long, bool> Func;

        public DelegateFuncToWhereIndexStructOperator(Func<TSource, long, bool> Func) => this.Func = Func;

        public bool Calc(ref TSource value, long index) => Func(value, index);

        public static implicit operator DelegateFuncToWhereIndexStructOperator<TSource>(Func<TSource, long, bool> Func)
            => new DelegateFuncToWhereIndexStructOperator<TSource>(Func);
    }

    public struct
        DelegateFuncToSelectIndexStructOperator<TSource, TResult>
        : ISelectIndex<TSource, TResult>
        where TSource : unmanaged
        where TResult : unmanaged
    {
        public Func<TSource, long, TResult> Func;

        public DelegateFuncToSelectIndexStructOperator(Func<TSource, long, TResult> Func) => this.Func = Func;

        public void Execute(ref TSource source, long index, ref TResult result) => result = Func(source, index);

        public static implicit operator DelegateFuncToSelectIndexStructOperator<TSource, TResult>(Func<TSource, long, TResult> Func)
            => new DelegateFuncToSelectIndexStructOperator<TSource, TResult>(Func);
    }
}
