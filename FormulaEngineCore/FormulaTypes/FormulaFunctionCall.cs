using FormulaEngineCore.Miscellaneous;

namespace FormulaEngineCore.FormulaTypes
{
    /// <summary>
    ///     Represents the method that will handle a formula function call
    /// </summary>
    /// <param name="args">All the arguments that the function was called with</param>
    /// <param name="result">The object where the function's result will be stored</param>
    /// <param name="engine">A reference to the formula engine</param>
    /// <remarks>
    ///     All methods that you wish to be able to be called from within a formula must have the signature of this
    ///     delegate.
    /// </remarks>
    public delegate void FormulaFunctionCall(Argument[] args, FunctionResult result, FormulaEngine engine);
}