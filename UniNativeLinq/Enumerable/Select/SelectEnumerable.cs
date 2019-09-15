using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UniNativeLinq
{
    public unsafe struct
        SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrev, T, TAction>
        : IRefEnumerable<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrev, T, TAction>.Enumerator, T>
        where TPrev : unmanaged
        where TPrevEnumerable : struct, IRefEnumerable<TPrevEnumerator, TPrev>
        where TPrevEnumerator : struct, IRefEnumerator<TPrev>
        where T : unmanaged
        where TAction : struct, IRefAction<TPrev, T>
    {
        private TPrevEnumerable enumerable;
        private TAction action;

        public bool CanIndexAccess() => false;

        public ref T this[long index] => throw new NotSupportedException();

        public SelectEnumerable(in TPrevEnumerable enumerable, in TAction action)
        {
            this.enumerable = enumerable;
            this.action = action;
        }

        public Enumerator GetEnumerator() => new Enumerator(enumerable.GetEnumerator(), action);

        [LocalRefReturn]
        public struct Enumerator : IRefEnumerator<T>
        {
            private TPrevEnumerator enumerator;
            private T element;
            private TAction action;

            internal Enumerator(in TPrevEnumerator enumerator, in TAction action)
            {
                this.enumerator = enumerator;
                element = default;
                this.action = action;
            }

            public ref T Current => throw new NotImplementedException();
            T IEnumerator<T>.Current => Current;
            object IEnumerator.Current => Current;

            public void Dispose()
            {
                enumerator.Dispose();
            }

            public bool MoveNext()
            {
                if (!enumerator.MoveNext()) return false;
                action.Execute(ref enumerator.Current, ref element);
                return true;
            }

            public void Reset() => throw new InvalidOperationException();

            public ref T TryGetNext(out bool success)
            {
                ref var value = ref enumerator.TryGetNext(out success);
                if (!success) return ref Pseudo.AsRefNull<T>();
                action.Execute(ref value, ref element);
                throw new NotImplementedException();
            }

            public bool TryMoveNext(out T value)
            {
                if(!enumerator.TryMoveNext(out var prevValue))
                {
                    value = default;
                    return false;
                }
                action.Execute(ref prevValue, ref element);
                value = element;
                return true;
            }
        }

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFastCount() => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Any() => enumerable.Any();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count() => enumerable.Count();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long LongCount() => enumerable.LongCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long CopyTo(T* dest)
        {
            var enumerator = GetEnumerator();
            long answer = 0;
            while (enumerator.MoveNext())
            {
                *dest++ = enumerator.Current;
                answer++;
            }
            enumerator.Dispose();
            return answer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray()
        {
            var count = LongCount();
            if (count == 0) return Array.Empty<T>();
            var answer = new T[count];
            CopyTo(Pseudo.AsPointer(ref answer[0]));
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