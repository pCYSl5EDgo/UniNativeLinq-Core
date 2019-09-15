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
        private T element;
        public bool CanIndexAccess() => enumerable.CanIndexAccess();

        public ref T this[long index]
        {
            get
            {
                var length = enumerable.LongCount();
                if (index >= length + 1) throw new ArgumentOutOfRangeException();
                if (index == length) throw new NotImplementedException();
                return ref enumerable[index];
            }
        }
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
            private T element;
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

            public void Dispose() => enumerator.Dispose();

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
        public Enumerator GetEnumerator() => new Enumerator(enumerable.GetEnumerator(), element);

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFastCount() => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Any() => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count()
            => (int)LongCount();

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
            var answer = new T[LongCount()];
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetLast(out T value)
        {
            value = element;
            return true;
        }
    }
}