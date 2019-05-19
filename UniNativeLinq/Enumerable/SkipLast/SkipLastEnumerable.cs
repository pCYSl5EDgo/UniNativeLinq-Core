using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    public unsafe partial struct
        SkipLastEnumerable<TEnumerable, TEnumerator, TSource>
        : IRefEnumerable<SkipLastEnumerable<TEnumerable, TEnumerator, TSource>.Enumerator, TSource>
        where TSource : unmanaged
        where TEnumerator : struct, IRefEnumerator<TSource>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
    {
        private TEnumerable enumerable;
        private readonly long skipCount;
        private readonly Allocator alloc;

        public SkipLastEnumerable(in TEnumerable enumerable, long skipCount, Allocator allocator)
        {
            this.enumerable = enumerable;
            this.skipCount = skipCount;
            alloc = allocator;
        }

        public struct Enumerator : IRefEnumerator<TSource>
        {
            private TEnumerator enumerator;
            private readonly TSource* buffer;
            private long index;
            private readonly long skipCount;
            private readonly Allocator allocator;

            public readonly ref TSource Current => ref buffer[index];
            readonly TSource IEnumerator<TSource>.Current => Current;
            readonly object IEnumerator.Current => Current;

            public Enumerator(in TEnumerable enumerable, long skipCount, Allocator allocator)
            {
                enumerator = enumerable.GetEnumerator();
                this.skipCount = skipCount;
                this.allocator = allocator;
                buffer = skipCount <= 0 ? null : UnsafeUtilityEx.Malloc<TSource>(skipCount + 1, allocator);
                index = 0;
                for (var i = 0L; i < skipCount; i++)
                {
                    if (!enumerator.MoveNext())
                    {
                        enumerator.Dispose();
                        UnsafeUtility.Free(buffer, allocator);
                        return;
                    }
                    buffer[i + 1] = enumerator.Current;
                }
            }

            public bool MoveNext()
            {
                if (!enumerator.MoveNext())
                    return false;
                buffer[index++] = enumerator.Current;
                if (index > skipCount)
                    index = 0L;
                return true;
            }

            public void Reset() => throw new InvalidOperationException();

            public void Dispose()
            {
                enumerator.Dispose();
                if (buffer != null && UnsafeUtility.IsValidAllocator(allocator))
                    UnsafeUtility.Free(buffer, allocator);
                this = default;
            }

            public ref TSource TryGetNext(out bool success)
            {
                ref var value = ref enumerator.TryGetNext(out success);
                if (!success) return ref value;
                buffer[index++] = value;
                if (index > skipCount)
                    index = 0L;
                return ref buffer[index];
            }

            public bool TryMoveNext(out TSource value)
            {
                if (!enumerator.TryMoveNext(out value))
                    return false;
                buffer[index++] = value;
                if (index > skipCount)
                    index = 0L;
                value = buffer[index];
                return true;
            }
        }

        public readonly Enumerator GetEnumerator() => new Enumerator(enumerable, skipCount, alloc);

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool CanFastCount() => enumerable.CanFastCount();

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
        public readonly long LongCount()
        {
            var count = enumerable.LongCount() - skipCount;
            return count < 0 ? 0 : count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void CopyTo(TSource* dest)
        {
            var enumerator = GetEnumerator();
            while (enumerator.MoveNext())
                *dest++ = enumerator.Current;
            enumerator.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly TSource[] ToArray()
        {
            var count = LongCount();
            if (count == 0) return Array.Empty<TSource>();
            var answer = new TSource[count];
            CopyTo((TSource*)Unsafe.AsPointer(ref answer[0]));
            return answer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly NativeEnumerable<TSource> ToNativeEnumerable(Allocator allocator)
        {
            var count = LongCount();
            var ptr = UnsafeUtilityEx.Malloc<TSource>(count, allocator);
            CopyTo(ptr);
            return new NativeEnumerable<TSource>(ptr, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly NativeArray<TSource> ToNativeArray(Allocator allocator)
        {
            var count = Count();
            if (count == 0) return default;
            var answer = new NativeArray<TSource>(count, allocator, NativeArrayOptions.UninitializedMemory);
            CopyTo(UnsafeUtilityEx.GetPointer(answer));
            return answer;
        }
        #endregion
    }
}
