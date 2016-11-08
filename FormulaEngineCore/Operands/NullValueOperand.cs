using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.Operands
{
    [Serializable]
    internal class NullValueOperand : PrimitiveOperand
    {
        public NullValueOperand()
        {
        }

        private NullValueOperand(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override object Value
        {
            get { return null; }
        }

        public override OperandType NativeType
        {
            get { return OperandType.Blank; }
        }


        protected override void SetDeserializedValue(object value)
        {
        }

        public override int Compare(PrimitiveOperand rhs)
        {
            Debug.Assert(false, "should not be called");
            return -1;
        }

        public override bool CanConvertForCompare(OperandType ot)
        {
            return true;
        }

        protected override IOperand ConvertToBoolean()
        {
            return new BooleanOperand(false);
        }

        protected override IOperand ConvertToDouble()
        {
            return new DoubleOperand(0.0);
        }

        protected override IOperand ConvertToInteger()
        {
            return new IntegerOperand(0);
        }

        protected override IOperand ConvertToString()
        {
            return new StringOperand(string.Empty);
        }

        protected override IOperand ConvertToBlank()
        {
            return this;
        }
    }
}