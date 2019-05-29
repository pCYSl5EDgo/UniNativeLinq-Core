using System;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UniNativeLinq
{
    public static class NativeEnumerable
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Aggregate<TEnumerable, TEnumerator, T, TAccumulate0, TFunc0>(in this AppendEnumerable<TEnumerable, TEnumerator, T> @this, ref TAccumulate0 seed, TFunc0 func)
            where T : unmanaged
            where TEnumerator : struct, IRefEnumerator<T>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, T>
            where TFunc0 : IRefAction<TAccumulate0, T>
        {
            var enumerator = @this.GetEnumerator();
            while (true)
            {
                ref var current = ref enumerator.TryGetNext(out var success);
                if (!success) break;
                func.Execute(ref seed, ref current);
            }
            enumerator.Dispose();
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