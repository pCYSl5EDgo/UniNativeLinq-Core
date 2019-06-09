using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    [SlowCount]
    [PseudoIsReadOnly]
    public unsafe struct
        SetOperationEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, T, TSetOperation>
        : IRefEnumerable<SetOperationEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, T, TSetOperation>.Enumerator, T>
        where T : unmanaged
        where TFirstEnumerable : struct, IRefEnumerable<TFirstEnumerator, T>
        where TFirstEnumerator : struct, IRefEnumerator<T>
        where TSecondEnumerable : struct, IRefEnumerable<TSecondEnumerator, T>
        where TSecondEnumerator : struct, IRefEnumerator<T>
        where TSetOperation : struct, ISetOperation<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, T>
    {
        [PseudoIsReadOnly] private TFirstEnumerable firstEnumerable;
        [PseudoIsReadOnly] private TSecondEnumerable secondEnumerable;
        private readonly Allocator alloc;

        [PseudoIsReadOnly] private TSetOperation setOperation;

        public SetOperationEnumerable(in TFirstEnumerable firstEnumerable, in TSecondEnumerable secondEnumerable, in TSetOperation setOperation, Allocator alloc)
        {
            this.firstEnumerable = firstEnumerable;
            this.secondEnumerable = secondEnumerable;
            this.setOperation = setOperation;
            this.alloc = alloc;
        }

        public struct Enumerator : IRefEnumerator<T>
        {
            private NativeEnumerable<T>.Enumerator mergedEnumerator;
            private readonly Allocator allocator;

            public Enumerator([PseudoIsReadOnly]ref TFirstEnumerable firstEnumerable, [PseudoIsReadOnly]ref TSecondEnumerable secondEnumerable, [PseudoIsReadOnly]ref TSetOperation setOperation, Allocator allocator)
            {
                this.allocator = allocator;
                mergedEnumerator = setOperation.Calc(ref firstEnumerable, ref secondEnumerable, allocator).GetEnumerator();
            }

            public bool MoveNext() => mergedEnumerator.MoveNext();
            public void Reset() => throw new InvalidOperationException();
            public readonly ref T Current => ref mergedEnumerator.Current;
            readonly T IEnumerator<T>.Current => Current;
            readonly object IEnumerator.Current => Current;

            public void Dispose()
            {
                if (mergedEnumerator.Ptr != null && UnsafeUtility.IsValidAllocator(allocator))
                    UnsafeUtility.Free(mergedEnumerator.Ptr, allocator);
                this = default;
            }

            public ref T TryGetNext(out bool success) => ref mergedEnumerator.TryGetNext(out success);

            public bool TryMoveNext(out T value) => mergedEnumerator.TryMoveNext(out value);
        }

        [PseudoIsReadOnly] public Enumerator GetEnumerator() => new Enumerator(ref firstEnumerable, ref secondEnumerable, ref setOperation, alloc);

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
