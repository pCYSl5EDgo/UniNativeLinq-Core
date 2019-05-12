﻿using System;
using System.Collections.Generic;
using System.Text;

namespace pcysl5edgo.Collections.LINQ
{
    public interface IRangeRepeat<TSource>
    {
        void Execute(ref TSource value);
        void Back(ref TSource value);
        void Execute(ref TSource value, long count);
        void Back(ref TSource value, long count);
    }
}
