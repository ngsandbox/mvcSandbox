using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.Operators
{
    internal class RowsRemovedOperator : ReferenceOperator
    {
        private readonly int _count;
        private readonly int _removeAt;

        public RowsRemovedOperator(int removeAt, int count)
        {
            _removeAt = removeAt;
            _count = count;
        }

        public override ReferenceOperationResultType Operate(Reference @ref)
        {
            return @ref.GridOps.OnRowsRemoved(_removeAt, _count);
        }
    }
}