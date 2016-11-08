using System.Collections.Generic;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Miscellaneous;

namespace FormulaEngineCore.Processors
{
    /// <summary>
    ///     Processor that works with a list of doubles
    /// </summary>
    internal abstract class DoubleBasedReferenceValueProcessor : VariableArgumentFunctionProcessor
    {
        protected DoubleBasedReferenceValueProcessor()
        {
            Values = new List<double>();
        }

        public IList<double> Values { get; private set; }

        protected override bool StopOnError
        {
            get { return true; }
            set { }
        }

        protected override bool ProcessPrimitiveArgument(Argument arg)
        {
            if (arg.IsDouble)
            {
                Values.Add(arg.ValueAsDouble);
                return true;
            }
            
            SetError(ErrorValueType.Value);
            return false;
        }
    }
}