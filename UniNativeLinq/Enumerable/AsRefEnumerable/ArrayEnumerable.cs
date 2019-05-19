using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    public unsafe partial struct
        ArrayEnumerable<TSource>
        : IRefEnumerable<ArrayEnumerable<TSource>.Enumerator, TSource>
        where TSource : unmanaged
    {
        private readonly TSource[] array;
        private readonly long offset;
        internal readonly long Length;

        internal readonly TSource* GetPointer() => (TSource*)Unsafe.AsPointer(ref array[offset]);
        private readonly TSource* GetPinPointer(out ulong gcHandle) => (TSource*)UnsafeUtility.PinGCArrayAndGetDataAddress(array, out gcHandle) + offset;

        public ArrayEnumerable(TSource[] array)
        {
            this.array = array ?? throw new ArgumentNullException();
            Length = array.LongLength;
            offset = 0;
        }

        public ArrayEnumerable(ArraySegment<TSource> segment)
        {
            array = segment.Array ?? throw new ArgumentNullException();
            Length = segment.Count;
            offset = segment.Offset;
        }

        public ArrayEnumerable(TSource[] array, long offset, long count)
        {
            this.array = array ?? throw new ArgumentNullException();
            Length = count;
            this.offset = offset;
        }

        public struct Enumerator : IRefEnumerator<TSource>
        {
            private readonly TSource* ptr;
            private readonly long length;
            private readonly ulong gcHandle;
            private long index;

            internal Enumerator(TSource* ptr, long length, ulong gcHandle)
            {
                this.ptr = ptr;
                this.length = length;
                this.gcHandle = gcHandle;
                index = -1;
            }

            public bool MoveNext() => ++index < length;
            public void Reset() => index = -1;
            public readonly ref TSource Current => ref ptr[index];
            readonly TSource IEnumerator<TSource>.Current => Current;
            readonly object IEnumerator.Current => Current;

            public void Dispose()
            {
                if (ptr != null)
                    UnsafeUtility.ReleaseGCObject(gcHandle);
                this = default;
            }

            public ref TSource TryGetNext(out bool success)
            {
                success = ++index < length;
                if (success)
                    return ref ptr[index];
                index = length;
                return ref Unsafe.AsRef<TSource>(null);
            }

            public bool TryMoveNext(out TSource value)
            {
                if (++index < length)
                {
                    value = ptr[index];
                    return true;
                }
                else
                {
                    index = length;
                    value = default;
                    return false;
                }
            }
        }

        public struct ReverseEnumerator : IRefEnumerator<TSource>
        {
            private readonly TSource* ptr;
            private readonly long length;
            private readonly ulong gcHandle;
            private long index;

            internal ReverseEnumerator(TSource* ptr, long length, ulong gcHandle)
            {
                this.ptr = ptr;
                this.length = length;
                this.gcHandle = gcHandle;
                index = length;
            }

            public bool MoveNext() => --index >= 0;
            public void Reset() => index = length;
            public ref TSource Current => ref ptr[index];
            TSource IEnumerator<TSource>.Current => Current;
            object IEnumerator.Current => Current;

            public void Dispose()
            {
                if (ptr != null)
                    UnsafeUtility.ReleaseGCObject(gcHandle);
                this = default;
            }

            public ref TSource TryGetNext(out bool success)
            {
                success = --index >= 0;
                if (success)
                    return ref ptr[index];
                index = 0;
                return ref Unsafe.AsRef<TSource>(null);
            }

            public bool TryMoveNext(out TSource value)
            {
                if(--index >= 0)
                {
                    value = ptr[index];
                    return true;
                }
                else
                {
                    index = 0;
                    value = default;
                    return false;
                }
            }
        }

        public readonly Enumerator GetEnumerator()
        {
            if (array is null || array.Length == 0)
                return default;
            return new Enumerator(GetPinPointer(out var gcHandle), Length, gcHandle);
        }

        public readonly ArrayEnumerable<TSource> Slice(long length)
        {
            if (length > Length)
                length = Length;
            return length <= 0 ? new ArrayEnumerable<TSource>(Array.Empty<TSource>(), 0, 0) : new ArrayEnumerable<TSource>(array, offset, length);
        }
        public readonly ArrayEnumerable<TSource> Slice(long offset, long length)
        {
            if (array.Length == 0) return this;
            var rest = this.offset + Length - offset;
            if (length <= 0 || rest <= 0) return new ArrayEnumerable<TSource>(Array.Empty<TSource>(), 0, 0);
            if (length > rest)
                length = rest;
            return new ArrayEnumerable<TSource>(array, offset + this.offset, length);
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
        public readonly void CopyTo(TSource* dest) => UnsafeUtilityEx.MemCpy(dest, GetPointer(), Length);

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
    }
}
