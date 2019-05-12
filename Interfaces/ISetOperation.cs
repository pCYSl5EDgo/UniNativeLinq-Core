using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public interface ISetOperation<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>
        where TSource : unmanaged
        where TFirstEnumerator : struct, IRefEnumerator<TSource>
        where TFirstEnumerable : struct, IRefEnumerable<TFirstEnumerator, TSource>
        where TSecondEnumerator : struct, IRefEnumerator<TSource>
        where TSecondEnumerable : struct, IRefEnumerable<TSecondEnumerator, TSource>
    {
        NativeEnumerable<TSource> Calc(ref TFirstEnumerable first, ref TSecondEnumerable second, Allocator allocator);
    }
}
