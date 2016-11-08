using System;

namespace FormulaEngineCore.FormulaRererences
{
    internal class VolatileFunctionReference : NonGridReference
    {
        public VolatileFunctionReference()
        {
            ComputeHashCode();
        }

        protected override bool EqualsReferenceInternal(Reference @ref)
        {
            return ReferenceEquals(@ref.GetType(), typeof(VolatileFunctionReference));
        }

        protected override string Format()
        {
            return "VolatileFunction";
        }

        protected override byte[] GetHashData()
        {
            int typeHashCode = typeof(VolatileFunctionReference).GetHashCode();
            return BitConverter.GetBytes(typeHashCode);
        }
    }
}