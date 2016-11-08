using System;
using FormulaEngineCore.Operators;

namespace FormulaEngineCore.Processors
{
    internal class MatchNullOrEmptyStringPredicate : SheetValuePredicate
    {
        public override bool IsMatch(object value)
        {
            return String.IsNullOrEmpty(value as string);
        }
    }
}