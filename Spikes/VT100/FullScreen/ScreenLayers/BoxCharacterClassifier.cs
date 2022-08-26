using System;
using System.Collections.Generic;
using System.Linq;

namespace VT100.FullScreen.ScreenLayers
{
    /// <summary>
    /// Analyse the box character enumeration and extract the characteristic combinations represented.
    /// </summary>
    internal static class BoxCharacterClassifier
    {
        public static IEnumerable<BoxCharacter> Classify()
        {
            var members = Enum.GetNames(typeof(BoxCharacterType));
            var boxCharacters = members.Select(i => ParseEnumElement(i));
            return boxCharacters;
        }

        private static BoxCharacter ParseEnumElement(string memberName)
        {
            var identifiers = new[]
            {
                "LEFT", "RIGHT", "UP", "DOWN", "HORIZONTAL", "VERTICAL"
            };

            CornerType cornerType = CornerType.Box;
            var left = (Edge)null;;
            var right = left;
            var up = left;
            var down = left;
            var unparsed = new List<string>();

            var words = memberName.Split('_').ToList();

            bool TryTakeArcSpec()
            {
                SkipWeight(0, out var arcIx, out LineWeight weight);
                if (words.Count <= arcIx || words[arcIx] != "ARC") return false;
                var edgeIdentifiers = new List<string>();
                var edgeIx = arcIx + 1;
                do
                {
                    if (SkipMandatoryEdge(edgeIx, out var newIx, out var newEdgeIdentifiers))
                    {
                        edgeIdentifiers.AddRange(newEdgeIdentifiers);
                    }
                    else break;

                    if (words.Count > newIx && words[newIx] == "AND")
                        ++newIx;

                    edgeIx = newIx;
                } while (words.Count > edgeIx);

                if (!edgeIdentifiers.Any()) return false;

                var edge = new Edge(weight, LineCount.Single, DashType.None);
                foreach (var edgeIdentifier in edgeIdentifiers)
                {
                    ApplyEdge(edgeIdentifier, edge);
                }
                
                cornerType = CornerType.Arc;
                words.RemoveRange(0, edgeIx);
                return true;
            }

            bool TryTakeEdgeSuffixSpec()
            {
                if (SkipWeight(0, out var dashIx, out LineWeight weight)
                    && SkipDash(dashIx, out var countIx, out DashType dash)
                    && SkipLineCount(countIx, out var horzIx, out LineCount lineCount)
                    && words.Count > horzIx)
                {
                    var edgesSet = false;
                    var edge = new Edge(weight, lineCount, dash);

                    while (words.Count > horzIx)
                    {
                        var edgeName = identifiers.FirstOrDefault(i => i == words[horzIx]);
                        if (edgeName == null) break;
                        
                        ApplyEdge(edgeName, edge);
                        
                        edgesSet = true;

                        ++horzIx;
                        if (words.Count == horzIx + 2 && words[horzIx] == "AND" && identifiers.Contains(words[horzIx + 1]))
                            ++horzIx;
                        else 
                            break;
                    }

                    if (edgesSet)
                        words.RemoveRange(0, horzIx);
                    return edgesSet;
                }
                return false;
            }

            bool TryTakeEdgeSpec()
            {
                if (SkipMandatoryEdge(0, out var weightIx, out List<string> edgeIdentifiers)
                    && SkipWeight(weightIx, out var dashIx, out LineWeight weight)
                    && SkipDash(dashIx, out var countIx, out DashType dash)
                    && SkipLineCount(countIx, out var finalIx, out LineCount lineCount)
                    && finalIx > 1)
                {
                    var edge = new Edge(weight, lineCount, dash); 
                    words.RemoveRange(0, finalIx);
                    foreach (var edgeIdentifier in edgeIdentifiers)
                    {
                        ApplyEdge(edgeIdentifier, edge);
                    }

                    return true;
                }
                return false;
            }

            bool SkipWeight(int index, out int outIndex, out LineWeight lineWeight)
            {
                outIndex = index;
                lineWeight = LineWeight.Light;
                if (words.Count <= index) return true;

                if (words[index] == "LIGHT")
                {
                    outIndex++;
                    return true;
                }

                if (words[index] == "HEAVY")
                {
                    outIndex++;
                    lineWeight = LineWeight.Heavy;
                    return true;
                }

                return true;
            }

            bool SkipLineCount(int index, out int outIndex, out LineCount lineCount)
            {
                outIndex = index;
                lineCount = LineCount.Single;
                if (words.Count <= index) return true;

                if (words[index] == "SINGLE")
                {
                    outIndex++;
                    return true;
                }

                if (words[index] == "DOUBLE")
                {
                    lineCount = LineCount.Double;
                    outIndex++;
                    return true;
                }

                return true;
            }

            bool SkipMandatoryEdge(int index, out int outIndex, out List<string> edgeIdentifiers)
            {
                outIndex = index;
                edgeIdentifiers = new List<string>();
                if (words.Count <= index) return true;

                do
                {
                    var word = words[outIndex];
                    var thisIdentifier = identifiers.FirstOrDefault(i => i == word);
                    if (thisIdentifier == null) break;

                    edgeIdentifiers.Add(thisIdentifier);
                    outIndex++;
                } while (outIndex < words.Count);

                return edgeIdentifiers.Any(); //edge name is mandatory - therefore if we don't find it, we return the fact
            }

            bool SkipDash(int index, out int outIndex, out DashType dashType)
            {
                outIndex = index;
                dashType = DashType.None;
                if (words.Count <= index + 1) return true;

                if (words[index + 1] != "DASH") return true;

                if (words[index] == "DOUBLE")
                {
                    outIndex += 2;
                    dashType = DashType.Double;
                    return true;
                }

                if (words[index] == "TRIPLE")
                {
                    outIndex += 2;
                    dashType = DashType.Triple;
                    return true;
                }

                if (words[index] == "QUADRUPLE")
                {
                    outIndex += 2;
                    dashType = DashType.Quadruple;
                    return true;
                }

                return true;
            }
            
            void ApplyEdge(string edgeName, Edge edge)
            {
                switch (edgeName)
                {
                    case "LEFT":
                        left = edge;
                        break;

                    case "RIGHT":
                        right = edge;
                        break;

                    case "UP":
                        up = edge;
                        break;

                    case "DOWN":
                        down = edge;
                        break;

                    case "HORIZONTAL":
                        right = edge;
                        left = edge;
                        break;

                    case "VERTICAL":
                        up = edge;
                        down = edge;
                        break;
                }
            }

            while (words.Any())
            {
                if (!TryTakeArcSpec()
                    && !TryTakeEdgeSpec()
                    && !TryTakeEdgeSuffixSpec())
                {
                    var unparsedWord = words[0];
                    if (unparsedWord != "AND")
                        unparsed.Add(unparsedWord);
                    words.RemoveAt(0);
                }
            }

            var boxCharacterType = (BoxCharacterType)Enum.GetValues(typeof(BoxCharacterType)).OfType<object>().Single(e => Enum.GetName(typeof(BoxCharacterType), e) == memberName);

            return new BoxCharacter(boxCharacterType, cornerType, left, right, up, down, unparsed);
        }

    }

    internal enum LineWeight
    {
        Light,
        Heavy
    }

    internal enum LineCount
    {
        Single, 
        Double,
    }

    internal enum CornerType
    {
        Box,
        Arc
    }
    
    internal enum DashType
    {
        None,
        Double,
        Triple,
        Quadruple
    }
    
    internal class BoxCharacter
    {
        public CornerType CornerType { get; }
        public LineCount LineCount { get; }
        public Edge Left { get; }
        public Edge Right { get; }
        public Edge Up { get; }
        public Edge Down { get; }
        public List<string> Unparsed { get; }
        public BoxCharacterType Source { get; }

        public BoxCharacter(BoxCharacterType source, CornerType cornerType, Edge left, Edge right, Edge up, Edge down, List<string> unparsed)
        {
            Source = source;
            CornerType = cornerType;
            Left = left;
            Right = right;
            Up = up;
            Down = down;
            Unparsed = unparsed;
        }    
        
    }

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
            return LineWeight == edge.LineWeight && LineCount == edge.LineCount && DashType == edge.DashType;
        }
    }
}