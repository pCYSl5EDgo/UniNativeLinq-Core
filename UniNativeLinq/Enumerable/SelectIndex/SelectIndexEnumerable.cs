using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    public unsafe readonly partial struct
        SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>
        : IRefEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>.Enumerator, TSource>
        where TPrevSource : unmanaged
        where TSource : unmanaged
        where TAction : struct, ISelectIndex<TPrevSource, TSource>
        where TPrevEnumerator : struct, IRefEnumerator<TPrevSource>
        where TPrevEnumerable : struct, IRefEnumerable<TPrevEnumerator, TPrevSource>
    {
        private readonly TPrevEnumerable enumerable;
        private readonly TAction acts;
        private readonly Allocator alloc;

        public SelectIndexEnumerable(in TPrevEnumerable enumerable, TAction acts, Allocator allocator)
        {
            this.enumerable = enumerable;
            this.acts = acts;
            alloc = allocator;
        }

        public struct Enumerator : IRefEnumerator<TSource>
        {
            private TPrevEnumerator enumerator;
            private readonly TSource* element;
            private readonly Allocator allocator;
            private TAction action;
            private long index;

            internal Enumerator(in TPrevEnumerator enumerator, TAction action, Allocator allocator)
            {
                this.enumerator = enumerator;
                element = UnsafeUtilityEx.Malloc<TSource>(1, allocator);
                *element = default;
                this.action = action;
                this.allocator = allocator;
                index = -1;
            }

            public bool MoveNext()
            {
                ++index;
                if (!enumerator.MoveNext()) return false;
                action.Execute(ref enumerator.Current, index, ref *element);
                return true;
            }

            public void Reset() => throw new InvalidOperationException();
            public readonly ref TSource Current => ref *element;
            readonly TSource IEnumerator<TSource>.Current => Current;
            readonly object IEnumerator.Current => Current;
            public void Dispose() => UnsafeUtility.Free(element, allocator);

            public ref TSource TryGetNext(out bool success)
            {
                ++index;
                ref var prevSource = ref enumerator.TryGetNext(out success);
                if (!success) return ref Unsafe.AsRef<TSource>(null);
                action.Execute(ref enumerator.Current, index, ref *element);
                return ref *element;
            }

            public bool TryMoveNext(out TSource value)
            {
                ++index;
                if(!enumerator.TryMoveNext(out var prevSource))
                {
                    value = default;
                    return false;
                }
                action.Execute(ref prevSource, index, ref *element);
                value = *element;
                return true;
            }
        }

        public readonly Enumerator GetEnumerator() => new Enumerator(enumerable.GetEnumerator(), acts, alloc);
        
        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool CanFastCount() => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Any() => enumerable.Any();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int Count() => enumerable.Count();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly long LongCount() => enumerable.LongCount();

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