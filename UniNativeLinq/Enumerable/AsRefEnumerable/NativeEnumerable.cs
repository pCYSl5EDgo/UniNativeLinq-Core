using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    [FastCount]
    public unsafe struct
        NativeEnumerable<T>
        : IRefEnumerable<NativeEnumerable<T>.Enumerator, T>
        where T : unmanaged
    {
        public T* Ptr;
        public long Length;

        public ref T this[long index] => ref Ptr[index];

        public NativeEnumerable(NativeArray<T> array)
        {
            if (array.IsCreated)
            {
                Ptr = array.GetPointer();
                Length = array.Length;
            }
            else
            {
                Ptr = null;
                Length = 0;
            }
        }

        public static NativeEnumerable<T> Create(NativeArray<T> array, long offset, long length)
        {
            NativeEnumerable<T> answer = default;
            if (array.IsCreated && length > 0)
            {
                if (offset >= 0)
                {
                    answer.Ptr = array.GetPointer() + offset;
                    answer.Length = length;
                }
                else
                {
                    answer.Ptr = array.GetPointer();
                    answer.Length = length + offset;
                }
            }
            else
            {
                answer.Ptr = null;
                answer.Length = 0;
            }
            return answer;
        }

        public static NativeEnumerable<T> Create(T* ptr, long length)
        {
            if (length <= 0 || ptr == null)
            {
                return default;
            }
            NativeEnumerable<T> answer = default;
            answer.Ptr = ptr;
            answer.Length = length;
            return answer;
        }

        public void Dispose(Allocator allocator)
        {
            if (Ptr == null || !UnsafeUtility.IsValidAllocator(allocator)) return;
            UnsafeUtility.Free(Ptr, allocator);
        }

        public Enumerator GetEnumerator() => new Enumerator(this);

        public struct Enumerator : IRefEnumerator<T>
        {
            internal T* Ptr;
            internal long Length;
            private long index;

            public ref T Current => ref Ptr[index];
            T IEnumerator<T>.Current => Current;
            object IEnumerator.Current => Current;

            internal Enumerator(in NativeEnumerable<T> parent)
            {
                index = -1;
                Ptr = parent.Ptr;
                Length = parent.Length;
            }

            public void Dispose() { }

            public bool MoveNext() => ++index < Length;

            public void Reset() => index = -1;

            public ref T TryGetNext(out bool success)
            {
                success = ++index < Length;
                if (success)
                    return ref Ptr[index];
                index = Length;
                return ref Pseudo.AsRefNull<T>();
            }

            public bool TryMoveNext(out T value)
            {
                if (++index < Length)
                {
                    value = Ptr[index];
                    return true;
                }
                else
                {
                    value = default;
                    index = Length;
                    return false;
                }
            }
        }

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFastCount() => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Any() => Length != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count() => (int)Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long LongCount() => Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(T* dest) => UnsafeUtilityEx.MemCpy(dest, Ptr, Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray()
        {
            var count = LongCount();
            if (count == 0) return Array.Empty<T>();
            var answer = new T[count];
            CopyTo(Pseudo.AsPointer(ref answer[0]));
            return answer;
        }

        public bool CanIndexAccess() => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeEnumerable<T> ToNativeEnumerable(Allocator allocator)
        {
            var count = LongCount();
            var ptr = UnsafeUtilityEx.Malloc<T>(count, allocator);
            CopyTo(ptr);
            return Create(ptr, count);
        }

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

        public long FindIndexBinarySearch<TComparer>(ref T searchItem, in TComparer comparer)
            where TComparer : struct, IRefFunc<T, T, int>
        {
            var minInclusive = 0L;
            var maxInclusive = Length - 1;
            while (minInclusive <= maxInclusive)
            {
                var index = (minInclusive + maxInclusive) >> 1;
                var code = comparer.Calc(ref searchItem, ref Ptr[index]);
                if (code == 0)
                    return index;
                if (code < 0)
                    maxInclusive = index - 1;
                else
                    minInclusive = index + 1;
            }
            return -1L;
        }

        public NativeEnumerable<T>
            Skip(long count)
            => Create(Ptr + count, Length - count);

        public NativeEnumerable<T>
            SkipLast(long count)
            => Create(Ptr, Length - count);

        public NativeEnumerable<T>
            Take(long count)
        {
            if (count >= Length) return this;
            if (count <= 0) return default;
            return Create(Ptr, count);
        }

        public NativeEnumerable<T>
            TakeLast(long count)
        {
            if (count >= Length) return this;
            if (Ptr == null || count <= 0) return default;
            return Create(Ptr + Length - count, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetLast(out T value)
        {
            var answer = Length != 0;
            value = Ptr[Length - 1];
            return answer;
        }

        public static implicit operator NativeArray<T>(NativeEnumerable<T> enumerable)
            => NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(enumerable.Ptr, (int)enumerable.Length, Allocator.None);

        public static implicit operator NativeEnumerable<T>(NativeArray<T> array)
            => new NativeEnumerable<T>(array);
    }
}