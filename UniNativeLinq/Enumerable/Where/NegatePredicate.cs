namespace UniNativeLinq
{
    public readonly struct NegatePredicate<T, TPredicate> : IRefFunc<T, bool>
        where TPredicate : IRefFunc<T, bool>
    {
        public readonly TPredicate Predicate;
        public NegatePredicate(in TPredicate predicate) => Predicate = predicate;
        public bool Calc(ref T arg0) => !Predicate.Calc(ref arg0);

        public static implicit operator NegatePredicate<T, TPredicate>(in TPredicate predicate) => new NegatePredicate<T, TPredicate>(predicate);
    }
}
