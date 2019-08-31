using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UniNativeLinq
{
    [SlowCount]
    public unsafe struct
        SelectManyEnumerable<TEnumerable, TEnumerator, TPrev, TAnotherEnumerable, TAnotherEnumerator, T, TAction>
        : IRefEnumerable<SelectManyEnumerable<TEnumerable, TEnumerator, TPrev, TAnotherEnumerable, TAnotherEnumerator, T, TAction>.Enumerator, T>
        where TPrev : unmanaged
        where TEnumerator : struct, IRefEnumerator<TPrev>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, TPrev>
        where T : unmanaged
        where TAnotherEnumerator : struct, IRefEnumerator<T>
        where TAnotherEnumerable : struct, IRefEnumerable<TAnotherEnumerator, T>
        where TAction : struct, IRefAction<TPrev, TAnotherEnumerable>
    {
        private TEnumerable enumerable;
        private TAction acts;

        public bool CanIndexAccess() => false;

        public ref T this[long index] => throw new NotSupportedException();

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

            internal Enumerator(in TEnumerator enumerator, in TAction action)
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
            public ref T Current => ref resultEnumerator.Current;
            T IEnumerator<T>.Current => Current;
            object IEnumerator.Current => Current;

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

        public Enumerator GetEnumerator() => new Enumerator(enumerable.GetEnumerator(), acts);

        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFastCount() => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count()
            => (int)LongCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long LongCount()
        {
            var enumerator = GetEnumerator();
            var count = 0L;
            while (enumerator.MoveNext())
                ++count;
            enumerator.Dispose();
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(T* dest)
        {
            var enumerator = GetEnumerator();
            while (enumerator.MoveNext())
                *dest++ = enumerator.Current;
            enumerator.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray()
        {
            var count = LongCount();
            if (count == 0) return Array.Empty<T>();
            var answer = new T[count];
            CopyTo(Pseudo.AsPointer<T>(ref answer[0]));
            return answer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeEnumerable<T> ToNativeEnumerable(Allocator allocator)
        {
            var count = LongCount();
            var ptr = UnsafeUtilityEx.Malloc<T>(count, allocator);
            CopyTo(ptr);
            return NativeEnumerable<T>.Create(ptr, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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