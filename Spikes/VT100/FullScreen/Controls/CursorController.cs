using System;
using System.Diagnostics;
using VT100.Utilities.ReadConsole;

namespace VT100.FullScreen.Controls
{
    internal class CursorController
    {
        public CursorController(int characterOffset, int visualOffset, int maxVisualOffset, int cursorPosX, int cursorPosY, int maxCharacterOffset, int initiualDataLength)
        {
            CharacterOffset = characterOffset;
            VisualOffset = visualOffset;
            MaxVisualOffset = maxVisualOffset;
            MaxCharacterOffset = maxCharacterOffset;
            CursorPosX = cursorPosX;
            CursorPosY = cursorPosY;
            RootCursorX = cursorPosX;
            RootCursorY = cursorPosY;
            DataLength = initiualDataLength;
        }

        public event EventHandler Redraw;
        public event EventHandler<(IFullScreenConsole Console, int X, int Y, int CharacterOffset)> MoveCursor;

        public int CharacterOffset { get; private set; }
        public int VisualOffset { get; private set; }
        public int MaxVisualOffset { get; private set; }
        public int MaxCharacterOffset { get; private set; }
        
        public int DataLength { get; private set; }
        public int CursorPosX { get; private set; }
        public int CursorPosY { get; private set; }
        public int RootCursorX { get; private set; }
        public int RootCursorY { get; private set; }

        public void AdvanceCursor(IFullScreenConsole console)
        {
            if (VisualOffset == MaxVisualOffset)
                CharacterOffset += 1;
            else
                VisualOffset += 1;
            ResetCursor(console);
        }

        public bool CursorControl(IFullScreenConsole console, ControlSequence key)
        {
            switch (key.ResolvedCode)
            {
                case ResolvedCode.CursorBackwards:
                    CursorBackward();
                    ResetCursor(console);

                    return true;

                case ResolvedCode.CursorForward:
                    CursorForward();
                    ResetCursor(console);

                    return true;

                case ResolvedCode.CursorUp:
                case ResolvedCode.CursorDown:
                    Debug.WriteLine($"Got {key.ResolvedCode}");
                    return true;
                
                case ResolvedCode.End:
                    if (CursorEndIfNotAlreadyThere())
                        ResetCursor(console);
                    return true;
                
                case ResolvedCode.Home:
                    if (CursorHomeIfNotAlreadyThere())
                        ResetCursor(console);
                    return true;
            }
            return false;
        }

        private void CursorBackward()
        {
            if (VisualOffset == 0 && CharacterOffset > 0)
                CharacterOffset -= 1;
            else if (VisualOffset > 0)
                VisualOffset -= 1;
        }

        private void CursorForward()
        {
            if (VisualOffset == MaxVisualOffset && CharacterOffset < MaxCharacterOffset)
                CharacterOffset += 1;
            else if (VisualOffset < MaxVisualOffset)
                VisualOffset += 1;
        }

        /// <summary>
        /// Move the cursor to the beginning of the text, if it's not already there.
        /// </summary>
        /// <returns>True if the cursor is moved.</returns>
        private bool CursorHomeIfNotAlreadyThere()
        {
            var moved = false;
            if (CharacterOffset > 0)
            {
                CharacterOffset = 0;
                moved = true;
            }

            if (VisualOffset > 0)
            {
                VisualOffset = 0;
                moved = true;
            }

            return moved;
        }

        /// <summary>
        /// Move the cursor to the end of the text, if it's not already there.
        /// </summary>
        /// <returns>True if the cursor is moved.</returns>
        private bool CursorEndIfNotAlreadyThere()
        {
            var moved = false;
            if (CharacterOffset < MaxCharacterOffset)
            {
                CharacterOffset = MaxCharacterOffset;
                moved = true;
            }

            if (MaxCharacterOffset == 0)
            {
                if (VisualOffset < DataLength)
                {
                    VisualOffset = DataLength;
                    moved = true;
                }
            }
            else if (VisualOffset < MaxVisualOffset)
            {
                VisualOffset = MaxVisualOffset;
                moved = true;
            }

            return moved;
        }

        private void ResetCursor(IFullScreenConsole console)
        {
            CursorPosX = RootCursorX + VisualOffset;
            CursorPosY = RootCursorY;
            MoveCursor?.Invoke(this, (console, CursorPosX, CursorPosY, CharacterOffset));
        }

        public int GetCharacterPosition()
        {
            return CharacterOffset + VisualOffset;
        }

        public void MoveCursorBack(IFullScreenConsole console, int n = 1)
        {
            var moved = false;
            for (int done = 0; done < n; ++done)
            {
                CursorBackward();
                moved = true;
            }
            if (moved)
                ResetCursor(console);
        }

        public void SetDataLength(int dataLength)
        {
            var difference = dataLength - DataLength;
            DataLength = dataLength;

            var newMaxCharacterOffset = dataLength > MaxVisualOffset
                ? MaxCharacterOffset + difference
                : 0;
            if (newMaxCharacterOffset < 0)
                MaxCharacterOffset = 0;
            else
                MaxCharacterOffset = newMaxCharacterOffset;
        }

        public void RefreshCursor(IFullScreenConsole console)
        {
            ResetCursor(console);
        }
    }
}