using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    public unsafe readonly partial struct
        PrependEnumerable<TEnumerable, TEnumerator, TSource>
        : IRefEnumerable<PrependEnumerable<TEnumerable, TEnumerator, TSource>.Enumerator, TSource>
        where TSource : unmanaged
        where TEnumerator : struct, IRefEnumerator<TSource>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
    {
        private readonly TEnumerable enumerable;
        private readonly TSource prepend;
        private readonly Allocator alloc;

        public PrependEnumerable(TEnumerable enumerable, TSource prepend, Allocator alloc)
        {
            this.enumerable = enumerable;
            this.prepend = prepend;
            this.alloc = alloc;
        }

        public struct Enumerator : IRefEnumerator<TSource>
        {
            private TEnumerator enumerator;
            private readonly TSource* element;
            private readonly Allocator allocator;
            private bool isInitialState;
            private bool isPrependNow;

            public Enumerator(in TEnumerator enumerator, in TSource element, Allocator allocator)
            {
                this.allocator = allocator;
                this.element = UnsafeUtilityEx.Malloc<TSource>(1, allocator);
                *this.element = element;
                this.enumerator = enumerator;
                isInitialState = true;
                isPrependNow = true;
            }

            public ref TSource Current
            {
                get
                {
                    if (isPrependNow)
                        return ref *element;
                    return ref enumerator.Current;
                }
            }

            TSource IEnumerator<TSource>.Current => Current;
            object IEnumerator.Current => Current;

            public void Dispose()
            {
                enumerator.Dispose();
                if (element != null && allocator != Allocator.None)
                    UnsafeUtility.Free(element, allocator);
                this = default;
            }

            public bool MoveNext()
            {
                if(isInitialState)
                {
                    isInitialState = false;
                    return true;
                }
                if(isPrependNow)
                    isPrependNow = false;
                return enumerator.MoveNext();
            }

            public void Reset() => throw new InvalidOperationException();

            public ref TSource TryGetNext(out bool success)
            {
                if(isInitialState)
                {
                    isInitialState = false;
                    success = true;
                    return ref *element;
                }
                return ref enumerator.TryGetNext(out success);
            }

            public bool TryMoveNext(out TSource value)
            {
                if (isInitialState)
                {
                    isInitialState = false;
                    value = *element;
                    return true;
                }
                return enumerator.TryMoveNext(out value);
            }
        }

        public readonly Enumerator GetEnumerator() => new Enumerator(enumerable.GetEnumerator(), prepend, alloc);

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool CanFastCount() => enumerable.CanFastCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Any() => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int Count() => enumerable.Count() + 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly long LongCount() => enumerable.LongCount() + 1;

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