using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Processors;

namespace FormulaEngineCore.Operators
{
    /// <summary>
    ///     Base class for predicates used in conditional functions like SumIf
    /// </summary>
    internal abstract class SheetValuePredicate
    {
        protected CompareType CompareType { get; private set; }

        public static SheetValuePredicate Create(object criteria)
        {
            SheetValuePredicate pred;

            var s = criteria as string;

            if (s != null)
            {
                var info = new StringCriteriaInfo(s);
                pred = info.CreatePredicate();
            }
            else
            {
                pred = ComparerBasedPredicate.CreateDefault(criteria);
                pred.SetCompareType(CompareType.Equal);
            }

            return pred;
        }

        public void SetCompareType(CompareType ct)
        {
            CompareType = ct;
        }

        public abstract bool IsMatch(object value);
    }
}