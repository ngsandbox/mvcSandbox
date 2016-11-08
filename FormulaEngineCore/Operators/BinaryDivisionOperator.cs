using System;
using System.Runtime.Serialization;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Operands;

namespace FormulaEngineCore.Operators
{
    [Serializable]
    internal class BinaryDivisionOperator : BinaryOperator
    {
        public BinaryDivisionOperator()
        {
        }

        private BinaryDivisionOperator(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        protected override OperandType ArgumentType
        {
            get { return OperandType.Double; }
        }

        protected override IOperand ComputeValue(IOperand lhs, IOperand rhs, FormulaEngine engine)
        {
            double rhsDouble = ((DoubleOperand) rhs).ValueAsDouble;
            return rhsDouble == 0 ? new ErrorValueOperand(ErrorValueType.Div0) : DoDoubleOperation((DoubleOperand) lhs, (DoubleOperand) rhs, DoDiv);
        }

        private double DoDiv(double lhs, double rhs)
        {
            return lhs/rhs;
        }
    }
}