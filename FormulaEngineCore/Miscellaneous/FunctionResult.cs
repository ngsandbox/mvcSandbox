using System;
using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Operands;
using FormulaEngineCore.Operators;

namespace FormulaEngineCore.Miscellaneous
{
    /// <summary>
    ///     Represents the result of a formula function
    /// </summary>
    /// <remarks>
    ///     This class is responsible for storing the result of a formula function.  It has methods for storing a result of
    ///     various data types.  An instance of it is passed to all methods acting as formula functions and each such method
    ///     must produce
    ///     a result and store it in the passed instance or an exception will be raised.
    /// </remarks>
    /// <example>
    ///     This example shows a formula function that expects one argument of type double and sets its result as that value
    ///     incremented by one:
    ///     <code>
    ///  &lt;FixedArgumentFormulaFunction(1, New OperandType() {OperandType.Double})&gt; _
    ///  Public Sub PlusOne(ByVal args As Argument(), ByVal result As FunctionResult, ByVal engine As FormulaEngine)
    /// 		Dim value as Double = args(0).ValueAsDouble
    /// 		result.SetValue(value + 1)
    ///  End Sub
    ///  </code>
    /// </example>
    public sealed class FunctionResult
    {
        private IOperand MyOperand;

        internal FunctionResult()
        {
        }

        internal IOperand Operand
        {
            get { return MyOperand; }
        }

        /// <summary>
        ///     Sets the formula function result to a double
        /// </summary>
        /// <param name="value">The value you wish to be the result of the function</param>
        /// <remarks>Use this method when the result of your function is a double</remarks>
        public void SetValue(double value)
        {
            if (OperatorBase.IsInvalidDouble(value))
            {
                SetError(ErrorValueType.Num);
            }
            else
            {
                MyOperand = new DoubleOperand(value);
            }
        }

        /// <summary>
        ///     Sets the formula function result to an integer
        /// </summary>
        /// <param name="value">The value you wish to be the result of the function</param>
        /// <remarks>Use this method when the result of your function is an integer</remarks>
        public void SetValue(int value)
        {
            MyOperand = new IntegerOperand(value);
        }

        /// <summary>
        ///     Sets the formula function result to a boolean
        /// </summary>
        /// <param name="value">The value you wish to be the result of the function</param>
        /// <remarks>Use this method when the result of your function is a boolean</remarks>
        public void SetValue(bool value)
        {
            MyOperand = new BooleanOperand(value);
        }

        /// <summary>
        ///     Sets the formula function result to a string
        /// </summary>
        /// <param name="value">The value you wish to be the result of the function</param>
        /// <remarks>Use this method when the result of your function is a string</remarks>
        public void SetValue(string value)
        {
            FormulaEngine.ValidateNonNull(value, "value");
            MyOperand = new StringOperand(value);
        }

        /// <summary>
        ///     Sets the formula function result to a DateTime
        /// </summary>
        /// <param name="value">The value you wish to be the result of the function</param>
        /// <remarks>Use this method when the result of your function is a DateTime</remarks>
        public void SetValue(DateTime value)
        {
            MyOperand = new DateTimeOperand(value);
        }

        /// <summary>
        ///     Sets the formula function result to an error
        /// </summary>
        /// <param name="value">The type of the error you wish to be the result of the function</param>
        /// <remarks>Use this method when you need to return an error as the result of your function</remarks>
        public void SetError(ErrorValueType value)
        {
            MyOperand = new ErrorValueOperand(value);
        }

        /// <summary>
        ///     Sets the formula function result to a reference
        /// </summary>
        /// <param name="value">The value you wish to be the result of the function</param>
        /// <remarks>
        ///     Use this method when the result of your function is a reference.  Usually used when wishing to implement
        ///     dynamic references
        /// </remarks>
        public void SetValue(IReference value)
        {
            FormulaEngine.ValidateNonNull(value, "value");
            MyOperand = (IOperand)value;
        }

        /// <summary>
        ///     Sets the formula function result to a given argument
        /// </summary>
        /// <param name="arg">The value you wish to be the result of the function</param>
        /// <remarks>
        ///     Use this method when you wish to use one of the arguments supplied to your function as its result without altering
        ///     the value.
        ///     The if function, for example, uses this method.
        /// </remarks>
        public void SetValue(Argument arg)
        {
            FormulaEngine.ValidateNonNull(arg, "arg");
            MyOperand = arg.ValueAsOperand;
        }

        /// <summary>
        ///     Sets the formula function result to a sheet value
        /// </summary>
        /// <param name="value">The value you wish to be the result of the function</param>
        /// <remarks>Use this method when you have a value on a sheet that you wish to use as the result of your function</remarks>
        public void SetValueFromSheet(object value)
        {
            MyOperand = OperandFactory.CreateDynamic(value);
        }
    }
}