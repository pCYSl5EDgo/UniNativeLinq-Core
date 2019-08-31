using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;

namespace UniNativeLinq
{
    public unsafe struct
        AdjustedZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirst, TSecondEnumerable, TSecondEnumerator, TSecond, T, TAction>
        : IRefEnumerable<AdjustedZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirst, TSecondEnumerable, TSecondEnumerator, TSecond, T, TAction>.Enumerator, T>
        where TFirst : unmanaged
        where TSecond : unmanaged
        where T : unmanaged
        where TFirstEnumerator : struct, IRefEnumerator<TFirst>
        where TFirstEnumerable : struct, IRefEnumerable<TFirstEnumerator, TFirst>
        where TSecondEnumerator : struct, IRefEnumerator<TSecond>
        where TSecondEnumerable : struct, IRefEnumerable<TSecondEnumerator, TSecond>
        where TAction : struct, IRefAction<TFirst, TSecond, T>
    {
        private TFirstEnumerable firstCollection;
        private TSecondEnumerable secondCollection;
        private TAction action;
        public bool CanIndexAccess() => false;
        public ref T this[long index] => throw new NotSupportedException();
        public AdjustedZipEnumerable(in TFirstEnumerable firstCollection, in TSecondEnumerable secondCollection, in TAction action)
        {
            this.firstCollection = firstCollection;
            this.secondCollection = secondCollection;
            this.action = action;
        }

        public AdjustedZipEnumerable(in TFirstEnumerable firstCollection, in TSecondEnumerable secondCollection)
        {
            this.firstCollection = firstCollection;
            this.secondCollection = secondCollection;
            action = default;
        }

        [LocalRefReturn]
        public struct Enumerator : IRefEnumerator<T>
        {
            private TFirstEnumerator firstEnumerator;
            private TSecondEnumerator secondEnumerator;
            private T element;
            private TAction action;

            internal Enumerator(in TFirstEnumerator firstEnumerator, in TSecondEnumerator secondEnumerator, in TAction action)
            {
                this.firstEnumerator = firstEnumerator;
                this.secondEnumerator = secondEnumerator;
                element = default;
                this.action = action;
            }

            public ref T TryGetNext(out bool success)
            {
                ref var first = ref firstEnumerator.TryGetNext(out success);
                if (success)
                {
                    ref var second = ref secondEnumerator.TryGetNext(out success);
                    if (!success)
                    {
                        success = false;
                        return ref Pseudo.AsRefNull<T>();
                    }
                    action.Execute(ref first, ref second, ref element);
                }
                else
                {
                    if (secondEnumerator.MoveNext())
                    {
                        success = false;
                        return ref Pseudo.AsRefNull<T>();
                    }
                    success = false;
                }
                throw new NotImplementedException();
            }

            public bool TryMoveNext(out T value)
            {
                ref var first = ref firstEnumerator.TryGetNext(out var success0);
                ref var second = ref secondEnumerator.TryGetNext(out var success1);
                if (success0)
                {
                    if (!success1)
                    {
                        value = default;
                        return false;
                    }
                    action.Execute(ref first, ref second, ref element);
                    value = element;
                    return true;
                }
                if (success1)
                {
                    value = default;
                    return false;
                }
                value = default;
                return false;
            }

            public bool MoveNext()
            {
                ref var first = ref firstEnumerator.TryGetNext(out var success);
                if (!success)
                    return false;

                ref var second = ref secondEnumerator.TryGetNext(out success);
                if (!success)
                    return false;

                action.Execute(ref first, ref second, ref element);
                return true;
            }

            public void Reset() => throw new InvalidOperationException();
            public ref T Current => throw new NotImplementedException();
            T IEnumerator<T>.Current => Current;
            object IEnumerator.Current => Current;

            public void Dispose()
            {
                firstEnumerator.Dispose();
                secondEnumerator.Dispose();
                this = default;
            }
        }

        public Enumerator GetEnumerator() => new Enumerator(firstCollection.GetEnumerator(), secondCollection.GetEnumerator(), action);

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFastCount() => firstCollection.CanFastCount() && secondCollection.CanFastCount();

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