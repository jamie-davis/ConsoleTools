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
                return (elements.ToList(), new List<string>());

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
                if (exhausted) return;

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

            List<ControlElement> PullParameter()
            {
                var result = new List<ControlElement>();
                if (exhausted)
                    return new List<ControlElement>();

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

                parameters.Add(string.Concat(parameter.Select(e => e.KeyChar)));
            }

            return (seq, parameters);
        }

    }
}
