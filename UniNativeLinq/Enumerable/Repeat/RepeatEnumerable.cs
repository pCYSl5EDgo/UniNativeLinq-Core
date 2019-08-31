using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    public unsafe struct
        RepeatEnumerable<TEnumerable, TEnumerator, T>
        : IRefEnumerable<RepeatEnumerable<TEnumerable, TEnumerator, T>.Enumerator, T>
        where T : unmanaged
        where TEnumerator : struct, IRefEnumerator<T>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, T>
    {
        private TEnumerable enumerable;
        private long repeatCount;

        public RepeatEnumerable(in TEnumerable enumerable, long repeatCount)
        {
            this.enumerable = enumerable;
            this.repeatCount = repeatCount;
        }

        public struct Enumerator : IRefEnumerator<T>
        {
            private TEnumerator originalEnumerator;
            private TEnumerator enumerator;
            private long count;
            private long index;

            internal Enumerator(ref TEnumerable enumerable, long count)
            {
                this.count = count;
                if (count <= 0)
                {
                    this = default;
                    return;
                }
                originalEnumerator = enumerable.GetEnumerator();
                if (!originalEnumerator.MoveNext())
                {
                    originalEnumerator.Dispose();
                    this = default;
                    return;
                }
                index = -1;
                enumerator = originalEnumerator;
            }

            public bool MoveNext()
            {
                if (index < 0)
                {
                    index = 0;
                    return true;
                }
                if (enumerator.MoveNext())
                    return true;
                enumerator = originalEnumerator;
                return count > ++index;
            }

            public void Reset()
            {
                index = -1;
                enumerator = originalEnumerator;
            }

            public ref T Current => ref enumerator.Current;

            public ref T TryGetNext(out bool success)
            {
                if (index < 0)
                {
                    index = 0;
                    success = true;
                    return ref originalEnumerator.Current;
                }
                ref var current = ref enumerator.TryGetNext(out success);
                if (success)
                    return ref current;
                enumerator = originalEnumerator;
                success = count > ++index;
                return ref originalEnumerator.Current;
            }

            public bool TryMoveNext(out T value)
            {
                if (index < 0)
                {
                    index = 0;
                    value = originalEnumerator.Current;
                    return true;
                }
                if (enumerator.TryMoveNext(out value))
                    return true;
                enumerator = originalEnumerator;
                value = originalEnumerator.Current;
                return count > ++index;
            }

            T IEnumerator<T>.Current => Current;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                originalEnumerator.Dispose();
                this = default;
            }
        }

        public Enumerator GetEnumerator() => new Enumerator(ref enumerable, repeatCount);
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool CanFastCount() => repeatCount <= 0 || enumerable.CanFastCount();

        public bool Any() => repeatCount > 0 && enumerable.Any();

        public int Count() => (int)LongCount();

        public long LongCount() => repeatCount <= 0 ? 0 : repeatCount * enumerable.LongCount();

        private void CopyTo(T* destination, long count)
        {
            enumerable.CopyTo(destination);
            if (repeatCount == 1) return;
            var size = (int)count * sizeof(T);
            UnsafeUtility.MemCpyStride(destination + count, size, destination, 0, size, (int)repeatCount - 1);
        }

        public void CopyTo(T* destination)
        {
            if (repeatCount <= 0) return;
            enumerable.CopyTo(destination);
            if (repeatCount == 1) return;
            var count = enumerable.LongCount();
            var size = (int)count * sizeof(T);
            UnsafeUtility.MemCpyStride(destination + count, size, destination, 0, size, (int)repeatCount - 1);
        }

        public NativeEnumerable<T> ToNativeEnumerable(Allocator allocator)
        {
            if (repeatCount <= 0) return default;
            var count = enumerable.LongCount();
            var length = count * repeatCount;
            var answer = UnsafeUtilityEx.Malloc<T>(length, allocator);
            CopyTo(answer, count);
            return NativeEnumerable<T>.Create(answer, length);
        }

        public NativeArray<T> ToNativeArray(Allocator allocator)
        {
            if (repeatCount <= 0) return default;
            var count = enumerable.LongCount();
            var length = count * repeatCount;
            if (length > int.MaxValue) throw new IndexOutOfRangeException();
            var answer = new NativeArray<T>((int)length, allocator, NativeArrayOptions.UninitializedMemory);
            CopyTo(answer.GetPointer(), count);
            return answer;
        }

        public T[] ToArray()
        {
            if (repeatCount <= 0) return Array.Empty<T>();
            var count = enumerable.LongCount();
            var length = count * repeatCount;
            var answer = new T[length];
            CopyTo(Pseudo.AsPointer(ref answer[0]), count);
            return answer;
        }

        public bool CanIndexAccess() => false;

        public ref T this[long index] => throw new NotSupportedException();
    }
}
