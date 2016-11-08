// Implementations of all reference types

using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Miscellaneous;
using FormulaEngineCore.Operands;

namespace FormulaEngineCore.FormulaRererences
{
    /// <summary>
    ///     Base class for all references
    /// </summary>
    [Serializable]
    public abstract class Reference : IFormulaComponent, IOperand, IReference, ISerializable
    {
        protected const int REFERENCE_HASH_SIZE = 4;

        private const int VERSION = 1;
        private int _hashCode;

        protected Reference()
        {
            Valid = true;
        }

        protected Reference(SerializationInfo info, StreamingContext context)
        {
            Valid = info.GetBoolean("Valid");
            Engine = (FormulaEngine)info.GetValue("Engine", typeof(FormulaEngine));
        }

        public bool Valid { get; private set; }

        public virtual bool CanRangeLink
        {
            get { return false; }
        }

        protected FormulaEngine Engine { get; private set; }

        public GridOperationsBase GridOps
        {
            get { return CreateGridOps(); }
        }

        public void Evaluate(Stack state, FormulaEngine engine)
        {
            state.Push(this);
        }

        public void EvaluateForDependencyReference(IList references, FormulaEngine engine)
        {
            references.Add(this);
        }

        public object Clone()
        {
            var refClone = (Reference)MemberwiseClone();
            InitializeClone(refClone);
            return refClone;
        }

        public virtual void Validate(FormulaEngine engine)
        {
        }

        public IOperand Convert(OperandType convertType)
        {
            switch (convertType)
            {
                case OperandType.Double:
                    break;
                case OperandType.String:
                    break;
                case OperandType.Boolean:
                    break;
                case OperandType.Integer:
                    break;
                case OperandType.Reference:
                case OperandType.Self:
                    return this;
                case OperandType.SheetReference:
                    return this as SheetReference;
                case OperandType.Primitive:
                    break;
                case OperandType.Error:
                    break;
                case OperandType.Blank:
                    break;
                case OperandType.DateTime:
                    break;
            }

            return !Valid ? ConvertWhenInvalid(convertType) : ConvertInternal(convertType);
        }

        public object Value
        {
            get { return this; }
        }

        public OperandType NativeType
        {
            get { return OperandType.Reference; }
        }

        public bool Equals(IReference @ref)
        {
            return EqualsIReference(@ref);
        }

        public override string ToString()
        {
            return ToStringIReference();
        }

        public void GetReferenceValues(IReferenceValueProcessor processor)
        {
            Debug.Assert(Valid, "invalid reference should not be getting here");
            GetReferenceValuesInternal(processor);
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Version", VERSION);
            info.AddValue("Valid", Valid);
            info.AddValue("Engine", Engine);
        }

        public void SetEngine(FormulaEngine engine)
        {
            Engine = engine;
            OnEngineSet(engine);
        }

        protected abstract GridOperationsBase CreateGridOps();


        public virtual void ProcessParseProperties(ReferenceParseProperties props, FormulaEngine engine)
        {
        }

        public static string References2String(IList refs)
        {
            var arr = new string[refs.Count];

            for (int i = 0; i < refs.Count; i++)
            {
                arr[i] = refs[i].ToString();
            }

            return string.Join(", ", arr);
        }


        protected virtual void OnEngineSet(FormulaEngine engine)
        {
        }

        protected virtual void CopyToReference(Reference target)
        {
            target.SetEngine(Engine);
        }


        public virtual void Validate()
        {
        }

        public int GetReferenceHashCode()
        {
            Debug.Assert(_hashCode != 0, "hash code not set");
            return _hashCode;
        }

        public void ComputeHashCode()
        {
            _hashCode = ComputeHashCodeInternal();
        }

        protected virtual int ComputeHashCodeInternal()
        {
            byte[] hashData = GetHashData();
            return ComputeJooatHash(hashData);
        }

        protected abstract byte[] GetHashData();

        protected virtual void GetBaseHashData(byte[] bytes)
        {
            int typeHashCode = GetType().GetHashCode();
            IntegerToBytes(typeHashCode, bytes, 0);
        }

        protected void IntegerToBytes(int i, byte[] bytes, int startIndex)
        {
            bytes[startIndex] = (byte)i;
            bytes[startIndex + 1] = (byte)(i >> 8);
            bytes[startIndex + 2] = (byte)(i >> 16);
            bytes[startIndex + 3] = (byte)(i >> 24);
        }

        protected void RowColumnIndexToBytes(int index, byte[] dest, int startIndex)
        {
            dest[startIndex] = (byte)index;
            dest[startIndex + 1] = (byte)(index >> 8);
        }

        private int ComputeJooatHash(byte[] key)
        {
            int hash = 0;

            for (int i = 0; i <= key.Length - 1; i++)
            {
                hash += key[i];
                hash += hash << 10;
                hash = hash ^ (hash >> 6);
            }

            hash += hash << 3;
            hash = hash ^ (hash >> 11);
            hash += hash << 15;

            return hash;
        }

        public abstract bool IsOnSheet(ISheet sheet);
        public abstract bool Intersects(Reference @ref);

        public bool EqualsReference(Reference @ref)
        {
            return Valid && @ref.Valid &&
                (ReferenceEquals(@ref.GetType(), GetType()) && EqualsReferenceInternal(@ref));
        }

        protected abstract bool EqualsReferenceInternal(Reference @ref);

        public bool EqualsIReference(IReference @ref)
        {
            var realRef = (Reference)@ref;
            return EqualsReference(realRef);
        }

        protected abstract string Format();
        protected abstract string FormatWithProps(ReferenceProperties props);

        public string ToStringIReference()
        {
            return !Valid ? GetRefString() : Format();
        }

        public virtual string ToStringFormula(ReferenceProperties props)
        {
            return !Valid ? GetRefString() : FormatWithProps(props);
        }

        private string GetRefString()
        {
            return new ErrorValueWrapper(ErrorValueType.Ref).ToString();
        }

        public abstract void OnCopy(int rowOffset, int colOffset, ISheet destSheet, ReferenceProperties props);
        public abstract bool IsReferenceEqualForCircularReference(Reference @ref);


        protected virtual void InitializeClone(Reference clone)
        {
        }

        private IOperand ConvertWhenInvalid(OperandType convertType)
        {
            return convertType == OperandType.Error ? new ErrorValueOperand(ErrorValueType.Ref) : null;
        }

        protected virtual IOperand ConvertInternal(OperandType convertType)
        {
            return null;
        }


        protected virtual void GetReferenceValuesInternal(IReferenceValueProcessor processor)
        {
        }

        public void MarkAsInvalid()
        {
            Valid = false;
        }
    }
}