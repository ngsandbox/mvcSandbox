using System;
using System.Diagnostics;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Miscellaneous;
using FormulaEngineCore.Processors;

namespace FormulaEngineCore.Operators
{
    internal struct StringCriteriaInfo
    {
        public string Operator;

        public object Value;

        public StringCriteriaInfo(string criteria) : this()
        {
            GetInfo(criteria);
        }

        private void GetInfo(string criteria)
        {
            string stringValue;
            if (criteria.StartsWith("<>") | criteria.StartsWith(">=") | criteria.StartsWith("<="))
            {
                Operator = criteria.Substring(0, 2);
                stringValue = criteria.Substring(2);
            }
            else if (criteria.StartsWith("=") | criteria.StartsWith(">") | criteria.StartsWith("<"))
            {
                Operator = criteria.Substring(0, 1);
                stringValue = criteria.Substring(1);
            }
            else
            {
                Operator = null;
                stringValue = criteria;
            }

            Value = Utility.Parse(stringValue);
        }

        public SheetValuePredicate CreatePredicate()
        {
            var s = Value as string;

            SheetValuePredicate pred = s == null ? ComparerBasedPredicate.CreateDefault(Value) : CreateStringPredicate(s);

            pred.SetCompareType(Operator == null ? CompareType.Equal : GetCompareType());

            return pred;
        }

        private SheetValuePredicate CreateStringPredicate(string value)
        {
            if (value.Length == 0)
            {
                return Operator == null
                    ? (SheetValuePredicate)new MatchNullOrEmptyStringPredicate()
                    : new MatchNullPredicate();
            }

            return value.IndexOfAny(new[] { '?', '*' }) != -1
                ? (SheetValuePredicate)new WildcardPredicate(value)
                : new ComparerBasedPredicate(value, StringComparer.OrdinalIgnoreCase);
        }

        private CompareType GetCompareType()
        {
            switch (Operator)
            {
                case "=":
                    return CompareType.Equal;
                case "<>":
                    return CompareType.NotEqual;
                case ">=":
                    return CompareType.GreaterThanOrEqual;
                case "<=":
                    return CompareType.LessThanOrEqual;
                case ">":
                    return CompareType.GreaterThan;
                case "<":
                    return CompareType.LessThan;
            }

            Debug.Assert(false, "unknown operator");
            throw new NotImplementedException("unknown operator");
        }
    }
}