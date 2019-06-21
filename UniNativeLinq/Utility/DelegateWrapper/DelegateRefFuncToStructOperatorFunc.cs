namespace UniNativeLinq
{
    public struct
        DelegateRefFuncToStructOperatorFunc<T0, T1>
        : IRefFunc<T0, T1>
    {
        public RefFunc<T0, T1> Func;
        public DelegateRefFuncToStructOperatorFunc(RefFunc<T0, T1> func) => Func = func;
        public static implicit operator DelegateRefFuncToStructOperatorFunc<T0, T1>(RefFunc<T0, T1> func) => new DelegateRefFuncToStructOperatorFunc<T0, T1>(func);
        public T1 Calc(ref T0 arg0) => Func(ref arg0);
    }

    public struct
        DelegateRefFuncToStructOperatorFunc<T0, T1, T2>
        : IRefFunc<T0, T1, T2>
    {
        public RefFunc<T0, T1, T2> Func;
        public DelegateRefFuncToStructOperatorFunc(RefFunc<T0, T1, T2> func) => Func = func;
        public static implicit operator DelegateRefFuncToStructOperatorFunc<T0, T1, T2>(RefFunc<T0, T1, T2> func) => new DelegateRefFuncToStructOperatorFunc<T0, T1, T2>(func);
        public T2 Calc(ref T0 arg0, ref T1 arg1) => Func(ref arg0, ref arg1);
    }
}
