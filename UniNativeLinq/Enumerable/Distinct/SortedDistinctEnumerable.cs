using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    internal unsafe struct
        SortedDistinctEnumerable<TEnumerable, TEnumerator, TSource, TComparer>
        : IRefEnumerable<SortedDistinctEnumerable<TEnumerable, TEnumerator, TSource, TComparer>.Enumerator, TSource>
        where TSource : unmanaged
        where TComparer : struct, IRefFunc<TSource, TSource, int>
        where TEnumerator : struct, IRefEnumerator<TSource>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
    {
        private TEnumerable enumerable;
        private readonly TComparer orderComparer;
        private readonly Allocator alloc;

        public SortedDistinctEnumerable(in TEnumerable enumerable, in TComparer orderComparer, Allocator allocator)
        {
            this.enumerable = enumerable;
            this.orderComparer = orderComparer;
            alloc = allocator;
        }

        public readonly Enumerator GetEnumerator() => new Enumerator(in enumerable, orderComparer, alloc);

        public struct Enumerator : IRefEnumerator<TSource>
        {
            private TEnumerator enumerator;
            private TSource* ptr;
            private long count;
            private long capacity;
            private long lastInsertIndex;
            private TComparer comparer;
            private readonly Allocator allocator;

            internal Enumerator(in TEnumerable enumerable, in TComparer comparer, Allocator allocator)
            {
                enumerator = enumerable.GetEnumerator();
                this.allocator = allocator;
                count = 0L;
                capacity = enumerable.CanFastCount() ? enumerable.LongCount() : 16L;
                ptr = UnsafeUtilityEx.Malloc<TSource>(capacity, allocator);
                this.comparer = comparer;
                lastInsertIndex = -1L;
            }


            private bool TryInsert(ref TSource value)
            {
                var minInclude = 0L;
                var maxInclude = count - 1;
                while (minInclude <= maxInclude)
                {
                    var index = (minInclude + maxInclude) >> 1;
                    var code = comparer.Calc(ref value, ref ptr[index]);
                    if (code == 0) return false;
                    if (code < 0)
                        maxInclude = index - 1;
                    else
                        minInclude = index + 1;
                }
                Insert(ref value, minInclude);
                return true;
            }

            private void Insert(ref TSource value, long index)
            {
                if (capacity == count)
                    ReAlloc(index);
                else if (index != count)
                    UnsafeUtilityEx.MemCpy(ptr + index + 1, ptr + index, count - index);
                lastInsertIndex = index;
                ptr[index] = value;
                count++;
            }

            private void ReAlloc(long index)
            {
                var tmp = UnsafeUtilityEx.Malloc<TSource>(capacity << 1, allocator);
                if (index == 0)
                    UnsafeUtilityEx.MemCpy(tmp + 1, ptr, count);
                else if (index == count)
                    UnsafeUtilityEx.MemCpy(tmp, ptr, count);
                else
                {
                    UnsafeUtilityEx.MemCpy(tmp, ptr, index);
                    UnsafeUtilityEx.MemCpy(tmp + index + 1, ptr + index, count - index);
                }
                UnsafeUtility.Free(ptr, allocator);
                ptr = tmp;
                capacity <<= 1;
            }

            public void Reset() => throw new InvalidOperationException();
            public ref TSource Current => ref ptr[lastInsertIndex];
            TSource IEnumerator<TSource>.Current => Current;
            object IEnumerator.Current => Current;

            public void Dispose()
            {
                enumerator.Dispose();
                if (ptr != null && UnsafeUtility.IsValidAllocator(allocator))
                    UnsafeUtility.Free(ptr, allocator);
                this = default;
            }

            public NativeEnumerable<TSource> AsEnumerable()
            => new NativeEnumerable<TSource>(ptr, count);

            public bool MoveNext()
            {
                do
                {
                    if (!enumerator.MoveNext()) return false;
                } while (!TryInsert(ref enumerator.Current));
                return true;
            }

            public ref TSource TryGetNext(out bool success)
            {
                while (true)
                {
                    ref var value = ref enumerator.TryGetNext(out success);
                    if (!success) return ref value;
                    success = TryInsert(ref value);
                    if (success)
                        return ref value;
                }
            }

            public bool TryMoveNext(out TSource value)
            {
                while (enumerator.TryMoveNext(out value))
                    if (TryInsert(ref value))
                        return true;
                return false;
            }
        }

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool CanFastCount() => false;

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
            var enumerator = GetEnumerator();
            var count = 0L;
            while (enumerator.MoveNext())
                ++count;
            enumerator.Dispose();
            return count;
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
        public readonly NativeEnumerable<TSource> ToNativeEnumerable()
            => GetEnumerator().AsEnumerable();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly NativeArray<TSource> ToNativeArray(Allocator allocator)
        {
            var count = Count();
            if (count == 0) return default;
            var answer = new NativeArray<TSource>(count, allocator, NativeArrayOptions.UninitializedMemory);
            CopyTo(answer.GetPointer());
            return answer;
        }
        #endregion
    }
}
