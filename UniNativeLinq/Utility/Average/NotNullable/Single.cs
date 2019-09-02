using System;

namespace UniNativeLinq.Average
{
    public struct SingleAverage : IAverageOperator<Single, float>
    {
        private float count;
        public void Execute(ref float arg0, ref Single arg1)
        {
            arg0 += arg1;
            ++count;
        }

        public bool TryCalculateResult(float accumulate, out float result)
        {
            if (count == default)
            {
                result = default;
                return false;
            }
            result = accumulate / count;
            return true;
        }
    }
}