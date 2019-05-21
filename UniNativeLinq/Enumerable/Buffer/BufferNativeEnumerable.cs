using System.Collections;
using System.Collections.Generic;

namespace UniNativeLinq
{
    public readonly unsafe struct
        BufferNativeEnumerable<T>
        : IEnumerable<NativeEnumerable<T>>
        where T : unmanaged
    {
        private readonly NativeEnumerable<T> enumerable;
        private readonly long count;

        public BufferNativeEnumerable(in NativeEnumerable<T> enumerable, long count)
        {
            this.enumerable = enumerable;
            this.count = count;
        }

        public Enumerator GetEnumerator() => new Enumerator(this);
        IEnumerator<NativeEnumerable<T>> IEnumerable<NativeEnumerable<T>>.GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<NativeEnumerable<T>>
        {
            private readonly NativeEnumerable<T> enumerable;
            private readonly long count;
            private long index;

            internal Enumerator(in BufferNativeEnumerable<T> @this)
            {
                enumerable = @this.enumerable;
                count = @this.count;
                index = -count;
            }

            public NativeEnumerable<T> Current
            {
                get
                {
                    var rest = enumerable.Length - index;
                    return new NativeEnumerable<T>(enumerable.Ptr + index, rest > count ? count : rest);
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
        public readonly NativeEnumerable<T> Flatten() => enumerable;
    }
}
