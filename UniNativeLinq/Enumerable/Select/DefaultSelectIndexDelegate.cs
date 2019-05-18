using System;

namespace UniNativeLinq
{
    public readonly struct
        DefaultSelectIndexDelegate<TPrevSource, TSource>
        : ISelectIndex<TPrevSource, TSource>
    {
        private readonly Func<TPrevSource, long, TSource> func;

        public DefaultSelectIndexDelegate(Func<TPrevSource, long, TSource> func) => this.func = func;

        public void Execute(ref TPrevSource source, long index, ref TSource result) => result = func(source, index);

        public static implicit operator DefaultSelectIndexDelegate<TPrevSource, TSource>(Func<TPrevSource, long, TSource> func) => new DefaultSelectIndexDelegate<TPrevSource, TSource>(func);
    }
}
