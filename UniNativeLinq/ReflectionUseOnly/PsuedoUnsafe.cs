using System;
using System.Collections.Generic;
using System.Text;

namespace UniNativeLinq
{
    internal static class PsuedoUnsafe
    {
        internal static ref T AsRefNull<T>() => throw new NotImplementedException();
    }
}
