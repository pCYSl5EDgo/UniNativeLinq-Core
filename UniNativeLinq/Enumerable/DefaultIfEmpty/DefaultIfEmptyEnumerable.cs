using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UniNativeLinq
{
    public unsafe struct
        DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, T>
        : IRefEnumerable<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, T>.Enumerator, T>
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
                if (!enumerable.Any())
                {
                    if (index != 0) throw new ArgumentOutOfRangeException();
                    throw new NotImplementedException();
                }
                return ref enumerable[index];
            }
        }
        public DefaultIfEmptyEnumerable(in TEnumerable enumerable, in T element)
        {
            this.enumerable = enumerable;
            this.element = element;
        }

        public Enumerator GetEnumerator() => new Enumerator(enumerable.GetEnumerator(), element);

        [LocalRefReturn]
        public struct Enumerator : IRefEnumerator<T>
        {
            private TEnumerator enumerator;
            private T element;
            private bool isFirst;
            private bool isDefault;

            internal Enumerator(in TEnumerator enumerator, in T element)
            {
                this.enumerator = enumerator;
                this.element = element;
                isFirst = true;
                isDefault = false;
            }

            public ref T Current
            {
                get
                {
                    if (isDefault)
                        throw new NotImplementedException();
                    return ref enumerator.Current;
                }
            }

            T IEnumerator<T>.Current => Current;
            object IEnumerator.Current => Current;

            public void Dispose() => enumerator.Dispose();

            public bool MoveNext()
            {
                if (!isFirst) return !isDefault && enumerator.MoveNext();
                isFirst = false;
                isDefault = !enumerator.MoveNext();
                return true;
            }

            public void Reset() => throw new InvalidOperationException();

            public ref T TryGetNext(out bool success)
            {
                if (isFirst)
                {
                    isFirst = false;
                    ref var value = ref enumerator.TryGetNext(out success);
                    if (success)
                    {
                        isDefault = false;
                        return ref value;
                    }
                    isDefault = true;
                    success = true;
                    throw new NotImplementedException();
                }
                if (isDefault)
                {
                    success = false;
                    throw new NotImplementedException();
                }
                return ref enumerator.TryGetNext(out success);
            }

            public bool TryMoveNext(out T value)
            {
                if (isFirst)
                {
                    isFirst = false;
                    if (enumerator.TryMoveNext(out value))
                    {
                        isDefault = false;
                        return true;
                    }
                    isDefault = true;
                    value = element;
                    return true;
                }
                if (isDefault)
                {
                    value = default;
                    return false;
                }
                return enumerator.TryMoveNext(out value);
            }
        }

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
        public int Count()
            => (int)LongCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long LongCount()
        {
            var count = enumerable.LongCount();
            return count == 0 ? 1 : count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(T* dest)
        {
            var enumerator = GetEnumerator();
            while (enumerator.MoveNext())
                *dest++ = enumerator.Current;
            enumerator.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray()
        {
            var count = LongCount();
            if (count == 0) return Array.Empty<T>();
            var answer = new T[count];
            CopyTo(Pseudo.AsPointer<T>(ref answer[0]));
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