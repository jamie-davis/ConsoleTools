using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    internal static class ColourControlSplitter
    {
        public static List<ColourControlItem> Split(string data)
        {
            var controlItems = new List<ColourControlItem>();
            var components = data.Split(AdapterConfiguration.ControlSequenceIntroducer);
            foreach (var component in components.Where(c => c.Length > 0))
            {
                string text;
                var parts = component.Split(AdapterConfiguration.ControlSequenceTerminator);
                if (parts.Length == 2)
                {
                    controlItems.Add(GetInstructionItem(parts[0]));
                    text = parts[1];
                }
                else
                    text = parts[0];

                if (text.Length > 0)
                    AddTextControlItems(controlItems, text);
            }

            return controlItems;
        }

        private static void AddTextControlItems(List<ColourControlItem> controlItems, string text)
        {
            while (text.Length > 0)
            {
                var newLinePos = text.IndexOfAny(Environment.NewLine.ToCharArray());
                if (newLinePos >= 0)
                {
                    if (newLinePos > 0)
                        controlItems.Add(new ColourControlItem(text.Substring(0, newLinePos)));

                    controlItems.Add(
                        new ColourControlItem(instructions: new[]
                        {
                            new ColourControlItem.ControlInstruction(ColourControlItem.ControlCode.NewLine)
                        }));

                    text = text.Substring(newLinePos).StartsWith(Environment.NewLine) 
                        ? text.Substring(newLinePos + Environment.NewLine.Length)
                        : text.Substring(newLinePos + 1);
                }
                else
                {
                    controlItems.Add(new ColourControlItem(text));
                    break;
                }
            }
        }

        private static ColourControlItem GetInstructionItem(string instructions)
        {
            var parsedInstructions = new List<ColourControlItem.ControlInstruction>();
            var index = 0;
            bool prefix = false, suffix = false;
            while (index < instructions.Length)
            {
                var instructionCode = instructions[index++];
                if (instructionCode == AdapterConfiguration.PushInstruction[0])
                    parsedInstructions.Add(new ColourControlItem.ControlInstruction(ColourControlItem.ControlCode.Push));
                else if (instructionCode == AdapterConfiguration.PrefixAffinity[0])
                    prefix = true;                    
                else if (instructionCode == AdapterConfiguration.SuffixAffinity[0])
                    suffix = true;                    
                else if (instructionCode == AdapterConfiguration.PopInstruction[0])
                    parsedInstructions.Add(new ColourControlItem.ControlInstruction(ColourControlItem.ControlCode.Pop));
                else
                {
                    if (index < instructions.Length)
                    {
                        var arg = ColourConverter.Convert(instructions[index++].ToString());
                        var code = (instructionCode == AdapterConfiguration.SetForegroundInstruction[0])
                            ? ColourControlItem.ControlCode.SetForeground
                            : ColourControlItem.ControlCode.SetBackground;
                        parsedInstructions.Add(new ColourControlItem.ControlInstruction(code, arg));
                    }
                }
            }

            return new ColourControlItem(instructions: parsedInstructions, prefixAffinity: prefix, suffixAffinity: suffix);
        }
    }
}