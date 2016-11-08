using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.Operators
{
    internal class MatchNullPredicate : SheetValuePredicate
    {
        public override bool IsMatch(object value)
        {
            return value == null ? CompareType == CompareType.Equal : CompareType == CompareType.NotEqual;
        }
    }
}