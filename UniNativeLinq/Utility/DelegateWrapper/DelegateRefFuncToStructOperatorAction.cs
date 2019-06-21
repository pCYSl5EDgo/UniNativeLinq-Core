namespace UniNativeLinq.Utility.DelegateWrapper
{
    public struct
        DelegateRefActionToStructOperatorAction<T0, T1>
    {
        public RefAction<T0, T1> Action;
        public DelegateRefActionToStructOperatorAction(RefAction<T0, T1> action) => Action = action;
        public static implicit operator DelegateRefActionToStructOperatorAction<T0, T1>(RefAction<T0, T1> action) => new DelegateRefActionToStructOperatorAction<T0, T1>(action);
    }

    public struct
        DelegateRefActionToStructOperatorAction<T0, T1, T2>
    {
        public RefAction<T0, T1, T2> Action;
        public DelegateRefActionToStructOperatorAction(RefAction<T0, T1, T2> action) => Action = action;
        public static implicit operator DelegateRefActionToStructOperatorAction<T0, T1, T2>(RefAction<T0, T1, T2> action) => new DelegateRefActionToStructOperatorAction<T0, T1, T2>(action);
    }
}
