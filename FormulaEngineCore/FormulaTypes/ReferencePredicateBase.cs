using FormulaEngineCore.FormulaRererences;

namespace FormulaEngineCore.FormulaTypes
{
    /// <summary>
    ///     Base class for a predicate on a reference
    /// </summary>
    internal abstract class ReferencePredicateBase
    {
        public abstract bool IsMatch(Reference @ref);
    }
}