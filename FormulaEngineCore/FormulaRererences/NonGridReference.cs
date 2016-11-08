using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Miscellaneous;

namespace FormulaEngineCore.FormulaRererences
{
    /// <summary>
    ///     Base class for references that aren't on a sheet
    /// </summary>
    [Serializable]
    internal abstract class NonGridReference : Reference
    {
        protected NonGridReference()
        {
        }

        protected NonGridReference(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        protected override string FormatWithProps(ReferenceProperties props)
        {
            Debug.Assert(false, "should not be called");
            return null;
        }

        public override bool Intersects(Reference @ref)
        {
            return EqualsReference(@ref);
        }

        public override bool IsOnSheet(ISheet sheet)
        {
            return false;
        }

        public override bool IsReferenceEqualForCircularReference(Reference @ref)
        {
            return EqualsReference(@ref);
        }


        public override void OnCopy(int rowOffset, int colOffset, ISheet destSheet, ReferenceProperties props)
        {
        }

        protected override GridOperationsBase CreateGridOps()
        {
            return new NullGridOps();
        }
    }
}