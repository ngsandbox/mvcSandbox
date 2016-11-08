using System;
using FormulaEngineCore.Miscellaneous;

namespace FormulaEngineCore.Processors
{
    internal class SumProcessor : DoubleBasedReferenceValueProcessor
    {
        protected override void ProcessReferenceValue(object value, Type valueType)
        {
            if (Utility.IsNumericType(valueType))
            {
                double d = Utility.NormalizeNumericValue(value);
                Values.Add(d);
            }
        }
    }
}