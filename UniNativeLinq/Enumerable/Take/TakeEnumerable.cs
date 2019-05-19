using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;

namespace UniNativeLinq
{
    public unsafe readonly partial struct
        TakeEnumerable<TEnumerable, TEnumerator, TSource>
        : IRefEnumerable<TakeEnumerable<TEnumerable, TEnumerator, TSource>.Enumerator, TSource>
        where TSource : unmanaged
        where TEnumerator : struct, IRefEnumerator<TSource>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
    {
        private readonly TEnumerable enumerable;
        private readonly long takeCount;
        public TakeEnumerable(in TEnumerable enumerable, long takeCount)
        {
            this.enumerable = enumerable;
            this.takeCount = takeCount;
        }

        public struct Enumerator : IRefEnumerator<TSource>
        {
            private TEnumerator enumerator;
            private long takeCount;

            internal Enumerator(in TEnumerator enumerator, long lastCount)
            {
                this.enumerator = enumerator;
                takeCount = lastCount;
            }

            public readonly ref TSource Current => ref enumerator.Current;
            readonly TSource IEnumerator<TSource>.Current => Current;
            readonly object IEnumerator.Current => Current;

            public void Dispose()
            {
                enumerator.Dispose();
                this = default;
            }

            public bool MoveNext() => --takeCount >= 0 && enumerator.MoveNext();
            public void Reset() => throw new InvalidOperationException();

            public ref TSource TryGetNext(out bool success)
            {
                success = --takeCount >= 0;
                if(!success)
                {
                    takeCount = 0;
                    return ref Unsafe.AsRef<TSource>(null);
                }
                return ref enumerator.TryGetNext(out success);
            }

            public bool TryMoveNext(out TSource value)
            {
                if(--takeCount >= 0)
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
        readonly IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();

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
        public readonly void CopyTo(TSource* dest)
        {
            var enumerator = GetEnumerator();
            while (enumerator.MoveNext())
                *dest++ = enumerator.Current;
            enumerator.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly TSource[] ToArray()
        {
            var count = LongCount();
            if (count == 0) return Array.Empty<TSource>();
            var answer = new TSource[count];
            CopyTo((TSource*)Unsafe.AsPointer(ref answer[0]));
            return answer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly NativeEnumerable<TSource> ToNativeEnumerable(Allocator allocator)
        {
            var count = LongCount();
            var ptr = UnsafeUtilityEx.Malloc<TSource>(count, allocator);
            CopyTo(ptr);
            return new NativeEnumerable<TSource>(ptr, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly NativeArray<TSource> ToNativeArray(Allocator allocator)
        {
            var count = Count();
            if (count == 0) return default;
            var answer = new NativeArray<TSource>(count, allocator, NativeArrayOptions.UninitializedMemory);
            CopyTo(UnsafeUtilityEx.GetPointer(answer));
            return answer;
        }
        #endregion
    }
}
