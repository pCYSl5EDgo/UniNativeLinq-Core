using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    public unsafe struct
        MaxByUInt64Enumerable<TEnumerable, TEnumerator, T, TKeySelector>
        : IRefEnumerable<MaxByUInt64Enumerable<TEnumerable, TEnumerator, T, TKeySelector>.Enumerator, T>
        where T : unmanaged
        where TKeySelector : struct, IRefFunc<T, ulong>
        where TEnumerator : struct, IRefEnumerator<T>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, T>
    {
        private TEnumerable enumerable;
        private TKeySelector keySelector;
        private Allocator alloc;
        public bool CanIndexAccess() => false;
        public ref T this[long index] => throw new NotSupportedException();
        public MaxByUInt64Enumerable(in TEnumerable enumerable, in TKeySelector keySelector, Allocator allocator)
        {
            this.enumerable = enumerable;
            this.keySelector = keySelector;
            alloc = allocator;
        }

        public struct Enumerator : IRefEnumerator<T>
        {
            internal NativeEnumerable<T>.Enumerator NativeEnumerator;
            private Allocator allocator;

            internal Enumerator(ref TEnumerable enumerable, ref TKeySelector keySelector, Allocator allocator)
            {
                this.allocator = allocator;
                var refEnumerator = enumerable.GetEnumerator();
                ref var current = ref refEnumerator.TryGetNext(out var success);
                if (!success)
                {
                    refEnumerator.Dispose();
                    this = default;
                    return;
                }
                var value = keySelector.Calc(ref current);
                var target = value;
                var list = new NativeList<T>(allocator);
                list.Add(in current);
                while (true)
                {
                    current = ref refEnumerator.TryGetNext(out success);
                    if (!success)
                    {
                        NativeEnumerator = list.AsNativeEnumerable().GetEnumerator();
                        refEnumerator.Dispose();
                        return;
                    }
                    value = keySelector.Calc(ref current);
                    if (value == target)
                    {
                        list.Add(current);
                    }
                    else if (value > target)
                    {
                        target = value;
                        list.Clear();
                        list.Add(current);
                    }
                }
            }

            public bool MoveNext() => NativeEnumerator.MoveNext();

            public void Reset() => NativeEnumerator.Reset();

            public ref T TryGetNext(out bool success) => ref NativeEnumerator.TryGetNext(out success);
            public bool TryMoveNext(out T value) => NativeEnumerator.TryMoveNext(out value);

            public ref T Current => ref NativeEnumerator.Current;
            T IEnumerator<T>.Current => NativeEnumerator.Current;
            object IEnumerator.Current => ((IEnumerator)NativeEnumerator).Current;

            public void Dispose()
            {
                if (NativeEnumerator.Ptr == null || !UnsafeUtility.IsValidAllocator(allocator)) return;
                UnsafeUtility.Free(NativeEnumerator.Ptr, allocator);
            }
        }

        public Enumerator GetEnumerator() => new Enumerator(ref enumerable, ref keySelector, alloc);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        public bool CanFastCount() => false;

        public bool Any() => enumerable.Any();

        public int Count() => (int)LongCount();

        public long LongCount()
        {
            var enumerator = GetEnumerator();
            var count = enumerator.NativeEnumerator.Length;
            enumerator.Dispose();
            return count;
        }

        public long CopyTo(T* destination)
        {
            var enumerator = GetEnumerator();
            ref var nativeEnumerator = ref enumerator.NativeEnumerator;
            var answer = nativeEnumerator.Length;
            UnsafeUtilityEx.MemCpy(destination, nativeEnumerator.Ptr, answer);
            enumerator.Dispose();
            return answer;
        }

        public NativeEnumerable<T> ToNativeEnumerable(Allocator allocator)
        {
            var enumerator = GetEnumerator();
            ref var nativeEnumerator = ref enumerator.NativeEnumerator;
            var length = nativeEnumerator.Length;
            if (length == 0)
            {
                enumerator.Dispose();
                return default;
            }
            if (alloc == allocator)
                return NativeEnumerable<T>.Create(nativeEnumerator.Ptr,  length);
            var ptr = UnsafeUtilityEx.Malloc<T>(length, allocator);
            UnsafeUtilityEx.MemCpy(ptr, nativeEnumerator.Ptr, length);
            return NativeEnumerable<T>.Create(ptr, length);
        }

        public NativeArray<T> ToNativeArray(Allocator allocator)
        {
            var enumerator = GetEnumerator();
            ref var nativeEnumerator = ref enumerator.NativeEnumerator;
            var length = (int)nativeEnumerator.Length;
            if (length == 0)
            {
                enumerator.Dispose();
                return default;
            }
            var answer = new NativeArray<T>(length, allocator, NativeArrayOptions.UninitializedMemory);
            UnsafeUtilityEx.MemCpy(answer.GetPointer(), nativeEnumerator.Ptr, length);
            enumerator.Dispose();
            return answer;
        }

        public T[] ToArray()
        {
            var enumerator = GetEnumerator();
            ref var nativeEnumerator = ref enumerator.NativeEnumerator;
            if (nativeEnumerator.Length == 0)
            {
                enumerator.Dispose();
                return Array.Empty<T>();
            }
            var answer = new T[nativeEnumerator.Length];
            UnsafeUtilityEx.MemCpy(Pseudo.AsPointer(ref answer[0]), nativeEnumerator.Ptr, nativeEnumerator.Length);
            enumerator.Dispose();
            return answer;
        }
    }
}
