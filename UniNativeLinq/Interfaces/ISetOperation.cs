using Unity.Collections;

namespace UniNativeLinq
{
    public interface ISetOperation<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, T>
        where T : unmanaged
        where TFirstEnumerator : struct, IRefEnumerator<T>
        where TFirstEnumerable : struct, IRefEnumerable<TFirstEnumerator, T>
        where TSecondEnumerator : struct, IRefEnumerator<T>
        where TSecondEnumerable : struct, IRefEnumerable<TSecondEnumerator, T>
    {
        NativeEnumerable<T> Calc(ref TFirstEnumerable first, ref TSecondEnumerable second, Allocator allocator);
    }
}
