namespace UniNativeLinq
{
    public struct DefaultGetHashCodeFunc<T> : IRefFunc<T, int>
        where T : unmanaged
    {
        public int Calc(ref T arg0) => arg0.GetHashCode();
    }
}