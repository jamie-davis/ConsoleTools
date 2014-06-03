using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ConsoleToolkit.ConsoleIO.Internal;

namespace ConsoleToolkit.ConsoleIO
{
    /// <summary>
    /// This class "seals" a set of lines with colours, so that the colour changes cannot "leak" out if the
    /// lines were used as a column. It also has the effect of covering up any excess colour Pushes that were 
    /// never popped. It cannot help with excessive pops.
    /// 
    /// Given valid input, each line in the result is effectively colour neutral, i.e. after displaying the line
    /// the console colours are the same as they were before displaying the line. Behaviour is undefined if the 
    /// input contains more pops than pushes (but most likely will not give the desired result). 
    /// </summary>
    internal static class ColourSeal
    {
        private struct ColourSet
        {
            public string ForegroundSetter;
            public string BackgroundSetter;
        }

        public static string[] Seal(string[] lines)
        {
            var push = MakeInstruction(AdapterConfiguration.PushInstruction);
            var pop = MakeInstruction(AdapterConfiguration.PopInstruction);

            var currentColours = default(ColourSet);
            var stack = new Stack<ColourSet>();
            var output = new List<string>();
            foreach (var line in lines)
            {
                var popRequired = false;
                var stackRecover = RegenStack(stack.Reverse()) + MakeColourSetRestore(currentColours);
                if (stackRecover != string.Empty)
                {
                    stackRecover = push + stackRecover;
                    popRequired = true;
                }                

                var colourInstructions = ColourControlSplitter.Split(line);
                foreach (var item in colourInstructions.Where(i => i.Text == null && i.Instructions != null))
                {
                    foreach (var instruction in item.Instructions)
                    {
                        if (!popRequired && instruction.Code != ColourControlItem.ControlCode.NewLine)
                        {
                            stackRecover = push + stackRecover;
                            popRequired = true;
                        }

                        switch (instruction.Code)
                        {
                            case ColourControlItem.ControlCode.Push:
                                stack.Push(currentColours);
                                break;
                            case ColourControlItem.ControlCode.Pop:
                                currentColours = stack.Pop();
                                break;
                            case ColourControlItem.ControlCode.SetForeground:
                                currentColours.ForegroundSetter = instruction.GenerateCode();
                                break;
                            case ColourControlItem.ControlCode.SetBackground:
                                currentColours.BackgroundSetter = instruction.GenerateCode();
                                break;
                        }
                    }
                }

                output.Add(stackRecover + line + GenerateSequence(pop, stack.Count + (popRequired ? 1 : 0)));
            }

            return output.ToArray();
        }

        private static string RegenStack(IEnumerable<ColourSet> stack)
        {
            var sb = new StringBuilder();

            foreach (var colourSet in stack)
            {
                var colourSetter = MakeColourSetRestore(colourSet);
                sb.Append(colourSetter);
                sb.Append(MakeInstruction(AdapterConfiguration.PushInstruction));
            }

            return sb.ToString();
        }

        private static string MakeColourSetRestore(ColourSet colourSet)
        {
            var colourSetter = string.Empty;
            if (colourSet.ForegroundSetter != null)
                colourSetter += MakeInstruction(colourSet.ForegroundSetter);
            if (colourSet.BackgroundSetter != null)
                colourSetter += MakeInstruction(colourSet.BackgroundSetter);
            return colourSetter;
        }

        private static string MakeInstruction(string setter)
        {
            return AdapterConfiguration.ControlSequenceIntroducer + setter + AdapterConfiguration.ControlSequenceTerminator;
        }

        private static string GenerateSequence(string element, int count)
        {
            if (count == 0) return string.Empty;
            return Enumerable.Repeat(element, count).Aggregate((t, i) => t + i);
        }
    }
}