using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UniNativeLinq
{
    public readonly unsafe struct
        RangeRepeatEnumerable<T, TAction>
        : IRefEnumerable<RangeRepeatEnumerable<T, TAction>.Enumerator, T>
        where T : unmanaged
        where TAction : struct, IRangeRepeat<T>
    {
        private readonly T start;
        private readonly long length;
        private readonly TAction acts;

        public RangeRepeatEnumerable(T start, long length, TAction acts)
        {
            this.start = start;
            this.length = length;
            this.acts = acts;
        }

        [LocalRefReturn]
        public struct Enumerator : IRefEnumerator<T>
        {
            private T element;
            private readonly long count;
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
            public readonly ref T Current => throw new NotImplementedException();
            readonly T IEnumerator<T>.Current => Current;
            readonly object IEnumerator.Current => Current;
            public void Dispose() => this = default;

            public ref T TryGetNext(out bool success)
            {
                success = ++index >= count;
                if (!success)
                {
                    index = count;
                    return ref Unsafe.AsRef<T>(null);
                }
                if (index > 0)
                    action.Execute(ref element);
                throw new NotImplementedException();
            }

            public bool TryMoveNext(out T value)
            {
                if(++index < count)
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

        public readonly Enumerator GetEnumerator() => new Enumerator(start, length, acts);

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool CanFastCount() => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Any() => length != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int Count() => (int)length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly long LongCount() => length;

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