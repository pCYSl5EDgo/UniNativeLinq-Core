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
                return Psuedo.As<T, byte>(ref arg0).CompareTo(Psuedo.As<T, byte>(ref arg1));
            else if (typeof(T) == typeof(sbyte))
                return Psuedo.As<T, sbyte>(ref arg0).CompareTo(Psuedo.As<T, sbyte>(ref arg1));
            else if (typeof(T) == typeof(short))
                return Psuedo.As<T, short>(ref arg0).CompareTo(Psuedo.As<T, short>(ref arg1));
            else if (typeof(T) == typeof(ushort))
                return Psuedo.As<T, ushort>(ref arg0).CompareTo(Psuedo.As<T, ushort>(ref arg1));
            else if (typeof(T) == typeof(int))
                return Psuedo.As<T, int>(ref arg0).CompareTo(Psuedo.As<T, int>(ref arg1));
            else if (typeof(T) == typeof(uint))
                return Psuedo.As<T, uint>(ref arg0).CompareTo(Psuedo.As<T, uint>(ref arg1));
            else if (typeof(T) == typeof(long))
                return Psuedo.As<T, long>(ref arg0).CompareTo(Psuedo.As<T, long>(ref arg1));
            else if (typeof(T) == typeof(ulong))
                return Psuedo.As<T, ulong>(ref arg0).CompareTo(Psuedo.As<T, ulong>(ref arg1));
            else if (typeof(T) == typeof(float))
                return Psuedo.As<T, float>(ref arg0).CompareTo(Psuedo.As<T, float>(ref arg1));
            else if (typeof(T) == typeof(double))
                return Psuedo.As<T, double>(ref arg0).CompareTo(Psuedo.As<T, double>(ref arg1));
            else if (typeof(T) == typeof(decimal))
                return Psuedo.As<T, decimal>(ref arg0).CompareTo(Psuedo.As<T, decimal>(ref arg1));
            return UnsafeUtility.MemCmp(Psuedo.AsPointer(ref arg0), Psuedo.AsPointer(ref arg1), sizeof(T));
        }
    }
}