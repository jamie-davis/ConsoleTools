using System;
using System.Collections.Generic;
using System.Text;

namespace VT100.Utilities.ReadConsole
{
    interface IInputMonitor
    {
        void SequenceCaptured(IEnumerable<ControlElement> elements);
    }
}
