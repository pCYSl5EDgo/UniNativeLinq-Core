using System;

namespace UniNativeLinq
{
    internal static unsafe class Pseudo
    {
        internal static ref T AsRefNull<T>() => throw new NotImplementedException();
        internal static T* AsPointer<T>(ref T value) where T : unmanaged => throw new NotImplementedException();
        internal static ref TTo As<TFrom, TTo>(ref TFrom value) => throw new NotImplementedException();
    }
}
