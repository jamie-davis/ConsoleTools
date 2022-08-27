namespace VT100.FullScreen.ScreenLayers
{
    internal class Edge
    {
        public LineWeight LineWeight { get; }
        public LineCount LineCount { get; }
        public DashType DashType { get; }

        public Edge(LineWeight lineWeight, LineCount lineCount, DashType dashType)
        {
            LineWeight = lineWeight;
            LineCount = lineCount;
            DashType = dashType;
        }

        public override string ToString()
        {
            return $"{LineWeight} {LineCount} {DashType}";
        }

        public bool Matches(Edge edge)
        {
            if (edge == null) return false;
            return LineWeight == edge.LineWeight && LineCount == edge.LineCount && DashType == edge.DashType;
        }
    }
}