using System;
using System.Diagnostics;
using VT100.Utilities.ReadConsole;

namespace Vt100.FullScreen.Controls
{
    internal class CursorController
    {
        public CursorController(int characterOffset, int visualOffset, int maxVisualOffset, int cursorPosX, int cursorPosY, int maxCharacterOffset)
        {
            CharacterOffset = characterOffset;
            VisualOffset = visualOffset;
            MaxVisualOffset = maxVisualOffset;
            MaxCharacterOffset = maxCharacterOffset;
            CursorPosX = cursorPosX;
            CursorPosY = cursorPosY;
            RootCursorX = cursorPosX;
            RootCursorY = cursorPosY;
        }

        public event EventHandler Redraw;
        public event EventHandler<(int X, int Y, int CharacterOffset)> MoveCursor;

        public int CharacterOffset { get; private set; }
        public int VisualOffset { get; private set; }
        public int MaxVisualOffset { get; private set; }
        public int MaxCharacterOffset { get; }
        public int CursorPosX { get; private set; }
        public int CursorPosY { get; private set; }
        public int RootCursorX { get; private set; }
        public int RootCursorY { get; private set; }

        public void AdvanceCursor()
        {
            if (VisualOffset == MaxVisualOffset)
            {
                CharacterOffset += 1;
                ResetCursor();
            }
            else
            {
                VisualOffset += 1;
                ResetCursor();
            }
        }

        public bool CursorControl(ControlSequence key)
        {
            switch (key.ResolvedCode)
            {
                case ResolvedCode.CursorBackwards:
                    if (VisualOffset == 0 && CharacterOffset > 0)
                    {
                        CharacterOffset -= 1;
                        ResetCursor();
                    }
                    else if (VisualOffset > 0)
                    {
                        VisualOffset -= 1;
                        ResetCursor();
                    }

                    return true;

                case ResolvedCode.CursorForward:
                    if (VisualOffset == MaxVisualOffset && CharacterOffset < MaxVisualOffset)
                    {
                        CharacterOffset += 1;
                        ResetCursor();
                    }
                    else if (VisualOffset < MaxVisualOffset)
                    {
                        VisualOffset += 1;
                        ResetCursor();
                    }

                    return true;

                case ResolvedCode.CursorUp:
                case ResolvedCode.CursorDown:
                    Debug.WriteLine($"Got {key.ResolvedCode}");
                    return true;
            }
            return false;
        }

        private void ResetCursor()
        {
            CursorPosX = RootCursorX + VisualOffset;
            CursorPosY = RootCursorY;
            MoveCursor?.Invoke(this, (CursorPosX, CursorPosY, CharacterOffset));
        }
    }
}