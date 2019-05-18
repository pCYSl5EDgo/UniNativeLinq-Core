using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    public unsafe partial struct
        AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>
        : IRefEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>.Enumerator, TSource>
        where TSource : unmanaged
        where TPrevEnumerable : struct, IRefEnumerable<TPrevEnumerator, TSource>
        where TPrevEnumerator : struct, IRefEnumerator<TSource>
    {
        private TPrevEnumerable enumerable;
        private TSource append;
        private readonly Allocator alloc;

        public AppendEnumerable(in TPrevEnumerable enumerable, in TSource append, Allocator alloc)
        {
            this.enumerable = enumerable;
            this.append = append;
            this.alloc = alloc;
        }

        public struct Enumerator
            : IRefEnumerator<TSource>
        {
            private TPrevEnumerator enumerator;
            private readonly TSource* element;
            private readonly Allocator allocator;
            private bool isCurrentEnumerator;

            public Enumerator(in TPrevEnumerator enumerator, in TSource element, Allocator allocator)
            {
                this.allocator = allocator;
                this.element = UnsafeUtilityEx.Malloc<TSource>(1, allocator);
                *this.element = element;
                this.enumerator = enumerator;
                isCurrentEnumerator = true;
            }

            public ref TSource Current
            {
                get
                {
                    if (isCurrentEnumerator)
                        return ref enumerator.Current;
                    return ref *element;
                }
            }

            TSource IEnumerator<TSource>.Current => Current;
            object IEnumerator.Current => Current;

            public void Dispose()
            {
                enumerator.Dispose();
                if (element != null && allocator != Allocator.None)
                    UnsafeUtility.Free(element, allocator);
                this = default;
            }

            public bool MoveNext()
            {
                if (!isCurrentEnumerator)
                    return false;
                if (!enumerator.MoveNext())
                    isCurrentEnumerator = false;
                return true;
            }

            public void Reset() => throw new InvalidOperationException();

            public ref TSource TryGetNext(out bool success)
            {
                success = isCurrentEnumerator;
                if (!success)
                    return ref *element;
                ref var value = ref enumerator.TryGetNext(out success);
                if (!success)
                    isCurrentEnumerator = false;
                return ref value;
            }

            public bool TryMoveNext(out TSource value)
            {
                if (!isCurrentEnumerator)
                {
                    value = *element;
                    return true;
                }
                if (enumerator.TryMoveNext(out value))
                    return true;
                isCurrentEnumerator = false;
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Enumerator GetEnumerator() => new Enumerator(enumerable.GetEnumerator(), append, alloc);

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool CanFastCount() => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Any() => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int Count()
            => (int)LongCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly long LongCount() => enumerable.LongCount() + 1;

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
            var answer = new TSource[LongCount()];
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