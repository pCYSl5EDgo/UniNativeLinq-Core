using System;

namespace UniNativeLinq
{
    [AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = true)]
    sealed class LocalRefReturnAttribute : Attribute
    {
    }
}
