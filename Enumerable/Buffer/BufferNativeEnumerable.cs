using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using System.Runtime.CompilerServices;

namespace pcysl5edgo.Collections.LINQ
{
    public unsafe readonly struct
        BufferNativeEnumerable<TSource>
        : IEnumerable<NativeEnumerable<TSource>>
        where TSource : unmanaged
    {
        private readonly NativeEnumerable<TSource> enumerable;
        private readonly long count;

        public BufferNativeEnumerable(in NativeEnumerable<TSource> enumerable, long count)
        {
            this.enumerable = enumerable;
            this.count = count;
        }

        public Enumerator GetEnumerator() => new Enumerator(this);
        IEnumerator<NativeEnumerable<TSource>> IEnumerable<NativeEnumerable<TSource>>.GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<NativeEnumerable<TSource>>
        {
            private readonly NativeEnumerable<TSource> enumerable;
            private readonly long count;
            private long index;

            internal Enumerator(in BufferNativeEnumerable<TSource> @this)
            {
                enumerable = @this.enumerable;
                count = @this.count;
                index = -count;
            }

            public NativeEnumerable<TSource> Current
            {
                get
                {
                    var rest = enumerable.Length - index;
                    return new NativeEnumerable<TSource>(enumerable.Ptr + index, rest > count ? count : rest);
                }
            }
            object IEnumerator.Current => Current;

            public void Dispose() => this = default;

            public bool MoveNext()
            {
                index += count;
                return index < enumerable.Length;
            }

            public void Reset() => index = -count;
        }
        public readonly NativeEnumerable<TSource> Flatten() => enumerable;
    }
}
