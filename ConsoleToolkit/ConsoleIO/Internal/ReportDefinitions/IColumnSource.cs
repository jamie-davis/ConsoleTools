using System.Collections.Generic;
using ConsoleToolkit.ConsoleIO.ReportDefinitions;

namespace ConsoleToolkit.ConsoleIO.Internal.ReportDefinitions
{
    internal class ColumnSource
    {
        private List<ColumnFormat> _columns = new List<ColumnFormat>(); 
        private List<ColumnFormat> _children = new List<ColumnFormat>(); 

        internal void AddColumn(ColumnConfig config)
        {
            _columns.Add(config.ColumnFormat);
        }

        public IEnumerable<ColumnFormat> Columns { get { return _columns; } }
    }
}