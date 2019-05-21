using System.Linq;

namespace UniNativeLinq
{
    public interface IRefOrderedEnumerable<TEnumerator, T, TPrevEnumerable, TPrevEnumerator, TComparer>
        : IRefEnumerable<TEnumerator, T>, IOrderedEnumerable<T>
        where T : unmanaged
        where TEnumerator : struct, IRefEnumerator<T>
        where TPrevEnumerable : struct, IRefEnumerable<TPrevEnumerator, T>
        where TPrevEnumerator : struct, IRefEnumerator<T>
        where TComparer : struct, IRefFunc<T, T, int>
    {
        OrderByEnumerable<TPrevEnumerable, TPrevEnumerator, T, CompoundOrderBy<T, TComparer, OrderByKeySelector<T, TKey0, TKeySelector0, TComparer0>>>
        CreateRefOrderedEnumerable<TKey0, TKeySelector0, TComparer0>(TKeySelector0 keySelector, TComparer0 comparer, bool descending)
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefAction<T, TKey0>
            where TComparer0 : struct, IRefFunc<TKey0, TKey0, int>;
    }
}