using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    public unsafe struct
        IntersectOperation<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TComparer>
        : ISetOperation<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>
        where T : unmanaged
        where TEnumerator0 : struct, IRefEnumerator<T>
        where TEnumerator1 : struct, IRefEnumerator<T>
        where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
        where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T>
        where TComparer : struct, IRefFunc<T, T, int>
    {
        private TComparer comparer;

        public IntersectOperation(in TComparer comparer) => this.comparer = comparer;

        public NativeEnumerable<T> Calc(ref TEnumerable0 first, ref TEnumerable1 second, Allocator allocator)
        {
            var smaller = new SortedDistinctEnumerable<TEnumerable0, TEnumerator0, T, TComparer>(first, comparer, Allocator.Temp).ToNativeEnumerable();
            if (smaller.Length == 0)
            {
                smaller.Dispose(Allocator.Temp);
                return default;
            }
            var capacity = smaller.Length;
            var larger = new SortedDistinctEnumerable<TEnumerable1, TEnumerator1, T, TComparer>(second, comparer, Allocator.Temp).ToNativeEnumerable();
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
            var ptr = UnsafeUtilityEx.Malloc<T>(capacity, allocator);
            var count = 0L;
            for (var i = 0L; i < capacity; i++)
            {
                if (larger.FindIndexBinarySearch(ref smaller[i], comparer) != -1) continue;
                ptr[count++] = smaller[i];
            }
            smaller.Dispose(Allocator.Temp);
            larger.Dispose(Allocator.Temp);
            if (count != 0) return NativeEnumerable<T>.Create(ptr, count);
            UnsafeUtility.Free(ptr, allocator);
            return default;
        }

        public static implicit operator
            IntersectOperation<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TComparer>
            (in TComparer comparer)
            => new IntersectOperation<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TComparer>
            (comparer);
    }
}
