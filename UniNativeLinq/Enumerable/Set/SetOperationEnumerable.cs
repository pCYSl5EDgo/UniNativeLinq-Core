using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    public unsafe readonly partial struct
        SetOperationEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource, TSetOperation>
        : IRefEnumerable<SetOperationEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource, TSetOperation>.Enumerator, TSource>
        where TSource : unmanaged
        where TFirstEnumerable : struct, IRefEnumerable<TFirstEnumerator, TSource>
        where TFirstEnumerator : struct, IRefEnumerator<TSource>
        where TSecondEnumerable : struct, IRefEnumerable<TSecondEnumerator, TSource>
        where TSecondEnumerator : struct, IRefEnumerator<TSource>
        where TSetOperation : struct, ISetOperation<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>
    {
        private readonly TFirstEnumerable firstEnumerable;
        private readonly TSecondEnumerable secondEnumerable;
        private readonly Allocator alloc;

        private readonly TSetOperation setOperation;

        public SetOperationEnumerable(in TFirstEnumerable firstEnumerable, in TSecondEnumerable secondEnumerable, TSetOperation setOperation, Allocator alloc)
        {
            this.firstEnumerable = firstEnumerable;
            this.secondEnumerable = secondEnumerable;
            this.setOperation = setOperation;
            this.alloc = alloc;
        }

        public struct Enumerator : IRefEnumerator<TSource>
        {
            private NativeEnumerable<TSource>.Enumerator mergedEnumerator;
            private readonly Allocator allocator;

            public Enumerator(in TFirstEnumerable firstEnumerable, in TSecondEnumerable secondEnumerable, TSetOperation setOperation, Allocator allocator)
            {
                this.allocator = allocator;
                mergedEnumerator = setOperation.Calc(ref Unsafe.AsRef(firstEnumerable), ref Unsafe.AsRef(secondEnumerable), allocator).GetEnumerator();
            }

            public bool MoveNext() => mergedEnumerator.MoveNext();
            public void Reset() => throw new InvalidOperationException();
            public readonly ref TSource Current => ref mergedEnumerator.Current;
            readonly TSource IEnumerator<TSource>.Current => Current;
            readonly object IEnumerator.Current => Current;

            public void Dispose()
            {
                if (mergedEnumerator.Ptr != null && UnsafeUtility.IsValidAllocator(allocator))
                    UnsafeUtility.Free(mergedEnumerator.Ptr, allocator);
                this = default;
            }

            public ref TSource TryGetNext(out bool success) => ref mergedEnumerator.TryGetNext(out success);

            public bool TryMoveNext(out TSource value) => mergedEnumerator.TryMoveNext(out value);
        }

        public readonly Enumerator GetEnumerator() => new Enumerator(firstEnumerable, secondEnumerable, setOperation, alloc);

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();

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
