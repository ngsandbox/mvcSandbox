using System;
using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Operands;

namespace FormulaEngineCore.Miscellaneous
{
    /// <summary>
    ///     Creates an operand dynamically based on a value
    /// </summary>
    internal class OperandFactory
    {
        public static IOperand CreateDynamic(object value)
        {
            if (value == null)
            {
                return new NullValueOperand();
            }

            Type t = value.GetType();
            IOperand op;

            if (ReferenceEquals(t, typeof(double)))
            {
                op = new DoubleOperand((double)value);
            }
            else if (ReferenceEquals(t, typeof(string)))
            {
                op = new StringOperand((string)value);
            }
            else if (ReferenceEquals(t, typeof(bool)))
            {
                op = new BooleanOperand((bool)value);
            }
            else if (ReferenceEquals(t, typeof(int)))
            {
                op = new IntegerOperand((int)value);
            }
            else if ((value) is IReference)
            {
                op = (IOperand)value;
            }
            else if (ReferenceEquals(t, typeof(ErrorValueWrapper)))
            {
                op = new ErrorValueOperand((ErrorValueWrapper)value);
            }
            else if (ReferenceEquals(t, typeof(DateTime)))
            {
                op = new DateTimeOperand((DateTime)value);
            }
            else
            {
                throw new ArgumentException(String.Format("The type {0} is not supported as an operand", t.Name));
            }

            return op;
        }
    }
}