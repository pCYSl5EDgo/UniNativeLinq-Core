using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    public unsafe partial struct
        NativeEnumerable<TSource>
        : IRefEnumerable<NativeEnumerable<TSource>.Enumerator, TSource>
        where TSource : unmanaged
    {
        public readonly TSource* Ptr;
        public readonly long Length;

        public ref TSource this[long index] => ref Ptr[index];

        public NativeEnumerable(NativeArray<TSource> array)
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

        public NativeEnumerable(NativeArray<TSource> array, long offset, long length)
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

        public NativeEnumerable(TSource* ptr, long length)
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

        public struct Enumerator : IRefEnumerator<TSource>
        {
            internal readonly TSource* Ptr;
            private readonly long length;
            private long index;

            public ref TSource Current => ref Ptr[index];
            TSource IEnumerator<TSource>.Current => Current;
            object IEnumerator.Current => Current;

            internal Enumerator(in NativeEnumerable<TSource> parent)
            {
                index = -1;
                Ptr = parent.Ptr;
                length = parent.Length;
            }

            public void Dispose() => this = default;

            public bool MoveNext() => ++index < length;

            public void Reset() => index = -1;

            public ref TSource TryGetNext(out bool success)
            {
                success = ++index < length;
                if (success)
                    return ref Ptr[index];
                index = length;
                return ref Unsafe.AsRef<TSource>(null);
            }

            public bool TryMoveNext(out TSource value)
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

        public struct ReverseEnumerator : IRefEnumerator<TSource>
        {
            internal readonly TSource* Ptr;
            private readonly long length;
            private long index;

            public ref TSource Current => ref Ptr[index];
            TSource IEnumerator<TSource>.Current => Current;
            object IEnumerator.Current => Current;

            internal ReverseEnumerator(in NativeEnumerable<TSource> parent)
            {
                index = parent.Length;
                Ptr = parent.Ptr;
                length = parent.Length;
            }

            public void Dispose() => this = default;

            public bool MoveNext() => --index >= 0;

            public void Reset() => index = length;

            public ref TSource TryGetNext(out bool success)
            {
                success = --index >= 0;
                if (success)
                    return ref Ptr[index];
                index = 0;
                return ref Unsafe.AsRef<TSource>(null);
            }

            public bool TryMoveNext(out TSource value)
            {
                if(--index >= 0)
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
        readonly IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();

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
        public readonly void CopyTo(TSource* dest) => UnsafeUtilityEx.MemCpy(dest, Ptr, Length);

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
            CopyTo(answer.GetPointer());
            return answer;
        }
        #endregion

        public long FindIndexBinarySearch<TComparer>(ref TSource searchItem, in TComparer comparer)
            where TComparer : struct, IRefFunc<TSource, TSource, int>
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
    }
}