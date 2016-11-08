using System;
using System.Globalization;
using System.Runtime.Serialization;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.Operands
{
    [Serializable]
    internal class DoubleOperand : PrimitiveOperand
    {
        public DoubleOperand(double value)
        {
            ValueAsDouble = value;
        }

        private DoubleOperand(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ValueAsDouble = info.GetDouble("Value");
        }

        public override object Value
        {
            get { return ValueAsDouble; }
        }

        public double ValueAsDouble { get; private set; }

        public override OperandType NativeType
        {
            get { return OperandType.Double; }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Value", ValueAsDouble);
        }

        protected override void SetDeserializedValue(object value)
        {
            ValueAsDouble = (double) value;
        }

        public override int Compare(PrimitiveOperand rhs)
        {
            double dRhs = ((DoubleOperand) rhs).ValueAsDouble;
            return ValueAsDouble.CompareTo(dRhs);
        }

        protected override IOperand ConvertToBoolean()
        {
            return new BooleanOperand(System.Convert.ToBoolean(ValueAsDouble));
        }

        protected override IOperand ConvertToDouble()
        {
            return this;
        }

        protected override IOperand ConvertToInteger()
        {
            if (ValueAsDouble < Int32.MinValue | ValueAsDouble > Int32.MaxValue)
            {
                return null;
            }
            // Excel does a hard truncate ....system.convert will do a round
            double d = Math.Truncate(ValueAsDouble);
            return new IntegerOperand(System.Convert.ToInt32(d));
        }

        protected override IOperand ConvertToString()
        {
            return new StringOperand(ValueAsDouble.ToString(CultureInfo.InvariantCulture));
        }

        public override bool CanConvertForCompare(OperandType ot)
        {
            return ot == OperandType.Double;
        }
    }
}