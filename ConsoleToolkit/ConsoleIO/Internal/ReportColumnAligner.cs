using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    /// <summary>
    /// Aligns columns of data into a single string. The columns are padded such that all of the data occupies a consistent number of lines. 
    /// Alignment can either be such that every column starts at the top of the row, or ends at the bottom of the row. 
    /// </summary>
    internal static class ReportColumnAligner
    {
        /// <summary>
        /// Format a row given an array of column widths, and an array of values.
        /// </summary>
        /// <param name="widths">An array containing the required width for each column.</param>
        /// <param name="data">The data to be displayed in each column. This must be aligned with <see cref="widths"/>.</param>
        /// <param name="alignment">Whether the data in each column should be aligned with the top of the column or the bottom.</param>
        /// <param name="columnSeperator">A string to place between each column.</param>
        /// <returns></returns>
        public static string AlignColumns(int[] widths, string[][] data,
            ColVerticalAligment alignment = ColVerticalAligment.Top, 
            string columnSeperator = " ")
        {
            Debug.Assert(widths.Length == data.Length);
            var formattedColumns = widths.Select((width, ix) => FormatColumn(width, data[ix])).ToList();
            var totalLines = formattedColumns.Max(c => c.Length);
            var padding = formattedColumns
                .Select((c, colIndex) => c.Length < totalLines
                    ? Enumerable.Range(0, totalLines - c.Length)
                        .Select(i => new string(' ', widths[colIndex])).ToArray()
                    : new string[] {});

            List<string[]> alignedColumns;
            if (alignment == ColVerticalAligment.Top)
            {
                alignedColumns = padding
                    .Select((blanks, index) => formattedColumns[index].Concat(blanks)
                        .ToArray())
                    .ToList();
            }
            else
            {
                alignedColumns = padding
                    .Select((blanks, index) => blanks.Concat(formattedColumns[index])
                        .ToArray())
                    .ToList();
            }

            var outputLines = Enumerable.Range(0, totalLines)
                .Select(i => string.Join(columnSeperator, alignedColumns.Select(c => c[i]))).ToList();
            return string.Join(Environment.NewLine, outputLines)
                   + Environment.NewLine;
        }

        private static string[] FormatColumn(int width, IEnumerable<string> columnLines)
        {
            return columnLines
                .Select(l =>
                {
                    var length = ColourString.Length(l);
                    return length >= width
                                     ? ColourString.Substring(l, 0, width)
                                     : l + new string(' ', width - length);
                })
                .ToArray();
        }
    }
}