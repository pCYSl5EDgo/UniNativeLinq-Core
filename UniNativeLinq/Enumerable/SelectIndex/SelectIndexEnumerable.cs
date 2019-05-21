using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UniNativeLinq
{
    public readonly unsafe struct
        SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrev, T, TAction>
        : IRefEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrev, T, TAction>.Enumerator, T>
        where TPrev : unmanaged
        where T : unmanaged
        where TAction : struct, ISelectIndex<TPrev, T>
        where TPrevEnumerator : struct, IRefEnumerator<TPrev>
        where TPrevEnumerable : struct, IRefEnumerable<TPrevEnumerator, TPrev>
    {
        private readonly TPrevEnumerable enumerable;
        private readonly TAction acts;

        public SelectIndexEnumerable(in TPrevEnumerable enumerable, in TAction acts)
        {
            this.enumerable = enumerable;
            this.acts = acts;
        }

        [LocalRefReturn]
        public struct Enumerator : IRefEnumerator<T>
        {
            private TPrevEnumerator enumerator;
            private T element;
            private TAction action;
            private long index;

            internal Enumerator(in TPrevEnumerator enumerator, in TAction action)
            {
                this.enumerator = enumerator;
                element =  default;
                this.action = action;
                index = -1;
            }

            public bool MoveNext()
            {
                ++index;
                if (!enumerator.MoveNext()) return false;
                action.Execute(ref enumerator.Current, index, ref element);
                return true;
            }

            public void Reset() => throw new InvalidOperationException();
            public readonly ref T Current => throw new NotImplementedException();
            readonly T IEnumerator<T>.Current => Current;
            readonly object IEnumerator.Current => Current;
            public void Dispose() => this = default;

            public ref T TryGetNext(out bool success)
            {
                ++index;
                ref var prevSource = ref enumerator.TryGetNext(out success);
                if (!success) return ref Unsafe.AsRef<T>(null);
                action.Execute(ref enumerator.Current, index, ref element);
                throw new NotImplementedException();
            }

            public bool TryMoveNext(out T value)
            {
                ++index;
                if(!enumerator.TryMoveNext(out var prevSource))
                {
                    value = default;
                    return false;
                }
                action.Execute(ref prevSource, index, ref element);
                value = element;
                return true;
            }
        }

        public readonly Enumerator GetEnumerator() => new Enumerator(enumerable.GetEnumerator(), acts);
        
        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool CanFastCount() => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Any() => enumerable.Any();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int Count() => enumerable.Count();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly long LongCount() => enumerable.LongCount();

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
            CopyTo((T*)Unsafe.AsPointer(ref answer[0]));
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