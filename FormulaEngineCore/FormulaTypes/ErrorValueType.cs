namespace FormulaEngineCore.FormulaTypes
{
    /// <summary>
    ///     Defines constants for all errors that a formula can generate during evaluation
    /// </summary>
    /// <remarks>
    ///     Formulas that produce an error during calculation will produce an
    ///     <see cref="T:ciloci.FormulaEngine.ErrorValueWrapper" /> around
    ///     one of these values.  The values map directly to the values used by Excel.
    /// </remarks>
    public enum ErrorValueType
    {
        /// <summary>A division by zero was encountered</summary>
        Div0,

        /// <summary>A formula referenced a name that is not defined</summary>
        Name,

        /// <summary>A result is not available</summary>
        NA,

        /// <summary>The two given sheet references do not intersect</summary>
        Null,

        /// <summary>A calculation is invalid for numerical reasons</summary>
        Num,

        /// <summary>A reference is not valid</summary>
        Ref,

        /// <summary>The given value is not valid</summary>
        Value
    }
}