using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.Operators
{
    internal class SheetReferencePredicate : ReferencePredicateBase
    {
        private readonly ISheet _target;

        public SheetReferencePredicate(ISheet target)
        {
            _target = target;
        }

        public override bool IsMatch(Reference @ref)
        {
            return @ref.IsOnSheet(_target);
        }
    }
}