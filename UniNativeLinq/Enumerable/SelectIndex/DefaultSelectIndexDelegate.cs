using System;

namespace UniNativeLinq
{
    public readonly struct
        DefaultSelectIndexDelegate<TPrev, T>
        : ISelectIndex<TPrev, T>
    {
        private readonly Func<TPrev, long, T> func;

        public DefaultSelectIndexDelegate(Func<TPrev, long, T> func) => this.func = func;

        public void Execute(ref TPrev source, long index, ref T result) => result = func(source, index);

        public static implicit operator DefaultSelectIndexDelegate<TPrev, T>(Func<TPrev, long, T> func) => new DefaultSelectIndexDelegate<TPrev, T>(func);
    }
}
