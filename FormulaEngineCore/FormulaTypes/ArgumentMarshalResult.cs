using FormulaEngineCore.Operands;

namespace FormulaEngineCore.FormulaTypes
{
    internal struct ArgumentMarshalResult
    {
        public IOperand Result;
        public bool Success;

        public ArgumentMarshalResult(bool success, IOperand result)
        {
            Success = success;
            Result = result;
        }
    }
}