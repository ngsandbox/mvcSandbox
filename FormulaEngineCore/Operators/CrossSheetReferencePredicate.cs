using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.Operators
{
    internal class CrossSheetReferencePredicate : ReferencePredicateBase
    {
        private readonly ISheet _destination;
        private readonly ISheet _source;

        public CrossSheetReferencePredicate(ISheet source, ISheet dest)
        {
            _source = source;
            _destination = dest;
        }

        public override bool IsMatch(Reference @ref)
        {
            return @ref.IsOnSheet(_source) | @ref.IsOnSheet(_destination);
        }
    }
}