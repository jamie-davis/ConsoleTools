using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    internal class ColourControlItem
    {
        public enum ControlCode
        {
            Push,
            Pop,
            SetForeground,
            SetBackground,
            NewLine
        }

        public class ControlInstruction
        {
            public ControlCode Code { get; private set; }
            public ConsoleColor Arg { get; private set; }

            public ControlInstruction(ControlCode code, ConsoleColor arg = ConsoleColor.Black)
            {
                Code = code;
                Arg = arg;
            }

            public override string ToString()
            {
                return string.Format("{0}{1}", Code, 
                    (Code == ControlCode.Push || Code == ControlCode.Pop) ? string.Empty : " " + Arg);
            }

            public string GenerateCode()
            {
                switch (Code)
                {
                    case ControlCode.Push:
                        return AdapterConfiguration.PushInstruction;

                    case ControlCode.Pop:
                        return AdapterConfiguration.PopInstruction;

                    case ControlCode.SetForeground:
                        return AdapterConfiguration.SetForegroundInstruction + AdapterConfiguration.ConsoleColourToCode(Arg);

                    case ControlCode.SetBackground:
                        return AdapterConfiguration.SetBackgroundInstruction + AdapterConfiguration.ConsoleColourToCode(Arg);

                    case ControlCode.NewLine:
                        return Environment.NewLine;

                    default:
                        return string.Empty;
                }
            }
        }

        public string Text { get; set; }

        public List<ControlInstruction> Instructions { get; private set; }

        public bool PrefixAffinity { get; private set; }

        public bool SuffixAffinity { get; private set; }

        public ColourControlItem(string text = null, IEnumerable<ControlInstruction> instructions = null, 
            bool prefixAffinity = false, bool suffixAffinity = false)
        {
            Text = text;
            if (instructions != null)
                Instructions = instructions.ToList();

            PrefixAffinity = prefixAffinity;
            SuffixAffinity = !PrefixAffinity && suffixAffinity;

#if DEBUG
            if (text == null && instructions == null)
                throw new ArgumentException("No arguments specified.");

            if (text != null && instructions != null)
                throw new ArgumentException("Only text or instructions should be specified.");
#endif
        }

        public override string ToString()
        {
            return string.Format("ControlInstruction: Text:\"{0}\" Instructions:[{1}]{2}", Text, 
                string.Join(",", Instructions ?? new List<ControlInstruction>()), 
                PrefixAffinity ? ">" : (SuffixAffinity ? "<" : string.Empty));
        }
    }
}