using System;
using System.Drawing;
using System.Globalization;
using System.Text;
using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.Miscellaneous
{
    /// <summary>
    ///     Provides various utility functions
    /// </summary>
    /// <remarks>This class provides various methods for doing common tasks when dealing with formulas</remarks>
    public static class Utility
    {
        private static readonly Type[] OurNumericTypes = CreateNumericTypes();

        private static Type[] CreateNumericTypes()
        {
            return new[]
            {
                typeof (double),
                typeof (int)
            };
        }

        /// <summary>
        ///     Tries to convert a string into a value similarly to Excel
        /// </summary>
        /// <param name="text">The string to parse</param>
        /// <returns>A value from the parsed string</returns>
        /// <remarks>
        ///     This method will try to parse text into a value.  It will try to convert the text into a Boolean,
        ///     ErrorValueWrapper,
        ///     DateTime, Integer, Double, or if all of the previous conversions fail, a string.
        /// </remarks>
        public static object Parse(string text)
        {
            FormulaEngine.ValidateNonNull(text, "text");
            bool b;
            if (bool.TryParse(text, out b))
            {
                return b;
            }

            ErrorValueWrapper wrapper = ErrorValueWrapper.TryParse(text);

            if (wrapper != null)
            {
                return wrapper;
            }

            DateTime dt;

            if (DateTime.TryParseExact(text, new[]
            {
                "D",
                "d",
                "G",
                "g",
                "t",
                "T"
            }, null, DateTimeStyles.AllowWhiteSpaces, out dt))
            {
                return dt;
            }

            double d;
            bool success = double.TryParse(text, NumberStyles.Integer, null, out d);

            if (success & d >= int.MinValue & d <= int.MaxValue)
            {
                return (int) d;
            }

            success = double.TryParse(text, NumberStyles.Float, null, out d);

            if (success)
            {
                return d;
            }

            return text;
        }

        /// <summary>
        ///     Determines whether a type is a numeric type
        /// </summary>
        /// <param name="t">The type to test</param>
        /// <returns>True if the type is numeric; False otherwise</returns>
        /// <remarks>Useful when you are processing sheet values and need to know that a type is numeric</remarks>
        public static bool IsNumericType(Type t)
        {
            return Array.IndexOf(OurNumericTypes, t) != -1;
        }

        /// <summary>
        ///     Determines whether a value is of a numeric type
        /// </summary>
        /// <param name="value">The value to test</param>
        /// <returns>True if the value is numeric; False otherwise</returns>
        /// <remarks>
        ///     This does the same thing as <see cref="M:ciloci.FormulaEngine.Utility.IsNumericType(System.Type)" /> except that it
        ///     acts
        ///     on a value.
        /// </remarks>
        public static bool IsNumericValue(object value)
        {
            if (value == null)
            {
                return false;
            }
            return IsNumericType(value.GetType());
        }

        /// <summary>
        ///     Converts a numeric value to a double
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The value converted to a double</returns>
        /// <remarks>
        ///     Since there are many types of numeric values in .NET, there exists a need to have a common denominator format that
        ///     they all can be converted to.  The type chosen here is the Double.
        /// </remarks>
        /// <exception cref="System.ArgumentException">The value is not of a numeric type</exception>
        public static double NormalizeNumericValue(object value)
        {
            if (IsNumericValue(value) == false)
            {
                throw new ArgumentException("Value is not numeric");
            }
            return ((IConvertible) value).ToDouble(null);
        }

        /// <summary>
        ///     Normalizes a value if it is of a numeric type
        /// </summary>
        /// <param name="value">The value to try to normalize</param>
        /// <returns>The value normalized value</returns>
        /// <remarks>This function normalizes value if it is of a numeric type; otherwise it returns the value unchanged</remarks>
        public static object NormalizeIfNumericValue(object value)
        {
            if (value == null)
            {
                return null;
            }
            if (IsNumericType(value.GetType()))
            {
                return ((IConvertible) value).ToDouble(null);
            }
            return value;
        }

        /// <summary>
        ///     Gets the regular expression equivalent pattern of an excel wildcard expression
        /// </summary>
        /// <param name="pattern">The pattern to convert</param>
        /// <returns>A regular expression representation of pattern</returns>
        /// <remarks>
        ///     Excel has its own syntax for pattern matching that many functions use.  This method converts such an expression
        ///     into its regular expression equivalent.
        /// </remarks>
        public static string Wildcard2Regex(string pattern)
        {
            FormulaEngine.ValidateNonNull(pattern, "pattern");
            pattern = EscapeRegex(pattern);
            var sb = new StringBuilder(pattern.Length);
            bool ignoreChar = false;

            for (int i = 0; i <= pattern.Length - 1; i++)
            {
                char c = pattern[i];
                if (ignoreChar)
                {
                    ignoreChar = false;
                }
                else if (c == '~')
                {
                    // Escape char
                    if (i == pattern.Length - 1)
                    {
                        // If the escape char is the last char then just match it
                        sb.Append('~');
                    }
                    else
                    {
                        char nextChar = pattern[i + 1];
                        if (nextChar == '?')
                        {
                            sb.Append("\\?");
                        }
                        else if (nextChar == '*')
                        {
                            sb.Append("\\*");
                        }
                        else
                        {
                            sb.Append(nextChar);
                        }

                        ignoreChar = true;
                    }
                }
                else if (c == '?')
                {
                    sb.Append(".");
                }
                else if (c == '*')
                {
                    sb.Append(".*");
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        private static string EscapeRegex(string pattern)
        {
            var sb = new StringBuilder(pattern);
            sb.Replace("\\", "\\\\");
            sb.Replace("[", "\\[");
            sb.Replace("^", "\\^");
            sb.Replace("$", "\\$");
            sb.Replace(".", "\\.");
            sb.Replace("|", "\\|");
            sb.Replace("+", "\\+");
            sb.Replace("(", "\\(");
            sb.Replace(")", "\\)");
            return sb.ToString();
        }

        /// <summary>
        ///     Determines whether a rectangle is inside the bounds of a given sheet
        /// </summary>
        /// <param name="rect">The rectangle to test</param>
        /// <param name="sheet">The sheet to use</param>
        /// <returns>True if the sheet contains the rectangle; False otherwise</returns>
        /// <remarks>
        ///     Use this function when you have a rectangle and a sheet and need to know if the rectangle is inside the
        ///     sheet's bounds.
        /// </remarks>
        public static bool IsRectangleInSheet(Rectangle rect, ISheet sheet)
        {
            FormulaEngine.ValidateNonNull(sheet, "sheet");
            return SheetReference.IsRectangleInSheet(rect, sheet);
        }

        /// <summary>
        ///     Gets a row from a table of values
        /// </summary>
        /// <param name="table">The table to get the row from</param>
        /// <param name="rowIndex">The index of the row to get</param>
        /// <returns>An array containing the values from the requested row</returns>
        /// <remarks>
        ///     This method is used when you have a table of values (like the ones returned by
        ///     <see cref="M:ciloci.FormulaEngine.ISheetReference.GetValuesTable" />)
        ///     and you need to get the values from a row.
        /// </remarks>
        public static object[] GetTableRow(object[,] table, int rowIndex)
        {
            FormulaEngine.ValidateNonNull(table, "table");
            var arr = new object[table.GetLength(1)];

            for (int col = 0; col < arr.Length; col++)
            {
                arr[col] = table[rowIndex, col];
            }

            return arr;
        }

        /// <summary>
        ///     Gets a column from a table of values
        /// </summary>
        /// <param name="table">The table to get the column from</param>
        /// <param name="columnIndex">The index of the column to get</param>
        /// <returns>An array containing the values from the requested column</returns>
        /// <remarks>
        ///     This method is used when you have a table of values (like the ones returned by
        ///     <see cref="M:ciloci.FormulaEngine.ISheetReference.GetValuesTable" />)
        ///     and you need to get the values from a column.
        /// </remarks>
        public static object[] GetTableColumn(object[,] table, int columnIndex)
        {
            FormulaEngine.ValidateNonNull(table, "table");
            var arr = new object[table.GetLength(0)];

            for (int row = 0; row < arr.Length; row++)
            {
                arr[row] = table[row, columnIndex];
            }

            return arr;
        }

        /// <summary>
        ///     Gets the label for a column index
        /// </summary>
        /// <param name="columnIndex">The index whose label you wish to get</param>
        /// <returns>A string with the colum label</returns>
        /// <remarks>This function is handy when you have a column index and you want to get its associated label.</remarks>
        /// <example>
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Column index</term><description>Resultant label</description>
        ///         </listheader>
        ///         <item>
        ///             <term>1</term><description>"A"</description>
        ///         </item>
        ///         <item>
        ///             <term>14</term><description>"N"</description>
        ///         </item>
        ///         <item>
        ///             <term>123</term><description>"DS"</description>
        ///         </item>
        ///         <item>
        ///             <term>256</term><description>"IV"</description>
        ///         </item>
        ///     </list>
        /// </example>
        public static string ColumnIndex2Label(int columnIndex)
        {
            return SheetReference.ColumnIndex2Label(columnIndex);
        }

        /// <summary>
        ///     Gets the column index from a column label
        /// </summary>
        /// <param name="label">The label whose column index you wish to get</param>
        /// <returns>An index representing the label</returns>
        /// <remarks>This function is handy when you have a column label and you want to get its associated index.</remarks>
        /// <example>
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Column label</term><description>Resultant index</description>
        ///         </listheader>
        ///         <item>
        ///             <term>"A"</term><description>1</description>
        ///         </item>
        ///         <item>
        ///             <term>"N"</term><description>14</description>
        ///         </item>
        ///         <item>
        ///             <term>"DS"</term><description>123</description>
        ///         </item>
        ///         <item>
        ///             <term>"IV"</term><description>256</description>
        ///         </item>
        ///     </list>
        /// </example>
        public static int ColumnLabel2Index(string label)
        {
            FormulaEngine.ValidateNonNull(label, "label");
            if (label.Length < 1 || label.Length > 2)
            {
                throw new ArgumentException("The given label must be one or two characters long");
            }

            char c2 = Char.MinValue;
            if (label.Length == 2)
            {
                c2 = label[1];
            }
            return SheetReference.ColumnLabel2Index(label[0], c2);
        }

//    internal static bool IsNumericType(Type valueType)
//    {
//        throw new NotImplementedException();
//    }
    }
}