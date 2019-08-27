using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;

namespace UniNativeLinq
{
    public readonly unsafe struct
        TakeEnumerable<TEnumerable, TEnumerator, T>
        : IRefEnumerable<TakeEnumerable<TEnumerable, TEnumerator, T>.Enumerator, T>
        where T : unmanaged
        where TEnumerator : struct, IRefEnumerator<T>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, T>
    {
        private readonly TEnumerable enumerable;
        private readonly long takeCount;
        public TakeEnumerable(in TEnumerable enumerable, long takeCount)
        {
            this.enumerable = enumerable;
            this.takeCount = takeCount;
        }

        public struct Enumerator : IRefEnumerator<T>
        {
            private TEnumerator enumerator;
            private long takeCount;

            internal Enumerator(in TEnumerator enumerator, long lastCount)
            {
                this.enumerator = enumerator;
                takeCount = lastCount;
            }

            public readonly ref T Current => ref enumerator.Current;
            readonly T IEnumerator<T>.Current => Current;
            readonly object IEnumerator.Current => Current;

            public void Dispose()
            {
                enumerator.Dispose();
                this = default;
            }

            public bool MoveNext() => --takeCount >= 0 && enumerator.MoveNext();
            public void Reset() => throw new InvalidOperationException();

            public ref T TryGetNext(out bool success)
            {
                success = --takeCount >= 0;
                if (!success)
                {
                    takeCount = 0;
                    return ref Pseudo.AsRefNull<T>();
                }
                return ref enumerator.TryGetNext(out success);
            }

            public bool TryMoveNext(out T value)
            {
                if (--takeCount >= 0)
                    return enumerator.TryMoveNext(out value);
                takeCount = 0;
                value = default;
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator() => new Enumerator(enumerable.GetEnumerator(), takeCount);

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool CanFastCount() => enumerable.CanFastCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Any() => takeCount != 0 && enumerable.Any();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int Count()
            => (int)LongCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly long LongCount()
        {
            var count = enumerable.LongCount();
            return count > takeCount ? takeCount : count;
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

        public bool CanIndexAccess => enumerable.CanIndexAccess;

        public ref T this[long index] => ref enumerable[index];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly NativeEnumerable<T> ToNativeEnumerable(Allocator allocator)
        {
            var count = LongCount();
            var ptr = UnsafeUtilityEx.Malloc<T>(count, allocator);
            CopyTo(ptr);
            return NativeEnumerable<T>.Create(ptr, count);
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
