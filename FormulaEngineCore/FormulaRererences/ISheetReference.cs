using System.Drawing;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.FormulaRererences
{
    /// <summary>
    ///     Represents a reference to cells on a sheet
    /// </summary>
    /// <remarks>
    ///     Sheet references are the most common type of reference and consist of a sheet and an area on that sheet.  Any
    ///     formula
    ///     that needs values from a sheet will use sheet references and all formulas that are on a sheet must be bound to
    ///     them.
    /// </remarks>
    public interface ISheetReference : IReference
    {
        /// <summary>Gets row of the reference</summary>
        /// <remarks>The row of the reference on its sheet</remarks>
        int Row { get; }

        /// <summary>Gets the column of the reference</summary>
        /// <remarks>The column of the reference on its sheet</remarks>
        int Column { get; }

        /// <summary>Gets number of rows in the reference</summary>
        /// <remarks>The number of rows the reference spans on its sheet</remarks>
        int Height { get; }

        /// <summary>Gets the number of columns in the reference</summary>
        /// <remarks>The number of columns the reference spans on its sheet</remarks>
        int Width { get; }

        /// <summary>The reference's area as a rectangle</summary>
        /// <remarks>A convenience property for getting a reference's area as a rectangle</remarks>
        Rectangle Area { get; }

        /// <summary>Gets the sheet that the reference is on</summary>
        /// <remarks>This property lets you access the sheet that the reference is on</remarks>
        ISheet Sheet { get; }

        /// <summary>Returns a table of the reference's values</summary>
        /// <remarks>
        ///     This function returns a table that represents the reference's values from its sheet.  The first dimension is the
        ///     rows
        ///     and the second dimension is columns.  This method is useful when you wish to do lookups on a sheet reference's
        ///     values.
        /// </remarks>
        object[,] GetValuesTable();
    }
}