using System;
using Unity.Collections;

namespace UniNativeLinq
{
    public readonly struct
        NativeArrayToNativeEnumerableActionWrapper<TSource, TResult>
        : IRefAction<TSource, NativeEnumerable<TResult>>
        where TSource : unmanaged
        where TResult : unmanaged
    {
        private readonly Func<TSource, NativeArray<TResult>> func;
        public NativeArrayToNativeEnumerableActionWrapper(Func<TSource, NativeArray<TResult>> func)
            => this.func = func;

        public void Execute(ref TSource arg0, ref NativeEnumerable<TResult> arg1) 
            => arg1 = func(arg0).AsRefEnumerable();

        public static implicit operator
            NativeArrayToNativeEnumerableActionWrapper<TSource, TResult>
            (Func<TSource, NativeArray<TResult>> func)
            => new NativeArrayToNativeEnumerableActionWrapper<TSource, TResult>(func);
    }
}
