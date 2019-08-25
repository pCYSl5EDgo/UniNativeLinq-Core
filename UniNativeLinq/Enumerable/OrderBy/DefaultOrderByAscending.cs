using System;
using System.Runtime.CompilerServices;

namespace UniNativeLinq
{
    public readonly struct DefaultOrderByAscending<T>
        : IRefFunc<T, T, int>
        where T : unmanaged, IComparable<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Calc(ref T arg0, ref T arg1) => arg0.CompareTo(arg1);
    }

    public readonly struct DefaultOrderByAscendingByte
        : IRefFunc<byte, byte, int>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Calc(ref byte arg0, ref byte arg1) => arg0 - arg1;
    }

    public readonly struct DefaultOrderByAscendingSByte
        : IRefFunc<sbyte, sbyte, int>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Calc(ref sbyte arg0, ref sbyte arg1) => arg0 - arg1;
    }

    public readonly struct DefaultOrderByAscendingInt16
        : IRefFunc<short, short, int>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Calc(ref short arg0, ref short arg1) => arg0 - arg1;
    }

    public readonly struct DefaultOrderByAscendingUInt16
        : IRefFunc<ushort, ushort, int>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Calc(ref ushort arg0, ref ushort arg1) => arg0 - arg1;
    }

    public readonly struct DefaultOrderByAscendingInt32
        : IRefFunc<int, int, int>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Calc(ref int arg0, ref int arg1) => arg0 - arg1;
    }

    public readonly struct DefaultOrderByAscendingUInt32
        : IRefFunc<uint, uint, int>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Calc(ref uint arg0, ref uint arg1) => arg0.CompareTo(arg1);
    }

    public readonly struct DefaultOrderByAscendingInt64
        : IRefFunc<long, long, int>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Calc(ref long arg0, ref long arg1) => arg0.CompareTo(arg1);
    }

    public readonly struct DefaultOrderByAscendingUInt64
        : IRefFunc<ulong, ulong, int>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Calc(ref ulong arg0, ref ulong arg1) => arg0.CompareTo(arg1);
    }

    public readonly struct DefaultOrderByAscendingSingle
        : IRefFunc<float, float, int>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Calc(ref float arg0, ref float arg1) => arg0.CompareTo(arg1);
    }

    public readonly struct DefaultOrderByAscendingDouble
        : IRefFunc<double, double, int>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Calc(ref double arg0, ref double arg1) => arg0.CompareTo(arg1);
    }
}