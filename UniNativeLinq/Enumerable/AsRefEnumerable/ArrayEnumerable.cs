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
        ArrayEnumerable<T>
        : IRefEnumerable<ArrayEnumerable<T>.Enumerator, T>
        where T : unmanaged
    {
        private T[] array;
        private long offset;
        public long Length;

        internal T* GetPointer() => Pseudo.AsPointer(ref array[offset]);
        private T* GetPinPointer(out ulong gcHandle) => (T*)UnsafeUtility.PinGCArrayAndGetDataAddress(array, out gcHandle) + offset;

        public ArrayEnumerable(T[] array)
        {
            this.array = array ?? throw new ArgumentNullException();
            Length = array.LongLength;
            offset = 0;
        }

        public static ArrayEnumerable<T> Create(T[] array, long offset, long count)
        {
            return new ArrayEnumerable<T>(array) { Length = count, offset = offset };
        }

        public ref T this[long index] => ref array[offset + index];

        public struct Enumerator : IRefEnumerator<T>
        {
            private T* ptr;
            private long length;
            private ulong gcHandle;
            private long index;

            internal Enumerator(T* ptr, long length, ulong gcHandle)
            {
                this.ptr = ptr;
                this.length = length;
                this.gcHandle = gcHandle;
                index = -1;
            }

            public bool MoveNext() => ++index < length;
            public void Reset() => index = -1;
            public ref T Current => ref ptr[index];
            T IEnumerator<T>.Current => Current;
            object IEnumerator.Current => Current;

            public void Dispose()
            {
                if (ptr != null)
                    UnsafeUtility.ReleaseGCObject(gcHandle);
                this = default;
            }

            public ref T TryGetNext(out bool success)
            {
                success = ++index < length;
                if (success)
                    return ref ptr[index];
                index = length;
                return ref Pseudo.AsRefNull<T>();
            }

            public bool TryMoveNext(out T value)
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

        public Enumerator GetEnumerator()
        {
            if (array is null || array.Length == 0)
                return default;
            return new Enumerator(GetPinPointer(out var gcHandle), Length, gcHandle);
        }

        public ArrayEnumerable<T> Slice(long length)
        {
            if (length > Length)
                length = Length;
            return length <= 0 ? Create(Array.Empty<T>(), 0, 0) : Create(array, offset, length);
        }
        public ArrayEnumerable<T> Slice(long offset, long length)
        {
            if (array.Length == 0) return this;
            var rest = this.offset + Length - offset;
            if (length <= 0 || rest <= 0) return Create(Array.Empty<T>(), 0, 0);
            if (length > rest)
                length = rest;
            return Create(array, offset + this.offset, length);
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
        public void CopyTo(T* dest) => UnsafeUtilityEx.MemCpy(dest, GetPointer(), Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray()
        {
            var count = LongCount();
            if (count == 0) return Array.Empty<T>();
            var answer = new T[count];
            CopyTo(Pseudo.AsPointer<T>(ref answer[0]));
            return answer;
        }

        public bool CanIndexAccess() => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeEnumerable<T> ToNativeEnumerable(Allocator allocator)
        {
            var count = LongCount();
            var ptr = UnsafeUtilityEx.Malloc<T>(count, allocator);
            CopyTo(ptr);
            return NativeEnumerable<T>.Create(ptr, count);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetLast(out T value)
        {
            var answer = Length != 0;
            value = array[offset + Length - 1];
            return answer;
        }
    }
}
