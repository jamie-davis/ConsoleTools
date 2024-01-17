namespace VT100.FullScreen
{
    internal class MeasurementOutcome
    {
        public MeasurementOutcome((int width, int height) minDimensions, (int width, int height) maxDimensions)
        {
            MinDimensions = minDimensions;
            MaxDimensions = maxDimensions;
        }

        public (int width, int height) MinDimensions { get; }
        public (int width, int height) MaxDimensions { get; }
    }
}