using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    public readonly unsafe struct DefaultOrderByAscending<T>
        : IRefFunc<T, T, int>
        where T : unmanaged
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Calc(ref T arg0, ref T arg1)
        {
            if (typeof(T) == typeof(byte))
                return Pseudo.As<T, byte>(ref arg0).CompareTo(Pseudo.As<T, byte>(ref arg1));
            else if (typeof(T) == typeof(sbyte))
                return Pseudo.As<T, sbyte>(ref arg0).CompareTo(Pseudo.As<T, sbyte>(ref arg1));
            else if (typeof(T) == typeof(short))
                return Pseudo.As<T, short>(ref arg0).CompareTo(Pseudo.As<T, short>(ref arg1));
            else if (typeof(T) == typeof(ushort))
                return Pseudo.As<T, ushort>(ref arg0).CompareTo(Pseudo.As<T, ushort>(ref arg1));
            else if (typeof(T) == typeof(int))
                return Pseudo.As<T, int>(ref arg0).CompareTo(Pseudo.As<T, int>(ref arg1));
            else if (typeof(T) == typeof(uint))
                return Pseudo.As<T, uint>(ref arg0).CompareTo(Pseudo.As<T, uint>(ref arg1));
            else if (typeof(T) == typeof(long))
                return Pseudo.As<T, long>(ref arg0).CompareTo(Pseudo.As<T, long>(ref arg1));
            else if (typeof(T) == typeof(ulong))
                return Pseudo.As<T, ulong>(ref arg0).CompareTo(Pseudo.As<T, ulong>(ref arg1));
            else if (typeof(T) == typeof(float))
                return Pseudo.As<T, float>(ref arg0).CompareTo(Pseudo.As<T, float>(ref arg1));
            else if (typeof(T) == typeof(double))
                return Pseudo.As<T, double>(ref arg0).CompareTo(Pseudo.As<T, double>(ref arg1));
            else if (typeof(T) == typeof(decimal))
                return Pseudo.As<T, decimal>(ref arg0).CompareTo(Pseudo.As<T, decimal>(ref arg1));
            return UnsafeUtility.MemCmp(Pseudo.AsPointer(ref arg0), Pseudo.AsPointer(ref arg1), sizeof(T));
        }
    }
}