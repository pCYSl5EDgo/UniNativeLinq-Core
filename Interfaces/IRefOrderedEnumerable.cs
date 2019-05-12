using System.Linq;

namespace pcysl5edgo.Collections.LINQ
{
    public interface IRefOrderedEnumerable<TEnumerator, TSource, TPrevEnumerable, TPrevEnumerator, TComparer>
        : IRefEnumerable<TEnumerator, TSource>, IOrderedEnumerable<TSource>
        where TSource : unmanaged
        where TEnumerator : struct, IRefEnumerator<TSource>
        where TPrevEnumerable : struct, IRefEnumerable<TPrevEnumerator, TSource>
        where TPrevEnumerator : struct, IRefEnumerator<TSource>
        where TComparer : struct, IRefFunc<TSource, TSource, int>
    {
        OrderByEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, CompoundOrderBy<TSource, TComparer, OrderByKeySelector<TSource, TKey0, TKeySelector0, TComparer0>>>
        CreateRefOrderedEnumerable<TKey0, TKeySelector0, TComparer0>(TKeySelector0 keySelector, TComparer0 comparer, bool descending)
            where TKey0 : unmanaged
            where TKeySelector0 : struct, IRefAction<TSource, TKey0>
            where TComparer0 : struct, IRefFunc<TKey0, TKey0, int>;
    }
}