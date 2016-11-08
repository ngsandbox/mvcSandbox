using System;
using FormulaEngineCore.Miscellaneous;

namespace FormulaEngineCore.Processors
{
    internal class AverageAProcessor : DoubleBasedReferenceValueProcessor
    {
        protected override void ProcessReferenceValue(object value, Type valueType)
        {
            double d;
            if (ReferenceEquals(value.GetType(), typeof (bool)))
            {
                var b = (bool) value;
                d = Convert.ToDouble(b);
            }
            else if (ReferenceEquals(value.GetType(), typeof (string)))
            {
                d = 0.0;
            }
            else if (Utility.IsNumericType(valueType))
            {
                d = Utility.NormalizeNumericValue(value);
            }
            else
            {
                throw new InvalidOperationException("Unknown type");
            }

            Values.Add(d);
        }
    }
}