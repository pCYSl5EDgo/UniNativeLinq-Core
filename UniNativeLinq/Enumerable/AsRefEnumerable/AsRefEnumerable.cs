using System;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UniNativeLinq
{
    public static class NativeEnumerable
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TResult0 Aggregate<TEnumerable, TEnumerator, T, TAccumulate0, TFunc0, TResult0, TResultFunc0>(in this AppendEnumerable<TEnumerable, TEnumerator, T> @this, ref TAccumulate0 seed, Func<TAccumulate0, T, TAccumulate0> func, Func<TAccumulate0, TResult0> resultFunc)
            where T : unmanaged
            where TEnumerator : struct, IRefEnumerator<T>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, T>
        {
            var enumerator = @this.GetEnumerator();
            while (true)
            {
                ref var current = ref enumerator.TryGetNext(out var success);
                if (!success) break;
                seed = func(seed, current);
            }
            enumerator.Dispose();
            return resultFunc(seed);
        }

        public static NativeEnumerable<T> AsRefEnumerable<T>(this NativeArray<T> array)
            where T : unmanaged
            => new NativeEnumerable<T>(array);

        public static ArrayEnumerable<T> AsRefEnumerable<T>(this T[] array)
            where T : unmanaged
            => new ArrayEnumerable<T>(array);

        public static ArrayEnumerable<T> AsRefEnumerable<T>(this T[] array, long offset, long count)
            where T : unmanaged
            => new ArrayEnumerable<T>(array, offset, count);

        public static ArrayEnumerable<T> AsRefEnumerable<T>(this ArraySegment<T> arraySegment)
            where T : unmanaged
            => new ArrayEnumerable<T>(arraySegment);
    }
}