using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    public readonly unsafe struct DefaultOrderByDescending<T>
        : IRefFunc<T, T, int>
        where T : unmanaged
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Calc(ref T arg0, ref T arg1)
        {
            return UnsafeUtility.MemCmp(Pseudo.AsPointer(ref arg1), Pseudo.AsPointer(ref arg0), sizeof(T));
        }
    }
}