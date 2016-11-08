using System;

namespace FormulaEngineCore.FormulaTypes
{
    /// <summary>
    ///     A convenient wrapper around an error value
    /// </summary>
    /// <remarks>
    ///     This class encapsulates the parsing and formatting of an <see cref="T:ciloci.FormulaEngine.ErrorValueType" />.
    ///     It exists so that the error returned by a formula will be nicely formatted without any additional work on the
    ///     person working
    ///     with the formula engine.
    /// </remarks>
    public class ErrorValueWrapper
    {
        private const string DIV0_STRING = "#DIV/0!";
        private const string NA_STRING = "#N/A";
        private const string NAME_STRING = "#NAME?";
        private const string NULL_STRING = "#NULL!";
        private const string REF_STRING = "#REF!";
        private const string VALUE_STRING = "#VALUE!";

        private const string NUM_STRING = "#NUM!";
        private readonly ErrorValueType _errorValue;

        internal ErrorValueWrapper(ErrorValueType value)
        {
            _errorValue = value;
        }

        /// <summary>
        ///     Gets the actual error value that the class contains
        /// </summary>
        /// <value>The error value</value>
        /// <remarks>Returns the error value that the wrapper contains.</remarks>
        public ErrorValueType ErrorValue { get { return _errorValue; } }

        /// <summary>
        ///     Convenience function for equality
        /// </summary>
        /// <param name="obj">The value to test against</param>
        /// <returns>True if the wrapper equals obj</returns>
        /// <remarks>Compares the current wrapper against another value for equality.</remarks>
        public override bool Equals(object obj)
        {
            if ((obj) is ErrorValueType)
            {
                return _errorValue == (ErrorValueType)obj;
            }

            ErrorValueWrapper wrapper = (obj as ErrorValueWrapper);
            if (wrapper != null)
            {
                return _errorValue == wrapper._errorValue;
            }

            return true;
        }

        /// <summary>
        ///     Tries to parse a string in to an error value wrapper
        /// </summary>
        /// <param name="s">The string to parse</param>
        /// <returns>A wrapper instance if the string was sucessfully parsed; a null reference otherwise</returns>
        /// <remarks>
        ///     Use this function when you wish to parse a string into an error value wrapper.
        ///     The function recognizes the following strings:
        ///     <list type="bullet">
        ///         <item>"#DIV/0!"</item>
        ///         <item>"#N/A"</item>
        ///         <item>"#NAME?"</item>
        ///         <item>"#NULL!"</item>
        ///         <item>"#REF!"</item>
        ///         <item>"#VALUE!"</item>
        ///         <item>"#NUM!"</item>
        ///     </list>
        /// </remarks>
        public static ErrorValueWrapper TryParse(string s)
        {
            ErrorValueType ev;

            if (s.Equals(DIV0_STRING, StringComparison.OrdinalIgnoreCase))
            {
                ev = ErrorValueType.Div0;
            }
            else if (s.Equals(NA_STRING, StringComparison.OrdinalIgnoreCase))
            {
                ev = ErrorValueType.NA;
            }
            else if (s.Equals(NAME_STRING, StringComparison.OrdinalIgnoreCase))
            {
                ev = ErrorValueType.Name;
            }
            else if (s.Equals(NULL_STRING, StringComparison.OrdinalIgnoreCase))
            {
                ev = ErrorValueType.Null;
            }
            else if (s.Equals(REF_STRING, StringComparison.OrdinalIgnoreCase))
            {
                ev = ErrorValueType.Ref;
            }
            else if (s.Equals(VALUE_STRING, StringComparison.OrdinalIgnoreCase))
            {
                ev = ErrorValueType.Value;
            }
            else if (s.Equals(NUM_STRING, StringComparison.OrdinalIgnoreCase))
            {
                ev = ErrorValueType.Num;
            }
            else
            {
                return null;
            }

            return new ErrorValueWrapper(ev);
        }

        /// <summary>
        ///     Formats the inner error value
        /// </summary>
        /// <returns>A string with the formatted error</returns>
        /// <remarks>
        ///     This method will format an error value similarly to Excel.  For example: the error value Ref will
        ///     be formatted as "#REF!"
        /// </remarks>
        public override string ToString()
        {
            switch (_errorValue)
            {
                case ErrorValueType.Div0:
                    return DIV0_STRING;
                case ErrorValueType.NA:
                    return NA_STRING;
                case ErrorValueType.Name:
                    return NAME_STRING;
                case ErrorValueType.Null:
                    return NULL_STRING;
                case ErrorValueType.Ref:
                    return REF_STRING;
                case ErrorValueType.Value:
                    return VALUE_STRING;
                case ErrorValueType.Num:
                    return NUM_STRING;
                default:
                    throw new InvalidOperationException("Unknown value");
            }
        }
    }
}