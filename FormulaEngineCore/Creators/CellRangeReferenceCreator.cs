using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.Creators
{
    internal class CellRangeReferenceCreator : ReferenceCreator
    {
        public override ReferenceProperties CreateProperties(bool implicitSheet)
        {
            return CellRangeReference.CreateProperties(implicitSheet, _image);
        }

        public override Reference CreateReference()
        {
            return CellRangeReference.FromString(_image);
        }
    }
}