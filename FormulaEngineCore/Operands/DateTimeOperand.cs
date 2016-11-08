using System;
using System.Runtime.Serialization;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.Operands
{
    [Serializable]
    internal class DateTimeOperand : PrimitiveOperand
    {
        public DateTimeOperand(DateTime value)
        {
            ValueAsDateTime = value;
        }

        private DateTimeOperand(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ValueAsDateTime = info.GetDateTime("Value");
        }

        public override OperandType NativeType
        {
            get { return OperandType.DateTime; }
        }

        public override object Value
        {
            get { return ValueAsDateTime; }
        }

        public DateTime ValueAsDateTime { get; private set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Value", ValueAsDateTime);
        }

        protected override void SetDeserializedValue(object value)
        {
            ValueAsDateTime = (DateTime) value;
        }

        public override bool CanConvertForCompare(OperandType ot)
        {
            return ot == OperandType.DateTime;
        }

        public override int Compare(PrimitiveOperand rhs)
        {
            DateTime rhsValue = ((DateTimeOperand) rhs).ValueAsDateTime;
            return ValueAsDateTime.CompareTo(rhsValue);
        }

        protected override IOperand ConvertToBoolean()
        {
            bool b = ValueAsDateTime.Equals(DateTime.MinValue) == false;
            return new BooleanOperand(b);
        }

        protected override IOperand ConvertToDouble()
        {
            return null;
        }

        protected override IOperand ConvertToInteger()
        {
            return null;
        }

        protected override IOperand ConvertToString()
        {
            return null;
        }

        protected override IOperand ConvertToDateTime()
        {
            return this;
        }
    }
}