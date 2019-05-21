using System;

namespace UniNativeLinq
{
    public readonly struct
        ArrayToArrayEnumerableActionWrapper<T, TResult>
        : IRefAction<T, ArrayEnumerable<TResult>>
        where T : unmanaged
        where TResult : unmanaged
    {
        private readonly Func<T, TResult[]> func;
        public ArrayToArrayEnumerableActionWrapper(Func<T, TResult[]> func)
            => this.func = func;

        public void Execute(ref T arg0, ref ArrayEnumerable<TResult> arg1) 
            => arg1 = func(arg0).AsRefEnumerable();

        public static implicit operator
            ArrayToArrayEnumerableActionWrapper<T, TResult>
            (Func<T, TResult[]> func)
            => new ArrayToArrayEnumerableActionWrapper<T, TResult>(func);
    }
}
