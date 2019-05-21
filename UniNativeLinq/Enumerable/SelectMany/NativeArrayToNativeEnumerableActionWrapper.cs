using System;
using Unity.Collections;

namespace UniNativeLinq
{
    public readonly struct
        NativeArrayToNativeEnumerableActionWrapper<T, TResult>
        : IRefAction<T, NativeEnumerable<TResult>>
        where T : unmanaged
        where TResult : unmanaged
    {
        private readonly Func<T, NativeArray<TResult>> func;
        public NativeArrayToNativeEnumerableActionWrapper(Func<T, NativeArray<TResult>> func)
            => this.func = func;

        public void Execute(ref T arg0, ref NativeEnumerable<TResult> arg1) 
            => arg1 = func(arg0).AsRefEnumerable();

        public static implicit operator
            NativeArrayToNativeEnumerableActionWrapper<T, TResult>
            (Func<T, NativeArray<TResult>> func)
            => new NativeArrayToNativeEnumerableActionWrapper<T, TResult>(func);
    }
}
