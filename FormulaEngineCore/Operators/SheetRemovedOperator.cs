using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.Operators
{
    internal class SheetRemovedOperator : ReferenceOperator
    {
        public override ReferenceOperationResultType Operate(Reference @ref)
        {
            return ReferenceOperationResultType.Invalidated;
        }
    }
}