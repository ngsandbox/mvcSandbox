using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.Operands
{
    /// <summary>
    ///     Implemented by objects that act as operands to operators and functions in a formula
    /// </summary>
    public interface IOperand
    {
        object Value { get; }
        OperandType NativeType { get; }
        IOperand Convert(OperandType convertType);
    }
}