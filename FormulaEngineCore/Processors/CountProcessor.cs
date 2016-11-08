using System;
using FormulaEngineCore.Miscellaneous;

namespace FormulaEngineCore.Processors
{
    internal class CountProcessor : CountBasedReferenceValueProcessor
    {
        protected override bool ProcessPrimitiveArgument(Argument arg)
        {
            if (arg.IsDouble)
            {
                IncrementCount();
            }
            return true;
        }

        protected override void ProcessReferenceValue(object value, Type valueType)
        {
            if (Utility.IsNumericType(valueType))
            {
                IncrementCount();
            }
        }
    }
}