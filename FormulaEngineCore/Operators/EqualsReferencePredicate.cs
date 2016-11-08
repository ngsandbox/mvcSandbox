using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.Operators
{
    internal class EqualsReferencePredicate : ReferencePredicateBase
    {
        private readonly Reference _target;

        public EqualsReferencePredicate(Reference target)
        {
            _target = target;
        }

        public override bool IsMatch(Reference @ref)
        {
            return _target.EqualsReference(@ref);
        }
    }
}