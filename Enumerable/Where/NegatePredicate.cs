namespace UniNativeLinq
{
    public readonly struct NegatePredicate<TSource, TPredicate> : IRefFunc<TSource, bool>
        where TPredicate : IRefFunc<TSource, bool>
    {
        public readonly TPredicate Predicate;
        public NegatePredicate(in TPredicate predicate) => Predicate = predicate;
        public bool Calc(ref TSource arg0) => !Predicate.Calc(ref arg0);

        public static implicit operator NegatePredicate<TSource, TPredicate>(in TPredicate predicate) => new NegatePredicate<TSource, TPredicate>(predicate);
    }
}
