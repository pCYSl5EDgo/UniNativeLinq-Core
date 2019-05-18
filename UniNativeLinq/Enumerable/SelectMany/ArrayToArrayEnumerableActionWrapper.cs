using System;

namespace UniNativeLinq
{
    public readonly struct
        ArrayToArrayEnumerableActionWrapper<TSource, TResult>
        : IRefAction<TSource, ArrayEnumerable<TResult>>
        where TSource : unmanaged
        where TResult : unmanaged
    {
        private readonly Func<TSource, TResult[]> func;
        public ArrayToArrayEnumerableActionWrapper(Func<TSource, TResult[]> func)
            => this.func = func;

        public void Execute(ref TSource arg0, ref ArrayEnumerable<TResult> arg1) 
            => arg1 = func(arg0).AsRefEnumerable();

        public static implicit operator
            ArrayToArrayEnumerableActionWrapper<TSource, TResult>
            (Func<TSource, TResult[]> func)
            => new ArrayToArrayEnumerableActionWrapper<TSource, TResult>(func);
    }
}
