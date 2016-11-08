using System;

namespace FormulaEngineCore.FormulaTypes
{
    /// <summary>
    ///     The exception that is thrown when attempting to create an invalid formula
    /// </summary>
    /// <remarks>
    ///     This exception will be thrown when attempting to create a formula that is invalid.  The most common (though not the
    ///     only) reason is that
    ///     the syntax of the formula does not conform to the parser's grammar.  The inner exception will always be initialized
    ///     and will
    ///     contain the specifics as to why the formula could not be created.
    /// </remarks>
    public class InvalidFormulaException : Exception
    {
        internal InvalidFormulaException(Exception innerException) : base("The formula is not valid", innerException)
        {
        }
    }

// Holds all information about a parsed reference
}