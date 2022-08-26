using System.Collections.Generic;
using System.Linq;

namespace VT100.FullScreen.ScreenLayers
{
    internal static class BoxCharacterSelector
    {
        private static readonly List<BoxCharacter> _characters;

        private static readonly List<BoxCharacter> _topLeft;
        private static readonly List<BoxCharacter> _topRight;
        private static readonly List<BoxCharacter> _horizontals;
        private static readonly List<BoxCharacter> _verticals;
        private static readonly List<BoxCharacter> _bottomLeft;
        private static readonly List<BoxCharacter> _bottomRight;

        static BoxCharacterSelector()
        {
            _characters = MakeBoxCharacterSet();
            _topLeft = MakeSet(false, true, false, true);
            _bottomLeft = MakeSet(false, true, true, false);
            _horizontals = MakeSet(true, true, false, false);
            _verticals = MakeSet(false, false, true, true);
            _topRight = MakeSet(true, false, false, true);
            _bottomRight = MakeSet(true, false, true, false);
        }
        
        private static List<BoxCharacter> MakeBoxCharacterSet()
        {
            var allCharacters = BoxCharacterClassifier.Classify().Where(c => c.Unparsed.Count == 0);
            return allCharacters.ToList();
        }

        private static List<BoxCharacter> MakeSet(bool left, bool right, bool up, bool down)
        {
            bool AsRequired(bool required, object value)
            {
                return (required && value != null) || (!required && value == null);
            }
            
            return _characters
                    .Where(c => AsRequired(left, c.Left)
                                && AsRequired(right, c.Right)
                                && AsRequired(up, c.Up)
                                && AsRequired(down, c.Down))
                    .ToList();
        }
        
        public static BoxCharRequest SelectTopLeft(BoxRegion boxRegion)
        {
            return Select(_topLeft, boxRegion);
        }

        public static BoxCharRequest SelectHorizontal(BoxRegion boxRegion)
        {
            return Select(_horizontals, boxRegion);
        }
        
        public static BoxCharRequest SelectVertical(BoxRegion boxRegion)
        {
            return Select(_verticals, boxRegion);
        }

        public static BoxCharRequest SelectTopRight(BoxRegion boxRegion)
        {
            return Select(_topRight, boxRegion);
        }

        public static BoxCharRequest SelectBottomLeft(BoxRegion boxRegion)
        {
            return Select(_bottomLeft, boxRegion);
        }
        
        public static BoxCharRequest SelectBottomRight(BoxRegion boxRegion)
        {
            return Select(_bottomRight, boxRegion);
        }

        private static BoxCharacter ChooseBest(List<BoxCharacter> available, BoxRegion boxRegion)
        {
            var edge = new Edge(boxRegion.LineWeight, boxRegion.LineCount, boxRegion.DashType);
            var exact = available.FirstOrDefault(a => Match(a, edge));
            if (exact != null) return exact;
            return null;
        }

        private static bool Match(BoxCharacter boxCharacter, Edge edge)
        {
            return (boxCharacter.Left == null || boxCharacter.Left.Matches(edge))
                    && (boxCharacter.Right == null || boxCharacter.Right.Matches(edge))
                    && (boxCharacter.Up == null || boxCharacter.Up.Matches(edge))
                    && (boxCharacter.Down == null || boxCharacter.Down.Matches(edge));
        }

        private static BoxCharRequest Select(List<BoxCharacter> boxCharacters, BoxRegion boxRegion)
        {
            var character = ChooseBest(boxCharacters, boxRegion);
            if (character == null) return new BoxCharRequest();

            return new BoxCharRequest { Class = character };
        }
    }
}