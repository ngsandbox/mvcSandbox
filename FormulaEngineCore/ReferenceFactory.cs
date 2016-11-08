using System;
using System.Drawing;
using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore
{
    /// <summary>
    ///     Creates all references used by the formula engine
    /// </summary>
    /// <remarks>
    ///     This class is responsible for creating all references that you will use when interacting with the formula engine.
    ///     It has methods for creating sheet and non-sheet references.  Sheet references can be created from a rectangle,
    ///     string, or
    ///     integer indices.  There are overloads for creating references to a specific sheet or to the currently active sheet.
    ///     Non-grid references
    ///     such as named and external references are also created by this class.
    /// </remarks>
    public sealed class ReferenceFactory
    {
        private readonly FormulaEngine _owner;

        internal ReferenceFactory(FormulaEngine owner)
        {
            _owner = owner;
        }

        /// <summary>
        ///     Creates a sheet reference from a string
        /// </summary>
        /// <param name="s">A string that contains a sheet reference expression</param>
        /// <returns>A sheet reference parsed from the given string</returns>
        /// <remarks>
        ///     This method creates a sheet reference by parsing a given string.
        ///     The method accepts strings with the following syntax:
        ///     <list type="table">
        ///         <listheader>
        ///             <term>String format</term><description>Resultant reference</description>
        ///         </listheader>
        ///         <item>
        ///             <term>Column letter followed by a row number: "C3"</term><description>Cell</description>
        ///         </item>
        ///         <item>
        ///             <term>Two column letter and row number pairs separated by a colon: "C3:D4"</term>
        ///             <description>Cell range</description>
        ///         </item>
        ///         <item>
        ///             <term>Two column letters separated by a colon: "E:G"</term><description>Columns</description>
        ///         </item>
        ///         <item>
        ///             <term>Two row numbers separated by a colon: "4:6"</term><description>Rows</description>
        ///         </item>
        ///     </list>
        ///     All of the above formats can specify a specific sheet by prefixing the reference with a sheet name
        ///     followed by an exclamation point (ie: "Sheet2!E:G")
        ///     If no sheet name is specified, the currently active sheet is used.
        /// </remarks>
        /// <example>
        ///     This example shows how you would create sheet references from various strings
        ///     <code>
        /// ' Get a reference to cell A1
        /// Dim cellRef As ISheetReference = factory.Parse("A1")
        /// ' Get a reference to cells B2 through E4
        /// Dim rangeRef As ISheetReference = factory.Parse("b2:e4")
        /// ' Get a reference to columns D through F
        /// Dim colsRef As ISheetReference = factory.Parse("D:F")
        /// 'Get a reference to rows 4 through 6
        /// Dim rowsRef As ISheetReference = factory.Parse("4:6")
        /// ' Get a reference to cell C4 on sheet 'Sheet4'
        /// Dim cellRef As ISheetReference = factory.Parse("Sheet4!C4")
        /// </code>
        /// </example>
        /// <exception cref="T:System.ArgumentException">
        ///     <para>The given string could not be parsed into a sheet reference</para>
        ///     <para>The string references a sheet name that is not defined</para>
        ///     <para>The resulting sheet reference is not within the bounds of its sheet</para>
        /// </exception>
        public ISheetReference Parse(string s)
        {
            FormulaEngine.ValidateNonNull(s, "s");
            SheetReference @ref = TryParseGridReference(s);
            if (@ref == null)
            {
                OnInvalidReferenceString();
            }

            InitializeParsedGridReference(@ref, s);

            return @ref;
        }

        private SheetReference TryParseGridReference(string s)
        {
            if (CellReference.IsValidString(s))
            {
                return CellReference.FromString(s);
            }
            if (CellRangeReference.IsValidString(s))
            {
                return CellRangeReference.FromString(s);
            }
            if (ColumnReference.IsValidString(s))
            {
                return ColumnReference.FromString(s);
            }
            if (RowReference.IsValidString(s))
            {
                return RowReference.FromString(s);
            }
            return null;
        }

        private void InitializeParsedGridReference(SheetReference @ref, string s)
        {
            ReferenceParseProperties parseProps = SheetReference.CreateParseProperties(s);

            @ref.SetEngine(_owner);
            @ref.ProcessParseProperties(parseProps, _owner);
            @ref.Validate();
            @ref.ComputeHashCode();
        }

        /// <summary>
        ///     Creates a sheet reference from a rectangle
        /// </summary>
        /// <param name="rect">The rectangle to create the sheet reference from</param>
        /// <returns>A sheet reference that matches the given rectangle</returns>
        /// <remarks>
        ///     This method is identical to
        ///     <see
        ///         cref="M:ciloci.FormulaEngine.ReferenceFactory.FromRectangle(ciloci.FormulaEngine.ISheet,System.Drawing.Rectangle)" />
        ///     except that the returned reference is on the currently active sheet.
        /// </remarks>
        public ISheetReference FromRectangle(Rectangle rect)
        {
            return FromRectangle(_owner.Sheets.ActiveSheet, rect);
        }

        /// <summary>
        ///     Creates a sheet reference from a rectangle and a sheet
        /// </summary>
        /// <param name="rect">The rectangle to create the sheet reference from</param>
        /// <param name="sheet">The sheet that the reference will be on</param>
        /// <returns>A sheet reference that matches the given rectangle and is on the given sheet</returns>
        /// <remarks>
        ///     Use this method when you have a rectangle that you would like translated into a sheet reference.  Note that the
        ///     top-left corner of the sheet is (1,1).  The method will try to create the appropriate type of reference based on
        ///     the
        ///     dimensions of the rectangle.  For example: A rectangle 1 unit wide and 1 unit tall will be translated into a cell
        ///     reference
        /// </remarks>
        /// <exception cref="T:System.ArgumentException">
        ///     <para>The resulting sheet reference is not within the bounds of its sheet</para>
        ///     <para>The given sheet argument is not registered with the SheetManager</para>
        /// </exception>
        /// <example>
        ///     The following code creates a reference to the range A1:B2 on the currently active sheet
        ///     <code>
        /// Dim engine As New FormulaEngine
        /// Dim rect As New Rectangle(1, 1, 2, 2)
        /// Dim ref As ISheetReference = factory.FromRectangle(rect)
        /// </code>
        /// </example>
        public ISheetReference FromRectangle(ISheet sheet, Rectangle rect)
        {
            FormulaEngine.ValidateNonNull(sheet, "sheet");
            SheetReference @ref;
            Rectangle sheetRect = SheetReference.GetSheetRectangle(sheet);

            if (rect.Width == 1 & rect.Height == 1)
            {
                @ref = new CellReference(rect.Top, rect.Left);
            }
            else if (rect.Height == sheetRect.Height)
            {
                @ref = new ColumnReference(rect.Left, rect.Right - 1);
            }
            else if (rect.Width == sheetRect.Width)
            {
                @ref = new RowReference(rect.Top, rect.Bottom - 1);
            }
            else
            {
                @ref = new CellRangeReference(rect);
            }

            InitializeGridReference(@ref, sheet);
            return @ref;
        }

        private void OnInvalidReferenceString()
        {
            throw new ArgumentException("The value could not be parsed into a reference");
        }

        private void InitializeGridReference(SheetReference @ref, ISheet sheet)
        {
            @ref.SetEngine(_owner);
            ValidateSheet(sheet);
            @ref.Sheet = sheet;
            @ref.Validate();
            @ref.ComputeHashCode();
        }

        private void ValidateSheet(ISheet sheet)
        {
            if (_owner.Sheets.Contains(sheet) == false)
            {
                throw new ArgumentException("The sheet does not exist in the sheets collection");
            }
        }

        /// <summary>
        ///     Creates a sheet reference to a specific cell on the active sheet
        /// </summary>
        /// <param name="row">The row of the cell; first row is 1</param>
        /// <param name="column">The column of the cell; first column is 1</param>
        /// <returns>A sheet reference to the specified row and column on the active sheet</returns>
        /// <remarks>
        ///     This method behaves exactly like
        ///     <see cref="M:ciloci.FormulaEngine.ReferenceFactory.Cell(ciloci.FormulaEngine.ISheet,System.Int32,System.Int32)" />
        ///     except that it uses the currently active sheet
        /// </remarks>
        public ISheetReference Cell(int row, int column)
        {
            return Cell(_owner.Sheets.ActiveSheet, row, column);
        }

        /// <summary>
        ///     Creates a sheet reference to a cell on a specific sheet
        /// </summary>
        /// <param name="sheet">The sheet the reference will be on</param>
        /// <param name="row">The row of the cell; first row is 1</param>
        /// <param name="column">The column of the cell; first column is 1</param>
        /// <returns>A sheet reference to the specified row and column and on the given sheet</returns>
        /// <remarks>
        ///     Use this method when you need a sheet reference to a specific cell and sheet and have
        ///     the row and column indices handy.
        ///     You usually need a cell reference when you wish to bind a formula to a cell
        /// </remarks>
        /// <exception cref="T:System.ArgumentException">
        ///     <para>The cell at row,col is not within the bounds of the given sheet</para>
        ///     <para>The given sheet argument is not registered with the SheetManager</para>
        /// </exception>
        /// <example>
        ///     The following code creates a reference to the cell C3 on the currently active sheet
        ///     <code>
        /// Dim engine As New FormulaEngine
        /// Dim ref As ISheetReference = engine.ReferenceFactory.Cell(3, 3)
        /// </code>
        /// </example>
        public ISheetReference Cell(ISheet sheet, int row, int column)
        {
            FormulaEngine.ValidateNonNull(sheet, "sheet");
            var @ref = new CellReference(row, column);
            InitializeGridReference(@ref, sheet);
            return @ref;
        }

        /// <summary>
        ///     Creates a sheet reference to a range of cells on the currently active sheet
        /// </summary>
        /// <param name="startRow">The top of the range</param>
        /// <param name="startColumn">The left of the range</param>
        /// <param name="endRow">The right of the range</param>
        /// <param name="endColumn">The bottom of the range</param>
        /// <returns>A sheet reference to the specified range on the currently active sheet</returns>
        /// <remarks>
        ///     This method behaves exactly like
        ///     <see
        ///         cref="M:ciloci.FormulaEngine.ReferenceFactory.Cells(ciloci.FormulaEngine.ISheet,System.Int32,System.Int32,System.Int32,System.Int32)" />
        ///     except that it uses the currently active sheet
        /// </remarks>
        public ISheetReference Cells(int startRow, int startColumn, int endRow, int endColumn)
        {
            return Cells(_owner.Sheets.ActiveSheet, startRow, startColumn, endRow, endColumn);
        }

        /// <summary>
        ///     Creates a sheet reference to a range of cells on a given sheet
        /// </summary>
        /// <param name="sheet">The sheet the reference will be on</param>
        /// <param name="startRow">The top row of the range</param>
        /// <param name="startColumn">The left column of the range</param>
        /// <param name="endRow">The bottom row of the range</param>
        /// <param name="endColumn">The right column of the range</param>
        /// <returns>A sheet reference to the specified range on the specified sheet</returns>
        /// <remarks>
        ///     Use this method when you need a sheet reference to a range of cells on a specific sheet and have the four
        ///     indices handy.
        /// </remarks>
        /// <exception cref="T:System.ArgumentException">
        ///     <para>The resultant range is not within the bounds of the given sheet</para>
        ///     <para>The given sheet argument is not registered with the SheetManager</para>
        /// </exception>
        /// <example>
        ///     The following example creates a reference to the range C3:E4 on the currently active sheet
        ///     <code>
        /// Dim engine As New FormulaEngine
        /// Dim ref As ISheetReference = engine.ReferenceFactory.Cells(3, 3, 4, 5)
        /// </code>
        /// </example>
        public ISheetReference Cells(ISheet sheet, int startRow, int startColumn, int endRow, int endColumn)
        {
            FormulaEngine.ValidateNonNull(sheet, "sheet");
            var @ref = new CellRangeReference(startRow, startColumn, endRow, endColumn);
            InitializeGridReference(@ref, sheet);
            return @ref;
        }

        /// <summary>
        ///     Creates a reference to a range of rows on the active sheet
        /// </summary>
        /// <param name="start">The top row of the range</param>
        /// <param name="finish">The bottom row of the range</param>
        /// <returns>A sheet reference to the range of rows on the active sheet</returns>
        /// <remarks>
        ///     This method behaves exactly like
        ///     <see cref="M:ciloci.FormulaEngine.ReferenceFactory.Rows(ciloci.FormulaEngine.ISheet,System.Int32,System.Int32)" />
        ///     except that it uses the currently active sheet
        /// </remarks>
        public ISheetReference Rows(int start, int finish)
        {
            return Rows(_owner.Sheets.ActiveSheet, start, finish);
        }

        /// <summary>
        ///     Creates a reference to a range of rows on a given sheet
        /// </summary>
        /// <param name="sheet">The sheet the reference will use</param>
        /// <param name="start">The top row of the range</param>
        /// <param name="finish">The bottom row of the range</param>
        /// <returns>A sheet reference to the range of rows on the given sheet</returns>
        /// <remarks>
        ///     This method will create a sheet reference to an entire range of rows on the given sheet.  Use it when
        ///     you wish to reference entire rows and have the two indices handy.
        /// </remarks>
        /// <exception cref="T:System.ArgumentException">
        ///     <para>The resultant range is not within the bounds of the given sheet</para>
        ///     <para>The given sheet argument is not registered with the SheetManager</para>
        /// </exception>
        /// <example>
        ///     The following example creates a reference to rows 5 through 7 on the currently active sheet
        ///     <code>
        /// Dim engine As New FormulaEngine
        /// Dim ref As ISheetReference = engine.ReferenceFactory.Rows(5, 7)
        /// </code>
        /// </example>
        public ISheetReference Rows(ISheet sheet, int start, int finish)
        {
            FormulaEngine.ValidateNonNull(sheet, "sheet");
            var @ref = new RowReference(start, finish);
            InitializeGridReference(@ref, sheet);
            return @ref;
        }

        /// <summary>
        ///     Creates a reference to a range of columns on the active sheet
        /// </summary>
        /// <param name="start">The left column of the range</param>
        /// <param name="finish">The right column of the range</param>
        /// <returns>A sheet reference to the range of columns on the given sheet</returns>
        /// <remarks>
        ///     This method behaves exactly like
        ///     <see cref="M:ciloci.FormulaEngine.ReferenceFactory.Columns(ciloci.FormulaEngine.ISheet,System.Int32,System.Int32)" />
        ///     except that it uses the currently active sheet
        /// </remarks>
        public ISheetReference Columns(int start, int finish)
        {
            return Columns(_owner.Sheets.ActiveSheet, start, finish);
        }

        /// <summary>
        ///     Creates a reference to a range of columns on a given sheet
        /// </summary>
        /// <param name="sheet">The sheet the reference will use</param>
        /// <param name="start">The left column of the range</param>
        /// <param name="finish">The right column of the range</param>
        /// <returns>A sheet reference to the range of columns on the given sheet</returns>
        /// <remarks>
        ///     This method will create a sheet reference to an entire range of columns on the given sheet.  Use it when you
        ///     want to reference entire columns and have the two indices handy.
        /// </remarks>
        /// <exception cref="T:System.ArgumentException">
        ///     <para>The resultant range is not within the bounds of the given sheet</para>
        ///     <para>The given sheet argument is not registered with the SheetManager</para>
        /// </exception>
        /// <example>
        ///     The following example creates a reference to columns A through C on the currently active sheet
        ///     <code>
        /// Dim engine As New FormulaEngine
        /// Dim ref As ISheetReference = engine.ReferenceFactory.Columns(1, 3)
        /// </code>
        /// </example>
        public ISheetReference Columns(ISheet sheet, int start, int finish)
        {
            FormulaEngine.ValidateNonNull(sheet, "sheet");
            var @ref = new ColumnReference(start, finish);
            InitializeGridReference(@ref, sheet);
            return @ref;
        }

        /// <summary>
        ///     Creates a named reference
        /// </summary>
        /// <param name="name">The name of the reference</param>
        /// <returns>A reference to the name</returns>
        /// <remarks>
        ///     A named reference lets you refer to a formula by a name and lets you refer to that name in other formulas.
        ///     A valid name must start with an underscore or letter and can be followed by any combination of underscores,
        ///     letters, and numbers.
        /// </remarks>
        /// <exception cref="T:System.ArgumentException">
        ///     <para>The name argument is not in the proper format for a named reference</para>
        /// </exception>
        public INamedReference Named(string name)
        {
            FormulaEngine.ValidateNonNull(name, "name");
            if (NamedReference.IsValidName(name) == false)
            {
                OnInvalidReferenceString();
            }
            var @ref = new NamedReference(name);
            @ref.SetEngine(_owner);
            @ref.ComputeHashCode();
            return @ref;
        }

        /// <summary>
        ///     Creates an external reference
        /// </summary>
        /// <returns>An external reference</returns>
        /// <remarks>
        ///     External references are useful when you need to have many formulas outside of a grid (hence the name) and don't
        ///     want to create unique names for each formula
        /// </remarks>
        public IExternalReference External()
        {
            var @ref = new ExternalReference();
            @ref.SetEngine(_owner);
            @ref.ComputeHashCode();
            return @ref;
        }
    }
}