using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.Operators
{
    internal class ColumnsInsertedOperator : ReferenceOperator
    {
        private readonly int _count;
        private readonly int _insertAt;

        public ColumnsInsertedOperator(int insertAt, int count)
        {
            _insertAt = insertAt;
            _count = count;
        }

        public override ReferenceOperationResultType Operate(Reference @ref)
        {
            return @ref.GridOps.OnColumnsInserted(_insertAt, _count);
        }
    }
}