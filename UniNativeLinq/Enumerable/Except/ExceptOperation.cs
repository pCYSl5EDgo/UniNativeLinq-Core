using Unity.Collections;

namespace UniNativeLinq
{
    public readonly unsafe struct
        ExceptOperation<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TComparer>
        : ISetOperation<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>
        where T : unmanaged
        where TComparer : struct, IRefFunc<T, T, int>
        where TEnumerator0 : struct, IRefEnumerator<T>
        where TEnumerator1 : struct, IRefEnumerator<T>
        where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
        where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T>
    {
        private readonly TComparer comparer;

        public ExceptOperation(in TComparer comparer) => this.comparer = comparer;

        public NativeEnumerable<T> Calc(ref TEnumerable0 first, ref TEnumerable1 second, Allocator allocator)
        {
            var targets = new SortedDistinctEnumerable<TEnumerable0, TEnumerator0, T, TComparer>(first, comparer, allocator).ToNativeEnumerable();
            if (targets.Length == 0)
            {
                targets.Dispose(allocator);
                return default;
            }
            var removes = new SortedDistinctEnumerable<TEnumerable1, TEnumerator1, T, TComparer>(second, comparer, Allocator.Temp).ToNativeEnumerable();
            if (removes.Length == 0)
            {
                removes.Dispose(Allocator.Temp);
                return targets;
            }
            var count = targets.Length;
            for (var index = count; --index >= 0;)
            {
                if (removes.FindIndexBinarySearch(ref targets[index], comparer) == -1) continue;
                if (index != --count)
                    targets[index] = targets[count];
            }
            return NativeEnumerable<T>.Create(targets.Ptr, count);
        }

        public static implicit operator
            ExceptOperation<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TComparer>
            (in TComparer comparer)
            => new ExceptOperation<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TComparer>
            (comparer);
    }
}
