using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;

namespace UniNativeLinq
{
    [SlowCount]
    public unsafe struct
        TakeWhileEnumerable<TEnumerable, TEnumerator, T, TPredicate>
        : IRefEnumerable<TakeWhileEnumerable<TEnumerable, TEnumerator, T, TPredicate>.Enumerator, T>
        where T : unmanaged
        where TEnumerator : struct, IRefEnumerator<T>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, T>
        where TPredicate : struct, IRefFunc<T, bool>
    {
        private TEnumerable enumerable;
        private TPredicate predicate;

        public bool CanIndexAccess() => false;

        public ref T this[long index] => throw new NotSupportedException();

        public TakeWhileEnumerable(in TEnumerable enumerable, in TPredicate predicate)
        {
            this.enumerable = enumerable;
            this.predicate = predicate;
        }

        public struct Enumerator : IRefEnumerator<T>
        {
            private TEnumerator enumerator;
            private TPredicate predicate;

            internal Enumerator(in TEnumerator enumerator, in TPredicate predicate)
            {
                this.enumerator = enumerator;
                this.predicate = predicate;
            }

            public ref T Current => ref enumerator.Current;
            T IEnumerator<T>.Current => Current;
            object IEnumerator.Current => Current;

            public void Dispose()
            {
                enumerator.Dispose();
                this = default;
            }

            public bool MoveNext() => enumerator.MoveNext() && predicate.Calc(ref enumerator.Current);
            public void Reset() => throw new InvalidOperationException();

            public ref T TryGetNext(out bool success)
            {
                ref var value = ref enumerator.TryGetNext(out success);
                if (!success) return ref value;
                success = predicate.Calc(ref value);
                return ref value;
            }

            public bool TryMoveNext(out T value)
                => enumerator.TryMoveNext(out value) && predicate.Calc(ref value);
        }

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator() => new Enumerator(enumerable.GetEnumerator(), predicate);

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
