using System.Text.RegularExpressions;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Miscellaneous;

namespace FormulaEngineCore.Operators
{
    internal class WildcardPredicate : SheetValuePredicate
    {
        private readonly Regex _regex;

        public WildcardPredicate(string criteria)
        {
            criteria = string.Concat("^", Utility.Wildcard2Regex(criteria), "$");
            _regex = new Regex(criteria, RegexOptions.IgnoreCase);
        }

        public override bool IsMatch(object value)
        {
            return value != null && ReferenceEquals(value.GetType(), typeof (string)) && (_regex.IsMatch((string) value)
                ? CompareType == CompareType.Equal
                : CompareType == CompareType.NotEqual);
        }
    }
}