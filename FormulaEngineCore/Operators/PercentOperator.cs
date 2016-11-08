using System;
using System.Runtime.Serialization;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Operands;

namespace FormulaEngineCore.Operators
{
    [Serializable]
    internal class PercentOperator : UnaryOperator
    {
        private readonly int _scale;

        public PercentOperator(int factor)
        {
            _scale = (int) Math.Pow(100, factor);
        }

        private PercentOperator(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _scale = info.GetInt32("Scale");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Scale", _scale);
        }

        protected override OperandType GetKnownArgumentType()
        {
            return OperandType.Double;
        }

        protected override IOperand ComputeValue(IOperand value)
        {
            return new DoubleOperand(((DoubleOperand) value).ValueAsDouble/_scale);
        }
    }
}