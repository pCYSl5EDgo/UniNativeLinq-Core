using System;

namespace UniNativeLinq
{
    public struct
        DelegateFuncToStructOperatorAction<TArg0>
        : IRefAction<TArg0>
    {
        public Func<TArg0> Func;
        public DelegateFuncToStructOperatorAction(Func<TArg0> Func) => this.Func = Func;
        public void Execute(ref TArg0 arg0) => arg0 = Func();

        public static implicit operator DelegateFuncToStructOperatorAction<TArg0>(Func<TArg0> Func) => new DelegateFuncToStructOperatorAction<TArg0>(Func);
    }

    public struct
        DelegateFuncToStructOperatorAction<TArg0, TArg1>
        : IRefAction<TArg0, TArg1>
    {
        public Func<TArg0, TArg1> Func;
        public DelegateFuncToStructOperatorAction(Func<TArg0, TArg1> Func) => this.Func = Func;
        public void Execute(ref TArg0 arg0, ref TArg1 arg1) => arg1 = Func(arg0);

        public static implicit operator DelegateFuncToStructOperatorAction<TArg0, TArg1>(Func<TArg0, TArg1> Func) => new DelegateFuncToStructOperatorAction<TArg0, TArg1>(Func);
    }

    public struct
        DelegateFuncToStructOperatorAction<TArg0, TArg1, TArg2>
        : IRefAction<TArg0, TArg1, TArg2>
    {
        public Func<TArg0, TArg1, TArg2> Func;
        public DelegateFuncToStructOperatorAction(Func<TArg0, TArg1, TArg2> Func) => this.Func = Func;
        public void Execute(ref TArg0 arg0, ref TArg1 arg1, ref TArg2 arg2) => arg2 = Func(arg0, arg1);

        public static implicit operator DelegateFuncToStructOperatorAction<TArg0, TArg1, TArg2>(Func<TArg0, TArg1, TArg2> Func) => new DelegateFuncToStructOperatorAction<TArg0, TArg1, TArg2>(Func);
    }
}
