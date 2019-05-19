using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UniNativeLinq
{
    public unsafe readonly partial struct
        SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>
        : IRefEnumerable<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>.Enumerator, TSource>
        where TPrevSource : unmanaged
        where TEnumerator : struct, IRefEnumerator<TPrevSource>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, TPrevSource>
        where TSource : unmanaged
        where TSourceEnumerator : struct, IRefEnumerator<TSource>
        where TSourceEnumerable : struct, IRefEnumerable<TSourceEnumerator, TSource>
        where TAction : struct, IRefAction<TPrevSource, TSourceEnumerable>
    {
        private readonly TEnumerable enumerable;
        private readonly TAction acts;

        public SelectManyEnumerable(in TEnumerable enumerable, in TAction action)
        {
            this.enumerable = enumerable;
            acts = action;
        }

        public struct Enumerator : IRefEnumerator<TSource>
        {
            private TEnumerator enumerator;
            private TSourceEnumerable resultEnumerable;
            private TSourceEnumerator resultEnumerator;
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
            public readonly ref TSource Current => ref resultEnumerator.Current;
            readonly TSource IEnumerator<TSource>.Current => Current;
            readonly object IEnumerator.Current => Current;

            public void Dispose()
            {
                resultEnumerator.Dispose();
                enumerator.Dispose();
            }

            public ref TSource TryGetNext(out bool success)
            {
                if (isNotFirst)
                    return ref TryGetNextNotFirst(out success);
                return ref TryGetNextFirst(out success);
            }

            public bool TryMoveNext(out TSource value)
            {
                if (isNotFirst)
                    return TryMoveNextNotFirst(out value);
                return TryMoveNextFirst(out value);
            }

            private bool TryMoveNextFirst(out TSource value)
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

            private ref TSource TryGetNextFirst(out bool success)
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

            private bool TryMoveNextNotFirst(out TSource value)
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

            private ref TSource TryGetNextNotFirst(out bool success)
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
            CopyTo(UnsafeUtilityEx.GetPointer(answer));
            return answer;
        }
        #endregion
    }
}