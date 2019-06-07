using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;

namespace UniNativeLinq
{
    public readonly unsafe struct
        ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirst, TSecondEnumerable, TSecondEnumerator, TSecond, T, TAction>
        : IRefEnumerable<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirst, TSecondEnumerable, TSecondEnumerator, TSecond, T, TAction>.Enumerator, T>
        where TFirst : unmanaged
        where TSecond : unmanaged
        where T : unmanaged
        where TFirstEnumerator : struct, IRefEnumerator<TFirst>
        where TFirstEnumerable : struct, IRefEnumerable<TFirstEnumerator, TFirst>
        where TSecondEnumerator : struct, IRefEnumerator<TSecond>
        where TSecondEnumerable : struct, IRefEnumerable<TSecondEnumerator, TSecond>
        where TAction : struct, IRefAction<TFirst, TSecond, T>
    {
        private readonly TFirstEnumerable firstCollection;
        private readonly TSecondEnumerable secondCollection;
        private readonly TAction acts;
        private readonly TFirst firstDefault;
        private readonly TSecond secondDefault;

        public ZipEnumerable(in TFirstEnumerable firstCollection, in TSecondEnumerable secondCollection, in TAction acts, in TFirst firstDefault, in TSecond secondDefault)
        {
            this.firstCollection = firstCollection;
            this.secondCollection = secondCollection;
            this.acts = acts;
            this.firstDefault = firstDefault;
            this.secondDefault = secondDefault;
        }

        [LocalRefReturn]
        public struct Enumerator : IRefEnumerator<T>
        {
            private TFirstEnumerator firstEnumerator;
            private TSecondEnumerator secondEnumerator;
            private T element;
            private TAction action;
            private TFirst firstDefaultValue;
            private TSecond secondDefaultValue;
            private bool isFirstAlive, isSecondAlive;

            internal Enumerator(in TFirstEnumerator firstEnumerator, in TSecondEnumerator secondEnumerator, in TAction action, TFirst firstDefaultValue, TSecond secondDefaultValue)
            {
                this.firstEnumerator = firstEnumerator;
                this.secondEnumerator = secondEnumerator;
                element = default;
                this.action = action;
                this.firstDefaultValue = firstDefaultValue;
                this.secondDefaultValue = secondDefaultValue;
                isFirstAlive = isSecondAlive = true;
            }

            public ref T TryGetNext(out bool success)
            {
                if (isFirstAlive && !firstEnumerator.MoveNext())
                    isFirstAlive = false;
                if (isSecondAlive && !secondEnumerator.MoveNext())
                    isSecondAlive = false;
                if (isFirstAlive)
                {
                    success = true;
                    if (isSecondAlive)
                        action.Execute(ref firstEnumerator.Current, ref secondEnumerator.Current, ref element);
                    else
                        action.Execute(ref firstEnumerator.Current, ref secondDefaultValue, ref element);
                }
                else
                {
                    if (isSecondAlive)
                    {
                        action.Execute(ref firstDefaultValue, ref secondEnumerator.Current, ref element);
                        success = true;
                    }
                    else
                    {
                        success = false;
                    }
                }
                throw new NotImplementedException();
            }

            public bool TryMoveNext(out T value)
            {
                if (isFirstAlive && !firstEnumerator.MoveNext())
                    isFirstAlive = false;
                if (isSecondAlive && !secondEnumerator.MoveNext())
                    isSecondAlive = false;
                if (isFirstAlive)
                {
                    if (isSecondAlive)
                        action.Execute(ref firstEnumerator.Current, ref secondEnumerator.Current, ref element);
                    else
                        action.Execute(ref firstEnumerator.Current, ref secondDefaultValue, ref element);
                    value = element;
                    return true;
                }
                else
                {
                    if (isSecondAlive)
                    {
                        action.Execute(ref firstDefaultValue, ref secondEnumerator.Current, ref element);
                        value = element;
                        return true;
                    }
                    else
                    {
                        value = element;
                        return false;
                    }
                }
            }

            public bool MoveNext()
            {
                if (isFirstAlive && !firstEnumerator.MoveNext())
                    isFirstAlive = false;
                if (isSecondAlive && !secondEnumerator.MoveNext())
                    isSecondAlive = false;
                if (isFirstAlive)
                {
                    if (isSecondAlive)
                        action.Execute(ref firstEnumerator.Current, ref secondEnumerator.Current, ref element);
                    else
                        action.Execute(ref firstEnumerator.Current, ref secondDefaultValue, ref element);
                }
                else
                {
                    if (isSecondAlive)
                        action.Execute(ref firstDefaultValue, ref secondEnumerator.Current, ref element);
                    else
                        return false;
                }
                return true;
            }

            public void Reset() => throw new InvalidOperationException();
            public readonly ref T Current => throw new NotImplementedException();
            readonly T IEnumerator<T>.Current => Current;
            readonly object IEnumerator.Current => Current;

            public void Dispose() => this = default;
        }

        public readonly Enumerator GetEnumerator() => new Enumerator(firstCollection.GetEnumerator(), secondCollection.GetEnumerator(), acts, firstDefault, secondDefault);
        
        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool CanFastCount() => firstCollection.CanFastCount() && secondCollection.CanFastCount();

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
            CopyTo(Psuedo.AsPointer<T>(ref answer[0]));
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