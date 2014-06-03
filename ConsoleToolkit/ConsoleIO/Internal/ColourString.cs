using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    internal static class ColourString
    {
        private class Segment
        {
            public string Text;
            public string ColourInstructions;
        }

        public static int Length(string data)
        {
            var segments = GetSegments(data);
            return segments.Where(s => s.Text != null).Sum(s => s.Text.Length);
        }

        public static string Substring(string data, int start, int length)
        {
            return ExtractSection(data, start, length, false);
        }

        public static string Substring(string data, int start)
        {
            return ExtractSection(data, start, 0, true);
        }

        private static IEnumerable<Segment> GetSegments(string data)
        {
            var segments = new List<Segment>();
            var pos = 0;
            var prevPos = 0;
            while ((pos = data.IndexOf(AdapterConfiguration.ControlSequenceIntroducer, prevPos)) >= 0)
            {
                var text = prevPos < pos ? data.Substring(prevPos, pos - prevPos) : null;
                var end = data.IndexOf(AdapterConfiguration.ControlSequenceTerminator, pos) + 1;
                var codes = data.Substring(pos, end - pos);

                if (text != null)
                    segments.Add(new Segment {Text = text});

                segments.Add(new Segment{ ColourInstructions = codes});

                prevPos = end;
            }

            if (prevPos < data.Length)
                segments.Add(new Segment {Text = data.Substring(prevPos)});

            return segments;
        }

        private static string ExtractSection(string data, int start, int length, bool toEnd)
        {
            var pos = 0;
            var sb = new StringBuilder();
            foreach (var segment in GetSegments(data))
            {
                if (segment.Text == null && segment.ColourInstructions != null)
                    sb.Append(segment.ColourInstructions);
                else if (segment.Text != null)
                {
                    var allowable = GetAllowableSegmentPart(segment, pos, start, length, toEnd);
                    if (allowable != null)
                    {
                        sb.Append(allowable);
                    }
                    pos += segment.Text.Length;
                }
            }

            return sb.ToString();
        }

        private static string GetAllowableSegmentPart(Segment segment, int pos, 
            int extractStart, int extractLength, bool extractToEnd)
        {
            var segmentMin = pos;
            var segmentMax = pos + segment.Text.Length;
            var extractMax = extractStart + (extractToEnd ? segmentMax : extractLength);
            var startIsInsideSegment = extractStart >= segmentMin && extractStart <= segmentMax;
            var endIsInsideSegment = extractMax >= segmentMin && extractMax <= segmentMax;
            var segmentIsInsideRange = extractStart <= segmentMin && extractMax >= segmentMax;

            if (segmentIsInsideRange)
                return segment.Text;

            if (startIsInsideSegment && endIsInsideSegment)
            {
                var fragmentStart = extractStart - segmentMin;
                var fragmentLength = extractMax - extractStart;
                return segment.Text.Substring(fragmentStart, fragmentLength);
            }

            if (startIsInsideSegment)
                return segment.Text.Substring(extractStart - segmentMin);
            if (endIsInsideSegment) 
                return segment.Text.Substring(0, extractMax - segmentMin);

            return null;
        }
    }
}