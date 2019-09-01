using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UniNativeLinq
{
    public struct
        BufferArrayEnumerable<T>
        : IEnumerable<ArrayEnumerable<T>>
        where T : unmanaged
    {
        private ArrayEnumerable<T> enumerable;
        private long count;

        public BufferArrayEnumerable(in ArrayEnumerable<T> enumerable, long count)
        {
            this.enumerable = enumerable;
            this.count = count;
        }
        public Enumerator GetEnumerator() => new Enumerator(this);

        public struct Enumerator : IEnumerator<ArrayEnumerable<T>>
        {
            private ArrayEnumerable<T> enumerable;
            private long count;
            private long index;

            internal Enumerator(in BufferArrayEnumerable<T> @this)
            {
                enumerable = @this.enumerable;
                count = @this.count;
                index = -count;
            }

            public ArrayEnumerable<T> Current => enumerable.Slice(index, count);
            object IEnumerator.Current => throw new NotImplementedException();

            public void Dispose() { }

            public bool MoveNext()
            {
                index += count;
                return index < enumerable.Length;
            }

            public void Reset()
            {
                index = -count;
            }
        }

        public ArrayEnumerable<T> Flatten() => enumerable;

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<ArrayEnumerable<T>> IEnumerable<ArrayEnumerable<T>>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFastCount() => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Any()
        {
            var enumerator = GetEnumerator();
            if (enumerator.MoveNext())
            {
                enumerator.Dispose();
                return true;
            }
            enumerator.Dispose();
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count()
            => (int)LongCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long LongCount() => (enumerable.LongCount() - 1) / count + 1L;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ArrayEnumerable<T>[] ToArray()
        {
            var count = LongCount();
            if (count == 0) return Array.Empty<ArrayEnumerable<T>>();
            var answer = new ArrayEnumerable<T>[count];
            var enumerator = GetEnumerator();
            var index = 0L;
            while (enumerator.MoveNext())
                answer[index++] = enumerator.Current;
            enumerator.Dispose();
            return answer;
        }
        #endregion
    }
}
