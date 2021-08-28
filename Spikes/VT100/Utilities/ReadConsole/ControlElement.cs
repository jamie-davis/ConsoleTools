using System;

namespace VT100.Utilities.ReadConsole
{
    internal class ControlElement
    {
        public ControlElement()
        {
            
        }

        public ControlElement(ConsoleKeyInfo key)
        {
            KeyChar = key.KeyChar;
            Modifiers = key.Modifiers;
            Key = key.Key;
        }

        public char KeyChar { get; set; }
        public ConsoleModifiers Modifiers { get; set; }
        public ConsoleKey Key { get; set; }    }

    internal class KeyInfo
    {

    }
}