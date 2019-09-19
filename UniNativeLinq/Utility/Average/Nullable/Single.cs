namespace UniNativeLinq.Average
{
    public struct NullableSingleAverage : IAverageOperator<float?, float?>
    {
        private float count;
        private bool @bool;
        public void Execute(ref float? arg0, ref float? arg1)
        {
            @bool = true;
            if (!arg1.HasValue) return;
            ++count;
            arg0 = (arg0 ?? default) + arg1;
        }

        public bool TryCalculateResult(float? accumulate, out float? result)
        {
            result = count == default ? default : accumulate / count;
            return @bool;
        }
    }
}