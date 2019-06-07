using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    [SlowCount]
    public unsafe struct
        DistinctEnumerable<TEnumerable, TEnumerator, T, TEqualityComparer, TGetHashCodeFunc>
        : IRefEnumerable<DistinctEnumerable<TEnumerable, TEnumerator, T, TEqualityComparer, TGetHashCodeFunc>.Enumerator, T>
        where T : unmanaged
        where TEqualityComparer : struct, IRefFunc<T, T, bool>
        where TGetHashCodeFunc : struct, IRefFunc<T, int>
        where TEnumerator : struct, IRefEnumerator<T>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, T>
    {
        private TEnumerable enumerable;
        private readonly TEqualityComparer equalityComparer;
        private readonly TGetHashCodeFunc getHashCodeFunc;
        private readonly Allocator alloc;

        public DistinctEnumerable(in TEnumerable enumerable, TEqualityComparer equalityComparer, TGetHashCodeFunc getHashCodeFunc, Allocator allocator)
        {
            this.enumerable = enumerable;
            this.equalityComparer = equalityComparer;
            this.getHashCodeFunc = getHashCodeFunc;
            alloc = allocator;
        }

        public struct Enumerator : IRefEnumerator<T>
        {
            private TEnumerator enumerator;
            private T* ptr;
            private int* codes;
            private long capacity;
            private long count;
            private readonly Allocator alloc;
            private TEqualityComparer comparer;
            private TGetHashCodeFunc getHashCodeFunc;
            private long currentIndex;

            public Enumerator([PsuedoIsReadOnly]ref TEnumerable enumerable, in TEqualityComparer comparer, in TGetHashCodeFunc getHashCodeFunc, Allocator allocator)
            {
                if (enumerable.CanFastCount())
                {
                    if ((capacity = enumerable.LongCount()) == 0)
                    {
                        this = default;
                        return;
                    }
                }
                else
                {
                    capacity = 16;
                }
                count = 0;
                enumerator = enumerable.GetEnumerator();
                alloc = allocator;
                ptr = UnsafeUtilityEx.Malloc<T>(capacity, alloc);
                codes = UnsafeUtilityEx.Malloc<int>(capacity, alloc);
                this.comparer = comparer;
                this.getHashCodeFunc = getHashCodeFunc;
                currentIndex = 0;
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private bool TryInsert(ref T current, int hash)
            {
                if (hash < codes[0])
                {
                    currentIndex = 0;
                    Insert(ref current, hash);
                    return true;
                }
                if (hash > codes[count - 1])
                {
                    currentIndex = count;
                    Insert(ref current, hash);
                    return true;
                }
                var minInclusive = 0L;
                var maxInclusive = count - 1;
                while (minInclusive < maxInclusive)
                {
                    currentIndex = (minInclusive + maxInclusive) >> 1;
                    if (hash == codes[currentIndex])
                        return TryInsertAround(ref current, hash, minInclusive, maxInclusive);
                    if (hash > codes[currentIndex])
                        minInclusive = currentIndex + 1;
                    else if (hash < codes[currentIndex])
                        maxInclusive = currentIndex - 1;
                }
                if (codes[minInclusive] == hash || !comparer.Calc(ref current, ref ptr[minInclusive])) return false;
                currentIndex = minInclusive;
                Insert(ref current, hash);
                return true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private bool TryInsertAround(ref T current, int hash, long minInclusive, long maxInclusive)
            {
                if (hash == codes[currentIndex] && comparer.Calc(ref current, ref ptr[currentIndex]))
                    return false;
                for (var i = currentIndex; --i >= minInclusive;)
                {
                    if (hash != codes[i]) break;
                    if (comparer.Calc(ref current, ref ptr[i])) return false;
                }
                for (; ++currentIndex <= maxInclusive;)
                {
                    if (hash != codes[currentIndex]) break;
                    if (comparer.Calc(ref current, ref ptr[currentIndex])) return false;
                }
                Insert(ref current, hash);
                return true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private bool InitialInsert(ref T current, int hash)
            {
                currentIndex = 0;
                Insert(ref current, hash);
                return true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void Insert(ref T current, int hash)
            {
                if (capacity == count)
                {
                    InsertWithReallocation(ref current, hash, currentIndex);
                    return;
                }
                if (currentIndex < count)
                {
                    UnsafeUtilityEx.MemMove(ptr + currentIndex + 1, ptr + currentIndex, count - currentIndex);
                    UnsafeUtilityEx.MemMove(codes + currentIndex + 1, codes + currentIndex, count - currentIndex);
                }
                ptr[currentIndex] = current;
                codes[currentIndex] = hash;
                count++;
            }

            private void InsertWithReallocation(ref T current, int hash, long index)
            {
                var tmpPtr = UnsafeUtilityEx.Malloc<T>(capacity << 1, alloc);
                var tmpCodes = UnsafeUtilityEx.Malloc<int>(capacity << 1, alloc);
                capacity <<= 1;
                tmpPtr[count] = current;
                tmpCodes[count] = hash;
                if (index == 0)
                {
                    UnsafeUtilityEx.MemCpy(tmpPtr + 1, ptr, count);
                    UnsafeUtilityEx.MemCpy(tmpCodes + 1, codes, count);
                }
                else if (index == count)
                {
                    UnsafeUtilityEx.MemCpy(tmpPtr, ptr, count);
                    UnsafeUtilityEx.MemCpy(tmpCodes, codes, count);
                }
                else
                {
                    UnsafeUtilityEx.MemCpy(tmpPtr, ptr, index);
                    UnsafeUtilityEx.MemCpy(tmpPtr + index + 1, ptr + index, count - index);
                    UnsafeUtilityEx.MemCpy(tmpCodes, codes, index);
                    UnsafeUtilityEx.MemCpy(tmpCodes + index + 1, codes + index, count - index);
                }
                ++count;
                UnsafeUtility.Free(ptr, alloc);
                UnsafeUtility.Free(codes, alloc);
                ptr = tmpPtr;
                codes = tmpCodes;
            }

            public void Reset() => throw new InvalidOperationException();

            public ref T Current => ref ptr[currentIndex];
            T IEnumerator<T>.Current => Current;
            object IEnumerator.Current => Current;

            public void Dispose()
            {
                if (ptr != null)
                    UnsafeUtility.Free(ptr, alloc);
                if (codes != null)
                    UnsafeUtility.Free(codes, alloc);
                this = default;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                if (ptr == null) return false;
                while (enumerator.MoveNext())
                {
                    ref var current = ref enumerator.Current;
                    var hash = getHashCodeFunc.Calc(ref current);
                    if (count == 0)
                        return InitialInsert(ref current, hash);
                    if (TryInsert(ref current, hash))
                        return true;
                }
                return false;
            }

            public ref T TryGetNext(out bool success)
            {
                if (!(success = ptr != null))
                    return ref Psuedo.AsRefNull<T>();
                while (true)
                {
                    ref var current = ref enumerator.TryGetNext(out success);
                    if (!success)
                        return ref Psuedo.AsRefNull<T>();
                    var hash = getHashCodeFunc.Calc(ref current);
                    if (count == 0)
                    {
                        success = InitialInsert(ref current, hash);
                        return ref current;
                    }
                    if (TryInsert(ref current, hash))
                    {
                        success = true;
                        return ref current;
                    }
                }
            }

            public bool TryMoveNext(out T value)
            {
                if (ptr == null)
                {
                    value = default;
                    return false;
                }
                while (enumerator.TryMoveNext(out value))
                {
                    var hash = getHashCodeFunc.Calc(ref value);
                    if (count == 0)
                        return InitialInsert(ref value, hash);
                    if (TryInsert(ref value, hash))
                        return true;
                }
                return false;
            }
        }

        [PsuedoIsReadOnly] public Enumerator GetEnumerator() => new Enumerator(ref enumerable, equalityComparer, getHashCodeFunc, alloc);

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool CanFastCount() => false;

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
            CopyTo(answer.GetPointer());
            return answer;
        }
        #endregion
    }
}