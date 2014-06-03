using System;
using System.Collections.Generic;
using ConsoleToolkit.ConsoleIO.Internal;

namespace ConsoleToolkit.ConsoleIO
{
    /// <summary>
    /// This class performs write operations on a <see cref="IConsoleInterface"/>. It implements all of the colour 
    /// instructions that control the display of output on the console.
    /// <seealso cref="ColourControlItem"/>
    /// <seealso cref="ColourControlItem.ControlCode"/>
    /// </summary>
    internal class ColourWriter
    {
        private readonly IConsoleInterface _consoleInterface;

        private class ColourState
        {
            public ConsoleColor ForegroundColour;
            public ConsoleColor BackgroundColour;

            public ColourState(ConsoleColor foregroundColour, ConsoleColor backgroundColour)
            {
                ForegroundColour = foregroundColour;
                BackgroundColour = backgroundColour;
            }
        }

        private readonly Stack<ColourState> _colourStack = new Stack<ColourState>();
        private bool _lastWriteWasPassiveNewLine;

        public ColourWriter(IConsoleInterface consoleInterface)
        {
            _consoleInterface = consoleInterface;
        }

        public void Write(List<ColourControlItem> components)
        {
            foreach (var colourControlItem in components)
            {
                ProcessItem(colourControlItem);
            }
        }

        public void NewLine()
        {
            _consoleInterface.NewLine();
        }

        private void ProcessItem(ColourControlItem colourControlItem)
        {
            if (colourControlItem.Text != null)
                WriteText(colourControlItem);
            else
            {
                foreach (var instruction in colourControlItem.Instructions)
                {
                    switch (instruction.Code)
                    {
                        case ColourControlItem.ControlCode.SetBackground:
                            _consoleInterface.Background = instruction.Arg;
                            break;

                        case ColourControlItem.ControlCode.SetForeground:
                            _consoleInterface.Foreground = instruction.Arg;
                            break;

                        case ColourControlItem.ControlCode.NewLine:
                            if (!_lastWriteWasPassiveNewLine)
                                _consoleInterface.NewLine();
                            else
                                _lastWriteWasPassiveNewLine = false;
                            break;

                        case ColourControlItem.ControlCode.Push:
                            _colourStack.Push(new ColourState(_consoleInterface.Foreground, _consoleInterface.Background));
                            break;

                        case ColourControlItem.ControlCode.Pop:
                            var popped = _colourStack.Pop();
                            _consoleInterface.Foreground = popped.ForegroundColour;
                            _consoleInterface.Background = popped.BackgroundColour;
                            break;
                    }
                }
            }
        }

        private void WriteText(ColourControlItem colourControlItem)
        {
            var currentLine = _consoleInterface.CursorTop;
            _consoleInterface.Write(colourControlItem.Text);
            _lastWriteWasPassiveNewLine = (_consoleInterface.CursorTop == currentLine + 1 &&
                                           _consoleInterface.CursorLeft == 0);
        }
    }
}