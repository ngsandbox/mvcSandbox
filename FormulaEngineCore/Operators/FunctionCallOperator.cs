using System;
using System.Collections;
using System.Runtime.Serialization;
using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Miscellaneous;

namespace FormulaEngineCore.Operators
{
    [Serializable]
    internal class FunctionCallOperator : OperatorBase
    {
        private readonly int _argCount;
        private readonly string _funcName;

        public FunctionCallOperator(string functionName, int argumentCount)
        {
            _funcName = functionName;
            _argCount = argumentCount;
        }

        private FunctionCallOperator(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _funcName = info.GetString("FunctionName");
            _argCount = info.GetInt32("ArgumentCount");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("FunctionName", _funcName);
            info.AddValue("ArgumentCount", _argCount);
        }

        public override void Evaluate(Stack state, FormulaEngine engine)
        {
            FunctionLibrary fl = engine.FunctionLibrary;
            fl.InvokeFunction(_funcName, state, _argCount);
        }

        public override void EvaluateForDependencyReference(IList references, FormulaEngine engine)
        {
            base.EvaluateForDependencyReference(references, engine);
            if (engine.FunctionLibrary.IsFunctionVolatile(_funcName))
            {
                var @ref = new VolatileFunctionReference();
                references.Add(@ref);
            }
        }

        public override void Validate(FormulaEngine engine)
        {
            if (engine.FunctionLibrary.IsFunctionDefined(_funcName) == false)
            {
                var inner =
                    new InvalidOperationException(string.Format("The function {0} is not defined", _funcName));
                throw new InvalidFormulaException(inner);
            }
            if (engine.FunctionLibrary.IsValidArgumentCount(_funcName, _argCount) == false)
            {
                var inner =
                    new ArgumentException(string.Format("Invalid number of arguments provided for function {0}",
                        _funcName));
                throw new InvalidFormulaException(inner);
            }
        }
    }
}