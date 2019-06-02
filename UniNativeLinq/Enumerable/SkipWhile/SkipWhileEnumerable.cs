using System;
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

            public void Dispose()
            {
                enumerator.Dispose();
                this = default;
            }

            public bool MoveNext()
            {
                if(isFirstNotRead)
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
                if(isFirstNotRead)
                {
                    isFirstNotRead = false;
                    success = true;
                    return ref enumerator.Current;
                }
                return ref enumerator.TryGetNext(out success);
            }

            public bool TryMoveNext(out T value)
            {
                if(isFirstNotRead)
                {
                    isFirstNotRead = false;
                    value = enumerator.Current;
                    return true;
                }
                return enumerator.TryMoveNext(out value);
            }
        }

        public readonly Enumerator GetEnumerator()
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
        readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool CanFastCount() => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Any()
        {
            var enumerator = GetEnumerator();
            enumerator.TryGetNext(out var success);
            enumerator.Dispose();
            return success;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int Count()
            => (int)LongCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly long LongCount()
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
        public readonly void CopyTo(T* dest)
        {
            var enumerator = GetEnumerator();
            bool success;
            while (true)
            {
                ref var value = ref enumerator.TryGetNext(out success);
                if (!success) break;
                *dest++ = value;
            }
            enumerator.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T[] ToArray()
        {
            var count = LongCount();
            if (count == 0) return Array.Empty<T>();
            var answer = new T[count];
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
            CopyTo(UnsafeUtilityEx.GetPointer(answer));
            return answer;
        }
        #endregion
    }
}
