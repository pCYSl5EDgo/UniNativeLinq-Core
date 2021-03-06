﻿namespace UniNativeLinq.Average
{
    public struct NullableInt32Average : IAverageOperator<int?, double?>
    {
        private double count;
        private bool @bool;
        public void Execute(ref double? arg0, ref int? arg1)
        {
            @bool = true;
            if (!arg1.HasValue) return;
            ++count;
            arg0 = (arg0 ?? default) + arg1;
        }

        public bool TryCalculateResult(double? accumulate, out double? result)
        {
            result = count == default ? default : accumulate / count;
            return @bool;
        }
    }
}