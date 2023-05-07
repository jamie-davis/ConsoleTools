using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// ReSharper disable InconsistentNaming

namespace VT100.Utilities.ReadConsole
{
    internal static class AnsiRecognition
    {
        internal const char ESC = '\x1b';

        internal static IEnumerable<ControlSequence> Split(List<ControlElement> sequence, CodeAnalyserSettings settings = 0)
        {
            while (sequence.Count > 0)
            {
                var (haveSequence, extractedSequence, sequenceType) = TryTakeAnsiSequence(sequence);
                if (haveSequence)
                    yield return new ControlSequence(extractedSequence, sequenceType, settings);
                else
                {
                    var elementAsList = sequence.Take(1).ToList();
                    sequence.RemoveAt(0);
                    yield return new ControlSequence(elementAsList, AnsiCodeType.None, settings);
                }
            }
        }

        internal static (bool DidExtract, List<ControlElement> ExtractedSeq, AnsiCodeType AnsiCodeType) 
            TryTakeAnsiSequence(List<ControlElement> sequence)
        {
            List<ControlElement> result;

            if (TryTakeSS3Code(sequence, out result))
                return (true, result, AnsiCodeType.SS3);

            if (TryTakeC1Code(sequence, out result))
                return (true, result, AnsiCodeType.C1);

            if (TryTakeFeCode(sequence, out result))
                return (true, result, AnsiCodeType.Fe);

            if (TryTakeCSICode(sequence, out result))
                return (true, result, AnsiCodeType.CSI);

            return (false, null, AnsiCodeType.None);
        }

        private static bool TryTakeSS3Code(List<ControlElement> sequence, out List<ControlElement> result)
        {
            if (sequence.Count >= 3)
            {
                if (sequence[0].KeyChar == ESC
                    && sequence[1].KeyChar == 'O')
                {
                    result = sequence.Take(3).ToList();
                    sequence.RemoveRange(0,3);
                    return true;
                }
            }

            result = null;
            return false;
        }

        private static bool TryTakeC1Code(List<ControlElement> sequence, out List<ControlElement> result)
        {
            if (sequence.Count >= 2)
            {
                if (sequence[0].KeyChar == ESC
                    && sequence[1].KeyChar.Between('@', 'Z'))
                {
                    result = sequence.Take(2).ToList();
                    sequence.RemoveRange(0,2);
                    return true;
                }
            }

            result = null;
            return false;
        }

        private static bool TryTakeFeCode(List<ControlElement> sequence, out List<ControlElement> result)
        {
            if (sequence.Count >= 1)
            {
                var keyChar = sequence[0].KeyChar;
                if (keyChar.Between('\x80', '\x9A')
                    ||  keyChar.Between('\x9C', '\x9F'))
                {
                    result = sequence.Take(1).ToList();
                    sequence.RemoveAt(0);
                    return true;
                }
            }

            result = null;
            return false;
        }

        private static bool TryTakeCSICode(List<ControlElement> sequence, out List<ControlElement> result)
        {
            var csiLength = 0;

            if (sequence.Count >= 1)
            {
                if (sequence[0].KeyChar == '\x9B')
                    csiLength = 1;
            }
            if (csiLength == 0 && sequence.Count >= 2)
            {
                if (sequence[0].KeyChar == ESC
                    && sequence[1].KeyChar == '[')
                {
                    csiLength = 2;
                }
            }

            if (csiLength == 0)
            {
                result = null;
                return false;
            }

            int AcceptRange(int startPosition, char lower, char upper)
            {
                var takenCount = 0;
                while (sequence.Count > startPosition + takenCount 
                       && sequence[startPosition+takenCount].KeyChar.Between(lower, upper))
                {
                    ++takenCount;
                }

                return takenCount;
            }

            var paramCount = AcceptRange(csiLength, '0', '?');
            var interCount = AcceptRange(csiLength + paramCount, ' ', '/');
            var finalByte = AcceptRange(csiLength + paramCount, '@', '~') > 0
                ? 1
                : 0;

            var totalLength = csiLength + paramCount + interCount + finalByte;
            result = sequence.Take(totalLength).ToList();
            sequence.RemoveRange(0, totalLength);
            return true;
        }
    }
}
