using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UniNativeLinq
{
    internal static unsafe class Psuedo
    {
        internal static ref T AsRefNull<T>() => throw new NotImplementedException();
        internal static T* AsPointer<T>(ref T value) where T : unmanaged => throw new NotImplementedException();
        internal static ref TTo As<TFrom, TTo>(ref TFrom value) => throw new NotImplementedException();
    }
}
