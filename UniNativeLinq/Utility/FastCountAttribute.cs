using System;

namespace UniNativeLinq
{
    [AttributeUsage(AttributeTargets.Struct)]
    internal sealed class FastCountAttribute : Attribute
    {
        
    }

    [AttributeUsage(AttributeTargets.Struct)]
    internal sealed class SlowCountAttribute : Attribute
    {

    }
}
