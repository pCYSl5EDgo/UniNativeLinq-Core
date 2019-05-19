using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UniNativeLinq
{
    public unsafe readonly partial struct
        ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>
        : IRefEnumerable<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>.Enumerator, TSource>
        where TFirstSource : unmanaged
        where TSecondSource : unmanaged
        where TSource : unmanaged
        where TFirstEnumerator : struct, IRefEnumerator<TFirstSource>
        where TFirstEnumerable : struct, IRefEnumerable<TFirstEnumerator, TFirstSource>
        where TSecondEnumerator : struct, IRefEnumerator<TSecondSource>
        where TSecondEnumerable : struct, IRefEnumerable<TSecondEnumerator, TSecondSource>
        where TAction : struct, IRefAction<TFirstSource, TSecondSource, TSource>
    {
        private readonly TFirstEnumerable firstCollection;
        private readonly TSecondEnumerable secondCollection;
        private readonly Allocator alloc;
        private readonly TAction acts;
        private readonly TFirstSource firstDefault;
        private readonly TSecondSource secondDefault;

        public ZipEnumerable(in TFirstEnumerable firstCollection, in TSecondEnumerable secondCollection, TAction acts, in TFirstSource firstDefault, in TSecondSource secondDefault, Allocator alloc)
        {
            this.firstCollection = firstCollection;
            this.secondCollection = secondCollection;
            this.acts = acts;
            this.firstDefault = firstDefault;
            this.secondDefault = secondDefault;
            this.alloc = alloc;
        }

        public struct Enumerator : IRefEnumerator<TSource>
        {
            private TFirstEnumerator firstEnumerator;
            private TSecondEnumerator secondEnumerator;
            private readonly TSource* element;
            private readonly Allocator allocator;
            private TAction action;
            private TFirstSource firstDefaultValue;
            private TSecondSource secondDefaultValue;
            private bool isFirstAlive, isSecondAlive;

            internal Enumerator(in TFirstEnumerator firstEnumerator, in TSecondEnumerator secondEnumerator, TAction action, TFirstSource firstDefaultValue, TSecondSource secondDefaultValue, Allocator allocator)
            {
                this.firstEnumerator = firstEnumerator;
                this.secondEnumerator = secondEnumerator;
                element = UnsafeUtilityEx.Malloc<TSource>(1, allocator);
                this.allocator = allocator;
                this.action = action;
                this.firstDefaultValue = firstDefaultValue;
                this.secondDefaultValue = secondDefaultValue;
                isFirstAlive = isSecondAlive = true;
            }

            public ref TSource TryGetNext(out bool success)
            {
                if (isFirstAlive && !firstEnumerator.MoveNext())
                    isFirstAlive = false;
                if (isSecondAlive && !secondEnumerator.MoveNext())
                    isSecondAlive = false;
                if (isFirstAlive)
                {
                    success = true;
                    if (isSecondAlive)
                        action.Execute(ref firstEnumerator.Current, ref secondEnumerator.Current, ref *element);
                    else
                        action.Execute(ref firstEnumerator.Current, ref secondDefaultValue, ref *element);
                }
                else
                {
                    if (isSecondAlive)
                    {
                        action.Execute(ref firstDefaultValue, ref secondEnumerator.Current, ref *element);
                        success = true;
                    }
                    else
                    {
                        success = false;
                    }
                }
                return ref *element;
            }

            public bool TryMoveNext(out TSource value)
            {
                if (isFirstAlive && !firstEnumerator.MoveNext())
                    isFirstAlive = false;
                if (isSecondAlive && !secondEnumerator.MoveNext())
                    isSecondAlive = false;
                if (isFirstAlive)
                {
                    if (isSecondAlive)
                        action.Execute(ref firstEnumerator.Current, ref secondEnumerator.Current, ref *element);
                    else
                        action.Execute(ref firstEnumerator.Current, ref secondDefaultValue, ref *element);
                    value = *element;
                    return true;
                }
                else
                {
                    if (isSecondAlive)
                    {
                        action.Execute(ref firstDefaultValue, ref secondEnumerator.Current, ref *element);
                        value = *element;
                        return true;
                    }
                    else
                    {
                        value = *element;
                        return false;
                    }
                }
            }

            public bool MoveNext()
            {
                if (isFirstAlive && !firstEnumerator.MoveNext())
                    isFirstAlive = false;
                if (isSecondAlive && !secondEnumerator.MoveNext())
                    isSecondAlive = false;
                if (isFirstAlive)
                {
                    if (isSecondAlive)
                        action.Execute(ref firstEnumerator.Current, ref secondEnumerator.Current, ref *element);
                    else
                        action.Execute(ref firstEnumerator.Current, ref secondDefaultValue, ref *element);
                }
                else
                {
                    if (isSecondAlive)
                        action.Execute(ref firstDefaultValue, ref secondEnumerator.Current, ref *element);
                    else
                        return false;
                }
                return true;
            }

            public void Reset() => throw new InvalidOperationException();
            public readonly ref TSource Current => ref *element;
            readonly TSource IEnumerator<TSource>.Current => Current;
            readonly object IEnumerator.Current => Current;

            public void Dispose()
            {
                if (element != null)
                    UnsafeUtility.Free(element, allocator);
                this = default;
            }
        }

        public readonly Enumerator GetEnumerator() => new Enumerator(firstCollection.GetEnumerator(), secondCollection.GetEnumerator(), acts, firstDefault, secondDefault, alloc);
        
        #region Interface Implementation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool CanFastCount() => firstCollection.CanFastCount() && secondCollection.CanFastCount();

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