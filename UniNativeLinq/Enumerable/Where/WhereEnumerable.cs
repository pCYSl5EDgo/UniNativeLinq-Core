using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UniNativeLinq
{
    [SlowCount]
    public unsafe struct
        WhereEnumerable<TEnumerable, TEnumerator, T, TPredicate>
        : IRefEnumerable<WhereEnumerable<TEnumerable, TEnumerator, T, TPredicate>.Enumerator, T>
        where T : unmanaged
        where TPredicate : struct, IRefFunc<T, bool>
        where TEnumerator : struct, IRefEnumerator<T>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, T>
    {
        private TEnumerable enumerable;
        private TPredicate predicts;

        public WhereEnumerable(in TEnumerable enumerable, in TPredicate predicts)
        {
            this.enumerable = enumerable;
            this.predicts = predicts;
        }

        public Enumerator GetEnumerator() => new Enumerator(enumerable.GetEnumerator(), predicts);

        public struct Enumerator : IRefEnumerator<T>
        {
            private TEnumerator enumerator;
            private TPredicate predicts;

            internal Enumerator(in TEnumerator enumerator, in TPredicate predicts)
            {
                this.enumerator = enumerator;
                this.predicts = predicts;
            }

            public bool MoveNext()
            {
                while (true)
                {
                    ref var value = ref enumerator.TryGetNext(out var success);
                    if (!success) return false;
                    if (predicts.Calc(ref value)) return true;
                }
            }

            public void Reset() => throw new InvalidOperationException();

            public ref T Current => ref enumerator.Current;

            T IEnumerator<T>.Current => Current;

            object IEnumerator.Current => Current;

            public void Dispose() => enumerator.Dispose();

            public ref T TryGetNext(out bool success)
            {
                while (true)
                {
                    ref var value = ref enumerator.TryGetNext(out success);
                    if (!success)
                    {
                        return ref Pseudo.AsRefNull<T>();
                    }
                    if (predicts.Calc(ref value))
                    {
                        return ref value;
                    }
                }
            }

            public bool TryMoveNext(out T value)
            {
                while (true)
                {
                    ref var temp = ref enumerator.TryGetNext(out var success);
                    if (!success)
                    {
                        value = default;
                        return false;
                    }
                    if (predicts.Calc(ref temp))
                    {
                        value = temp;
                        return true;
                    }
                }
            }
        }

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFastCount() => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Any()
        {
            var enumerator = GetEnumerator();
            if (enumerator.MoveNext())
            {
                enumerator.Dispose();
                return true;
            }
            enumerator.Dispose();
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count()
            => (int)LongCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long LongCount()
        {
            var enumerator = GetEnumerator();
            var count = 0L;
            while (enumerator.MoveNext())
                ++count;
            enumerator.Dispose();
            return count;
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

        public bool CanIndexAccess() => false;
        public ref T this[long index] => throw new NotSupportedException();

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