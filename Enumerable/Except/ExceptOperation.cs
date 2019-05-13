using Unity.Collections;

namespace UniNativeLinq
{
    public readonly unsafe struct
        ExceptOperation<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource, TComparer>
        : ISetOperation<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>
        where TSource : unmanaged
        where TComparer : struct, IRefFunc<TSource, TSource, int>
        where TEnumerator0 : struct, IRefEnumerator<TSource>
        where TEnumerator1 : struct, IRefEnumerator<TSource>
        where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
        where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
    {
        private readonly TComparer comparer;

        public ExceptOperation(in TComparer comparer) => this.comparer = comparer;

        public NativeEnumerable<TSource> Calc(ref TEnumerable0 first, ref TEnumerable1 second, Allocator allocator)
        {
            var targets = new SortedDistinctEnumerable<TEnumerable0, TEnumerator0, TSource, TComparer>(first, comparer, allocator).ToNativeEnumerable();
            if (targets.Length == 0)
            {
                targets.Dispose(allocator);
                return default;
            }
            var removes = new SortedDistinctEnumerable<TEnumerable1, TEnumerator1, TSource, TComparer>(second, comparer, Allocator.Temp).ToNativeEnumerable();
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
            return new NativeEnumerable<TSource>(targets.Ptr, count);
        }

        public static implicit operator
            ExceptOperation<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource, TComparer>
            (in TComparer comparer)
            => new ExceptOperation<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource, TComparer>
            (comparer);
    }
}
