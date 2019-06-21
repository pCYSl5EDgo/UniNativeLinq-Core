namespace UniNativeLinq
{
    public struct
        DelegateRefActionToStructOperatorAction<T0, T1>
        : IRefAction<T0, T1>
    {
        public RefAction<T0, T1> Action;
        public DelegateRefActionToStructOperatorAction(RefAction<T0, T1> action) => Action = action;
        public static implicit operator DelegateRefActionToStructOperatorAction<T0, T1>(RefAction<T0, T1> action) => new DelegateRefActionToStructOperatorAction<T0, T1>(action);
        public void Execute(ref T0 arg0, ref T1 arg1) => Action(ref arg0, ref arg1);
    }

    public struct
        DelegateRefActionToStructOperatorAction<T0, T1, T2>
        : IRefAction<T0, T1, T2>
    {
        public RefAction<T0, T1, T2> Action;
        public DelegateRefActionToStructOperatorAction(RefAction<T0, T1, T2> action) => Action = action;
        public static implicit operator DelegateRefActionToStructOperatorAction<T0, T1, T2>(RefAction<T0, T1, T2> action) => new DelegateRefActionToStructOperatorAction<T0, T1, T2>(action);
        public void Execute(ref T0 arg0, ref T1 arg1, ref T2 arg2) => Action(ref arg0, ref arg1, ref arg2);
    }
}
