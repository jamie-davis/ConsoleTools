using System;
using System.Collections.Generic;
using System.Text;
using ConsoleToolkit.ConsoleIO.Internal;

namespace ConsoleToolkit.ConsoleIO
{
    /// <summary>
    /// This class performs write operations on a <see cref="IConsoleOutInterface"/>. It implements all of the colour 
    /// instructions that control the display of output on the console.
    /// <seealso cref="ColourControlItem"/>
    /// <seealso cref="ColourControlItem.ControlCode"/>
    /// </summary>
    internal class ColourWriter
    {
        private readonly IConsoleOutInterface _consoleOutInterface;

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

        public ColourWriter(IConsoleOutInterface consoleOutInterface)
        {
            _consoleOutInterface = consoleOutInterface;
        }

        public Encoding Encoding {
            get { return _consoleOutInterface.Encoding; }
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
            _consoleOutInterface.NewLine();
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
                            _consoleOutInterface.Background = instruction.Arg;
                            break;

                        case ColourControlItem.ControlCode.SetForeground:
                            _consoleOutInterface.Foreground = instruction.Arg;
                            break;

                        case ColourControlItem.ControlCode.NewLine:
                            if (!_lastWriteWasPassiveNewLine)
                                _consoleOutInterface.NewLine();
                            else
                                _lastWriteWasPassiveNewLine = false;
                            break;

                        case ColourControlItem.ControlCode.Push:
                            _colourStack.Push(new ColourState(_consoleOutInterface.Foreground, _consoleOutInterface.Background));
                            break;

                        case ColourControlItem.ControlCode.Pop:
                            var popped = _colourStack.Pop();
                            _consoleOutInterface.Foreground = popped.ForegroundColour;
                            _consoleOutInterface.Background = popped.BackgroundColour;
                            break;
                    }
                }
            }
        }

        private void WriteText(ColourControlItem colourControlItem)
        {
            var currentLine = _consoleOutInterface.CursorTop;
            _consoleOutInterface.Write(colourControlItem.Text);
            _lastWriteWasPassiveNewLine = (_consoleOutInterface.CursorTop == currentLine + 1 &&
                                           _consoleOutInterface.CursorLeft == 0);
        }
    }
}