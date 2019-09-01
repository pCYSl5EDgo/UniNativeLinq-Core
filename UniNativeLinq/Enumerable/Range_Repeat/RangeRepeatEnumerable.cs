using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UniNativeLinq
{
    [FastCount]
    public unsafe struct
        RangeRepeatEnumerable<T, TAction>
        : IRefEnumerable<RangeRepeatEnumerable<T, TAction>.Enumerator, T>
        where T : unmanaged
        where TAction : struct, IRangeRepeat<T>
    {
        private T start;
        // ReSharper disable once InconsistentNaming
        private long Length;
        private TAction acts;

        public bool CanIndexAccess() => false;

        ref T IRefEnumerable<Enumerator, T>.this[long index] => throw new NotSupportedException();

        public T this[long index]
        {
            get
            {
                var answer = start;
                acts.Execute(ref answer, index);
                return answer;
            }
        }

        public RangeRepeatEnumerable(in T start, long length, in TAction acts)
        {
            this.start = start;
            Length = length;
            this.acts = acts;
        }

        public RangeRepeatEnumerable(in T start, long length)
        {
            this.start = start;
            Length = length;
            acts = default;
        }

        [LocalRefReturn]
        public struct Enumerator : IRefEnumerator<T>
        {
            private T element;
            private long count;
            private long index;
            private TAction action;

            internal Enumerator(T start, long count, TAction action)
            {
                element = start;
                this.count = count;
                index = -1;
                this.action = action;
            }

            public bool MoveNext()
            {
                if (++index >= count) return false;
                if (index > 0) action.Execute(ref element);
                return true;
            }

            public void Reset()
            {
                if (index <= 0) return;
                action.Back(ref element, index);
                index = 0;
            }
            public ref T Current => throw new NotImplementedException();
            T IEnumerator<T>.Current => Current;
            object IEnumerator.Current => Current;
            public void Dispose() { }

            public ref T TryGetNext(out bool success)
            {
                success = ++index < count;
                if (success)
                {
                    if (index > 0)
                        action.Execute(ref element);
                    throw new NotImplementedException();
                }
                index = count;
                return ref Pseudo.AsRefNull<T>();
            }

            public bool TryMoveNext(out T value)
            {
                if (++index < count)
                {
                    if (index > 0)
                        action.Execute(ref element);
                    value = element;
                    return true;
                }
                value = default;
                return false;
            }
        }

        public Enumerator GetEnumerator() => new Enumerator(start, Length, acts);

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFastCount() => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Any() => Length != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count() => (int)Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long LongCount() => Length;

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