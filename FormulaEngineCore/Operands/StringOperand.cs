using System;
using System.Globalization;
using System.Runtime.Serialization;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.Operands
{
    [Serializable]
    internal class StringOperand : PrimitiveOperand
    {
        public StringOperand(string value)
        {
            ValueAsString = value;
        }

        private StringOperand(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ValueAsString = info.GetString("Value");
        }

        public string ValueAsString { get; private set; }

        public override object Value
        {
            get { return ValueAsString; }
        }

        public override OperandType NativeType
        {
            get { return OperandType.String; }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Value", ValueAsString);
        }

        protected override void SetDeserializedValue(object value)
        {
            ValueAsString = (string)value;
        }

        public override int Compare(PrimitiveOperand rhs)
        {
            string s1 = Value.ToString();
            string s2 = ((StringOperand)rhs).ValueAsString;
            return string.Compare(s1, s2, StringComparison.OrdinalIgnoreCase);
        }

        public override bool CanConvertForCompare(OperandType ot)
        {
            return ot == OperandType.String;
        }

        protected override IOperand ConvertToBoolean()
        {
            if (string.Equals(ValueAsString, bool.TrueString, StringComparison.OrdinalIgnoreCase))
            {
                return new BooleanOperand(true);
            }
            if (string.Equals(ValueAsString, bool.FalseString, StringComparison.OrdinalIgnoreCase))
            {
                return new BooleanOperand(false);
            }
            return null;
        }

        protected override IOperand ConvertToDouble()
        {
            double result;

            bool success = double.TryParse(ValueAsString, NumberStyles.Float, null, out result);

            return success ? new DoubleOperand(result) : null;
        }

        protected override IOperand ConvertToInteger()
        {
            var op = (DoubleOperand)ConvertToDouble();
            return op != null ? op.Convert(OperandType.Integer) : null;
        }

        protected override IOperand ConvertToString()
        {
            return this;
        }
    }
}