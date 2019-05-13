using Unity.Collections;

namespace UniNativeLinq
{
    public struct
        GroupJoinPredicate<TSource, TKey, TEqualityComparer>
        : IWhereIndex<TSource>
        where TSource : unmanaged
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
            GroupJoinPredicate<TSource, TKey, TEqualityComparer>
            Create<TEnumerable, TEnumerator, TKeySelector>(in TEnumerable enumerable, in TKeySelector selector, in TEqualityComparer comparer, Allocator allocator)
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TKeySelector : struct, IRefFunc<TSource, TKey>
            => new GroupJoinPredicate<TSource, TKey, TEqualityComparer>(
                new SelectEnumerable<TEnumerable, TEnumerator, TSource, TKey, FuncToAction<TKeySelector, TSource, TKey>>(enumerable, selector, allocator).ToNativeEnumerable(allocator),
                comparer,
                allocator
            );

        public static
            GroupJoinPredicate<TSource, TKey, TEqualityComparer>
            Create<TKeySelector>(in NativeEnumerable<TSource> enumerable, in TKeySelector selector, in TEqualityComparer comparer, Allocator allocator)
            where TKeySelector : struct, IRefFunc<TSource, TKey>
            => new GroupJoinPredicate<TSource, TKey, TEqualityComparer>(
                new SelectEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource, TKey, FuncToAction<TKeySelector, TSource, TKey>>(enumerable, selector, allocator).ToNativeEnumerable(allocator),
                comparer,
                allocator
            );

        public bool Calc(ref TSource value, long index) => comparer.Calc(ref Key, ref keys[index]);

        public void Dispose()
        {
            keys.Dispose(allocator);
            this = default;
        }
    }
}
