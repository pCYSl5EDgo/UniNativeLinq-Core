namespace UniNativeLinq
{
    public struct 
        DelegateRefFuncToStructOperatorFunc<T0, T1>
    {
        public RefFunc<T0, T1> Func;
        public DelegateRefFuncToStructOperatorFunc(RefFunc<T0, T1> func) => Func = func;
        public static implicit operator DelegateRefFuncToStructOperatorFunc<T0, T1>(RefFunc<T0, T1> func) => new DelegateRefFuncToStructOperatorFunc<T0, T1>(func);
    }

    public struct
        DelegateRefFuncToStructOperatorFunc<T0, T1, T2>
    {
        public RefFunc<T0, T1, T2> Func;
        public DelegateRefFuncToStructOperatorFunc(RefFunc<T0, T1, T2> func) => Func = func;
        public static implicit operator DelegateRefFuncToStructOperatorFunc<T0, T1, T2>(RefFunc<T0, T1, T2> func) => new DelegateRefFuncToStructOperatorFunc<T0, T1, T2>(func);
    }
}
