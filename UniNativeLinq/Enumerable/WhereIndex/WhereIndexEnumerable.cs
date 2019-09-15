using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UniNativeLinq
{
    [SlowCount]
    public unsafe struct
        WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, T, TPredicate>
        : IRefEnumerable<WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, T, TPredicate>.Enumerator, T>
        where T : unmanaged
        where TPredicate : struct, IWhereIndex<T>
        where TPrevEnumerator : struct, IRefEnumerator<T>
        where TPrevEnumerable : struct, IRefEnumerable<TPrevEnumerator, T>
    {
        internal TPrevEnumerable Enumerable;
        internal TPredicate Predication;

        public WhereIndexEnumerable(in TPrevEnumerable enumerable, in TPredicate predication)
        {
            Enumerable = enumerable;
            Predication = predication;
        }

        public struct Enumerator : IRefEnumerator<T>
        {
            private TPrevEnumerator enumerator;
            private TPredicate predicts;
            private long index;

            internal Enumerator(in TPrevEnumerator enumerator, TPredicate predicts)
            {
                this.enumerator = enumerator;
                this.predicts = predicts;
                index = -1;
            }

            public bool MoveNext()
            {
                while (enumerator.MoveNext())
                    if (predicts.Calc(ref enumerator.Current, ++index))
                        return true;
                return false;
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
                    if (!success || predicts.Calc(ref value, ++index))
                        return ref value;
                }
            }

            public bool TryMoveNext(out T value)
            {
                while (enumerator.TryMoveNext(out value))
                    if (predicts.Calc(ref value, ++index))
                        return true;
                return false;
            }
        }

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator() => new Enumerator(Enumerable.GetEnumerator(), Predication);

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