namespace FormulaEngineCore.FormulaTypes
{
    /// <summary>
    ///     Represents a worksheet as seen by the formula engine
    /// </summary>
    /// <remarks>
    ///     This interface defines the contract that any class wishing to act as a worksheet must implement.  All interaction
    ///     with cell values is done through this interface.
    /// </remarks>
    public interface ISheet
    {
        /// <summary>
        ///     Gets the name of the worksheet
        /// </summary>
        /// <value>The name of the worksheet</value>
        /// <remarks>
        ///     The name of a worksheet is used by the formula engine to find a sheet when its name is used in a reference.
        ///     For example: When evaluating the formula "=Sheet3!A1 * 2", the formula engine will look through all sheets until
        ///     it finds the one with the name "Sheet3".
        /// </remarks>
        /// <note>Sheet names are treated without regard to case</note>
        string Name { get; }

        /// <summary>
        ///     Gets number of rows in the sheet
        /// </summary>
        /// <value>The number of rows in the sheet</value>
        /// <remarks>This property is used by the engine to determine sheet bounds</remarks>
        int RowCount { get; }

        /// <summary>
        ///     Gets number of columns in the sheet
        /// </summary>
        /// <value>The number of columns in the sheet</value>
        /// <remarks>This property is used by the engine to determine sheet bounds</remarks>
        int ColumnCount { get; }

        /// <summary>
        ///     Gets the value of a particular cell. 
        ///     Stores the value of a formula into a cell
        /// </summary>
        /// <param name="row">The row of the required cell.  First row is 1</param>
        /// <param name="column">The column of the required cell.  First column is 1</param>
        /// <returns>The value of the cell at row,col</returns>
        /// <remarks>
        ///     The formula engine will call this method when the value of a cell is required.  Your implementation should
        ///     lookup the cell at row,col and return its value
        /// </remarks>
        object this[int row, int column] { get; set; }
    }
}