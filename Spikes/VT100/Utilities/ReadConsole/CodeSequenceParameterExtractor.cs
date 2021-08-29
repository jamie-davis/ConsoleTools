using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VT100.Utilities.ReadConsole
{
    /// <summary>
    /// Accept a list of control elements and the type of control sequence. Return a copy of the list with the parameters removed,
    /// and a list of the parameters.
    /// </summary>
    internal static class CodeSequenceParameterExtractor
    {
        
        internal static (List<ControlElement> Sequence, List<string> Parameters) Extract(IEnumerable<ControlElement> elements, AnsiCodeType ansiCodeType)
        {
            if (ansiCodeType != AnsiCodeType.CSI)
                return (elements.ToList(), new List<string>()); //parameters can only be extracted from CSI sequences

            var exhausted = false;
            var seq = new List<ControlElement>();
            var iterator = elements.GetEnumerator();
            ControlElement last = null;

            //Take the next element
            ControlElement Accept()
            {
                if (!iterator.MoveNext())
                {
                    exhausted = true;
                    last = null;
                    return null;
                }

                last = iterator.Current;
                return last;
            }

            //Take a character and add it to the sequence if the sequence is not finished
            void AcceptAndCopyIfPossible()
            {
                var element = Accept();
                if (element != null)
                    seq.Add(element);
            }

            void CopyCodeElements()
            {
                Accept();

                while (!exhausted && last.KeyChar != ';')
                {
                    seq.Add(last);
                    Accept();
                }
            }

            string MakeParameterString(List<ControlElement> controlElements)
            {
                return string.Concat(controlElements.Select(e => e.KeyChar));
            }

            List<ControlElement> PullParameter()
            {
                var result = new List<ControlElement>();
                do
                {
                    Accept();
                    if (last != null && last.KeyChar != ';') result.Add(last);
                } while (!exhausted && last.KeyChar != ';');

                return result;
            }

            AcceptAndCopyIfPossible(); //ESC

            AcceptAndCopyIfPossible(); //[
            CopyCodeElements();

            var parameters = new List<string>();
            while (!exhausted)
            {
                var parameter = PullParameter();
                if (exhausted && parameter.Count >= 1)
                {
                    seq.Add(parameter.Last());
                    parameter = parameter.Take(parameter.Count - 1).ToList();
                }

                parameters.Add(MakeParameterString(parameter));
            }

            var terminator = seq.Last();
            if (IsDirectParameterSequence(terminator))
            {
                //some sequences have a parameter not prefixed with ; eg "CSI 5;10R" where "5" is a parameter that needs to be extracted
                //in the example this is a cursor position report where 5 is the row, and 10 is the column.
                if (seq.Count > 3)
                {
                    var parameter = seq.Skip(2).Take(seq.Count - 3).ToList();
                    for (var delIx = 0; delIx < parameter.Count; ++delIx)
                        seq.RemoveAt(2);
                    parameters.Insert(0, MakeParameterString(parameter));
                }
            }
            return (seq, parameters);
        }

        private static bool IsDirectParameterSequence(ControlElement terminator)
        {
            const string directParameterTerminators = "R";
            return directParameterTerminators.Contains(terminator.KeyChar);
        }
    }
}
