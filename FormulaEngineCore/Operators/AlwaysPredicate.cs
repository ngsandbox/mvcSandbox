using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.Operators
{
    internal class AlwaysPredicate : ReferencePredicateBase
    {
        public override bool IsMatch(Reference @ref)
        {
            return true;
        }
    }
}