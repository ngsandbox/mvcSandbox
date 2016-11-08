using System;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Miscellaneous;
using FormulaEngineCore.Operands;

namespace FormulaEngineCore.Processors.Attributes
{
    /// <summary>
    ///     Marks a formula function as taking a variable number of arguments
    /// </summary>
    /// <remarks>
    ///     Use this attribute on a method that you wish to act as a formula function with a variable number of arguments.
    ///     The formula engine will allow calls to your method as long as it is called with at least one and no more than
    ///     <see cref="F:FunctionLibrary.MAX_ARGUMENT_COUNT" />
    ///     arguments.  No validation is done on the type of each argument; it is up to you to examine each one and act on it
    ///     as you see fit
    /// </remarks>
    /// <example>
    ///     The following example declares the method sum as a formula function taking a variable number of arguments
    ///     <code>
    /// &lt;VariableArgumentFormulaFunction()&gt; _
    /// Public Sub Sum(ByVal args As Argument(), ByVal result As FunctionResult, ByVal engine As FormulaEngine)
    /// End Sub
    /// </code>
    /// </example>
    [Serializable]
    public class VariableArgumentFormulaFunctionAttribute : FormulaFunctionAttribute
    {
        internal override bool IsValidMaxArgCount(int count)
        {
            return count <= FunctionLibrary.MAX_ARGUMENT_COUNT;
        }

        internal override bool IsValidMinArgCount(int count)
        {
            // Do like excel and require at least 1 argument
            return count >= 1;
        }

        internal override ArgumentMarshalResult MarshalArgument(int position, IOperand op)
        {
            // Marshaling always succeeds in these types of functions
            return new ArgumentMarshalResult(true, op);
        }
    }
}