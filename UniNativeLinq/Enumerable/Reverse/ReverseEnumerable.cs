using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    public readonly unsafe struct
        ReverseEnumerable<TEnumerable, TEnumerator, T>
        : IRefEnumerable<ReverseEnumerable<TEnumerable, TEnumerator, T>.Enumerator, T>
        where T : unmanaged
        where TEnumerator : struct, IRefEnumerator<T>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, T>
    {
        internal readonly TEnumerable Enumerable;
        private readonly Allocator alloc;

        public ReverseEnumerable(in TEnumerable enumerable, Allocator alloc)
        {
            Enumerable = enumerable;
            this.alloc = alloc;
        }

        public ReverseEnumerable(NativeEnumerable<T> enumerable)
        {
            Enumerable = Psuedo.As<NativeEnumerable<T>, TEnumerable>(ref enumerable);
            alloc = Allocator.None;
        }

        public ReverseEnumerable(ArrayEnumerable<T> enumerable)
        {
            Enumerable = Psuedo.As<ArrayEnumerable<T>, TEnumerable>(ref enumerable);
            alloc = Allocator.None;
        }

        public struct Enumerator : IRefEnumerator<T>
        {
            private readonly ReverseEnumerableKind kind;
            private readonly Allocator allocator;
            private NativeEnumerable<T>.ReverseEnumerator enumerator;

            internal Enumerator(TEnumerable enumerable, Allocator allocator)
            {
                kind = ReverseEnumerableKind.Other;
                enumerator = enumerable.ToNativeEnumerable(allocator).GetReverseEnumerator();
                this.allocator = allocator;
            }

            internal Enumerator([PsuedoIsReadOnly]ref NativeEnumerable<T> enumerable)
            {
                kind = ReverseEnumerableKind.NativeArray;
                enumerator = enumerable.GetReverseEnumerator();
                allocator = Allocator.None;
            }

            public bool MoveNext() => enumerator.MoveNext();

            public void Reset() => enumerator.Reset();

            public readonly ref T Current => ref enumerator.Current;

            readonly T IEnumerator<T>.Current => Current;

            readonly object IEnumerator.Current => Current;

            public void Dispose()
            {
                switch (kind)
                {
                    case ReverseEnumerableKind.None:
                        return;
                    case ReverseEnumerableKind.NativeArray:
                        enumerator.Dispose();
                        break;
                    case ReverseEnumerableKind.Other:
                        UnsafeUtility.Free(enumerator.Ptr, allocator);
                        enumerator.Dispose();
                        break;
                }
                this = default;
            }

            public ref T TryGetNext(out bool success) => ref enumerator.TryGetNext(out success);

            public bool TryMoveNext(out T value) => enumerator.TryMoveNext(out value);
        }

        public readonly Enumerator GetEnumerator() => new Enumerator(Enumerable, alloc);

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool CanFastCount() => Enumerable.CanFastCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Any() => Enumerable.Any();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int Count() => Enumerable.Count();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly long LongCount() => Enumerable.LongCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void CopyTo(T* dest)
        {
            var enumerator = GetEnumerator();
            while (enumerator.MoveNext())
                *dest++ = enumerator.Current;
            enumerator.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T[] ToArray()
        {
            var count = LongCount();
            if (count == 0) return Array.Empty<T>();
            var answer = new T[count];
            CopyTo(Psuedo.AsPointer<T>(ref answer[0]));
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
            CopyTo(UnsafeUtilityEx.GetPointer(answer));
            return answer;
        }
        #endregion
    }
}