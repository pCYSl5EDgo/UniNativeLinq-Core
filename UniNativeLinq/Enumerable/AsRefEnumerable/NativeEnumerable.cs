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
        public readonly T* Ptr;
        public readonly long Length;

        public readonly ref T this[long index] => ref Ptr[index];

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

        public NativeEnumerable(NativeArray<T> array, long offset, long length)
        {
            if (array.IsCreated && length > 0)
            {
                if (offset >= 0)
                {
                    Ptr = array.GetPointer() + offset;
                    Length = length;
                }
                else
                {
                    Ptr = array.GetPointer();
                    Length = length + offset;
                }
            }
            else
            {
                Ptr = null;
                Length = 0;
            }
        }

        public NativeEnumerable(T* ptr, long length)
        {
            if (length <= 0 || ptr == null)
            {
                this = default;
                return;
            }
            Ptr = ptr;
            Length = length;
        }

        public void Dispose(Allocator allocator)
        {
            if (Ptr != null)
                UnsafeUtility.Free(Ptr, allocator);
            this = default;
        }

        public readonly Enumerator GetEnumerator() => new Enumerator(this);
        public readonly ReverseEnumerator GetReverseEnumerator() => new ReverseEnumerator(this);

        public struct Enumerator : IRefEnumerator<T>
        {
            internal readonly T* Ptr;
            private readonly long length;
            private long index;

            public ref T Current => ref Ptr[index];
            T IEnumerator<T>.Current => Current;
            object IEnumerator.Current => Current;

            internal Enumerator(in NativeEnumerable<T> parent)
            {
                index = -1;
                Ptr = parent.Ptr;
                length = parent.Length;
            }

            public void Dispose() => this = default;

            public bool MoveNext() => ++index < length;

            public void Reset() => index = -1;

            public ref T TryGetNext(out bool success)
            {
                success = ++index < length;
                if (success)
                    return ref Ptr[index];
                index = length;
                return ref Unsafe.AsRef<T>(null);
            }

            public bool TryMoveNext(out T value)
            {
                if (++index < length)
                {
                    value = Ptr[index];
                    return true;
                }
                else
                {
                    value = default;
                    index = length;
                    return false;
                }
            }
        }

        public struct ReverseEnumerator : IRefEnumerator<T>
        {
            internal readonly T* Ptr;
            private readonly long length;
            private long index;

            public ref T Current => ref Ptr[index];
            T IEnumerator<T>.Current => Current;
            object IEnumerator.Current => Current;

            internal ReverseEnumerator(in NativeEnumerable<T> parent)
            {
                index = parent.Length;
                Ptr = parent.Ptr;
                length = parent.Length;
            }

            public void Dispose() => this = default;

            public bool MoveNext() => --index >= 0;

            public void Reset() => index = length;

            public ref T TryGetNext(out bool success)
            {
                success = --index >= 0;
                if (success)
                    return ref Ptr[index];
                index = 0;
                return ref Unsafe.AsRef<T>(null);
            }

            public bool TryMoveNext(out T value)
            {
                if (--index >= 0)
                {
                    value = Ptr[index];
                    return true;
                }
                else
                {
                    value = default;
                    index = 0;
                    return false;
                }
            }
        }

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool CanFastCount() => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Any() => Length != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int Count() => (int)Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly long LongCount() => Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void CopyTo(T* dest) => UnsafeUtilityEx.MemCpy(dest, Ptr, Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T[] ToArray()
        {
            var count = LongCount();
            if (count == 0) return Array.Empty<T>();
            var answer = new T[count];
            CopyTo((T*)Unsafe.AsPointer(ref answer[0]));
            return answer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly NativeEnumerable<T> ToNativeEnumerable(Allocator allocator)
        {
            var count = LongCount();
            var ptr = UnsafeUtilityEx.Malloc<T>(count, allocator);
            CopyTo(ptr);
            return new NativeEnumerable<T>(ptr, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly NativeArray<T> ToNativeArray(Allocator allocator)
        {
            var count = Count();
            if (count == 0) return default;
            var answer = new NativeArray<T>(count, allocator, NativeArrayOptions.UninitializedMemory);
            CopyTo(answer.GetPointer());
            return answer;
        }
        #endregion

        public readonly long FindIndexBinarySearch<TComparer>(ref T searchItem, in TComparer comparer)
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

        public readonly NativeEnumerable<T>
            Skip(long count)
            => new NativeEnumerable<T>(Ptr + count, Length - count);

        public readonly NativeEnumerable<T>
            SkipLast(long count)
            => new NativeEnumerable<T>(Ptr, Length - count);

        public readonly NativeEnumerable<T>
            Take(long count)
        {
            if (count >= Length) return this;
            if (count <= 0) return default;
            return new NativeEnumerable<T>(Ptr, count);
        }

        public readonly NativeEnumerable<T>
            TakeLast(long count)
        {
            if (count >= Length) return this;
            if (Ptr == null || count <= 0) return default;
            return new NativeEnumerable<T>(Ptr + Length - count, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryGetLast(out T value)
        {
            var answer = Length != 0;
            value = Ptr[Length - 1];
            return answer;
        }
    }
}