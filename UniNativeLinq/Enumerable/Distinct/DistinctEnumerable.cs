using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    public unsafe partial struct
        DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>
        : IRefEnumerable<DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>.Enumerator, TSource>
        where TSource : unmanaged
        where TEqualityComparer : struct, IRefFunc<TSource, TSource, bool>
        where TGetHashCodeFunc : struct, IRefFunc<TSource, int>
        where TEnumerator : struct, IRefEnumerator<TSource>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
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

        public struct Enumerator : IRefEnumerator<TSource>
        {
            private TEnumerator enumerator;
            private TSource* ptr;
            private int* codes;
            private long capacity;
            private long count;
            private readonly Allocator alloc;
            private TEqualityComparer comparer;
            private TGetHashCodeFunc getHashCodeFunc;
            private long currentIndex;

            public Enumerator(in TEnumerable enumerable, in TEqualityComparer comparer, in TGetHashCodeFunc getHashCodeFunc, Allocator allocator)
            {
                ref var _enumerable = ref Unsafe.AsRef(enumerable);
                if (_enumerable.CanFastCount())
                {
                    if ((capacity = _enumerable.LongCount()) == 0)
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
                enumerator = _enumerable.GetEnumerator();
                alloc = allocator;
                ptr = UnsafeUtilityEx.Malloc<TSource>(capacity, alloc);
                codes = UnsafeUtilityEx.Malloc<int>(capacity, alloc);
                this.comparer = comparer;
                this.getHashCodeFunc = getHashCodeFunc;
                currentIndex = 0;
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private bool TryInsert(ref TSource current, int hash)
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
            private bool TryInsertAround(ref TSource current, int hash, long minInclusive, long maxInclusive)
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
            private bool InitialInsert(ref TSource current, int hash)
            {
                currentIndex = 0;
                Insert(ref current, hash);
                return true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void Insert(ref TSource current, int hash)
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

            private void InsertWithReallocation(ref TSource current, int hash, long index)
            {
                var tmpPtr = UnsafeUtilityEx.Malloc<TSource>(capacity << 1, alloc);
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

            public ref TSource Current => ref ptr[currentIndex];
            TSource IEnumerator<TSource>.Current => Current;
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

            public ref TSource TryGetNext(out bool success)
            {
                if (!(success = ptr != null))
                    return ref Unsafe.AsRef<TSource>(null);
                while (true)
                {
                    ref var current = ref enumerator.TryGetNext(out success);
                    if (!success)
                        return ref Unsafe.AsRef<TSource>(null);
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

            public bool TryMoveNext(out TSource value)
            {
                if(ptr == null)
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

        public readonly Enumerator GetEnumerator() => new Enumerator(in enumerable, equalityComparer, getHashCodeFunc, alloc);

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();

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
        public readonly void CopyTo(TSource* dest)
        {
            var enumerator = GetEnumerator();
            while (enumerator.MoveNext())
                *dest++ = enumerator.Current;
            enumerator.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly TSource[] ToArray()
        {
            var count = LongCount();
            if (count == 0) return Array.Empty<TSource>();
            var answer = new TSource[count];
            CopyTo((TSource*)Unsafe.AsPointer(ref answer[0]));
            return answer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly NativeEnumerable<TSource> ToNativeEnumerable(Allocator allocator)
        {
            var count = LongCount();
            var ptr = UnsafeUtilityEx.Malloc<TSource>(count, allocator);
            CopyTo(ptr);
            return new NativeEnumerable<TSource>(ptr, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly NativeArray<TSource> ToNativeArray(Allocator allocator)
        {
            var count = Count();
            if (count == 0) return default;
            var answer = new NativeArray<TSource>(count, allocator, NativeArrayOptions.UninitializedMemory);
            CopyTo(answer.GetPointer());
            return answer;
        }
        #endregion
    }
}