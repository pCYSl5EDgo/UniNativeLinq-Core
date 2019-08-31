using System.Collections;
using System.Collections.Generic;

namespace UniNativeLinq
{
    public unsafe struct
        BufferNativeEnumerable<T>
        : IEnumerable<NativeEnumerable<T>>
        where T : unmanaged
    {
        private NativeEnumerable<T> enumerable;
        private long count;

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
            private NativeEnumerable<T> enumerable;
            private long count;
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
                    return NativeEnumerable<T>.Create(enumerable.Ptr + index, rest > count ? count : rest);
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
        public NativeEnumerable<T> Flatten() => enumerable;
    }
}
