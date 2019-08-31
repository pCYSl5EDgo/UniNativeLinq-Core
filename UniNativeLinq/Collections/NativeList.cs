using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    internal unsafe struct
        NativeList<T>
        : IRefEnumerable<NativeList<T>.Enumerator, T>, IDisposable
        where T : unmanaged
    {
        private T* Ptr;
        private long Capacity;
        private long Length;
        private Allocator Allocator;

        public NativeList(Allocator allocator)
        {
            Capacity = 16L;
            Ptr = UnsafeUtilityEx.Malloc<T>(Capacity, allocator);
            Length = 0L;
            Allocator = allocator;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator() => new Enumerator(this);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool IsEmpty => Length == 0;
        public bool IsFull => Length == Capacity;

        public bool CanIndexAccess() => true;

        public ref T this[long index]
        {
            get
            {
                if (index >= Length) throw new ArgumentOutOfRangeException();
                return ref Ptr[index];
            }
        }

        public void Add(in T value)
        {
            if (Length == Capacity)
            {
                var newCapacity = Capacity + (Capacity >> 1);
                UnsafeUtilityEx.ReAlloc(ref Ptr, Capacity, newCapacity, Allocator);
                Capacity = newCapacity;
            }
            Ptr[Length++] = value;
        }

        public void Clear() => Length = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFastCount() => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Any() => Length != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count() => (int)Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long LongCount() => Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeEnumerable<T> AsNativeEnumerable() => NativeEnumerable<T>.Create(Ptr, Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(T* dest) => UnsafeUtilityEx.MemCpy(dest, Ptr, Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeEnumerable<T> ToNativeEnumerable(Allocator allocator)
        {
            var ptr = UnsafeUtilityEx.Malloc<T>(Length, allocator);
            CopyTo(ptr);
            return NativeEnumerable<T>.Create(ptr, Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeArray<T> ToNativeArray(Allocator allocator)
        {
            var answer = new NativeArray<T>(Count(), allocator, NativeArrayOptions.UninitializedMemory);
            CopyTo(answer.GetPointer());
            return answer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray()
        {
            var answer = new T[Length];
            CopyTo(Pseudo.AsPointer<T>(ref answer[0]));
            return answer;
        }

        public struct Enumerator : IRefEnumerator<T>
        {
            private T* ptr;
            private long count;
            private long index;

            internal Enumerator(in NativeList<T> @this)
            {
                ptr = @this.Ptr;
                count = @this.Length;
                index = -1;
            }

            public ref T Current => ref ptr[index];
            T IEnumerator<T>.Current => Current;
            object IEnumerator.Current => Current;
            public void Dispose() => this = default;
            public void Dispose(Allocator allocator)
            {
                if (UnsafeUtility.IsValidAllocator(allocator) && ptr != null)
                    UnsafeUtility.Free(ptr, allocator);
                this = default;
            }
            public bool MoveNext() => ++index < count;
            public void Reset() => index = -1;

            public ref T TryGetNext(out bool success)
            {
                if (index >= count)
                    success = false;
                else
                    success = ++index < count;
                if (success)
                    return ref ptr[index];
                return ref Pseudo.AsRefNull<T>();
            }

            public bool TryMoveNext(out T value)
            {
                var success = ++index < count;
                if (success)
                {
                    value = ptr[index];
                }
                else
                {
                    index = count;
                    value = default;
                }
                return success;
            }
        }

        public void Dispose()
        {
            if (Ptr == null || !UnsafeUtility.IsValidAllocator(Allocator)) return;
            UnsafeUtility.Free(Ptr, Allocator);
            this = default;
        }
    }
}
