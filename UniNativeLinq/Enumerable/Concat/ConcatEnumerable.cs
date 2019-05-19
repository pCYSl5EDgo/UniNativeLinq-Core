using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UniNativeLinq
{
    public unsafe partial struct
        ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>
        : IRefEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>.Enumerator, TSource>
        where TSource : unmanaged
        where TFirstEnumerable : struct, IRefEnumerable<TFirstEnumerator, TSource>
        where TFirstEnumerator : struct, IRefEnumerator<TSource>
        where TSecondEnumerable : struct, IRefEnumerable<TSecondEnumerator, TSource>
        where TSecondEnumerator : struct, IRefEnumerator<TSource>
    {
        public TFirstEnumerable FirstEnumerable;
        public TSecondEnumerable SecondEnumerable;

        public ConcatEnumerable(in TFirstEnumerable firstEnumerable, in TSecondEnumerable secondEnumerable)
        {
            FirstEnumerable = firstEnumerable;
            SecondEnumerable = secondEnumerable;
        }

        public readonly Enumerator GetEnumerator() => new Enumerator(FirstEnumerable.GetEnumerator(), SecondEnumerable.GetEnumerator());

        public struct Enumerator : IRefEnumerator<TSource>
        {
            private TFirstEnumerator firstEnumerator;
            private TSecondEnumerator secondEnumerator;
            private bool isCurrentSecond;

            public ref TSource Current => ref (isCurrentSecond ? ref secondEnumerator.Current : ref firstEnumerator.Current);
            TSource IEnumerator<TSource>.Current => Current;
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

            public ref TSource TryGetNext(out bool success)
            {
                if (isCurrentSecond)
                    return ref secondEnumerator.TryGetNext(out success);
                ref var value = ref firstEnumerator.TryGetNext(out success);
                if (success) return ref value;
                isCurrentSecond = true;
                return ref secondEnumerator.TryGetNext(out success);
            }

            public bool TryMoveNext(out TSource value)
            {
                if (isCurrentSecond) return secondEnumerator.TryMoveNext(out value);
                return firstEnumerator.TryMoveNext(out value) || (isCurrentSecond = true && secondEnumerator.TryMoveNext(out value));
            }
        }

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool CanFastCount() => FirstEnumerable.CanFastCount() && SecondEnumerable.CanFastCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Any() => FirstEnumerable.Any() || SecondEnumerable.Any();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int Count()
            => (int)LongCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly long LongCount() => FirstEnumerable.LongCount() + SecondEnumerable.LongCount();

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
            if(count == 0) return Array.Empty<TSource>();
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
            if(count == 0) return default;
            var answer = new NativeArray<TSource>(count, allocator, NativeArrayOptions.UninitializedMemory);
            CopyTo(answer.GetPointer());
            return answer;
        }
        #endregion
    }
}