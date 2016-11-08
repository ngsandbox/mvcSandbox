using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.Creators
{
    internal class CellReferenceCreator : ReferenceCreator
    {
        public override ReferenceProperties CreateProperties(bool implicitSheet)
        {
            return CellReference.CreateProperties(implicitSheet, _image);
        }

        public override Reference CreateReference()
        {
            return CellReference.FromString(_image);
        }
    }
}