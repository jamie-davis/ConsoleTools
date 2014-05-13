using System.Collections.Generic;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    public class ColumnSizingParameters
    {
        public int TabLength { get; set; }
        public List<ColumnWidthNegotiator.ColumnSizerInfo> Sizers { get; set; }
        public int SeperatorLength { get; set; }
        public PropertyStackColumnSizer StackSizer { get; set; }
        public int StackedColumnWidth { get; set; }
        public List<PropertyColumnFormat> Columns { get; set; }
    }
}