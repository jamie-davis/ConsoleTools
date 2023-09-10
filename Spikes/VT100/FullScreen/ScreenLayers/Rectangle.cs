using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace VT100.FullScreen.ScreenLayers
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class Rectangle
    {
        private readonly Lazy<string> _debuggerDisplay;

        public Rectangle((int left, int top) topLeftPoint, (int width, int height) size, (int col, int row) keyPoint)
        {
            Top = topLeftPoint.top;
            Left = topLeftPoint.left;
            Width = size.width;
            Height = size.height;
            KeyCol = keyPoint.col;
            KeyRow = keyPoint.row;
            _debuggerDisplay = new Lazy<string>(Draw);
        }

        public int Top { get; }
        public int Left { get; }
        public int Width { get; }
        public int Height { get; }
        public int KeyCol { get; }
        public int KeyRow { get; }

        public string Describe()
        {
            return $"@({Left},{Top}) (W:{Width} H:{Height}){(IsDegenerate() ? "!" : string.Empty)} K({KeyCol},{KeyRow}){(KeyCol >= Left + Width || KeyCol < Left || KeyRow >= Top + Height || KeyRow < Top ? "!" : string.Empty)}";
        }

        public Rectangle Intersect(Rectangle other)
        {
            var left = Math.Max(Left, other.Left);
            var top = Math.Max(Top, other.Top);
            var width = Math.Min(Left + Width, other.Left + other.Width) - left;
            var height = Math.Min(Top + Height, other.Top + other.Height) - top;
            return new Rectangle((left, top), (width, height), (KeyCol, KeyRow));
        }

        public bool IsDegenerate()
        {
            return Width <= 0 || Height <= 0;
        }

        private string DebuggerDisplay => _debuggerDisplay.Value;

        private string Draw()
        {
            var sb = new StringBuilder();
            sb.AppendLine(Describe());
            var topBottom = new string('X', Width > 0 ? Width : 1);
            var nonKeyRow = "X" + new string(' ', Width > 2 ? Width - 2 : 1) + "X";
            for (int i = 0; i < Height && Width >= 0; i++)
            {
                var row = nonKeyRow;
                if (i == 0 || i == Height - 1)
                    row = topBottom;

                if (i == KeyRow - Top)
                    row = row.Substring(0, KeyCol - Left) + "@" + row.Substring(KeyCol - Left, Width - (KeyCol - Left) - 1);

                sb.AppendLine(row);
            }
            return sb.ToString();
        }
    }
    
}