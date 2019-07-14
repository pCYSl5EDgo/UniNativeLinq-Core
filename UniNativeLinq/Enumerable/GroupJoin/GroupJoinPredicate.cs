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

        public static unsafe
            GroupJoinPredicate<T, TKey, TEqualityComparer>
            Create<TKeySelector>(NativeEnumerable<T> enumerable, TKeySelector selector, in TEqualityComparer comparer, Allocator allocator)
            where TKeySelector : struct, IRefFunc<T, TKey>
        {
            var ptr = UnsafeUtilityEx.Malloc<TKey>(enumerable.Length, allocator);
            for (var i = 0L; i < enumerable.Length; i++)
                ptr[i] = selector.Calc(ref enumerable[i]);
            return new GroupJoinPredicate<T, TKey, TEqualityComparer>(
                new NativeEnumerable<TKey>(ptr, enumerable.Length), 
                comparer,
                allocator
            );
        }

        public bool Calc(ref T value, long index) => comparer.Calc(ref Key, ref keys[index]);

        public void Dispose()
        {
            keys.Dispose(allocator);
            this = default;
        }
    }
}
