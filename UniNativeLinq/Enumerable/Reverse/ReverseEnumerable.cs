using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    [PseudoIsReadOnly]
    public unsafe struct
        ReverseEnumerable<TEnumerable, TEnumerator, T>
        : IRefEnumerable<ReverseEnumerable<TEnumerable, TEnumerator, T>.Enumerator, T>
        where T : unmanaged
        where TEnumerator : struct, IRefEnumerator<T>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, T>
    {
        internal TEnumerable Enumerable;
        private readonly Allocator alloc;

        public ReverseEnumerable(in TEnumerable enumerable, Allocator allocator)
        {
            Enumerable = enumerable;
            alloc = allocator;
        }

        public ReverseEnumerable(NativeEnumerable<T> enumerable)
        {
            Enumerable = Pseudo.As<NativeEnumerable<T>, TEnumerable>(ref enumerable);
            alloc = Allocator.None;
        }

        public ReverseEnumerable(ArrayEnumerable<T> enumerable)
        {
            Enumerable = Pseudo.As<ArrayEnumerable<T>, TEnumerable>(ref enumerable);
            alloc = Allocator.None;
        }

        public struct Enumerator : IRefEnumerator<T>
        {
            private readonly ReverseEnumerableKind kind;
            private readonly Allocator allocator;
            private NativeEnumerable<T>.Enumerator enumerator;

            internal Enumerator(ref TEnumerable enumerable, Allocator allocator)
            {
                kind = ReverseEnumerableKind.Other;
                var nativeEnumerable = enumerable.ToNativeEnumerable(allocator);
                for (long i = 0L, j = nativeEnumerable.Length - 1; i < j; i++, j--)
                {
                    (nativeEnumerable[i], nativeEnumerable[j]) = (nativeEnumerable[j], nativeEnumerable[i]);
                }
                enumerator = nativeEnumerable.GetEnumerator();
                this.allocator = allocator;
            }

            public bool MoveNext() => enumerator.MoveNext();

            public void Reset() => enumerator.Reset();

            public ref T Current => ref enumerator.Current;

            T IEnumerator<T>.Current => Current;

            object IEnumerator.Current => Current;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        public Enumerator GetEnumerator() => new Enumerator(ref Enumerable, alloc);

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool CanFastCount() => Enumerable.CanFastCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Any() => Enumerable.Any();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int Count() => Enumerable.Count();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly long LongCount() => Enumerable.LongCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        public void CopyTo(T* dest)
        {
            var enumerator = GetEnumerator();
            while (enumerator.MoveNext())
                *dest++ = enumerator.Current;
            enumerator.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        public T[] ToArray()
        {
            var count = LongCount();
            if (count == 0) return Array.Empty<T>();
            var answer = new T[count];
            CopyTo(Pseudo.AsPointer<T>(ref answer[0]));
            return answer;
        }

        public bool CanIndexAccess => Enumerable.CanIndexAccess;
        public ref T this[long index] => ref Enumerable[Enumerable.LongCount() - 1L - index];

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        public NativeEnumerable<T> ToNativeEnumerable(Allocator allocator)
        {
            var count = LongCount();
            var ptr = UnsafeUtilityEx.Malloc<T>(count, allocator);
            CopyTo(ptr);
            return NativeEnumerable<T>.Create(ptr, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        public NativeArray<T> ToNativeArray(Allocator allocator)
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