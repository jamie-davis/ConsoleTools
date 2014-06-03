using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    internal static class ColourInstructionRebuilder
    {
        public static string Rebuild(string affinity, List<ColourControlItem.ControlInstruction> instructions)
        {
            var sb = new StringBuilder();

            foreach (var group in SplitInstructionGroups(instructions))
            {
                sb.Append(RebuildGroup(affinity, group));
            }

            return sb.ToString();
        }

        private static IEnumerable<IEnumerable<ColourControlItem.ControlInstruction>> SplitInstructionGroups(List<ColourControlItem.ControlInstruction> instructions)
        {
            List<ColourControlItem.ControlInstruction> group = null;
            foreach (var instruction in instructions)
            {
                if (instruction.Code == ColourControlItem.ControlCode.NewLine)
                {
                    if (group != null)
                        yield return group;

                    group = null;
                    yield return new[] {instruction};
                }
                else
                {
                    if (group == null)
                        group = new List<ColourControlItem.ControlInstruction>();

                    group.Add(instruction);
                }
            }

            if (group != null)
                yield return group;
        }

        private static string RebuildGroup(string affinity, IEnumerable<ColourControlItem.ControlInstruction> instructions)
        {
            var codes = instructions.Select(i => i.GenerateCode()).Aggregate((t, i) => t + i);
            if (codes == Environment.NewLine)
                return string.Empty;

            var sb = new StringBuilder();
            sb.Append(AdapterConfiguration.ControlSequenceIntroducer);
            sb.Append(affinity);
            sb.Append(codes);
            sb.Append(AdapterConfiguration.ControlSequenceTerminator);
            return sb.ToString();
        }
    }
}