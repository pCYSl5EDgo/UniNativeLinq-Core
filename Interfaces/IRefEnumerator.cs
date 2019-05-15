using System.Collections.Generic;

namespace UniNativeLinq
{
    public interface IRefEnumerator<T> : IEnumerator<T>
        where T : unmanaged
    {
        new ref T Current { get; }
        ref T TryGetNext(out bool success);
    }
}