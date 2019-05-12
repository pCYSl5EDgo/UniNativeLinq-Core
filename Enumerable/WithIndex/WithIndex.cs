namespace pcysl5edgo.Collections.LINQ
{
    public readonly struct
        WithIndex<T>
        : ISelectIndex<T, (T, long)>
        where T : unmanaged
    {
        public void Execute(ref T source, long index, ref (T, long) result)
        {
            result.Item1 = source;
            result.Item2 = index;
        }
    }
}
