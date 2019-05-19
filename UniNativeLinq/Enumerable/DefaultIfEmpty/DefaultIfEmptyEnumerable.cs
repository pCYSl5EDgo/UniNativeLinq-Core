using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    public unsafe partial struct
        DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>
        : IRefEnumerable<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>.Enumerator, TSource>
        where TSource : unmanaged
        where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
        where TEnumerator : struct, IRefEnumerator<TSource>
    {
        private TEnumerable enumerable;
        private readonly TSource val;
        private readonly Allocator alloc;

        public DefaultIfEmptyEnumerable(in TEnumerable enumerable, in TSource val, Allocator alloc)
        {
            this.enumerable = enumerable;
            this.val = val;
            this.alloc = alloc;
        }

        public readonly Enumerator GetEnumerator() => new Enumerator(enumerable.GetEnumerator(), val, alloc);

        public struct Enumerator : IRefEnumerator<TSource>
        {
            private TEnumerator enumerator;
            private readonly TSource* element;
            private readonly Allocator allocator;
            private bool isFirst;
            private bool isDefault;

            internal Enumerator(in TEnumerator enumerator, in TSource value, Allocator allocator)
            {
                this.enumerator = enumerator;
                element = UnsafeUtilityEx.Malloc<TSource>(1, allocator);
                *element = value;
                this.allocator = allocator;
                isFirst = true;
                isDefault = false;
            }

            internal Enumerator(in TEnumerator enumerator, TSource* ptr)
            {
                this.enumerator = enumerator;
                this.element = ptr;
                allocator = Allocator.None;
                isFirst = true;
                isDefault = false;
            }

            public ref TSource Current
            {
                get
                {
                    if (isDefault)
                        return ref *element;
                    return ref enumerator.Current;
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
                if (!isFirst) return !isDefault && enumerator.MoveNext();
                isFirst = false;
                isDefault = !enumerator.MoveNext();
                return true;
            }

            public void Reset() => throw new InvalidOperationException();

            public ref TSource TryGetNext(out bool success)
            {
                if (isFirst)
                {
                    isFirst = false;
                    ref var value = ref enumerator.TryGetNext(out success);
                    if (success)
                    {
                        isDefault = false;
                        return ref value;
                    }
                    isDefault = true;
                    success = true;
                    return ref *element;
                }
                if(isDefault)
                {
                    success = false;
                    return ref *element;
                }
                return ref enumerator.TryGetNext(out success);
            }

            public bool TryMoveNext(out TSource value)
            {
                if(isFirst)
                {
                    isFirst = false;
                    if(enumerator.TryMoveNext(out value))
                    {
                        isDefault = false;
                        return true;
                    }
                    isDefault = true;
                    value = *element;
                    return true;
                }
                if(isDefault)
                {
                    value = default;
                    return false;
                }
                return enumerator.TryMoveNext(out value);
            }
        }

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool CanFastCount() => enumerable.CanFastCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Any() => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int Count()
            => (int)LongCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly long LongCount()
        {
            var count = enumerable.LongCount();
            return count == 0 ? 1 : count;
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