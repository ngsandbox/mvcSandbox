using System;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Operands;

namespace FormulaEngineCore.Processors.Attributes
{
    /// <summary>
    ///     The base class for all attributes that mark formula function methods
    /// </summary>
    /// <remarks>
    ///     This attribute is the base class for the <see cref="T:FormulaEngineCore.Processors.Attributes.FixedArgumentFormulaFunctionAttribute" /> and
    ///     <see cref="T:FormulaEngineCore.Processors.Attributes.VariableArgumentFormulaFunctionAttribute" /> classes.
    ///     All methods that you wish to be able to be used in formulas must be marked with one of those attributes.  By
    ///     doing so, you give the formula engine information about the number and type of arguments that your function
    ///     requires.
    ///     This allows the engine to only call your function with the correct number and type of arguments and
    ///     eliminates the need for each function author to write manual validation code.
    /// </remarks>
    /// <example>
    ///     This example shows a method tagged with this attribute.  The formula engine will only call this method
    ///     if it is called with exactly one argument and that argument can be converted into a double.
    ///     <code>
    ///  &lt;FixedArgumentFormulaFunction(1, New OperandType() {OperandType.Double})&gt; _
    ///  Public Sub PlusOne(ByVal args As Argument(), ByVal result As FunctionResult, ByVal engine As FormulaEngine)
    /// 		Dim value as Double = args(0).ValueAsDouble
    /// 		result.SetValue(value + 1)
    ///  End Sub
    ///  </code>
    /// </example>
    [Serializable, AttributeUsage(AttributeTargets.Method)]
    public abstract class FormulaFunctionAttribute : Attribute
    {
        public bool IsVolatile;

        public string[] Names;

        internal abstract bool IsValidMinArgCount(int count);
        internal abstract bool IsValidMaxArgCount(int count);
        internal abstract ArgumentMarshalResult MarshalArgument(int position, IOperand op);
    }
}