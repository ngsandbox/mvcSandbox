using System;
using FormulaEngineCore.Miscellaneous;

namespace FormulaEngineCore.Processors
{
    internal class CountBlankProcessor : CountBasedReferenceValueProcessor
    {
        protected override bool ProcessPrimitiveArgument(Argument arg)
        {
            return false;
        }

        protected override void ProcessEmptyValue()
        {
            IncrementCount();
        }

        protected override void ProcessReferenceValue(object value, Type valueType)
        {
            var stringValue = value as string;
            if (String.IsNullOrEmpty(stringValue))
            {
                IncrementCount();
            }
        }
    }
}