using Unity.Collections;

namespace UniNativeLinq
{
    public static class SelectEnumerable
    {
        public static SelectEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TResult, TAction> Select<T, TResult, TAction>(this NativeArray<T> array, TAction action, Allocator allocator)
            where T : unmanaged
            where TResult : unmanaged
            where TAction : struct, IRefAction<T, TResult>
            => new SelectEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TResult, TAction>(array.AsRefEnumerable(), action, allocator);
    }
}