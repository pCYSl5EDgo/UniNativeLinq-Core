using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UniNativeLinq
{
    public readonly struct
        BufferArrayEnumerable<T>
        : IEnumerable<ArrayEnumerable<T>>
        where T : unmanaged
    {
        private readonly ArrayEnumerable<T> enumerable;
        private readonly long count;

        public BufferArrayEnumerable(in ArrayEnumerable<T> enumerable, long count)
        {
            this.enumerable = enumerable;
            this.count = count;
        }
        public Enumerator GetEnumerator() => new Enumerator(this);

        public struct Enumerator : IEnumerator<ArrayEnumerable<T>>
        {
            private readonly ArrayEnumerable<T> enumerable;
            private readonly long count;
            private long index;

            internal Enumerator(in BufferArrayEnumerable<T> @this)
            {
                enumerable = @this.enumerable;
                count = @this.count;
                index = -count;
            }

            public readonly ArrayEnumerable<T> Current => enumerable.Slice(index, count);
            readonly object IEnumerator.Current => throw new NotImplementedException();

            public void Dispose() => this = default;

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

        public readonly ArrayEnumerable<T> Flatten() => enumerable;

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator<ArrayEnumerable<T>> IEnumerable<ArrayEnumerable<T>>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool CanFastCount() => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Any()
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
        public readonly int Count()
            => (int)LongCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly long LongCount() => (enumerable.LongCount() - 1) / count + 1L;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ArrayEnumerable<T>[] ToArray()
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
