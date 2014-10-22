using System.Collections.Generic;
using System.Linq;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    internal class ColumnSizingParameters
    {
        public int TabLength { get; set; }
        public List<ColumnWidthNegotiator.ColumnSizerInfo> Sizers { get; set; }
        public int SeperatorLength { get; set; }
        public PropertyStackColumnSizer StackSizer { get; set; }
        public int StackedColumnWidth { get; set; }
        public List<PropertyColumnFormat> Columns { get; set; }

        public bool ProportionalColumnSizingRequired
        {
            get { return Sizers.Any(s => s.WidthIsProportional()); }
        }
    }
}