using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.Operators
{
    internal class RowsInsertedOperator : ReferenceOperator
    {
        private readonly int _count;
        private readonly int _insertAt;

        public RowsInsertedOperator(int insertAt, int count)
        {
            _insertAt = insertAt;
            _count = count;
        }

        public override ReferenceOperationResultType Operate(Reference @ref)
        {
            return @ref.GridOps.OnRowsInserted(_insertAt, _count);
        }
    }
}