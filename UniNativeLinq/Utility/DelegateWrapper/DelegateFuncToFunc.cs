using System;

namespace UniNativeLinq
{
    public struct
        DelegateFuncToStructOperatorFunc<TResult>
        : IFunc<TResult>
    {
        public Func<TResult> Func;

        public DelegateFuncToStructOperatorFunc(Func<TResult> Func) => this.Func = Func;

        public TResult Calc() => Func();

        public static implicit operator
            DelegateFuncToStructOperatorFunc<TResult>(Func<TResult> Func)
            => new DelegateFuncToStructOperatorFunc<TResult>(Func);
    }

    public struct
        DelegateFuncToStructOperatorFunc<TArg0, TResult>
        : IRefFunc<TArg0, TResult>
    {
        public Func<TArg0, TResult> Func;

        public DelegateFuncToStructOperatorFunc(Func<TArg0, TResult> Func) => this.Func = Func;

        public unsafe TResult Calc(ref TArg0 arg0) => Func(arg0);

        public static implicit operator
            DelegateFuncToStructOperatorFunc<TArg0, TResult>(Func<TArg0, TResult> Func)
            => new DelegateFuncToStructOperatorFunc<TArg0, TResult>(Func);
    }

    public struct
        DelegateFuncToStructOperatorFunc<TArg0, TArg1, TResult>
        : IRefFunc<TArg0, TArg1, TResult>
    {
        public Func<TArg0, TArg1, TResult> Func;

        public DelegateFuncToStructOperatorFunc(Func<TArg0, TArg1, TResult> Func) => this.Func = Func;

        public TResult Calc(ref TArg0 arg0, ref TArg1 arg1) => Func(arg0, arg1);

        public static implicit operator
            DelegateFuncToStructOperatorFunc<TArg0, TArg1, TResult>(Func<TArg0, TArg1, TResult> Func)
            => new DelegateFuncToStructOperatorFunc<TArg0, TArg1, TResult>(Func);
    }
}
