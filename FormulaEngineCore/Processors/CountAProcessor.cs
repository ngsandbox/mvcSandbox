using System;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Miscellaneous;

namespace FormulaEngineCore.Processors
{
    internal class CountAProcessor : CountBasedReferenceValueProcessor
    {
        protected override bool ProcessPrimitiveArgument(Argument arg)
        {
            IncrementCount();
            return true;
        }

        protected override void ProcessReferenceValue(object value, Type valueType)
        {
            IncrementCount();
        }

        protected override void OnErrorReferenceValue(ErrorValueWrapper value)
        {
            IncrementCount();
        }
    }
}