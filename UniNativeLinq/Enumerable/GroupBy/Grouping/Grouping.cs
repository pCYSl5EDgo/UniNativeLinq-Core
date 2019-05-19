using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    public unsafe partial struct
        Grouping<TKey, TSource>
        : IRefEnumerable<Grouping<TKey, TSource>.Enumerator, TSource>, IDisposable
        where TKey : unmanaged
        where TSource : unmanaged
    {
        public TKey Key;
        public TSource* Elements;
        public long Length;
        public Allocator Allocator;

        public Grouping(in TKey key, TSource* elements, long length, Allocator allocator)
        {
            Key = key;
            Elements = elements;
            Length = length;
            Allocator = allocator;
        }

        public Grouping(in TKey key, in NativeEnumerable<TSource> elements, Allocator allocator)
        {
            Key = key;
            Elements = elements.Ptr;
            Length = elements.Length;
            Allocator = allocator;
        }

        public struct Enumerator : IRefEnumerator<TSource>
        {
            private readonly TSource* elements;
            private readonly long count;
            private long index;

            public Enumerator(in Grouping<TKey, TSource> parent)
            {
                if (parent.Elements != null && parent.Length > 0)
                {
                    elements = parent.Elements;
                    count = parent.Length;
                }
                else
                {
                    elements = null;
                    count = 0;
                }
                index = -1;
            }

            public ref TSource Current => ref elements[index];
            TSource IEnumerator<TSource>.Current => Current;
            object IEnumerator.Current => Current;

            public void Dispose() => this = default;

            public bool MoveNext() => ++index < count;

            public void Reset() => index = -1;

            public ref TSource TryGetNext(out bool success)
            {
                success = ++index < count;
                if (success)
                    return ref elements[index];
                index = count;
                return ref Unsafe.AsRef<TSource>(null);
            }

            public bool TryMoveNext(out TSource value)
            {
                if (++index < count)
                {
                    value = elements[index];
                    return true;
                }
                else
                {
                    value = default;
                    index = count;
                    return false;
                }
            }
        }

        public readonly Enumerator GetEnumerator() => new Enumerator(in this);

        public void Dispose()
        {
            if (Elements != null && UnsafeUtility.IsValidAllocator(Allocator))
                UnsafeUtility.Free(Elements, Allocator);
            this = default;
        }

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool CanFastCount() => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Any() => Length != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int Count()
            => (int)Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly long LongCount() => Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void CopyTo(TSource* dest) => UnsafeUtilityEx.MemCpy(dest, Elements, Length);

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
            CopyTo(UnsafeUtilityEx.GetPointer(answer));
            return answer;
        }
        #endregion
    }
}
