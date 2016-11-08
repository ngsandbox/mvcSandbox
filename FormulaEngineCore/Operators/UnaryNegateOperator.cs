using System;
using System.Runtime.Serialization;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Operands;

namespace FormulaEngineCore.Operators
{
    [Serializable]
    internal class UnaryNegateOperator : UnaryOperator
    {
        public UnaryNegateOperator()
        {
        }

        private UnaryNegateOperator(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        protected override OperandType GetKnownArgumentType()
        {
            return OperandType.Double;
        }

        protected override IOperand ComputeValue(IOperand value)
        {
            return new DoubleOperand(-((DoubleOperand) value).ValueAsDouble);
        }
    }
}