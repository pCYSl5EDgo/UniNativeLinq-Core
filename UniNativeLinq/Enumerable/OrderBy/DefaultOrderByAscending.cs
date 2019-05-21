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
                return Unsafe.As<T, byte>(ref arg0).CompareTo(Unsafe.As<T, byte>(ref arg1));
            else if (typeof(T) == typeof(sbyte))
                return Unsafe.As<T, sbyte>(ref arg0).CompareTo(Unsafe.As<T, sbyte>(ref arg1));
            else if (typeof(T) == typeof(short))
                return Unsafe.As<T, short>(ref arg0).CompareTo(Unsafe.As<T, short>(ref arg1));
            else if (typeof(T) == typeof(ushort))
                return Unsafe.As<T, ushort>(ref arg0).CompareTo(Unsafe.As<T, ushort>(ref arg1));
            else if (typeof(T) == typeof(int))
                return Unsafe.As<T, int>(ref arg0).CompareTo(Unsafe.As<T, int>(ref arg1));
            else if (typeof(T) == typeof(uint))
                return Unsafe.As<T, uint>(ref arg0).CompareTo(Unsafe.As<T, uint>(ref arg1));
            else if (typeof(T) == typeof(long))
                return Unsafe.As<T, long>(ref arg0).CompareTo(Unsafe.As<T, long>(ref arg1));
            else if (typeof(T) == typeof(ulong))
                return Unsafe.As<T, ulong>(ref arg0).CompareTo(Unsafe.As<T, ulong>(ref arg1));
            else if (typeof(T) == typeof(float))
                return Unsafe.As<T, float>(ref arg0).CompareTo(Unsafe.As<T, float>(ref arg1));
            else if (typeof(T) == typeof(double))
                return Unsafe.As<T, double>(ref arg0).CompareTo(Unsafe.As<T, double>(ref arg1));
            else if (typeof(T) == typeof(decimal))
                return Unsafe.As<T, decimal>(ref arg0).CompareTo(Unsafe.As<T, decimal>(ref arg1));
            return UnsafeUtility.MemCmp(Unsafe.AsPointer(ref arg0), Unsafe.AsPointer(ref arg1), sizeof(T));
        }
    }
}