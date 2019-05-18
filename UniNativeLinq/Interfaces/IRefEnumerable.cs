using System.Collections.Generic;
using Unity.Collections;

namespace UniNativeLinq
{
    public unsafe interface IRefEnumerable<TEnumerator, T> : IEnumerable<T>
        where T : unmanaged
        where TEnumerator : struct, IRefEnumerator<T>
    {
        new TEnumerator GetEnumerator();

        bool CanFastCount();
        bool Any();
        int Count();
        long LongCount();

        void CopyTo(T* destination);
        NativeEnumerable<T> ToNativeEnumerable(Allocator allocator);
        NativeArray<T> ToNativeArray(Allocator allocator);
        T[] ToArray();
    }
}