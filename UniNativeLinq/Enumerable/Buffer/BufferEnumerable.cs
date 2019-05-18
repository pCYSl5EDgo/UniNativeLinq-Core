using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UniNativeLinq
{
    public unsafe readonly partial struct
        BufferEnumerable<TEnumerable, TEnumerator, TSource>
         : IEnumerable<SkipEnumerable<TEnumerable, TEnumerator, TSource>>
        where TSource : unmanaged
        where TEnumerator : unmanaged, IRefEnumerator<TSource>
        where TEnumerable : unmanaged, IRefEnumerable<TEnumerator, TSource>
    {
        private readonly TEnumerable enumerable;
        private readonly long count;

        public BufferEnumerable(in TEnumerable enumerable, long count)
        {
            this.enumerable = enumerable;
            this.count = count;
        }

        public Enumerator GetEnumerator() => new Enumerator(enumerable, count);

        public struct Enumerator : IEnumerator<SkipEnumerable<TEnumerable, TEnumerator, TSource>>
        {
            private readonly TEnumerable enumerable;
            private readonly long count;
            private long index;
            private long length;

            internal Enumerator(in TEnumerable enumerable, long count)
            {
                this.enumerable = enumerable;
                this.count = count;
                long lc = enumerable.LongCount();
                length = (lc - 1) / count + 1;
                index = -1;
            }

            public SkipEnumerable<TEnumerable, TEnumerator, TSource> Current => new SkipEnumerable<TEnumerable, TEnumerator, TSource>(enumerable, count * index);
            object IEnumerator.Current => Current;

            public void Dispose() => this = default;

            public bool MoveNext() => ++index < length;

            public void Reset()
            {
                long lc = enumerable.LongCount();
                length = (lc - 1) / count + 1;
                index = -1;
            }
        }
        public bool CanFastCount() => true;
        public int Count() => (int)LongCount();
        public long LongCount() => (enumerable.LongCount() - 1) / count + 1;
        public TEnumerable Flatten() => enumerable;

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator<SkipEnumerable<TEnumerable, TEnumerator, TSource>> IEnumerable<SkipEnumerable<TEnumerable, TEnumerator, TSource>>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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
        public void CopyTo(SkipEnumerable<TEnumerable, TEnumerator, TSource>* dest)
        {
            var enumerator = GetEnumerator();
            while (enumerator.MoveNext())
                *dest++ = enumerator.Current;
            enumerator.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SkipEnumerable<TEnumerable, TEnumerator, TSource>[] ToArray()
        {
            var count = LongCount();
            if (count == 0) return Array.Empty<SkipEnumerable<TEnumerable, TEnumerator, TSource>>();
            var answer = new SkipEnumerable<TEnumerable, TEnumerator, TSource>[count];
            CopyTo((SkipEnumerable<TEnumerable, TEnumerator, TSource>*)Unsafe.AsPointer(ref answer[0]));
            return answer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeEnumerable<SkipEnumerable<TEnumerable, TEnumerator, TSource>> ToNativeEnumerable(Allocator allocator)
        {
            var count = LongCount();
            var ptr = UnsafeUtilityEx.Malloc<SkipEnumerable<TEnumerable, TEnumerator, TSource>>(count, allocator);
            CopyTo(ptr);
            return new NativeEnumerable<SkipEnumerable<TEnumerable, TEnumerator, TSource>>(ptr, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeArray<SkipEnumerable<TEnumerable, TEnumerator, TSource>> ToNativeArray(Allocator allocator)
        {
            var count = Count();
            if (count == 0) return default;
            var answer = new NativeArray<SkipEnumerable<TEnumerable, TEnumerator, TSource>>(count, allocator, NativeArrayOptions.UninitializedMemory);
            CopyTo(UnsafeUtilityEx.GetPointer(answer));
            return answer;
        }
        #endregion
    }
}
