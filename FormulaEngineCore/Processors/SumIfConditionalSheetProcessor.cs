using System.Collections.Generic;
using FormulaEngineCore.Miscellaneous;

namespace FormulaEngineCore.Processors
{
    internal class SumIfConditionalSheetProcessor : ConditionalSheetProcessor
    {
        private object[,] _sumValues;

        public SumIfConditionalSheetProcessor()
        {
            Values = new List<double>();
        }

        public IList<double> Values { get; private set; }

        public void Initialize(object[,] sumValues)
        {
            _sumValues = sumValues;
        }

        public override void OnMatch(int row, int col)
        {
            object value = _sumValues[row, col];
            value = Utility.NormalizeIfNumericValue(value);

            if (value != null && ReferenceEquals(value.GetType(), typeof(double)))
            {
                Values.Add((double)value);
            }
        }
    }
}