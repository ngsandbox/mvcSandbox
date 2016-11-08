using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.Creators
{
    internal class ColumnReferenceCreator : ReferenceCreator
    {
        public override ReferenceProperties CreateProperties(bool implicitSheet)
        {
            return RowColumnReference.CreateProperties(implicitSheet, _image);
        }

        public override Reference CreateReference()
        {
            return ColumnReference.FromString(_image);
        }
    }
}