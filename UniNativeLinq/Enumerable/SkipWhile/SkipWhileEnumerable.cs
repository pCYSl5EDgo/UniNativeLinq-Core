﻿using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;

namespace UniNativeLinq
{
    [SlowCount]
    public unsafe struct
        SkipWhileEnumerable<TEnumerable, TEnumerator, T, TPredicate>
        : IRefEnumerable<SkipWhileEnumerable<TEnumerable, TEnumerator, T, TPredicate>.Enumerator, T>
        where T : unmanaged
        where TEnumerator : struct, IRefEnumerator<T>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, T>
        where TPredicate : struct, IRefFunc<T, bool>
    {
        private TEnumerable enumerable;
        private TPredicate predicate;

        public bool CanIndexAccess() => false;

        public ref T this[long index] => throw new NotSupportedException();

        public SkipWhileEnumerable(in TEnumerable enumerable, in TPredicate predicate)
        {
            this.enumerable = enumerable;
            this.predicate = predicate;
        }

        public struct Enumerator : IRefEnumerator<T>
        {
            private TEnumerator enumerator;
            private bool isFirstNotRead;
            public ref T Current => ref enumerator.Current;
            T IEnumerator<T>.Current => Current;
            object IEnumerator.Current => Current;

            internal Enumerator(in TEnumerator enumerator)
            {
                isFirstNotRead = true;
                this.enumerator = enumerator;
            }

            public void Dispose() => enumerator.Dispose();

            public bool MoveNext()
            {
                if (isFirstNotRead)
                {
                    isFirstNotRead = false;
                    return true;
                }
                return enumerator.MoveNext();
            }

            public void Reset()
            {
                isFirstNotRead = true;
                enumerator.Reset();
            }

            public ref T TryGetNext(out bool success)
            {
                if (isFirstNotRead)
                {
                    isFirstNotRead = false;
                    success = true;
                    return ref enumerator.Current;
                }
                return ref enumerator.TryGetNext(out success);
            }

            public bool TryMoveNext(out T value)
            {
                if (isFirstNotRead)
                {
                    isFirstNotRead = false;
                    value = enumerator.Current;
                    return true;
                }
                return enumerator.TryMoveNext(out value);
            }
        }

        public Enumerator GetEnumerator()
        {
            var enumerator = enumerable.GetEnumerator();
            while (true)
            {
                ref var current = ref enumerator.TryGetNext(out var success);
                if (!success)
                {
                    enumerator.Dispose();
                    return default;
                }
                if (predicate.Calc(ref current)) continue;
                return new Enumerator(enumerator);
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
            enumerator.TryGetNext(out var success);
            enumerator.Dispose();
            return success;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count()
            => (int)LongCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long LongCount()
        {
            var count = 0L;
            var enumerator = GetEnumerator();
            bool success;
            while (true)
            {
                enumerator.TryGetNext(out success);
                if (!success) break;
                ++count;
            }
            enumerator.Dispose();
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long CopyTo(T* dest)
        {
            var enumerator = GetEnumerator();
            long answer = 0;
            while (true)
            {
                ref var value = ref enumerator.TryGetNext(out var success);
                if (!success) break;
                *dest++ = value;
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
    }
}
