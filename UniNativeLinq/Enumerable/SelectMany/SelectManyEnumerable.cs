using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UniNativeLinq
{
    [SlowCount]
    public readonly unsafe struct
        SelectManyEnumerable<TEnumerable, TEnumerator, TPrev, T, TAnotherEnumerable, TAnotherEnumerator, TAction>
        : IRefEnumerable<SelectManyEnumerable<TEnumerable, TEnumerator, TPrev, T, TAnotherEnumerable, TAnotherEnumerator, TAction>.Enumerator, T>
        where TPrev : unmanaged
        where TEnumerator : struct, IRefEnumerator<TPrev>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, TPrev>
        where T : unmanaged
        where TAnotherEnumerator : struct, IRefEnumerator<T>
        where TAnotherEnumerable : struct, IRefEnumerable<TAnotherEnumerator, T>
        where TAction : struct, IRefAction<TPrev, TAnotherEnumerable>
    {
        private readonly TEnumerable enumerable;
        private readonly TAction acts;

        public SelectManyEnumerable(in TEnumerable enumerable, in TAction action)
        {
            this.enumerable = enumerable;
            acts = action;
        }

        public struct Enumerator : IRefEnumerator<T>
        {
            private TEnumerator enumerator;
            private TAnotherEnumerable resultEnumerable;
            private TAnotherEnumerator resultEnumerator;
            private TAction action;
            private bool isNotFirst;

            internal Enumerator(in TEnumerator enumerator, TAction action)
            {
                this.enumerator = enumerator;
                resultEnumerable = default;
                resultEnumerator = default;
                this.action = action;
                isNotFirst = false;
            }

            public bool MoveNext()
            {
                if (isNotFirst)
                {
                    if (resultEnumerator.MoveNext())
                        return true;
                    resultEnumerator.Dispose();
                    while (enumerator.MoveNext())
                    {
                        action.Execute(ref enumerator.Current, ref resultEnumerable);
                        resultEnumerator = resultEnumerable.GetEnumerator();
                        if (resultEnumerator.MoveNext())
                            return true;
                        resultEnumerator.Dispose();
                    }
                    return false;
                }
                isNotFirst = true;
                while (enumerator.MoveNext())
                {
                    action.Execute(ref enumerator.Current, ref resultEnumerable);
                    resultEnumerator = resultEnumerable.GetEnumerator();
                    if (resultEnumerator.MoveNext())
                        return true;
                    resultEnumerator.Dispose();
                }
                return false;
            }

            public void Reset() => throw new InvalidOperationException();
            public readonly ref T Current => ref resultEnumerator.Current;
            readonly T IEnumerator<T>.Current => Current;
            readonly object IEnumerator.Current => Current;

            public void Dispose()
            {
                resultEnumerator.Dispose();
                enumerator.Dispose();
            }

            public ref T TryGetNext(out bool success)
            {
                if (isNotFirst)
                    return ref TryGetNextNotFirst(out success);
                return ref TryGetNextFirst(out success);
            }

            public bool TryMoveNext(out T value)
            {
                if (isNotFirst)
                    return TryMoveNextNotFirst(out value);
                return TryMoveNextFirst(out value);
            }

            private bool TryMoveNextFirst(out T value)
            {
                isNotFirst = true;
                while (enumerator.TryMoveNext(out var prevSource))
                {
                    action.Execute(ref prevSource, ref resultEnumerable);
                    resultEnumerator = resultEnumerable.GetEnumerator();
                    if (resultEnumerator.TryMoveNext(out value))
                        return true;
                    resultEnumerator.Dispose();
                }
                return resultEnumerator.TryMoveNext(out value);
            }

            private ref T TryGetNextFirst(out bool success)
            {
                isNotFirst = true;
                while (true)
                {
                    ref var prevSource = ref enumerator.TryGetNext(out success);
                    if (!success)
                        return ref resultEnumerator.TryGetNext(out success);
                    action.Execute(ref prevSource, ref resultEnumerable);
                    resultEnumerator = resultEnumerable.GetEnumerator();
                    ref var value = ref resultEnumerator.TryGetNext(out success);
                    if (success)
                        return ref value;
                    resultEnumerator.Dispose();
                }
            }

            private bool TryMoveNextNotFirst(out T value)
            {
                if (resultEnumerator.TryMoveNext(out value))
                    return true;
                resultEnumerator.Dispose();
                while (enumerator.TryMoveNext(out var prevSource))
                {
                    action.Execute(ref prevSource, ref resultEnumerable);
                    resultEnumerator = resultEnumerable.GetEnumerator();
                    if (resultEnumerator.TryMoveNext(out value))
                        return true;
                    resultEnumerator.Dispose();
                }
                value = default;
                return false;
            }

            private ref T TryGetNextNotFirst(out bool success)
            {
                ref var value = ref resultEnumerator.TryGetNext(out success);
                if (success)
                    return ref value;
                resultEnumerator.Dispose();
                while (true)
                {
                    ref var prevSource = ref enumerator.TryGetNext(out success);
                    if (!success) return ref value;
                    action.Execute(ref prevSource, ref resultEnumerable);
                    resultEnumerator = resultEnumerable.GetEnumerator();
                    value = ref resultEnumerator.TryGetNext(out success);
                    if (success)
                        return ref value;
                    resultEnumerator.Dispose();
                }
            }
        }

        public readonly Enumerator GetEnumerator() => new Enumerator(enumerable.GetEnumerator(), acts);
        
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
            CopyTo(UnsafeUtilityEx.GetPointer(answer));
            return answer;
        }
        #endregion
    }
}