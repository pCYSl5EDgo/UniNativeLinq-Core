using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UniNativeLinq
{
    public unsafe struct
        PrependEnumerable<TEnumerable, TEnumerator, T>
        : IRefEnumerable<PrependEnumerable<TEnumerable, TEnumerator, T>.Enumerator, T>
        where T : unmanaged
        where TEnumerator : struct, IRefEnumerator<T>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, T>
    {
        private TEnumerable enumerable;
        private T element;

        public bool CanIndexAccess() => enumerable.CanIndexAccess();

        public ref T this[long index]
        {
            get
            {
                if (index == 0) throw new NotImplementedException();
                return ref enumerable[index - 1];
            }
        }

        public PrependEnumerable(in TEnumerable enumerable, in T element)
        {
            this.enumerable = enumerable;
            this.element = element;
        }

        [LocalRefReturn]
        public struct Enumerator : IRefEnumerator<T>
        {
            private TEnumerator enumerator;
            private T element;
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

            public void Dispose() => enumerator.Dispose();

            public bool MoveNext()
            {
                if (isInitialState)
                {
                    isInitialState = false;
                    return true;
                }
                if (isPrependNow)
                    isPrependNow = false;
                return enumerator.MoveNext();
            }

            public void Reset() => throw new InvalidOperationException();

            public ref T TryGetNext(out bool success)
            {
                if (isInitialState)
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

        public Enumerator GetEnumerator() => new Enumerator(enumerable.GetEnumerator(), element);

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFastCount() => enumerable.CanFastCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Any() => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count() => enumerable.Count() + 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long LongCount() => enumerable.LongCount() + 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long CopyTo(T* dest)
        {
            var enumerator = GetEnumerator();
            long answer = 0;
            while (enumerator.MoveNext())
            {
                *dest++ = enumerator.Current;
                answer++;
            }
            enumerator.Dispose();
            return answer;
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

        public bool TryGetFirst(out T value)
        {
            value = element;
            return true;
        }
    }
}