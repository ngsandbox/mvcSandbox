using System;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Miscellaneous;
using FormulaEngineCore.Operands;

namespace FormulaEngineCore.FormulaRererences
{
    [Serializable]
    public class CellReference : SheetReference, IFormulaSelfReference
    {
        private static readonly Regex _regex = new Regex(String.Concat("^", SHEET_REGEX, COLUMN_REGEX, ROW_REGEX, "$"));
        private readonly int _initialRowIndex;
        private readonly int _initialColumnIndex;

        public CellReference(int rowIndex, int colIndex)
        {
            ValidateColumnIndex(colIndex);
            ValidateRowIndex(rowIndex);
            _initialRowIndex = RowIndex = rowIndex;
            _initialColumnIndex = ColumnIndex = colIndex;
        }

        private CellReference(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            RowIndex = info.GetInt32("RowIndex");
            ColumnIndex = info.GetInt32("ColumnIndex");
            ComputeHashCode();
        }

        private IOperand TargetOperand
        {
            get
            {
                return Valid == false
                    ? new ErrorValueOperand(ErrorValueType.Ref)
                    : OperandFactory.CreateDynamic(TargetCellValue);
            }
        }

        public object TargetCellValue
        {
            get
            {
                return Sheet[RowIndex, ColumnIndex];
            }
        }

        public override Rectangle Range
        {
            get { return new Rectangle(ColumnIndex, RowIndex, 1, 1); }
        }

        public int RowIndex { get; private set; }

        public int ColumnIndex { get; private set; }

        public void OnFormulaRecalculate(Formula target)
        {
            object result = target.Evaluate();
            Sheet[RowIndex, ColumnIndex] = result;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("RowIndex", RowIndex);
            info.AddValue("ColumnIndex", ColumnIndex);
        }

        protected override GridOperationsBase CreateGridOps()
        {
            return new CellGridOps(this);
        }

        public static CellReference FromString(string image)
        {
            image = PrepareParseString(image);
            char c1 = image[0];
            char c2 = image[1];

            int rowStringIndex;
            int columnIndex;

            if (char.IsLetter(c2))
            {
                columnIndex = ColumnLabel2Index(c1, c2);
                rowStringIndex = 2;
            }
            else
            {
                columnIndex = ColumnLabel2Index(c1);
                rowStringIndex = 1;
            }

            string rowRef = image.Substring(rowStringIndex);
            int rowIndex = int.Parse(rowRef);

            return new CellReference(rowIndex, columnIndex);
        }

        public static ReferenceProperties CreateProperties(bool implicitSheet, string image)
        {
            var props = new Properties();
            GetProperties(implicitSheet, props);

            props.ColumnAbsolute = image.StartsWith("$");
            props.RowAbsolute = image.IndexOf("$", 1, StringComparison.Ordinal) != -1;

            return props;
        }

        public static bool IsValidString(string s)
        {
            return _regex.IsMatch(s);
        }

        public static void SwapRowProperties(ReferenceProperties props1, ReferenceProperties props2)
        {
            var realProps1 = (Properties)props1;
            var realProps2 = (Properties)props2;

            bool temp = realProps1.RowAbsolute;
            realProps1.RowAbsolute = realProps2.RowAbsolute;
            realProps2.RowAbsolute = temp;
        }

        public static void SwapColumnProperties(ReferenceProperties props1, ReferenceProperties props2)
        {
            var realProps1 = (Properties)props1;
            var realProps2 = (Properties)props2;

            bool temp = realProps1.ColumnAbsolute;
            realProps1.ColumnAbsolute = realProps2.ColumnAbsolute;
            realProps2.ColumnAbsolute = temp;
        }

        protected override void OnCopyInternal(int rowOffset, int colOffset, ISheet destSheet, ReferenceProperties props)
        {
            var realProps = (Properties)props;

            RowIndex = OffsetIndex(RowIndex, rowOffset, realProps.RowAbsolute);
            ColumnIndex = OffsetIndex(ColumnIndex, colOffset, realProps.ColumnAbsolute);
        }

        public override bool IsReferenceEqualForCircularReference(Reference @ref)
        {
            return ReferenceEquals(@ref, this);
        }

        protected override string FormatInternal()
        {
            return string.Concat(ColumnIndex2Label(ColumnIndex), RowIndex.ToString(CultureInfo.InvariantCulture));
        }

        public string FormatPlain()
        {
            return FormatInternal();
        }

        protected override string FormatWithPropsInternal(ReferenceProperties props)
        {
            var realProps = (Properties)props;

            string rowString = RowIndex.ToString(CultureInfo.InvariantCulture);
            string colString = ColumnIndex2Label(ColumnIndex);
            string rowAbsolute = GetAbsoluteString(realProps.RowAbsolute);
            string colAbsolute = GetAbsoluteString(realProps.ColumnAbsolute);

            return string.Concat(colAbsolute, colString, rowAbsolute, rowString);
        }

        public void Offset(int rowOffset, int colOffset)
        {
            RowIndex += rowOffset;
            ColumnIndex += colOffset;
        }

        public void SetColumn(int column)
        {
            ColumnIndex = column;
        }

        public void SetRow(int row)
        {
            RowIndex = row;
        }

        protected override IOperand ConvertInternal(OperandType convertType)
        {
            return TargetOperand.Convert(convertType);
        }

        protected override bool EqualsGridReference(SheetReference @ref)
        {
            var cellRef = (CellReference)@ref;
            return RowIndex == cellRef.RowIndex & ColumnIndex == cellRef.ColumnIndex;
        }

        protected override byte[] GetHashData()
        {
            var bytes = new byte[GRID_REFERENCE_HASH_SIZE + 4];
            GetBaseHashData(bytes);
            RowColumnIndexToBytes(RowIndex, bytes, GRID_REFERENCE_HASH_SIZE);
            RowColumnIndexToBytes(ColumnIndex, bytes, GRID_REFERENCE_HASH_SIZE + 2);
            return bytes;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hash = (int)2166136261;
                hash = hash * 16777619 ^ Sheet.Name.GetHashCode();
                hash = hash * 16777619 ^ _initialRowIndex.GetHashCode();
                hash = hash * 16777619 ^ _initialColumnIndex.GetHashCode();
                return hash;
            }
        }

        private class CellGridOps : GridOperationsBase
        {
            private readonly CellReference _owner;

            public CellGridOps(CellReference owner)
            {
                _owner = owner;
            }

            public override ReferenceOperationResultType OnColumnsInserted(int insertAt, int count)
            {
                if (_owner.ColumnIndex < insertAt) return ReferenceOperationResultType.NotAffected;
                // We are in the inserted area; we must adjust
                _owner.ColumnIndex += count;
                return ReferenceOperationResultType.Affected;
            }

            public override ReferenceOperationResultType OnColumnsRemoved(int removeAt, int count)
            {
                if (_owner.ColumnIndex > removeAt + count - 1)
                {
                    // We are to the right of the removed hole
                    _owner.ColumnIndex -= count;
                    return ReferenceOperationResultType.Affected;
                }
                if (_owner.ColumnIndex >= removeAt)
                {
                    // We are in the removed hole
                    return ReferenceOperationResultType.Invalidated;
                }
                return ReferenceOperationResultType.NotAffected;
            }

            public override ReferenceOperationResultType OnRangeMoved(SheetReference source, SheetReference dest)
            {
                Rectangle sourceRect = source.Range;
                Rectangle destRect = dest.Range;
                int rowOffset = destRect.Top - sourceRect.Top;
                int colOffset = destRect.Left - sourceRect.Left;
                Rectangle myRect = _owner.Range;

                bool isContainedInSource = sourceRect.Contains(myRect) & _owner.IsOnSheet(source.Sheet);
                bool isContainedInDest = destRect.Contains(myRect) & _owner.IsOnSheet(dest.Sheet);

                if (isContainedInSource)
                {
                    // We are in the moved range so we have to adjust
                    _owner.RowIndex += rowOffset;
                    _owner.ColumnIndex += colOffset;
                    _owner.SetSheetForRangeMove(dest.Sheet);
                    return ReferenceOperationResultType.Affected;
                }
                if (isContainedInDest)
                {
                    // We are overwritten by the moved range
                    return ReferenceOperationResultType.Invalidated;
                }
                // We are not affected
                return ReferenceOperationResultType.NotAffected;
            }

            public override ReferenceOperationResultType OnRowsInserted(int insertAt, int count)
            {
                if (_owner.RowIndex >= insertAt)
                {
                    _owner.RowIndex += count;
                    return ReferenceOperationResultType.Affected;
                }
                return ReferenceOperationResultType.NotAffected;
            }

            public override ReferenceOperationResultType OnRowsRemoved(int removeAt, int count)
            {
                if (_owner.RowIndex <= removeAt + count - 1)
                    return _owner.RowIndex >= removeAt
                        ? ReferenceOperationResultType.Invalidated
                        : ReferenceOperationResultType.NotAffected;
                // We are below the removed hole
                _owner.RowIndex -= count;
                return ReferenceOperationResultType.Affected;
            }
        }

        [Serializable]
        private class Properties : GridReferenceProperties
        {
            public bool ColumnAbsolute;
            public bool RowAbsolute;
        }
    }
}