using Unity.Collections;

namespace UniNativeLinq
{
    public struct
        IntersectOperation<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TComparer>
        : ISetOperation<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>
        where T : unmanaged
        where TEnumerator0 : struct, IRefEnumerator<T>
        where TEnumerator1 : struct, IRefEnumerator<T>
        where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
        where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T>
        where TComparer : struct, IRefFunc<T, T, bool>
    {
        public TComparer Func;

        public IntersectOperation(in TComparer comparer) => Func = comparer;

        public NativeEnumerable<T> Calc(ref TEnumerable0 first, ref TEnumerable1 second, Allocator allocator)
        {
            var d0 = new DistinctEnumerable<TEnumerable0, TEnumerator0, T, TComparer>(first, Func, Allocator.Temp).ToNativeEnumerable(Allocator.Temp);
            if (d0.Length == 0)
            {
                d0.Dispose(Allocator.Temp);
                return default;
            }
            var d1 = new DistinctEnumerable<TEnumerable1, TEnumerator1, T, TComparer>(second, Func, Allocator.Temp).ToNativeEnumerable(Allocator.Temp);
            if (d1.Length == 0)
            {
                d1.Dispose(Allocator.Temp);
                d0.Dispose(Allocator.Temp);
                return default;
            }
            var answer = new NativeList<T>(allocator);
            foreach (ref var i in d0)
            {
                foreach (ref var j in d1)
                {
                    if (!Func.Calc(ref i, ref j)) continue;
                    answer.Add(i);
                    break;
                }
            }
            return answer.AsNativeEnumerable();
        }

        public static implicit operator
            IntersectOperation<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TComparer>
            (in TComparer comparer)
            => new IntersectOperation<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TComparer>
            (comparer);
    }
}
