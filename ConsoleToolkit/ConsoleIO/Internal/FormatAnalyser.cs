using System;
using System.Collections.Generic;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    /// <summary>
    /// Matches a set of column foramt definitions with the public properties of a type.
    /// </summary>
    internal static class FormatAnalyser
    {
        /// <summary>
        /// Return a list of property formats for a type using the supplied column formats.
        /// 
        /// Supplied formats are matched positionally, and default formats will be provided for
        /// any properties for which no value is provided, inluding any properties for which the 
        /// corresponding entry in <see cref="specifiedColumns"/> is null. Excess formats will be
        /// ignored.
        /// </summary>
        /// <param name="type">The type from which the columns are drawn.</param>
        /// <param name="specifiedColumns">The user provided column formats.</param>
        /// <returns>Matched <see cref="PropertyColumnFormat"/> instances.</returns>
        public static List<PropertyColumnFormat> Analyse(Type type, IEnumerable<ColumnFormat> specifiedColumns)
        {
            var output = new List<PropertyColumnFormat>();

            var formatEnumerator = specifiedColumns == null ? null : specifiedColumns.GetEnumerator();
            var formatValid = formatEnumerator != null && formatEnumerator.MoveNext();
            foreach (var prop in type.GetProperties())
            {
                PropertyColumnFormat matchedFormat = null;
                if (formatValid)
                {
                    if (formatEnumerator.Current != null)
                        matchedFormat = new PropertyColumnFormat(prop, formatEnumerator.Current);
                    formatValid = formatEnumerator.MoveNext();
                }

                if (matchedFormat == null)
                    matchedFormat = new PropertyColumnFormat(prop, new ColumnFormat(PropertyNameConverter.ToHeading(prop), ConvertPropertyType(prop.PropertyType)));

                output.Add(matchedFormat);
            }

            return output;
        }

        private static Type ConvertPropertyType(Type propertyType)
        {
            if (typeof (IConsoleRenderer).IsAssignableFrom(propertyType))
                return typeof (IConsoleRenderer);
            return propertyType;
        }
    }
}