namespace UniNativeLinq
{
    public struct
        FuncToAction<TFunc, TResult>
        : IRefAction<TResult>
        where TFunc : IFunc<TResult>
    {
        private TFunc func;
        public FuncToAction(in TFunc func) => this.func = func;

        public void Execute(ref TResult arg1) => arg1 = func.Calc();

        public static implicit operator FuncToAction<TFunc, TResult>(in TFunc func) => new FuncToAction<TFunc, TResult>(func);
    }

    public struct
        FuncToAction<TFunc, TArg0, TResult>
        : IRefAction<TArg0, TResult>
        where TFunc : IRefFunc<TArg0, TResult>
    {
        private TFunc func;
        public FuncToAction(in TFunc func) => this.func = func;

        public void Execute(ref TArg0 arg0, ref TResult arg1) => arg1 = func.Calc(ref arg0);

        public static implicit operator FuncToAction<TFunc, TArg0, TResult>(in TFunc func) => new FuncToAction<TFunc, TArg0, TResult>(func);
    }

    public struct
        FuncToAction<TFunc, TArg0, TArg1, TResult>
        : IRefAction<TArg0, TArg1, TResult>
        where TFunc : IRefFunc<TArg0, TArg1, TResult>
    {
        private TFunc func;
        public FuncToAction(in TFunc func) => this.func = func;

        public void Execute(ref TArg0 arg0, ref TArg1 arg1, ref TResult arg2) => arg2 = func.Calc(ref arg0, ref arg1);

        public static implicit operator FuncToAction<TFunc, TArg0, TArg1, TResult>(in TFunc func) => new FuncToAction<TFunc, TArg0, TArg1, TResult>(func);
    }
}
