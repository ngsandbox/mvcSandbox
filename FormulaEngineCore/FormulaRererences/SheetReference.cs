using System;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.Runtime.Serialization;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.FormulaRererences
{
    /// <summary>
    ///     Base class for references that are on a sheet
    /// </summary>
    [Serializable]
    public abstract class SheetReference : Reference, ISheetReference
    {
        protected const string COLUMN_REGEX = "[a-zа-яA-ZА-Я]{1,2}";
        protected const string ROW_REGEX = "[0-9]{1,5}";
        protected const string SHEET_REGEX = "([_a-zа-яA-ZА-Я][a-zа-яA-ZА-Я]*!)?";
        protected const int MAX_ROW = UInt16.MaxValue;
        protected const int GRID_REFERENCE_HASH_SIZE = REFERENCE_HASH_SIZE + 4;
        protected static readonly int MAX_COLUMN = (int)((Math.Pow(26, 2)) + 26);
        protected const int MIN_INDEX = 1;
        private ISheet _sheet;
        private string _sheetName;


        protected SheetReference()
        {
        }

        protected SheetReference(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Sheet = (ISheet)info.GetValue("Sheet", typeof(ISheet));
        }

        public virtual bool IsEmptyIntersection
        {
            get { return false; }
        }

        public abstract Rectangle Range { get; }

        public Rectangle GridRectangle
        {
            get { return new Rectangle(MIN_INDEX, MIN_INDEX, Sheet.ColumnCount, Sheet.RowCount); }
        }

        public object[,] GetValuesTable()
        {
            Rectangle rect = Range;
            var arr = new object[rect.Height, rect.Width];

            for (int row = rect.Top; row < rect.Bottom; row++)
            {
                for (int col = rect.Left; col < rect.Right; col++)
                {
                    arr[row - rect.Top, col - rect.Left] = Sheet[row, col];
                }
            }

            return arr;
        }

        public Rectangle Area
        {
            get { return GridRectangle; }
        }

        public ISheet Sheet
        {
            get { return _sheet; }
            set
            {
                _sheet = value;
                OnSheetSet(_sheet);
            }
        }

        public int Row
        {
            get { return Range.Top; }
        }

        public int Column
        {
            get { return Range.Left; }
        }

        public int Height
        {
            get { return Range.Height; }
        }

        public int Width
        {
            get { return Range.Width; }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Sheet", Sheet);
        }

        public static bool IsValidColumnIndex(int columnIndex)
        {
            return !(columnIndex < MIN_INDEX || columnIndex > MAX_COLUMN);
        }

        public static bool IsValidRowIndex(int rowIndex)
        {
            return !(rowIndex < MIN_INDEX || rowIndex > MAX_ROW);
        }

        protected static void ValidateColumnIndex(int columnIndex)
        {
            if (IsValidColumnIndex(columnIndex) == false)
            {
                throw new ArgumentOutOfRangeException("columnIndex",
                    String.Format("Value must be greater than {0} and less than {1}", MIN_INDEX, MAX_COLUMN));
            }
        }

        protected static void ValidateRowIndex(int rowIndex)
        {
            if (IsValidRowIndex(rowIndex) == false)
            {
                throw new ArgumentOutOfRangeException("rowIndex",
                    String.Format("Value must be greater than {0} and less than {1}", MIN_INDEX, MAX_ROW));
            }
        }

        public static void GetProperties(bool implicitSheet, ReferenceProperties props)
        {
            ((GridReferenceProperties)props).ImplicitSheet = implicitSheet;
        }

        protected static void GetProperties(bool implicitSheet, GridReferenceProperties props)
        {
            props.ImplicitSheet = implicitSheet;
        }

        protected static string PrepareParseString(string s)
        {
            s = RemoveSheetReference(s);
            return s.Replace("$", string.Empty);
        }

        protected static string RemoveSheetReference(string s)
        {
            int bangIndex = s.IndexOf('!');
            int count = Math.Max(0, bangIndex + 1);
            s = s.Remove(0, count);
            return s;
        }

        public static ReferenceParseProperties CreateParseProperties(string s)
        {
            var props = new ReferenceParseProperties();

            int bangIndex = s.IndexOf('!');
            if (bangIndex != -1)
            {
                props.SheetName = s.Substring(0, bangIndex);
            }

            return props;
        }

        public override void ProcessParseProperties(ReferenceParseProperties props, FormulaEngine engine)
        {
            ISheet sheet = props.SheetName == null ? engine.Sheets.ActiveSheet :
                engine.Sheets.GetSheetByName(props.SheetName);
            _sheetName = props.SheetName;
            if (sheet != null)
            {
                Sheet = sheet;
            }
        }

        private Exception GetValidateException()
        {
            Exception ex;

            if (Engine.Sheets.Count == 0)
            {
                ex = new InvalidOperationException("The formula has a sheet reference but no sheets are defined");
            }
            else if (Sheet == null)
            {
                ex = new ArgumentException(String.Format("No sheet with that name '{0}' exists", _sheetName));
            }
            else if (!IsInGrid())
            {
                ex = new ArgumentException(String.Format("Grid reference {0} is not contained in sheet {1}", ToString(), Sheet.Name));
            }
            else
            {
                ex = null;
            }

            return ex;
        }

        // Validate for reference factory
        public override void Validate()
        {
            Exception ex = GetValidateException();

            if (ex != null)
            {
                throw ex;
            }
        }

        // Validate for formula
        public override void Validate(FormulaEngine engine)
        {
            base.Validate(engine);
            Exception ex = GetValidateException();
            if (ex != null)
            {
                throw new InvalidFormulaException(ex);
            }
        }

        protected override void CopyToReference(Reference target)
        {
            base.CopyToReference(target);
            var gRef = (SheetReference)target;
            gRef.Sheet = Sheet;
        }

        public static string ColumnIndex2Label(int columnIndex)
        {
            ValidateColumnIndex(columnIndex);

            columnIndex -= 1;

            int firstIndex = columnIndex / 26;
            int secondIndex = columnIndex % 26;

            return firstIndex == 0 ?
                System.Convert.ToChar(65 + secondIndex).ToString(CultureInfo.InvariantCulture) :
                new String(new[] { System.Convert.ToChar(65 + firstIndex - 1), System.Convert.ToChar(65 + secondIndex) });
        }

        public static int ColumnLabel2Index(char char1, char char2 = Char.MinValue)
        {
            char1 = Char.ToUpper(char1);
            int index1 = System.Convert.ToInt32(char1) - 65 + 1;

            if (char2 != Char.MinValue)
            {
                char2 = Char.ToUpper(char2);
                int index2 = System.Convert.ToInt32(char2) - 65;
                return index1 * 26 + (index2 + 1);
            }

            return index1;
        }

        protected static string GetAbsoluteString(bool absolute)
        {
            return absolute ? "$" : string.Empty;
        }

        protected static int OffsetIndex(int index, int offset, bool isAbsolute)
        {
            return isAbsolute == false ? index + offset : index;
        }

        public override sealed void OnCopy(int rowOffset, int colOffset, ISheet destSheet, ReferenceProperties props)
        {
            var gridProps = (GridReferenceProperties)props;
            if (gridProps.ImplicitSheet)
            {
                Sheet = destSheet;
            }

            OnCopyInternal(rowOffset, colOffset, destSheet, props);
            if (IsInGrid() == false)
            {
                MarkAsInvalid();
            }
        }

        protected abstract void OnCopyInternal(int rowOffset, int colOffset, ISheet destSheet, ReferenceProperties props);

        protected override sealed string FormatWithProps(ReferenceProperties props)
        {
            var realProps = (GridReferenceProperties)props;
            string refString = FormatWithPropsInternal(props);
            string sheetString = string.Empty;
            string bang = string.Empty;

            if (!realProps.ImplicitSheet)
            {
                sheetString = Sheet.Name;
                bang = "!";
            }

            return string.Concat(sheetString, bang, refString);
        }

        protected abstract string FormatWithPropsInternal(ReferenceProperties props);
        protected abstract string FormatInternal();

        protected override sealed string Format()
        {
            string refString = FormatInternal();
            return string.Concat(_sheetName, "!", refString);
        }

        protected override void GetReferenceValuesInternal(IReferenceValueProcessor processor)
        {
            Rectangle rect = Range;

            for (int row = rect.Top; row <= rect.Bottom - 1; row++)
            {
                for (int col = rect.Left; col <= rect.Right - 1; col++)
                {
                    object value = Sheet[row, col];
                    if (processor.ProcessValue(value) == false)
                    {
                        return;
                    }
                }
            }
        }

        public override sealed bool Intersects(Reference @ref)
        {
            if (!(@ref is SheetReference))
            {
                return false;
            }

            var gref = (SheetReference)@ref;
            return ReferenceEquals(Sheet, gref.Sheet) && Range.IntersectsWith(gref.Range);
        }

        protected virtual void OnSheetSet(ISheet sheet) { }

        public void SetSheetForRangeMove(ISheet sheet)
        {
            if (ReferenceEquals(Sheet, sheet))
            {
                return;
            }

            Sheet = sheet;
            IList props = Engine.GetReferenceProperties(this);
            ClearImplicitSheet(props);
        }

        private void ClearImplicitSheet(IList props)
        {
            foreach (GridReferenceProperties prop in props)
            {
                prop.ImplicitSheet = false;
            }
        }

        protected bool IsInGrid()
        {
            return GridRectangle.Contains(Range);
        }

        public static bool IsRectangleInSheet(Rectangle rect, ISheet sheet)
        {
            var gridRect = GetSheetRectangle(sheet);
            return gridRect.Contains(rect);
        }

        public override bool IsOnSheet(ISheet sheet)
        {
            return ReferenceEquals(Sheet, sheet);
        }

        protected override void GetBaseHashData(byte[] bytes)
        {
            base.GetBaseHashData(bytes);
            int sheetHashCode = Sheet.GetHashCode();
            IntegerToBytes(sheetHashCode, bytes, REFERENCE_HASH_SIZE);
        }

        protected override sealed bool EqualsReferenceInternal(Reference @ref)
        {
            var gridRef = (SheetReference)@ref;
            return IsOnSheet(gridRef.Sheet) && EqualsGridReference(gridRef);
        }

        public static Rectangle GetSheetRectangle(ISheet sheet)
        {
            return new Rectangle(MIN_INDEX, MIN_INDEX, sheet.ColumnCount, sheet.RowCount);
        }

        protected abstract bool EqualsGridReference(SheetReference @ref);

        [Serializable]
        public abstract class GridReferenceProperties : ReferenceProperties
        {
            // Was the sheet name explicitly specified in the formula?
            public bool ImplicitSheet;
        }
    }
}