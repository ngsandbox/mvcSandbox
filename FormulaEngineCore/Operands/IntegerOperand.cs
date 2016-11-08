using System;
using System.Globalization;
using System.Runtime.Serialization;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.Operands
{
    [Serializable]
    internal class IntegerOperand : PrimitiveOperand
    {
        public IntegerOperand(int value)
        {
            ValueAsInteger = value;
        }

        private IntegerOperand(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ValueAsInteger = info.GetInt32("Value");
        }

        public int ValueAsInteger { get; private set; }

        public override object Value
        {
            get { return ValueAsInteger; }
        }

        public override OperandType NativeType
        {
            get { return OperandType.Integer; }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Value", ValueAsInteger);
        }

        protected override void SetDeserializedValue(object value)
        {
            ValueAsInteger = (int) value;
        }

        public override int Compare(PrimitiveOperand rhs)
        {
            int iRhs = ((IntegerOperand) rhs).ValueAsInteger;
            return ValueAsInteger.CompareTo(iRhs);
        }

        public override bool CanConvertForCompare(OperandType ot)
        {
            return ot == OperandType.Integer | ot == OperandType.Double;
        }

        protected override IOperand ConvertToBoolean()
        {
            return new BooleanOperand(System.Convert.ToBoolean(ValueAsInteger));
        }

        protected override IOperand ConvertToDouble()
        {
            return new DoubleOperand(ValueAsInteger);
        }

        protected override IOperand ConvertToInteger()
        {
            return this;
        }

        protected override IOperand ConvertToString()
        {
            return new StringOperand(ValueAsInteger.ToString(CultureInfo.InvariantCulture));
        }
    }
}