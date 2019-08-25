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

        public SetOperationEnumerable(in TFirstEnumerable firstEnumerable, in TSecondEnumerable secondEnumerable, in TSetOperation setOperation, Allocator allocator)
        {
            this.firstEnumerable = firstEnumerable;
            this.secondEnumerable = secondEnumerable;
            this.setOperation = setOperation;
            alloc = allocator;
        }

        public SetOperationEnumerable(in TFirstEnumerable firstEnumerable, in TSecondEnumerable secondEnumerable, Allocator allocator)
        {
            this.firstEnumerable = firstEnumerable;
            this.secondEnumerable = secondEnumerable;
            setOperation = default;
            alloc = allocator;
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
            public ref T Current => ref mergedEnumerator.Current;
            T IEnumerator<T>.Current => Current;
            object IEnumerator.Current => Current;

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
        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        public bool CanFastCount() => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        public int Count()
            => (int)LongCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        public long LongCount()
        {
            var enumerator = GetEnumerator();
            var count = 0L;
            while (enumerator.MoveNext())
                ++count;
            enumerator.Dispose();
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        public void CopyTo(T* dest)
        {
            var enumerator = GetEnumerator();
            while (enumerator.MoveNext())
                *dest++ = enumerator.Current;
            enumerator.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        public T[] ToArray()
        {
            var count = LongCount();
            if (count == 0) return Array.Empty<T>();
            var answer = new T[count];
            CopyTo(Pseudo.AsPointer(ref answer[0]));
            return answer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        public NativeEnumerable<T> ToNativeEnumerable(Allocator allocator)
        {
            var count = LongCount();
            var ptr = UnsafeUtilityEx.Malloc<T>(count, allocator);
            CopyTo(ptr);
            return NativeEnumerable<T>.Create(ptr, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        public NativeArray<T> ToNativeArray(Allocator allocator)
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
