namespace UniNativeLinq
{
    public readonly struct
        WithIndex<T>
        : ISelectIndex<T, (T element, long index)>
        where T : unmanaged
    {
        public void Execute(ref T source, long index, ref (T element, long index) result)
        {
            result.Item1 = source;
            result.Item2 = index;
        }
    }
}
