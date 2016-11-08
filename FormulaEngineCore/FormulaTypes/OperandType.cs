namespace FormulaEngineCore.FormulaTypes
{
    /// <summary>
    ///     Defines constants to represent the data types of a formula function's arguments
    /// </summary>
    /// <remarks>
    ///     These values are used when working with functions to specify the desired data type of an argument and to get
    ///     its value
    /// </remarks>
    public enum OperandType
    {
        /// <summary>A standard .NET Double</summary>
        Double,

        /// <summary>A standard .NET String</summary>
        String,

        /// <summary>A standard .NET Boolean</summary>
        Boolean,

        /// <summary>Any type of reference</summary>
        Reference,

        /// <summary>A reference that points to cells on a sheet</summary>
        SheetReference,

        /// <summary>A standard .NET Integer</summary>
        Integer,

        /// <summary>A conversion of an operand to itself</summary>
        Self,

        /// <summary>Any data type except a reference</summary>
        Primitive,

        /// <summary>An error value</summary>
        Error,

        /// <summary>A blank value</summary>
        Blank,

        /// <summary>A standard .NET DateTime</summary>
        DateTime
    }
}