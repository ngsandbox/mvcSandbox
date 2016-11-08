using System;
using System.Runtime.Serialization;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Operands;

namespace FormulaEngineCore.Operators
{
    [Serializable]
    internal class BinaryPowerOperator : BinaryOperator
    {
        public BinaryPowerOperator()
        {
        }

        private BinaryPowerOperator(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        protected override OperandType ArgumentType
        {
            get { return OperandType.Double; }
        }

        protected override IOperand ComputeValue(IOperand lhs, IOperand rhs, FormulaEngine engine)
        {
            return DoDoubleOperation((DoubleOperand) lhs, (DoubleOperand) rhs, DoPower);
        }

        private double DoPower(double lhs, double rhs)
        {
            return Math.Pow(lhs, rhs);
        }
    }
}