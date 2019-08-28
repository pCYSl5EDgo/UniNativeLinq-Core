using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UniNativeLinq
{
    [SlowCount]
    [PseudoIsReadOnly]
    public unsafe struct
        DistinctEnumerable<TEnumerable, TEnumerator, T, TEqualityComparer>
        : IRefEnumerable<DistinctEnumerable<TEnumerable, TEnumerator, T, TEqualityComparer>.Enumerator, T>
        where T : unmanaged
        where TEqualityComparer : struct, IRefFunc<T, T, bool>
        where TEnumerator : struct, IRefEnumerator<T>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, T>
    {
        [PseudoIsReadOnly] private TEnumerable enumerable;
        private readonly TEqualityComparer equalityComparer;
        private readonly Allocator alloc;
        public bool CanIndexAccess() => false;
        public ref T this[long index] => throw new NotSupportedException();
        public DistinctEnumerable(in TEnumerable enumerable, in TEqualityComparer equalityComparer, Allocator allocator)
        {
            this.enumerable = enumerable;
            this.equalityComparer = equalityComparer;
            alloc = allocator;
        }

        public DistinctEnumerable(in TEnumerable enumerable, Allocator allocator)
        {
            this.enumerable = enumerable;
            equalityComparer = default;
            alloc = allocator;
        }

        public struct Enumerator : IRefEnumerator<T>
        {
            private TEnumerator enumerator;
            private NativeList<T> list;
            private TEqualityComparer comparer;

            public Enumerator([PseudoIsReadOnly]ref TEnumerable enumerable, in TEqualityComparer comparer, Allocator allocator)
            {
                enumerator = enumerable.GetEnumerator();
                list = new NativeList<T>(allocator);
                this.comparer = comparer;
            }

            public bool MoveNext()
            {
                while (enumerator.MoveNext())
                {
                    ref var current = ref enumerator.Current;
                    var isSame = false;
                    foreach (ref var item in list)
                    {
                        if (!comparer.Calc(ref current, ref item)) continue;
                        isSame = true;
                        break;
                    }
                    if (isSame) continue;
                    list.Add(current);
                    return true;
                }
                return false;
            }

            public void Reset() => throw new InvalidOperationException();

            public ref T Current => ref enumerator.Current;
            public ref T TryGetNext(out bool success)
            {
                while (true)
                {
                    ref var current = ref enumerator.TryGetNext(out success);
                    if (!success)
                    {
                        return ref Pseudo.AsRefNull<T>();
                    }
                    var isSame = false;
                    foreach (ref var item in list)
                    {
                        if (!comparer.Calc(ref current, ref item)) continue;
                        isSame = true;
                        break;
                    }
                    if (isSame) continue;
                    list.Add(current);
                    return ref current;
                }
            }

            public bool TryMoveNext(out T value)
            {
                while (enumerator.TryMoveNext(out value))
                {
                    var isSame = false;
                    foreach (ref var item in list)
                    {
                        if (!comparer.Calc(ref value, ref item)) continue;
                        isSame = true;
                        break;
                    }
                    if (isSame) continue;
                    list.Add(value);
                    return true;
                }
                return false;
            }

            T IEnumerator<T>.Current => Current;
            object IEnumerator.Current => Current;

            public void Dispose()
            {
                list.Dispose();
                this = default;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        public Enumerator GetEnumerator() => new Enumerator(ref enumerable, equalityComparer, alloc);

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        public bool CanFastCount() => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        public bool Any()
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

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        public int Count()
            => (int)LongCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        public long LongCount()
        {
            var enumerator = GetEnumerator();
            var count = 0L;
            while (enumerator.MoveNext())
                ++count;
            enumerator.Dispose();
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        public void CopyTo(T* dest)
        {
            var enumerator = GetEnumerator();
            while (enumerator.MoveNext())
                *dest++ = enumerator.Current;
            enumerator.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        public T[] ToArray()
        {
            var count = LongCount();
            if (count == 0) return Array.Empty<T>();
            var answer = new T[count];
            CopyTo(Pseudo.AsPointer(ref answer[0]));
            return answer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        public NativeEnumerable<T> ToNativeEnumerable(Allocator allocator)
        {
            var count = LongCount();
            var ptr = UnsafeUtilityEx.Malloc<T>(count, allocator);
            CopyTo(ptr);
            return NativeEnumerable<T>.Create(ptr, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), PseudoIsReadOnly]
        public NativeArray<T> ToNativeArray(Allocator allocator)
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