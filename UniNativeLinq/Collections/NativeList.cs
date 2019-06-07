using System.Collections;
using System.Collections.Generic;

using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    internal unsafe struct
        NativeList<T>
        : IRefEnumerable<NativeList<T>.Enumerator, T>
        where T : unmanaged
    {
        private T* Ptr;
        private long Capacity;
        private long Length;
        private readonly Allocator Allocator;

        public NativeList(Allocator allocator)
        {
            Capacity = 16L;
            Ptr = UnsafeUtilityEx.Malloc<T>(Capacity, allocator);
            Length = 0L;
            Allocator = allocator;
        }

        public readonly Enumerator GetEnumerator() => new Enumerator(this);
        readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public readonly bool IsEmpty => Length == 0;
        public readonly bool IsFull => Length == Capacity;

        public void Add(in T value)
        {
            if (Length == Capacity)
            {
                var newCapa = Capacity + (Capacity >> 1);
                UnsafeUtilityEx.ReAlloc(ref Ptr, Capacity, newCapa, Allocator);
                Capacity = newCapa;
            }
            Ptr[Length++] = value;
        }

        public void Clear() => Length = 0;

        public readonly bool CanFastCount() => true;

        public readonly bool Any() => Length != 0;

        public readonly int Count() => (int)Length;

        public readonly long LongCount() => Length;

        public readonly NativeEnumerable<T> AsNativeEnumerable() => new NativeEnumerable<T>(Ptr, Length);

        public readonly void CopyTo(T* dest) => UnsafeUtilityEx.MemCpy(dest, Ptr, Length);

        public readonly NativeEnumerable<T> ToNativeEnumerable(Allocator allocator)
        {
            var ptr = UnsafeUtilityEx.Malloc<T>(Length, allocator);
            CopyTo(ptr);
            return new NativeEnumerable<T>(ptr, Length);
        }

        public readonly NativeArray<T> ToNativeArray(Allocator allocator)
        {
            var answer = new NativeArray<T>(Count(), allocator, NativeArrayOptions.UninitializedMemory);
            CopyTo(answer.GetPointer());
            return answer;
        }

        public readonly T[] ToArray()
        {
            var answer = new T[Length];
            CopyTo(Pseudo.AsPointer<T>(ref answer[0]));
            return answer;
        }

        public struct Enumerator : IRefEnumerator<T>
        {
            private readonly T* ptr;
            private readonly long count;
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
                if(success)
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
    }
}
