using Unity.Collections;

namespace UniNativeLinq
{
    public unsafe struct
        ExceptOperation<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TComparer>
        : ISetOperation<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>
        where T : unmanaged
        where TComparer : struct, IRefFunc<T, T, int>
        where TEnumerator0 : struct, IRefEnumerator<T>
        where TEnumerator1 : struct, IRefEnumerator<T>
        where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
        where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T>
    {
        public TComparer Func;

        public ExceptOperation(in TComparer comparer) => Func = comparer;

        public NativeEnumerable<T> Calc(ref TEnumerable0 first, ref TEnumerable1 second, Allocator allocator)
        {
            var targets = new SortedDistinctEnumerable<TEnumerable0, TEnumerator0, T, TComparer>(first, Func, Allocator.Temp).ToNativeEnumerable();
            if (targets.Length == 0)
            {
                targets.Dispose(Allocator.Temp);
                return default;
            }
            var removes = new SortedDistinctEnumerable<TEnumerable1, TEnumerator1, T, TComparer>(second, Func, Allocator.Temp).ToNativeEnumerable();
            if (removes.Length == 0)
            {
                targets.Dispose(Allocator.Temp);
                removes.Dispose(Allocator.Temp);
                return targets;
            }
            var answer = new NativeList<T>(allocator);
            foreach (ref var target in targets)
            {
                var add = true;
                foreach (ref var remove in removes)
                {
                    var compare = Func.Calc(ref target, ref remove);
                    if (compare == 0)
                    {
                        add = false;
                        break;
                    }
                    if (compare < 0)
                    {
                        break;
                    }
                }
                if (!add) continue;
                answer.Add(target);
            }
            removes.Dispose(Allocator.Temp);
            targets.Dispose(Allocator.Temp);
            return answer.AsNativeEnumerable();
        }

        public static implicit operator
            ExceptOperation<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TComparer>
            (in TComparer comparer)
            => new ExceptOperation<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T, TComparer>
            (comparer);
    }
}
