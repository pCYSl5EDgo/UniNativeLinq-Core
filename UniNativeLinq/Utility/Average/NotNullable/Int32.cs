namespace UniNativeLinq.Average
{
    public struct Int32Average : IAverageOperator<int, double>
    {
        private double count;
        public void Execute(ref double arg0, ref int arg1)
        {
            arg0 += arg1;
            ++count;
        }

        public bool TryCalculateResult(double accumulate, out double result)
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