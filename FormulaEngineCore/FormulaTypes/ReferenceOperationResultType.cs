using System;

namespace FormulaEngineCore.FormulaTypes
{
    /// <summary>
    ///     Result from doing an operation on a reference
    /// </summary>
    [Flags]
    public enum ReferenceOperationResultType
    {
        NotAffected = 0,
        Affected = 1,
        Invalidated = 2
    }
}