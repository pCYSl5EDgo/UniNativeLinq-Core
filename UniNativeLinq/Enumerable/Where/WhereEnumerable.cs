using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UniNativeLinq
{
    [SlowCount]
    public readonly unsafe struct
        WhereEnumerable<TPrevEnumerable, TPrevEnumerator, T, TPredicate>
        : IRefEnumerable<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, T, TPredicate>.Enumerator, T>
        where T : unmanaged
        where TPredicate : struct, IRefFunc<T, bool>
        where TPrevEnumerator : struct, IRefEnumerator<T>
        where TPrevEnumerable : struct, IRefEnumerable<TPrevEnumerator, T>
    {
        private readonly TPrevEnumerable enumerable;
        private readonly TPredicate predicts;

        public WhereEnumerable(in TPrevEnumerable enumerable, in TPredicate predicts)
        {
            this.enumerable = enumerable;
            this.predicts = predicts;
        }

        public readonly Enumerator GetEnumerator() => new Enumerator(enumerable.GetEnumerator(), predicts);

        public struct Enumerator : IRefEnumerator<T>
        {
            private TPrevEnumerator enumerator;
            private TPredicate predicts;

            internal Enumerator(in TPrevEnumerator enumerator, TPredicate predicts)
            {
                this.enumerator = enumerator;
                this.predicts = predicts;
            }

            public bool MoveNext()
            {
                while (enumerator.MoveNext())
                    if (predicts.Calc(ref enumerator.Current))
                        return true;
                return false;
            }

            public void Reset() => throw new InvalidOperationException();

            public readonly ref T Current => ref enumerator.Current;

            readonly T IEnumerator<T>.Current => Current;

            readonly object IEnumerator.Current => Current;

            public void Dispose() => enumerator.Dispose();

            public ref T TryGetNext(out bool success)
            {
                ref var value = ref enumerator.TryGetNext(out success);
                while (success)
                {
                    if (predicts.Calc(ref value))
                        return ref value;
                    value = ref enumerator.TryGetNext(out success);
                }
                return ref value;
            }

            public bool TryMoveNext(out T value)
            {
                while (enumerator.TryMoveNext(out value))
                    if (predicts.Calc(ref value))
                        return true;
                return false;
            }
        }

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool CanFastCount() => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Any()
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
        public readonly int Count()
            => (int)LongCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly long LongCount()
        {
            var enumerator = GetEnumerator();
            var count = 0L;
            while (enumerator.MoveNext())
                ++count;
            enumerator.Dispose();
            return count;
        }

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
            CopyTo(Pseudo.AsPointer<T>(ref answer[0]));
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
    }
}