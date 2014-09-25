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
        private ConsoleColor _prefixForeground;
        private ConsoleColor _prefixBackground;

        public ColourWriter(IConsoleOutInterface consoleOutInterface)
        {
            _consoleOutInterface = consoleOutInterface;
            _prefixForeground = _consoleOutInterface.Foreground;
            _prefixBackground = _consoleOutInterface.Background;
        }

        public Encoding Encoding {
            get { return _consoleOutInterface.Encoding; }
        }

        public string PrefixText { get; set; }

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
            if (PrefixText != null)
                WriteTextWithPrefix(colourControlItem);
            else
                _consoleOutInterface.Write(colourControlItem.Text);

            _lastWriteWasPassiveNewLine = (_consoleOutInterface.CursorTop == currentLine + 1 &&
                                           _consoleOutInterface.CursorLeft == 0);
        }

        private void WriteTextWithPrefix(ColourControlItem colourControlItem)
        {
            var text = colourControlItem.Text;
            var remaining = text.Length;
            var textPos = 0;
            do
            {
                if (_consoleOutInterface.CursorLeft == 0)
                {
                    var currentFG = _consoleOutInterface.Foreground;
                    var currentBG = _consoleOutInterface.Background;
                    _consoleOutInterface.Foreground = _prefixForeground;
                    _consoleOutInterface.Background = _prefixBackground;
                    _consoleOutInterface.Write(PrefixText);
                    _consoleOutInterface.Foreground = currentFG;
                    _consoleOutInterface.Background = currentBG;
                }

                var available = _consoleOutInterface.BufferWidth - _consoleOutInterface.CursorLeft;
                var section = remaining > available ? text.Substring(textPos, available) : text.Substring(textPos);

                _consoleOutInterface.Write(section);
                textPos += section.Length;
                remaining -= section.Length;
            } while (remaining > 0);
        }
    }
}