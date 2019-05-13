using Unity.Collections;

namespace UniNativeLinq
{
    public static class SelectIndexEnumerable
    {
        public static SelectIndexEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TResult, TAction>
            SelectIndex<T, TResult, TAction>(this NativeArray<T> array, TAction action, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TResult : unmanaged
            where TAction : struct, ISelectIndex<T, TResult>
            => new SelectIndexEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TResult, TAction>(array.AsRefEnumerable(), action, allocator);

        public static SelectIndexEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TResult, TAction>
            SelectIndex<T, TResult, TAction>(this NativeEnumerable<T> enumerable, TAction action, Allocator allocator = Allocator.Temp)
            where T : unmanaged
            where TResult : unmanaged
            where TAction : struct, ISelectIndex<T, TResult>
            => new SelectIndexEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TResult, TAction>(enumerable, action, allocator);
    }
}