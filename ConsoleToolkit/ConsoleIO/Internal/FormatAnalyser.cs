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
        /// Return a list of property formats for a type using the supplied column formats.<para/>
        /// 
        /// Supplied formats are matched positionally, and default formats will be provided for
        /// any properties for which no value is provided, inluding any properties for which the 
        /// corresponding entry in <see cref="specifiedColumns"/> is null. Excess formats will be
        /// ignored.
        /// </summary>
        /// <param name="type">The type from which the columns are drawn.</param>
        /// <param name="specifiedColumns">The user provided column formats.</param>
        /// <param name="includeAllColumns">If true, include all columns from the type.</param>
        /// <returns>Matched <see cref="PropertyColumnFormat"/> instances.</returns>
        public static List<PropertyColumnFormat> Analyse(Type type, IEnumerable<ColumnFormat> specifiedColumns, bool includeAllColumns)
        {
            var output = new List<PropertyColumnFormat>();

            var formatEnumerator = specifiedColumns == null ? null : specifiedColumns.GetEnumerator();
            var formatValid = formatEnumerator != null && formatEnumerator.MoveNext();

            if (type.IsPrimitive)
                ProducePrimitiveColumn(type, includeAllColumns, formatValid, formatEnumerator, output);
            else
                ProduceColumnsUsingProperties(type, includeAllColumns, formatValid, formatEnumerator, output);

            return output;
        }

        private static void ProducePrimitiveColumn(Type type, bool includeAllColumns, bool formatValid, IEnumerator<ColumnFormat> formatEnumerator, List<PropertyColumnFormat> output)
        {
            if (formatValid)
                output.Add(new PropertyColumnFormat(null, formatEnumerator.Current));
            else
                output.Add(new PropertyColumnFormat(null,
                        new ColumnFormat(PropertyNameConverter.ToHeading(type.Name), ConvertPropertyType(type))));
        }

        private static void ProduceColumnsUsingProperties(Type type, bool includeAllColumns, bool formatValid,
            IEnumerator<ColumnFormat> formatEnumerator, List<PropertyColumnFormat> output)
        {
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
                {
                    if (!includeAllColumns) break;
                    matchedFormat = new PropertyColumnFormat(prop,
                        new ColumnFormat(PropertyNameConverter.ToHeading(prop), ConvertPropertyType(prop.PropertyType)));
                }

                output.Add(matchedFormat);
            }
        }

        private static Type ConvertPropertyType(Type propertyType)
        {
            if (typeof (IConsoleRenderer).IsAssignableFrom(propertyType))
                return typeof (IConsoleRenderer);
            return propertyType;
        }
    }
}