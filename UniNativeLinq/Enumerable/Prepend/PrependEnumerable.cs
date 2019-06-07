using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UniNativeLinq
{
    public readonly unsafe struct
        PrependEnumerable<TEnumerable, TEnumerator, T>
        : IRefEnumerable<PrependEnumerable<TEnumerable, TEnumerator, T>.Enumerator, T>
        where T : unmanaged
        where TEnumerator : struct, IRefEnumerator<T>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, T>
    {
        private readonly TEnumerable enumerable;
        private readonly T prepend;

        public PrependEnumerable(TEnumerable enumerable, in T prepend)
        {
            this.enumerable = enumerable;
            this.prepend = prepend;
        }

        [LocalRefReturn]
        public struct Enumerator : IRefEnumerator<T>
        {
            private TEnumerator enumerator;
            private readonly T element;
            private bool isInitialState;
            private bool isPrependNow;

            public Enumerator(in TEnumerator enumerator, in T element)
            {
                this.element = element;
                this.enumerator = enumerator;
                isInitialState = true;
                isPrependNow = true;
            }

            public ref T Current
            {
                get
                {
                    if (isPrependNow)
                        throw new NotImplementedException();
                    return ref enumerator.Current;
                }
            }

            T IEnumerator<T>.Current => Current;
            object IEnumerator.Current => Current;

            public void Dispose()
            {
                enumerator.Dispose();
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

            public ref T TryGetNext(out bool success)
            {
                if(isInitialState)
                {
                    isInitialState = false;
                    success = true;
                    throw new NotImplementedException();
                }
                return ref enumerator.TryGetNext(out success);
            }

            public bool TryMoveNext(out T value)
            {
                if (isInitialState)
                {
                    isInitialState = false;
                    value = element;
                    return true;
                }
                return enumerator.TryMoveNext(out value);
            }
        }

        public readonly Enumerator GetEnumerator() => new Enumerator(enumerable.GetEnumerator(), prepend);

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

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

        public readonly bool TryGetFirst(out T value)
        {
            value = prepend;
            return true;
        }
    }
}