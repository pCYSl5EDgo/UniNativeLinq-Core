using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    public unsafe readonly partial struct
        ReverseEnumerable<TEnumerable, TEnumerator, TSource>
        : IRefEnumerable<ReverseEnumerable<TEnumerable, TEnumerator, TSource>.Enumerator, TSource>
        where TSource : unmanaged
        where TEnumerator : struct, IRefEnumerator<TSource>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
    {
        internal readonly TEnumerable Enumerable;
        private readonly Allocator alloc;

        public ReverseEnumerable(in TEnumerable enumerable, Allocator alloc)
        {
            Enumerable = enumerable;
            this.alloc = alloc;
        }

        public ReverseEnumerable(NativeEnumerable<TSource> enumerable)
        {
            Enumerable = Unsafe.As<NativeEnumerable<TSource>, TEnumerable>(ref enumerable);
            alloc = Allocator.None;
        }

        public ReverseEnumerable(ArrayEnumerable<TSource> enumerable)
        {
            Enumerable = Unsafe.As<ArrayEnumerable<TSource>, TEnumerable>(ref enumerable);
            alloc = Allocator.None;
        }

        public struct Enumerator : IRefEnumerator<TSource>
        {
            private readonly ReverseEnumerableKind kind;
            private readonly Allocator allocator;
            private NativeEnumerable<TSource>.ReverseEnumerator enumerator;

            internal Enumerator(TEnumerable enumerable, Allocator allocator)
            {
                kind = ReverseEnumerableKind.Other;
                enumerator = enumerable.ToNativeEnumerable(allocator).GetReverseEnumerator();
                this.allocator = allocator;
            }

            internal Enumerator(in NativeEnumerable<TSource> enumerable)
            {
                kind = ReverseEnumerableKind.NativeArray;
                enumerator = enumerable.GetReverseEnumerator();
                allocator = Allocator.None;
            }

            public bool MoveNext() => enumerator.MoveNext();

            public void Reset() => enumerator.Reset();

            public readonly ref TSource Current => ref enumerator.Current;

            readonly TSource IEnumerator<TSource>.Current => Current;

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

            public ref TSource TryGetNext(out bool success) => ref enumerator.TryGetNext(out success);

            public bool TryMoveNext(out TSource value) => enumerator.TryMoveNext(out value);
        }

        public readonly Enumerator GetEnumerator() => new Enumerator(Enumerable, alloc);

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();

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
        public readonly void CopyTo(TSource* dest)
        {
            var enumerator = GetEnumerator();
            while (enumerator.MoveNext())
                *dest++ = enumerator.Current;
            enumerator.Dispose();
        }

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
            CopyTo(UnsafeUtilityEx.GetPointer(answer));
            return answer;
        }
        #endregion
    }
}