﻿namespace VT100.FullScreen.ScreenLayers
{
    /// <summary>
    /// Immutable class containing a box map generated by <see cref="BoxMapMaker"/>
    /// </summary>
    internal class BoxMap
    {
        private readonly int _width;
        public BoxCharRequest[] BoxCharacters { get; }

        public BoxMap(BoxCharRequest[] boxCharacters, int width)
        {
            _width = width;
            BoxCharacters = boxCharacters;
        }

        public BoxCharRequest GetAt(int x, int y)
        {
            var ix = CharacterArrayIndexCalculator.GetIndex(x, y, _width);
            if (ix < BoxCharacters.Length)
                return BoxCharacters[ix];

            return default;
        }
    }
}