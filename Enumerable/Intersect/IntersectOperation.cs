using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Collections.LINQ
{
    public unsafe struct
        IntersectOperation<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource, TComparer>
        : ISetOperation<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>
        where TSource : unmanaged
        where TEnumerator0 : struct, IRefEnumerator<TSource>
        where TEnumerator1 : struct, IRefEnumerator<TSource>
        where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
        where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
        where TComparer : struct, IRefFunc<TSource, TSource, int>
    {
        private readonly TComparer comparer;

        public IntersectOperation(in TComparer comparer) => this.comparer = comparer;

        public NativeEnumerable<TSource> Calc(ref TEnumerable0 first, ref TEnumerable1 second, Allocator allocator)
        {
            var smaller = new SortedDistinctEnumerable<TEnumerable0, TEnumerator0, TSource, TComparer>(first, comparer, Allocator.Temp).ToNativeEnumerable();
            if (smaller.Length == 0)
            {
                smaller.Dispose(Allocator.Temp);
                return default;
            }
            var capacity = smaller.Length;
            var larger = new SortedDistinctEnumerable<TEnumerable1, TEnumerator1, TSource, TComparer>(second, comparer, Allocator.Temp).ToNativeEnumerable();
            if (larger.Length == 0)
            {
                smaller.Dispose(Allocator.Temp);
                larger.Dispose(Allocator.Temp);
                return default;
            }
            if (capacity > larger.Length)
            {
                capacity = larger.Length;
                (smaller, larger) = (larger, smaller);
            }
            var ptr = UnsafeUtilityEx.Malloc<TSource>(capacity, allocator);
            var count = 0L;
            for (var i = 0L; i < capacity; i++)
            {
                if (larger.FindIndexBinarySearch(ref smaller[i], comparer) != -1) continue;
                ptr[count++] = smaller[i];
            }
            smaller.Dispose(Allocator.Temp);
            larger.Dispose(Allocator.Temp);
            if (count != 0) return new NativeEnumerable<TSource>(ptr, count);
            UnsafeUtility.Free(ptr, allocator);
            return default;
        }

        public static implicit operator
            IntersectOperation<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource, TComparer>
            (in TComparer comparer)
            => new IntersectOperation<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource, TComparer>
            (comparer);
    }
}
