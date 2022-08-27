using System.Collections.Generic;

namespace VT100.FullScreen.ScreenLayers
{
    internal class BoxCharacter
    {
        public CornerType CornerType { get; }
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
}