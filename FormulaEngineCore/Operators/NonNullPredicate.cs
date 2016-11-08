using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Miscellaneous;

namespace FormulaEngineCore.Operators
{
    internal abstract class NonNullPredicate : SheetValuePredicate
    {
        private readonly object _target;

        protected NonNullPredicate(object target)
        {
            _target = Utility.NormalizeIfNumericValue(target);
        }

        protected abstract int Compare(object value, object target);

        public override bool IsMatch(object value)
        {
            if (value == null)
            {
                return CompareType == CompareType.NotEqual;
            }

            value = Utility.NormalizeIfNumericValue(value);

            if (IsValidComparison(value, _target) == false)
            {
                return CompareType == CompareType.NotEqual;
            }
            CompareResult cr = LogicalOperator.Compare2CompareResult(Compare(value, _target));
            return LogicalOperator.GetBooleanResult(CompareType, cr);
        }

        private bool IsValidComparison(object value, object target)
        {
            return ReferenceEquals(value.GetType(), target.GetType());
        }
    }
}