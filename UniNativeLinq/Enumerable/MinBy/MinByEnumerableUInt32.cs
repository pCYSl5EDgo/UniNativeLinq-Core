using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    [PseudoIsReadOnly]
    public unsafe struct
        MinByEnumerableUInt32<TEnumerable, TEnumerator, T, TKeySelector>
        : IRefEnumerable<MinByEnumerableUInt32<TEnumerable, TEnumerator, T, TKeySelector>.Enumerator, T>
        where T : unmanaged
        where TKeySelector : struct, IRefFunc<T, uint>
        where TEnumerator : struct, IRefEnumerator<T>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, T>
    {
        [PseudoIsReadOnly] private TEnumerable enumerable;
        [PseudoIsReadOnly] private TKeySelector keySelector;
        private readonly Allocator alloc;

        public MinByEnumerableUInt32(in TEnumerable enumerable, in TKeySelector keySelector, Allocator allocator)
        {
            this.enumerable = enumerable;
            this.keySelector = keySelector;
            alloc = allocator;
        }

        public struct Enumerator : IRefEnumerator<T>
        {
            internal NativeEnumerable<T>.Enumerator NativeEnumerator;
            private readonly Allocator allocator;

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
                    else if (value < target)
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

            readonly ref T IRefEnumerator<T>.Current => ref NativeEnumerator.Current;
            readonly T IEnumerator<T>.Current => NativeEnumerator.Current;
            readonly object IEnumerator.Current => ((IEnumerator)NativeEnumerator).Current;

            public void Dispose()
            {
                if (NativeEnumerator.Ptr == null || !UnsafeUtility.IsValidAllocator(allocator)) return;
                UnsafeUtility.Free(NativeEnumerator.Ptr, allocator);
                this = default;
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

        public void CopyTo(T* destination)
        {
            var enumerator = GetEnumerator();
            ref var nativeEnumerator = ref enumerator.NativeEnumerator;
            UnsafeUtilityEx.MemCpy(destination, nativeEnumerator.Ptr, nativeEnumerator.Length);
            enumerator.Dispose();
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
                return new NativeEnumerable<T>(nativeEnumerator.Ptr, length);
            var ptr = UnsafeUtilityEx.Malloc<T>(length, allocator);
            UnsafeUtilityEx.MemCpy(ptr, nativeEnumerator.Ptr, length);
            return new NativeEnumerable<T>(ptr, length);
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
