using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.Operands
{
    [Serializable]
    internal class ErrorValueOperand : PrimitiveOperand
    {
        public ErrorValueOperand(ErrorValueType value)
        {
            ValueAsErrorType = value;
        }

        public ErrorValueOperand(ErrorValueWrapper wrapper)
        {
            ValueAsErrorType = wrapper.ErrorValue;
        }

        private ErrorValueOperand(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override object Value
        {
            get { return ValueAsErrorType; }
        }

        public ErrorValueType ValueAsErrorType { get; private set; }

        public ErrorValueWrapper ValueAsErrorWrapper
        {
            get { return new ErrorValueWrapper(ValueAsErrorType); }
        }

        public override OperandType NativeType
        {
            get { return OperandType.Error; }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Value", ValueAsErrorType);
        }

        protected override void SetDeserializedValue(object value)
        {
            ValueAsErrorType = (ErrorValueType) value;
        }

        public override int Compare(PrimitiveOperand rhs)
        {
            Debug.Assert(false, "should not be called");
            return -1;
        }

        public override bool CanConvertForCompare(OperandType ot)
        {
            Debug.Assert(false, "should not be called");
            return false;
        }

        protected override IOperand ConvertToBoolean()
        {
            return null;
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

        protected override IOperand ConvertToError()
        {
            return this;
        }
    }
}