using System;
using System.Runtime.Serialization;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.FormulaRererences
{
    [Serializable]
    internal class ExternalReference : NonGridReference, IFormulaSelfReference, IExternalReference
    {
        public ExternalReference() { }

        private ExternalReference(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Result = info.GetValue("Result", typeof(object));
        }

        public event EventHandler Recalculated;

        public object Result { get; private set; }

        public void OnFormulaRecalculate(Formula target)
        {
            Result = target.Evaluate();
            if (Recalculated != null)
                Recalculated.Invoke(this, EventArgs.Empty);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Result", Result);
        }

        protected override bool EqualsReferenceInternal(Reference @ref)
        {
            return ReferenceEquals(@ref, this);
        }

        protected override string Format()
        {
            int hashCode = GetHashCode();
            return String.Format("ExternalRef_{0}", hashCode.ToString("x"));
        }

        protected override int ComputeHashCodeInternal()
        {
            return GetHashCode();
        }

        protected override byte[] GetHashData()
        {
            return new byte[0];
        }
    }
}