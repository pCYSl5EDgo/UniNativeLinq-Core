using System.Collections;

namespace UniNativeLinq
{
    public static class EnumeratorHelper
    {
        public static long Count<TEnumerator>(this TEnumerator enumerator)
            where TEnumerator : struct, IEnumerator
        {
            var count = 0L;
            while (enumerator.MoveNext())
                ++count;
            return count;
        }
    }
}
