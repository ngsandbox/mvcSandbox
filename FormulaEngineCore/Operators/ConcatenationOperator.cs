using System;
using System.Runtime.Serialization;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Operands;

namespace FormulaEngineCore.Operators
{
    [Serializable]
    internal class ConcatenationOperator : BinaryOperator
    {
        public ConcatenationOperator()
        {
        }

        private ConcatenationOperator(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        protected override OperandType ArgumentType
        {
            get { return OperandType.String; }
        }

        protected override IOperand ComputeValue(IOperand lhs, IOperand rhs, FormulaEngine engine)
        {
            return new StringOperand(((StringOperand) lhs).ValueAsString + ((StringOperand) rhs).ValueAsString);
        }
    }
}