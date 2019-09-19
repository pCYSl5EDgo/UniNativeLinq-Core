namespace UniNativeLinq.Average
{
    public struct NullableDoubleAverage : IAverageOperator<double?, double?>
    {
        private double count;
        private bool @bool;
        public void Execute(ref double? arg0, ref double? arg1)
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