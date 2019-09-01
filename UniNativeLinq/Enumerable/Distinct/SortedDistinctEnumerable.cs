using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    internal unsafe struct
        SortedDistinctEnumerable<TEnumerable, TEnumerator, T, TComparer>
        : IRefEnumerable<SortedDistinctEnumerable<TEnumerable, TEnumerator, T, TComparer>.Enumerator, T>
        where T : unmanaged
        where TComparer : struct, IRefFunc<T, T, int>
        where TEnumerator : struct, IRefEnumerator<T>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, T>
    {
        private TEnumerable enumerable;
        private TComparer orderComparer;
        private Allocator alloc;

        public SortedDistinctEnumerable(in TEnumerable enumerable, in TComparer orderComparer, Allocator allocator)
        {
            this.enumerable = enumerable;
            this.orderComparer = orderComparer;
            alloc = allocator;
        }

        public Enumerator GetEnumerator() => new Enumerator(ref enumerable, orderComparer, alloc);

        public struct Enumerator : IRefEnumerator<T>
        {
            private TEnumerator enumerator;
            private T* ptr;
            private long count;
            private long capacity;
            private long lastInsertIndex;
            private TComparer comparer;
            private Allocator allocator;

            internal Enumerator([PseudoIsReadOnly]ref TEnumerable enumerable, in TComparer comparer, Allocator allocator)
            {
                enumerator = enumerable.GetEnumerator();
                this.allocator = allocator;
                count = 0L;
                capacity = enumerable.CanFastCount() ? enumerable.LongCount() : 16L;
                ptr = UnsafeUtilityEx.Malloc<T>(capacity, allocator);
                this.comparer = comparer;
                lastInsertIndex = -1L;
            }


            private bool TryInsert(ref T value)
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

            private void Insert(ref T value, long index)
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
                var tmp = UnsafeUtilityEx.Malloc<T>(capacity << 1, allocator);
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
            public ref T Current => ref ptr[lastInsertIndex];
            T IEnumerator<T>.Current => Current;
            object IEnumerator.Current => Current;

            public void Dispose()
            {
                enumerator.Dispose();
                if (ptr == null || !UnsafeUtility.IsValidAllocator(allocator)) return;
                UnsafeUtility.Free(ptr, allocator);
            }

            public NativeEnumerable<T> AsEnumerable()
            => NativeEnumerable<T>.Create(ptr, count);

            public bool MoveNext()
            {
                do
                {
                    if (!enumerator.MoveNext()) return false;
                } while (!TryInsert(ref enumerator.Current));
                return true;
            }

            public ref T TryGetNext(out bool success)
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

            public bool TryMoveNext(out T value)
            {
                while (enumerator.TryMoveNext(out value))
                    if (TryInsert(ref value))
                        return true;
                return false;
            }
        }

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFastCount() => false;

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
        public long LongCount()
        {
            var enumerator = GetEnumerator();
            var count = 0L;
            while (enumerator.MoveNext())
                ++count;
            enumerator.Dispose();
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(T* dest)
        {
            var enumerator = GetEnumerator();
            while (enumerator.MoveNext())
                *dest++ = enumerator.Current;
            enumerator.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray()
        {
            var count = LongCount();
            if (count == 0) return Array.Empty<T>();
            var answer = new T[count];
            CopyTo(Pseudo.AsPointer(ref answer[0]));
            return answer;
        }

        public bool CanIndexAccess() => false;

        public ref T this[long index] => throw new NotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeEnumerable<T> ToNativeEnumerable(Allocator allocator)
        {
            var count = LongCount();
            var ptr = UnsafeUtilityEx.Malloc<T>(count, allocator);
            CopyTo(ptr);
            return NativeEnumerable<T>.Create(ptr, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeEnumerable<T> ToNativeEnumerable()
            => GetEnumerator().AsEnumerable();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeArray<T> ToNativeArray(Allocator allocator)
        {
            var count = Count();
            if (count == 0) return default;
            var answer = new NativeArray<T>(count, allocator, NativeArrayOptions.UninitializedMemory);
            CopyTo(answer.GetPointer());
            return answer;
        }
        #endregion
    }
}
