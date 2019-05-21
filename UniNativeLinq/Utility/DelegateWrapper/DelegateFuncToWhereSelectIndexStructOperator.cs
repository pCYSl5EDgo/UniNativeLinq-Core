using System;

namespace UniNativeLinq
{
    public struct
        DelegateFuncToWhereIndexStructOperator<T>
        : IWhereIndex<T>
        where T : unmanaged
    {
        public Func<T, long, bool> Func;

        public DelegateFuncToWhereIndexStructOperator(Func<T, long, bool> Func) => this.Func = Func;

        public bool Calc(ref T value, long index) => Func(value, index);

        public static implicit operator DelegateFuncToWhereIndexStructOperator<T>(Func<T, long, bool> Func)
            => new DelegateFuncToWhereIndexStructOperator<T>(Func);
    }

    public struct
        DelegateFuncToSelectIndexStructOperator<T, TResult>
        : ISelectIndex<T, TResult>
        where T : unmanaged
        where TResult : unmanaged
    {
        public Func<T, long, TResult> Func;

        public DelegateFuncToSelectIndexStructOperator(Func<T, long, TResult> Func) => this.Func = Func;

        public void Execute(ref T source, long index, ref TResult result) => result = Func(source, index);

        public static implicit operator DelegateFuncToSelectIndexStructOperator<T, TResult>(Func<T, long, TResult> Func)
            => new DelegateFuncToSelectIndexStructOperator<T, TResult>(Func);
    }
}
