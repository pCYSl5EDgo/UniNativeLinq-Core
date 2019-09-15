using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    internal unsafe struct
        RingBuffer<T>
        : IRefEnumerable<RingBuffer<T>.Enumerator, T>, IDisposable
        where T : unmanaged
    {
        public T* Elements;
        public long Capacity;
        public long Length;
        public long StartIndex;
        public Allocator Allocator;

        public RingBuffer(long capacity, Allocator allocator)
        {
            Elements = UnsafeUtilityEx.Malloc<T>(capacity, allocator);
            Capacity = capacity;
            Length = 0;
            StartIndex = 0;
            Allocator = allocator;
        }

        public ref T this[long index]
        {
            get
            {
                index += StartIndex;
                if (index >= Capacity)
                    index -= Capacity;
                return ref Elements[index];
            }
        }
        public bool CanIndexAccess() => true;

        public bool IsFull => Length == Capacity;
        public bool IsEmpty => Length == 0L;


        public void Add(in T value)
        {
            if (Capacity <= Length)
                ReAllocate();
            var index = StartIndex + Length;
            if (index >= Capacity)
                index -= Capacity;
            Elements[index] = value;
            ++Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator() => new Enumerator(this);
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public struct Enumerator
            : IRefEnumerator<T>
        {
            internal RingBuffer<T> Parent;

            internal Enumerator(in RingBuffer<T> parent) => Parent = parent;

            public ref T Current => ref Parent.Elements[Parent.StartIndex];
            T IEnumerator<T>.Current => Current;
            object IEnumerator.Current => Current;

            public void Dispose() { }

            public bool MoveNext()
            {
                ++Parent.StartIndex;
                if (Parent.StartIndex >= Parent.Capacity)
                    Parent.StartIndex = 0;
                return --Parent.Length >= 0;
            }

            public void Reset() => throw new InvalidOperationException();

            public ref T TryGetNext(out bool success)
            {
                ++Parent.StartIndex;
                success = --Parent.Length >= 0;
                if (Parent.StartIndex >= Parent.Capacity)
                    Parent.StartIndex = 0;
                if (success)
                    return ref Parent.Elements[Parent.StartIndex];
                else
                    return ref Pseudo.AsRefNull<T>();
            }

            public bool TryMoveNext(out T value)
            {
                ++Parent.StartIndex;
                var success = --Parent.Length >= 0;
                if (Parent.StartIndex >= Parent.Capacity)
                    Parent.StartIndex = 0;
                value = success ? Parent.Elements[Parent.StartIndex] : default;
                return success;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Any() => Length != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFastCount() => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count() => (int)Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long LongCount() => Length;

        public void RemoveFirst()
        {
            --Length;
            ++StartIndex;
            if (StartIndex >= Capacity)
                StartIndex = 0;
        }

        public void RemoveLast() => --Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long CopyTo(T* dest)
        {
            if (Length == 0)
            {
                return 0;
            }
            if (StartIndex + Length <= Capacity)
            {
                UnsafeUtilityEx.MemCpy(dest, Elements + StartIndex, Length);
            }
            else
            {
                var count = Capacity - StartIndex;
                UnsafeUtilityEx.MemCpy(dest, Elements + StartIndex, count);
                UnsafeUtilityEx.MemCpy(dest + count, Elements, Length - count);
            }
            return Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray()
        {
            var answer = new T[Length];
            CopyTo(Pseudo.AsPointer(ref answer[0]));
            return answer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeArray<T> ToNativeArray(Allocator allocator)
        {
            var answer = new NativeArray<T>((int)Length, allocator, NativeArrayOptions.UninitializedMemory);
            CopyTo(answer.GetPointer());
            return answer;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeEnumerable<T> ToNativeEnumerable(Allocator allocator)
        {
            var ptr = UnsafeUtilityEx.Malloc<T>(Length, allocator);
            CopyTo(ptr);
            return NativeEnumerable<T>.Create(ptr, Length);
        }

        private void ReAllocate()
        {
            var tmp = UnsafeUtilityEx.Malloc<T>(Capacity << 1, Allocator);
            CopyTo(tmp);
            UnsafeUtility.Free(Elements, Allocator);
            Elements = tmp;
            Capacity <<= 1;
            StartIndex = 0;
        }

        public void Dispose()
        {
            if (Elements == null || !UnsafeUtility.IsValidAllocator(Allocator)) return;
            UnsafeUtility.Free(Elements, Allocator);
        }
    }
}
