using System;
using System.Collections.Generic;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Miscellaneous;

namespace FormulaEngineCore.Processors
{
    internal class LogicalFunctionProcessor : VariableArgumentFunctionProcessor
    {
        public LogicalFunctionProcessor()
        {
            Values = new List<bool>();
            StopOnError = true;
        }

        public IList<bool> Values { get; private set; }

        protected override bool StopOnError { get; set; }

        protected override bool ProcessPrimitiveArgument(Argument arg)
        {
            if (arg.IsBoolean)
            {
                Values.Add(arg.ValueAsBoolean);
                return true;
            }
            
            SetError(ErrorValueType.Value);
            return false;
        }

        protected override void ProcessReferenceValue(object value, Type valueType)
        {
            if (Utility.IsNumericType(valueType))
            {
                bool b = ((IConvertible) value).ToBoolean(null);
                Values.Add(b);
            }
            else if (ReferenceEquals(valueType, typeof (bool)))
            {
                Values.Add((bool) value);
            }
        }
    }
}