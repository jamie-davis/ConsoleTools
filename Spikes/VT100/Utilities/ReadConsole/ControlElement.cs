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
            Key = new KeyInfo();
            Key.KeyChar = key.KeyChar;
            Key.Modifiers = key.Modifiers;
            Key.Key = key.Key;
        }

        public KeyInfo Key { get; set; }
    }

    internal class KeyInfo
    {
        public char KeyChar { get; set; }
        public ConsoleModifiers Modifiers { get; set; }
        public ConsoleKey Key { get; set; }
    }
}