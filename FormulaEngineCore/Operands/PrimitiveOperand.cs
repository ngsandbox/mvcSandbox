using System;
using System.Collections;
using System.Runtime.Serialization;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.Operands
{
    /// <summary>
    ///     Base class for a constant, primitive value that can be converted to other values.
    /// </summary>
    [Serializable]
    internal abstract class PrimitiveOperand : IFormulaComponent, IOperand, ISerializable
    {
        private const int VERSION = 1;

        protected PrimitiveOperand()
        {
        }


        protected PrimitiveOperand(SerializationInfo info, StreamingContext context)
        {
        }

        public void Evaluate(Stack state, FormulaEngine engine)
        {
            state.Push(this);
        }


        public virtual void EvaluateForDependencyReference(IList references, FormulaEngine engine)
        {
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public void Validate(FormulaEngine engine)
        {
        }

        public IOperand Convert(OperandType convertType)
        {
            switch (convertType)
            {
                case OperandType.Double:
                    return ConvertToDouble();
                case OperandType.String:
                    return ConvertToString();
                case OperandType.Boolean:
                    return ConvertToBoolean();
                case OperandType.Integer:
                    return ConvertToInteger();
                case OperandType.Reference:
                case OperandType.SheetReference:
                    return null;
                case OperandType.Self:
                    return this;
                case OperandType.Primitive:
                    return this;
                case OperandType.Error:
                    return ConvertToError();
                case OperandType.Blank:
                    return ConvertToBlank();
                case OperandType.DateTime:
                    return ConvertToDateTime();
                default:
                    throw new ArgumentException("Unknown convert type");
            }
        }

        public abstract object Value { get; }
        public abstract OperandType NativeType { get; }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Version", VERSION);
        }

        protected abstract void SetDeserializedValue(object value);

        public abstract int Compare(PrimitiveOperand rhs);

        protected abstract IOperand ConvertToDouble();
        protected abstract IOperand ConvertToInteger();
        protected abstract IOperand ConvertToString();
        protected abstract IOperand ConvertToBoolean();

        protected virtual IOperand ConvertToError()
        {
            return null;
        }

        protected virtual IOperand ConvertToBlank()
        {
            return null;
        }

        protected virtual IOperand ConvertToDateTime()
        {
            return null;
        }

        public abstract bool CanConvertForCompare(OperandType ot);
    }
}