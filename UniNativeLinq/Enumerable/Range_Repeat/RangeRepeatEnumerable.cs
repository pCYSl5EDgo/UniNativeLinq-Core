using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    public unsafe readonly partial struct
        RangeRepeatEnumerable<TSource, TAction>
        : IRefEnumerable<RangeRepeatEnumerable<TSource, TAction>.Enumerator, TSource>
        where TSource : unmanaged
        where TAction : struct, IRangeRepeat<TSource>
    {
        private readonly TSource start;
        private readonly long length;
        private readonly TAction acts;
        private readonly Allocator alloc;

        public RangeRepeatEnumerable(TSource start, long length, TAction acts, Allocator allocator)
        {
            this.start = start;
            this.length = length;
            this.acts = acts;
            alloc = allocator;
        }

        public struct Enumerator : IRefEnumerator<TSource>
        {
            private readonly Allocator allocator;
            private readonly TSource* element;
            private readonly long count;
            private long index;
            private TAction action;

            internal Enumerator(TSource start, long count, TAction action, Allocator allocator)
            {
                element = UnsafeUtilityEx.Malloc<TSource>(1, allocator);
                *element = start;
                this.count = count;
                index = -1;
                this.action = action;
                this.allocator = allocator;
            }

            public bool MoveNext()
            {
                if (++index >= count) return false;
                if (index > 0) action.Execute(ref *element);
                return true;
            }

            public void Reset()
            {
                if (index <= 0) return;
                action.Back(ref *element, index);
                index = 0;
            }
            public readonly ref TSource Current => ref *element;
            readonly TSource IEnumerator<TSource>.Current => Current;
            readonly object IEnumerator.Current => Current;
            public void Dispose() => UnsafeUtility.Free(element, allocator);

            public ref TSource TryGetNext(out bool success)
            {
                success = ++index >= count;
                if (!success)
                {
                    index = count;
                    return ref Unsafe.AsRef<TSource>(null);
                }
                if (index > 0)
                    action.Execute(ref *element);
                return ref *element;
            }

            public bool TryMoveNext(out TSource value)
            {
                if(++index < count)
                {
                    if (index > 0)
                        action.Execute(ref *element);
                    value = *element;
                    return true;
                }
                value = default;
                return false;
            }
        }

        public readonly Enumerator GetEnumerator() => new Enumerator(start, length, acts, alloc);

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();

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
            CopyTo(UnsafeUtilityEx.GetPointer(answer));
            return answer;
        }
        #endregion
    }
}