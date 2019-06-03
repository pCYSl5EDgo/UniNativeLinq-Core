using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UniNativeLinq
{
    public unsafe struct
        AppendEnumerable<TEnumerable, TEnumerator, T>
        : IRefEnumerable<AppendEnumerable<TEnumerable, TEnumerator, T>.Enumerator, T>
        where T : unmanaged
        where TEnumerable : struct, IRefEnumerable<TEnumerator, T>
        where TEnumerator : struct, IRefEnumerator<T>
    {
        private TEnumerable enumerable;
        private readonly T element;

        public AppendEnumerable(in TEnumerable enumerable, in T element)
        {
            this.enumerable = enumerable;
            this.element = element;
        }

        [LocalRefReturn]
        public struct Enumerator
            : IRefEnumerator<T>
        {
            private TEnumerator enumerator;
            private readonly T element;
            private bool isCurrentEnumerator;

            public Enumerator(in TEnumerator enumerator, in T element)
            {
                this.element = element;
                this.enumerator = enumerator;
                isCurrentEnumerator = true;
            }

            public ref T Current
            {
                get
                {
                    if (isCurrentEnumerator)
                        return ref enumerator.Current;
                    throw new NotImplementedException();
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
                if (!isCurrentEnumerator)
                    return false;
                if (!enumerator.MoveNext())
                    isCurrentEnumerator = false;
                return true;
            }

            public void Reset() => throw new InvalidOperationException();

            public ref T TryGetNext(out bool success)
            {
                success = isCurrentEnumerator;
                if (!success)
                    throw new NotImplementedException();
                ref var value = ref enumerator.TryGetNext(out success);
                if (!success)
                    isCurrentEnumerator = false;
                return ref value;
            }

            public bool TryMoveNext(out T value)
            {
                if (!isCurrentEnumerator)
                {
                    value = element;
                    return true;
                }
                if (enumerator.TryMoveNext(out value))
                    return true;
                isCurrentEnumerator = false;
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Enumerator GetEnumerator() => new Enumerator(enumerable.GetEnumerator(), element);

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool CanFastCount() => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Any() => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int Count()
            => (int)LongCount();

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
            var answer = new T[LongCount()];
            CopyTo((T*)Unsafe.AsPointer(ref answer[0]));
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
            CopyTo(answer.GetPointer());
            return answer;
        }
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryGetLast(out T value)
        {
            value = element;
            return true;
        }
    }
}