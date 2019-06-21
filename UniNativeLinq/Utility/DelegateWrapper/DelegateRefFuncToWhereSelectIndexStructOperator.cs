namespace UniNativeLinq
{
    public struct
        DelegateRefFuncToWhereIndexStructOperator<T>
        : IWhereIndex<T>
        where T : unmanaged
    {
        public RefWhereIndex<T> Func;

        public DelegateRefFuncToWhereIndexStructOperator(RefWhereIndex<T> func) => Func = func;

        public bool Calc(ref T value, long index) => Func(ref value, index);

        public static implicit operator DelegateRefFuncToWhereIndexStructOperator<T>(RefWhereIndex<T> func)
            => new DelegateRefFuncToWhereIndexStructOperator<T>(func);
    }

    public struct
        DelegateRefFuncToSelectIndexStructOperator<T, TResult>
        : ISelectIndex<T, TResult>
        where T : unmanaged
        where TResult : unmanaged
    {
        public RefSelectIndex<T, TResult> Func;

        public DelegateRefFuncToSelectIndexStructOperator(RefSelectIndex<T, TResult> func) => Func = func;

        public void Execute(ref T source, long index, ref TResult result) => Func(ref source, index, ref result);

        public static implicit operator DelegateRefFuncToSelectIndexStructOperator<T, TResult>(RefSelectIndex<T, TResult> func)
            => new DelegateRefFuncToSelectIndexStructOperator<T, TResult>(func);
    }
}
