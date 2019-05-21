using Unity.Collections;

namespace UniNativeLinq
{
    public struct
        GroupJoinPredicate<T, TKey, TEqualityComparer>
        : IWhereIndex<T>
        where T : unmanaged
        where TKey : unmanaged
        where TEqualityComparer : struct, IRefFunc<TKey, TKey, bool>
    {
        public TKey Key;
        private readonly NativeEnumerable<TKey> keys;
        private readonly TEqualityComparer comparer;
        private readonly Allocator allocator;
        public long Length => keys.Length;

        public GroupJoinPredicate(in NativeEnumerable<TKey> inners, in TEqualityComparer comparer, Allocator allocator)
        {
            Key = default;
            keys = inners;
            this.comparer = comparer;
            this.allocator = allocator;
        }

        public static
            GroupJoinPredicate<T, TKey, TEqualityComparer>
            Create<TEnumerable, TEnumerator, TKeySelector>(in TEnumerable enumerable, in TKeySelector selector, in TEqualityComparer comparer, Allocator allocator)
            where TEnumerator : struct, IRefEnumerator<T>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, T>
            where TKeySelector : struct, IRefFunc<T, TKey>
            => new GroupJoinPredicate<T, TKey, TEqualityComparer>(
                new SelectEnumerable<TEnumerable, TEnumerator, T, TKey, FuncToAction<TKeySelector, T, TKey>>(enumerable, selector).ToNativeEnumerable(allocator),
                comparer,
                allocator
            );

        public static
            GroupJoinPredicate<T, TKey, TEqualityComparer>
            Create<TKeySelector>(in NativeEnumerable<T> enumerable, in TKeySelector selector, in TEqualityComparer comparer, Allocator allocator)
            where TKeySelector : struct, IRefFunc<T, TKey>
            => new GroupJoinPredicate<T, TKey, TEqualityComparer>(
                new SelectEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TKey, FuncToAction<TKeySelector, T, TKey>>(enumerable, selector).ToNativeEnumerable(allocator),
                comparer,
                allocator
            );

        public bool Calc(ref T value, long index) => comparer.Calc(ref Key, ref keys[index]);

        public void Dispose()
        {
            keys.Dispose(allocator);
            this = default;
        }
    }
}
