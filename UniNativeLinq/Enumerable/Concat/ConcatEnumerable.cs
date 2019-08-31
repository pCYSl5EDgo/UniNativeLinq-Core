using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UniNativeLinq
{
    public unsafe struct
        ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, T>
        : IRefEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, T>.Enumerator, T>
        where T : unmanaged
        where TFirstEnumerable : struct, IRefEnumerable<TFirstEnumerator, T>
        where TFirstEnumerator : struct, IRefEnumerator<T>
        where TSecondEnumerable : struct, IRefEnumerable<TSecondEnumerator, T>
        where TSecondEnumerator : struct, IRefEnumerator<T>
    {
        public TFirstEnumerable FirstEnumerable;
        public TSecondEnumerable SecondEnumerable;
        public bool CanIndexAccess() => FirstEnumerable.CanIndexAccess() && SecondEnumerable.CanIndexAccess();

        public ref T this[long index]
        {
            get
            {
                var firstCount = FirstEnumerable.LongCount();
                if (index < firstCount)
                    return ref FirstEnumerable[index];
                return ref SecondEnumerable[index - firstCount];
            }
        }
        public ConcatEnumerable(in TFirstEnumerable firstEnumerable, in TSecondEnumerable secondEnumerable)
        {
            FirstEnumerable = firstEnumerable;
            SecondEnumerable = secondEnumerable;
        }

        public Enumerator GetEnumerator() => new Enumerator(FirstEnumerable.GetEnumerator(), SecondEnumerable.GetEnumerator());

        public struct Enumerator : IRefEnumerator<T>
        {
            private TFirstEnumerator firstEnumerator;
            private TSecondEnumerator secondEnumerator;
            private bool isCurrentSecond;

            public ref T Current => ref (isCurrentSecond ? ref secondEnumerator.Current : ref firstEnumerator.Current);
            T IEnumerator<T>.Current => Current;
            object IEnumerator.Current => Current;

            internal Enumerator(in TFirstEnumerator firstEnumerator, in TSecondEnumerator secondEnumerator)
            {
                this.firstEnumerator = firstEnumerator;
                this.secondEnumerator = secondEnumerator;
                isCurrentSecond = false;
            }

            public void Dispose()
            {
                firstEnumerator.Dispose();
                secondEnumerator.Dispose();
            }

            public bool MoveNext()
            {
                if (isCurrentSecond) return secondEnumerator.MoveNext();
                if (firstEnumerator.MoveNext()) return true;
                isCurrentSecond = true;
                return secondEnumerator.MoveNext();
            }

            public void Reset() => throw new InvalidOperationException();

            public ref T TryGetNext(out bool success)
            {
                if (isCurrentSecond)
                    return ref secondEnumerator.TryGetNext(out success);
                ref var value = ref firstEnumerator.TryGetNext(out success);
                if (success) return ref value;
                isCurrentSecond = true;
                return ref secondEnumerator.TryGetNext(out success);
            }

            public bool TryMoveNext(out T value)
            {
                if (isCurrentSecond) return secondEnumerator.TryMoveNext(out value);
                return firstEnumerator.TryMoveNext(out value) || (isCurrentSecond = true && secondEnumerator.TryMoveNext(out value));
            }
        }

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFastCount() => FirstEnumerable.CanFastCount() && SecondEnumerable.CanFastCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Any() => FirstEnumerable.Any() || SecondEnumerable.Any();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count()
            => (int)LongCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long LongCount() => FirstEnumerable.LongCount() + SecondEnumerable.LongCount();

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