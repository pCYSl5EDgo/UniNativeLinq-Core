using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    [SlowCount]
    public unsafe struct
        GroupingEnumerable<TKey, T>
        : IRefEnumerable<GroupingEnumerable<TKey, T>.Enumerator, T>, IDisposable
        where TKey : unmanaged
        where T : unmanaged
    {
        public TKey Key;
        public T* Elements;
        public long Length;
        public Allocator Allocator;

        public bool CanIndexAccess() => true;

        public ref T this[long index]
        {
            get
            {
                if (index >= Length) throw new ArgumentOutOfRangeException();
                return ref Elements[index];
            }
        }

        public GroupingEnumerable(in TKey key, T* elements, long length, Allocator allocator)
        {
            Key = key;
            Elements = elements;
            Length = length;
            Allocator = allocator;
        }

        public GroupingEnumerable(in TKey key, in NativeEnumerable<T> elements, Allocator allocator)
        {
            Key = key;
            Elements = elements.Ptr;
            Length = elements.Length;
            Allocator = allocator;
        }

        public struct Enumerator : IRefEnumerator<T>
        {
            private T* elements;
            private long count;
            private long index;

            public Enumerator(in GroupingEnumerable<TKey, T> parent)
            {
                if (parent.Elements != null && parent.Length > 0)
                {
                    elements = parent.Elements;
                    count = parent.Length;
                }
                else
                {
                    elements = null;
                    count = 0;
                }
                index = -1;
            }

            public ref T Current => ref elements[index];
            T IEnumerator<T>.Current => Current;
            object IEnumerator.Current => Current;

            public void Dispose() { }

            public bool MoveNext() => ++index < count;

            public void Reset() => index = -1;

            public ref T TryGetNext(out bool success)
            {
                success = ++index < count;
                if (success)
                    return ref elements[index];
                index = count;
                return ref Pseudo.AsRefNull<T>();
            }

            public bool TryMoveNext(out T value)
            {
                if (++index < count)
                {
                    value = elements[index];
                    return true;
                }
                else
                {
                    value = default;
                    index = count;
                    return false;
                }
            }
        }

        public Enumerator GetEnumerator() => new Enumerator(in this);

        public void Dispose()
        {
            if (Elements == null || !UnsafeUtility.IsValidAllocator(Allocator)) return;
            UnsafeUtility.Free(Elements, Allocator);
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
        public int Count()
            => (int)Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long LongCount() => Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long CopyTo(T* dest)
        {
            if (Length != 0)
            {
                UnsafeUtilityEx.MemCpy(dest, Elements, Length);
            }
            return Length;
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
    }
}
