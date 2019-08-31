namespace UniNativeLinq
{
    public struct
        DelegateRefActionToStructOperatorAction<T0, T1>
        : IRefAction<T0, T1>
    {
        public RefAction<T0, T1> Func;
        public DelegateRefActionToStructOperatorAction(RefAction<T0, T1> func) => Func = func;
        public static implicit operator DelegateRefActionToStructOperatorAction<T0, T1>(RefAction<T0, T1> func) => new DelegateRefActionToStructOperatorAction<T0, T1>(func);
        public void Execute(ref T0 arg0, ref T1 arg1) => Func(ref arg0, ref arg1);
    }

    public struct
        DelegateRefActionToStructOperatorAction<T0, T1, T2>
        : IRefAction<T0, T1, T2>
    {
        public RefAction<T0, T1, T2> Func;
        public DelegateRefActionToStructOperatorAction(RefAction<T0, T1, T2> func) => Func = func;
        public static implicit operator DelegateRefActionToStructOperatorAction<T0, T1, T2>(RefAction<T0, T1, T2> func) => new DelegateRefActionToStructOperatorAction<T0, T1, T2>(func);
        public void Execute(ref T0 arg0, ref T1 arg1, ref T2 arg2) => Func(ref arg0, ref arg1, ref arg2);
    }
}
