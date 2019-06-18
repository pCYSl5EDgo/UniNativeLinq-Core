using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    public struct DefaultEqualityComparer<T> : IRefFunc<T, T, bool>
        where T : unmanaged, IEquatable<T>
    {
        public bool Calc(ref T arg0, ref T arg1) => arg0.Equals(arg1);
    }
}