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
        private static readonly List<BoxCharacter> _topJunction;
        private static readonly List<BoxCharacter> _leftJunction;
        private static readonly List<BoxCharacter> _bottomJunction;
        private static readonly List<BoxCharacter> _rightJunction;
        private static readonly List<BoxCharacter> _cross;

        static BoxCharacterSelector()
        {
            _characters = MakeBoxCharacterSet();
            _topLeft = MakeSet(false, true, false, true);
            _bottomLeft = MakeSet(false, true, true, false);
            _horizontals = MakeSet(true, true, false, false);
            _verticals = MakeSet(false, false, true, true);
            _topRight = MakeSet(true, false, false, true);
            _bottomRight = MakeSet(true, false, true, false);
            _leftJunction = MakeSet(false, true, true, true);
            _rightJunction = MakeSet(true, false, true, true);
            _topJunction = MakeSet(true, true, false, true);
            _bottomJunction = MakeSet(true, true, true, false);
            _cross = MakeSet(true, true, true, true);
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
            return SelectForRegion(_topLeft, boxRegion);
        }

        public static BoxCharRequest SelectHorizontal(BoxRegion boxRegion)
        {
            return SelectForRegion(_horizontals, boxRegion);
        }
        
        public static BoxCharRequest SelectVertical(BoxRegion boxRegion)
        {
            return SelectForRegion(_verticals, boxRegion);
        }

        public static BoxCharRequest SelectTopRight(BoxRegion boxRegion)
        {
            return SelectForRegion(_topRight, boxRegion);
        }

        public static BoxCharRequest SelectBottomLeft(BoxRegion boxRegion)
        {
            return SelectForRegion(_bottomLeft, boxRegion);
        }
        
        public static BoxCharRequest SelectBottomRight(BoxRegion boxRegion)
        {
            return SelectForRegion(_bottomRight, boxRegion);
        }

        private static BoxCharacter ChooseBest(List<BoxCharacter> available, Edge edge)
        {
            var exact = available.FirstOrDefault(a => Match(a, edge, edge, edge, edge));
            if (exact != null) return exact;
            return null;
        }

        private static BoxCharacter ChooseBest(List<BoxCharacter> available, Edge left, Edge right, Edge up, Edge down)
        {
            var exact = available.FirstOrDefault(a => Match(a, left, right, up, down));
            if (exact != null) return exact;
            return null;
        }

        private static bool Match(BoxCharacter boxCharacter, Edge left, Edge right, Edge up, Edge down)
        {
            return (boxCharacter.Left == null || boxCharacter.Left.Matches(left))
                    && (boxCharacter.Right == null || boxCharacter.Right.Matches(right))
                    && (boxCharacter.Up == null || boxCharacter.Up.Matches(up))
                    && (boxCharacter.Down == null || boxCharacter.Down.Matches(down));
        }

        private static BoxCharRequest SelectForRegion(List<BoxCharacter> boxCharacters, BoxRegion boxRegion)
        {
            var edge = new Edge(boxRegion.LineWeight, boxRegion.LineCount, boxRegion.DashType);
            var character = ChooseBest(boxCharacters, edge);
            if (character == null) return new BoxCharRequest();

            return new BoxCharRequest
            {
                Class = character,
                RequestedCornerType = character.CornerType,
                RequestedLeft = character.Left != null ? edge : null,
                RequestedRight = character.Right != null ? edge : null,
                RequestedUp = character.Up != null ? edge : null,
                RequestedDown = character.Down != null ? edge : null,
                
            };
        }

        public static BoxCharacter Select(BoxCharRequest boxCharacter)
        {
            var set = ChooseSet(boxCharacter);
            return ChooseBest(set, boxCharacter.RequestedLeft, boxCharacter.RequestedRight, boxCharacter.RequestedUp,
                boxCharacter.RequestedDown);
        }

        private static List<BoxCharacter> ChooseSet(BoxCharRequest boxCharacter)
        {
            var left = boxCharacter.RequestedLeft != null;
            var right = boxCharacter.RequestedRight != null;
            var up = boxCharacter.RequestedUp != null;
            var down = boxCharacter.RequestedDown != null;

            if (!left && right && !up && down) return _topLeft;
            if (left && !right && !up && down) return _topRight;
            if (!left && right && up && !down) return _bottomLeft;
            if (left && !right && up && !down) return _bottomRight;
            if (!left && right && up && down) return _leftJunction;
            if (left && !right && up && down) return _rightJunction;
            if (left && right && !up && down) return _topJunction;
            if (left && right && up && !down) return _bottomJunction;
            if (left && right && !up && !down) return _horizontals;
            if (!left && !right && up && down) return _verticals;
            return _cross;
        }
    }
}