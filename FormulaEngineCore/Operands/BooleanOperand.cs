using System;
using System.Runtime.Serialization;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.Operands
{
    [Serializable]
    internal class BooleanOperand : PrimitiveOperand
    {
        public BooleanOperand(bool value)
        {
            ValueAsBoolean = value;
        }

        private BooleanOperand(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ValueAsBoolean = info.GetBoolean("Value");
        }

        public bool ValueAsBoolean { get; private set; }

        public override object Value
        {
            get { return ValueAsBoolean; }
        }

        public override OperandType NativeType
        {
            get { return OperandType.Boolean; }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Value", ValueAsBoolean);
        }

        protected override void SetDeserializedValue(object value)
        {
            ValueAsBoolean = (bool) value;
        }

        public override int Compare(PrimitiveOperand rhs)
        {
            var i1 = (IntegerOperand) ConvertToInteger();
            var i2 = (IntegerOperand) rhs.Convert(OperandType.Integer);
            return i1.Compare(i2);
        }

        public override bool CanConvertForCompare(OperandType ot)
        {
            return ot == OperandType.Boolean;
        }

        protected override IOperand ConvertToBoolean()
        {
            return this;
        }

        protected override IOperand ConvertToDouble()
        {
            return new DoubleOperand(System.Convert.ToDouble(ValueAsBoolean));
        }

        protected override IOperand ConvertToInteger()
        {
            return new IntegerOperand(System.Convert.ToInt32(ValueAsBoolean));
        }

        protected override IOperand ConvertToString()
        {
            return new StringOperand(ValueAsBoolean.ToString().ToUpper());
        }
    }
}