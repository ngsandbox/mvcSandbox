using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.Creators
{
    internal class RowReferenceCreator : ReferenceCreator
    {
        public override ReferenceProperties CreateProperties(bool implicitSheet)
        {
            return RowColumnReference.CreateProperties(implicitSheet, _image);
        }

        public override Reference CreateReference()
        {
            return RowReference.FromString(_image);
        }
    }
}