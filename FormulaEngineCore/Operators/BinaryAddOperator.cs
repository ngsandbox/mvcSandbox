using System;
using System.Runtime.Serialization;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Operands;

namespace FormulaEngineCore.Operators
{
    [Serializable]
    internal class BinaryAddOperator : BinaryOperator
    {
        public BinaryAddOperator()
        {
        }

        private BinaryAddOperator(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        protected override OperandType ArgumentType
        {
            get { return OperandType.Double; }
        }

        protected override IOperand ComputeValue(IOperand lhs, IOperand rhs, FormulaEngine engine)
        {
            return DoDoubleOperation((DoubleOperand) lhs, (DoubleOperand) rhs, DoAdd);
        }

        private double DoAdd(double lhs, double rhs)
        {
            return lhs + rhs;
        }
    }
}