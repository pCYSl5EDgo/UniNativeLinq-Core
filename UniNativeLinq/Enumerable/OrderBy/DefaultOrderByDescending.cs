using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    public readonly unsafe struct DefaultOrderByDescending<T>
        : IRefFunc<T, T, int>
        where T : unmanaged
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Calc(ref T arg0, ref T arg1) => UnsafeUtility.MemCmp(Pseudo.AsPointer(ref arg1), Pseudo.AsPointer(ref arg0), sizeof(T));
    }

    public readonly struct DefaultOrderByDescendingByte
        : IRefFunc<byte, byte, int>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Calc(ref byte arg0, ref byte arg1) => arg1 - arg0;
    }

    public readonly struct DefaultOrderByDescendingSByte
        : IRefFunc<sbyte, sbyte, int>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Calc(ref sbyte arg0, ref sbyte arg1) => arg1 - arg0;
    }

    public readonly struct DefaultOrderByDescendingInt16
        : IRefFunc<short, short, int>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Calc(ref short arg0, ref short arg1) => arg1 - arg0;
    }

    public readonly struct DefaultOrderByDescendingUInt16
        : IRefFunc<ushort, ushort, int>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Calc(ref ushort arg0, ref ushort arg1) => arg1 - arg0;
    }

    public readonly struct DefaultOrderByDescendingInt32
        : IRefFunc<int, int, int>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Calc(ref int arg0, ref int arg1) => arg1 - arg0;
    }

    public readonly struct DefaultOrderByDescendingUInt32
        : IRefFunc<uint, uint, int>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Calc(ref uint arg0, ref uint arg1) => arg1.CompareTo(arg0);
    }

    public readonly struct DefaultOrderByDescendingInt64
        : IRefFunc<long, long, int>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Calc(ref long arg0, ref long arg1) => arg1.CompareTo(arg0);
    }

    public readonly struct DefaultOrderByDescendingUInt64
        : IRefFunc<ulong, ulong, int>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Calc(ref ulong arg0, ref ulong arg1) => arg1.CompareTo(arg0);
    }
}