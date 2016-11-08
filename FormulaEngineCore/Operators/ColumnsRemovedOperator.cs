using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.Operators
{
    internal class ColumnsRemovedOperator : ReferenceOperator
    {
        private readonly int MyCount;
        private readonly int MyRemoveAt;

        public ColumnsRemovedOperator(int removeAt, int count)
        {
            MyRemoveAt = removeAt;
            MyCount = count;
        }

        public override ReferenceOperationResultType Operate(Reference @ref)
        {
            return @ref.GridOps.OnColumnsRemoved(MyRemoveAt, MyCount);
        }
    }
}