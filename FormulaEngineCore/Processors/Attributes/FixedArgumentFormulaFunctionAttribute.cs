using System;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Miscellaneous;
using FormulaEngineCore.Operands;

namespace FormulaEngineCore.Processors.Attributes
{
    /// <summary>
    ///     Marks a method as a formula function that takes a fixed number of arguments
    /// </summary>
    /// <remarks>
    ///     By tagging a method with this attribute, you are informing the formula engine that your method expects a fixed
    ///     number of arguments and lets you specify their type.  All calls to a function marked with this attribute will only
    ///     happen
    ///     if the number and type of arguments match the ones specified.
    /// </remarks>
    /// <example>
    ///     The following example declares the method Tan as a formula function taking one argument of type Double
    ///     <code>
    /// &lt;FixedArgumentFormulaFunction(1, New OperandType() {OperandType.Double})&gt; _
    /// Public Sub Tan(ByVal args As Argument(), ByVal result As FunctionResult, ByVal engine As FormulaEngine)
    /// End Sub
    /// </code>
    /// </example>
    [Serializable]
    public class FixedArgumentFormulaFunctionAttribute : FormulaFunctionAttribute
    {
        private readonly OperandType[] _argumentTypes;
        private readonly int _maxArgumentCount;
        private readonly int _minArgumentCount;

        /// <summary>
        ///     Declares a formula function with an optional number of arguments
        /// </summary>
        /// <param name="minArgumentCount">The minimum number of arguments your function expects</param>
        /// <param name="maxArgumentCount">The maximum number of arguments your function expects</param>
        /// <param name="argumentTypes">
        ///     An array of <see cref="T:OperandType" /> that specifies the type of each argument your
        ///     function expects
        /// </param>
        /// <remarks>
        ///     Use this constructor when your function can have optional arguments.  The formula engine will allow calls to your
        ///     function
        ///     as long as at least minArgumentCount arguments and no more than maxArgumentCount arguments are specified.  Calls
        ///     with a number of arguments
        ///     between the two values will be allowed and it is up to you to interpret the values of the unspecified arguments.
        ///     You must specify the
        ///     type of all arguments including optional ones.
        /// </remarks>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <para>minArgumentCount is negative</para>
        ///     <para>
        ///         maxArgumentCount exceeds the <see cref="F:FunctionLibrary.MAX_ARGUMENT_COUNT">maximum</see> number of allowed
        ///         arguments
        ///     </para>
        ///     <para>maxArgumentCount is less than minArgumentCount</para>
        ///     <para>The number of argument types is not equal to maxArgumentCount</para>
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <para>argumentTypes is null</para>
        /// </exception>
        /// <example>
        ///     The following example declares the method Ceiling as a formula function that be called with one or two Double
        ///     arguments
        ///     <code>
        /// &lt;FixedArgumentFormulaFunction(1, 2, New OperandType() {OperandType.Double, OperandType.Double})&gt; _
        /// Public Sub Ceiling(ByVal args As Argument(), ByVal result As FunctionResult, ByVal engine As FormulaEngine)
        /// End Sub
        /// </code>
        /// </example>
        public FixedArgumentFormulaFunctionAttribute(int minArgumentCount, int maxArgumentCount, OperandType[] argumentTypes)
        {
            _minArgumentCount = minArgumentCount;
            _maxArgumentCount = maxArgumentCount;
            _argumentTypes = argumentTypes;

            if (minArgumentCount < 0 | maxArgumentCount > FunctionLibrary.MAX_ARGUMENT_COUNT |
                maxArgumentCount < minArgumentCount)
            {
                throw new ArgumentOutOfRangeException("argumentCount" + "", "Invalid argument count value");
            }

            if (argumentTypes == null)
            {
                throw new ArgumentNullException("argumentTypes");
            }

            if (argumentTypes.Length != maxArgumentCount)
            {
                throw new ArgumentOutOfRangeException("argumentTypes", "Invalid number of argument types");
            }
        }

        /// <summary>
        ///     Declares a formula function with a fixed number of arguments
        /// </summary>
        /// <param name="argumentCount">The number of arguments your function expects</param>
        /// <param name="argumentTypes">The type of each argument</param>
        /// <remarks>
        ///     Use this constructor when your function requires an exact number of arguments.  You must specify
        ///     the count you want and the type of each argument.
        /// </remarks>
        /// <example>
        ///     The following example declares a method as taking one argument of type Double
        ///     <code>
        /// &lt;FixedArgumentFormulaFunction(1, New OperandType() {OperandType.Double})&gt; _
        /// Public Sub Tan(ByVal args As Argument(), ByVal result As FunctionResult, ByVal engine As FormulaEngine)
        /// End Sub
        /// </code>
        /// </example>
        public FixedArgumentFormulaFunctionAttribute(int argumentCount, OperandType[] argumentTypes) :
            this(argumentCount, argumentCount, argumentTypes)
        {
        }

        internal override bool IsValidMaxArgCount(int count)
        {
            return count <= _maxArgumentCount;
        }

        internal override bool IsValidMinArgCount(int count)
        {
            return count >= _minArgumentCount;
        }

        internal override ArgumentMarshalResult MarshalArgument(int position, IOperand op)
        {
            // Get the operand type we expect
            OperandType opType = _argumentTypes[position];
            // Try to convert
            IOperand result = op.Convert(opType);

            bool success = result != null;

            if (result == null)
            {
                // Conversion failed; get the error
                result = GetErrorOperand(op);
            }

            // Return the result of marshaling
            return new ArgumentMarshalResult(success, result);
        }

        private IOperand GetErrorOperand(IOperand op)
        {
            IOperand errorOp = op.Convert(OperandType.Error);

            if (errorOp != null)
            {
                return errorOp;
            }
            return new ErrorValueOperand(ErrorValueType.Value);
        }
    }
}