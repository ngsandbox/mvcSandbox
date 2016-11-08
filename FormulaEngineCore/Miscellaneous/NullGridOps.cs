using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.Miscellaneous
{
    /// <summary>
    ///     Returned by references that don't interact with the grid
    /// </summary>
    internal class NullGridOps : GridOperationsBase
    {
        public override ReferenceOperationResultType OnColumnsInserted(int insertAt, int count)
        {
            return new ReferenceOperationResultType();
        }

        public override ReferenceOperationResultType OnColumnsRemoved(int removeAt, int count)
        {
            return new ReferenceOperationResultType();
        }

        public override ReferenceOperationResultType OnRangeMoved(SheetReference source, SheetReference dest)
        {
            return new ReferenceOperationResultType();
        }

        public override ReferenceOperationResultType OnRowsInserted(int insertAt, int count)
        {
            return new ReferenceOperationResultType();
        }

        public override ReferenceOperationResultType OnRowsRemoved(int removeAt, int count)
        {
            return new ReferenceOperationResultType();
        }
    }
}