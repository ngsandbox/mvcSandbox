using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Miscellaneous;

namespace FormulaEngineCore.FormulaRererences
{
    [Serializable]
    internal class CellRangeReference : SheetReference
    {
        private static readonly Regex _regex =
            new Regex(String.Format("^{0}{1}:{1}$", SHEET_REGEX, String.Concat(COLUMN_REGEX, ROW_REGEX)));
        private CellReference _finish;
        private CellReference _start;

        private CellRangeReference()
        {
        }

        private CellRangeReference(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _start = (CellReference)info.GetValue("Start", typeof(CellReference));
            _finish = (CellReference)info.GetValue("Finish", typeof(CellReference));
            ComputeHashCode();
        }

        public CellRangeReference(int startRow, int startCol, int finishRow, int finishCol)
        {
            SetRange(startRow, startCol, finishRow, finishCol);
        }

        public CellRangeReference(Rectangle rect)
        {
            SetRange(rect);
        }

        public override bool CanRangeLink
        {
            get { return true; }
        }

        public override Rectangle Range
        {
            get
            {
                int h = _finish.Row - _start.Row + 1;
                int w = _finish.Column - _start.Column + 1;
                return new Rectangle(_start.Column, _start.Row, w, h);
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Start", _start);
            info.AddValue("Finish", _finish);
        }

        public static CellRangeReference FromString(string image)
        {
            image = PrepareParseString(image);
            string[] arr = image.Split(':');
            CellReference start = CellReference.FromString(arr[0]);
            CellReference finish = CellReference.FromString(arr[1]);

            // Handle backwards references
            if (start.Row > finish.Row)
            {
                int tempRow = start.Row;
                start.SetRow(finish.Row);
                finish.SetRow(tempRow);
            }

            if (start.Column > finish.Column)
            {
                int tempcol = start.Column;
                start.SetColumn(finish.Column);
                finish.SetColumn(tempcol);
            }

            return new CellRangeReference { _start = start, _finish = finish };
        }

        public static ReferenceProperties CreateProperties(bool implicitSheet, string image)
        {
            var props = new Properties();
            GetProperties(implicitSheet, props);

            string[] parts = image.Split(':');

            props._startProps = CellReference.CreateProperties(true, parts[0]);
            props._finishProps = CellReference.CreateProperties(true, parts[1]);

            return props;
        }


        public static bool IsValidString(string s)
        {
            return _regex.IsMatch(s);
        }

        protected override GridOperationsBase CreateGridOps()
        {
            return new CellRangeGridOps(this);
        }

        private void SetRange(int startRow, int startCol, int finishRow, int finishCol)
        {
            _start = new CellReference(startRow, startCol);
            _finish = new CellReference(finishRow, finishCol);
            InitializeInnerRefs();
        }

        private void SetRange(Rectangle rect)
        {
            SetRange(rect.Top, rect.Left, rect.Bottom - 1, rect.Right - 1);
        }

        protected override void OnEngineSet(FormulaEngine engine)
        {
            InitializeInnerRefs();
        }

        protected override void OnSheetSet(ISheet sheet)
        {
            _start.Sheet = sheet;
            _finish.Sheet = sheet;
        }

        private void InitializeInnerRefs()
        {
            _start.SetEngine(Engine);
            _finish.SetEngine(Engine);
            _start.Sheet = Sheet;
            _finish.Sheet = Sheet;
        }

        protected override bool EqualsGridReference(SheetReference @ref)
        {
            var rangeRef = (CellRangeReference)@ref;
            return _start.EqualsReference(rangeRef._start) & _finish.EqualsReference(rangeRef._finish);
        }

        protected override void OnCopyInternal(int rowOffset, int colOffset, ISheet destSheet, ReferenceProperties props)
        {
            var realProps = (Properties)props;
            _start.OnCopy(rowOffset, colOffset, destSheet, realProps._startProps);
            _finish.OnCopy(rowOffset, colOffset, destSheet, realProps._finishProps);
            OrderReferencesAfterOffset(realProps);
        }

        private void OrderReferencesAfterOffset(Properties props)
        {
            int startColumn = _start.Column;
            int finishColumn = _finish.Column;

            int startRow = _start.Row;
            int finishRow = _finish.Row;

            if (startColumn > finishColumn)
            {
                int tempcol = startColumn;
                _start.SetColumn(finishColumn);
                _finish.SetColumn(tempcol);

                CellReference.SwapColumnProperties(props._startProps, props._finishProps);
            }

            if (startRow > finishRow)
            {
                int tempRow = startRow;
                _start.SetRow(finishRow);
                _finish.SetRow(tempRow);
                CellReference.SwapRowProperties(props._startProps, props._finishProps);
            }
        }

        public override bool IsReferenceEqualForCircularReference(Reference @ref)
        {
            return Intersects(@ref);
        }

        protected override void InitializeClone(Reference clone)
        {
            var rangeClone = (CellRangeReference)clone;
            rangeClone._start = (CellReference)_start.Clone();
            rangeClone._finish = (CellReference)_finish.Clone();
        }

        protected override string FormatInternal()
        {
            string startString = _start.FormatPlain();
            string finishString = _finish.FormatPlain();
            return String.Concat(startString, ":", finishString);
        }

        protected override string FormatWithPropsInternal(ReferenceProperties props)
        {
            var realProps = (Properties)props;
            string startString = _start.ToStringFormula(realProps._startProps);
            string finishString = _finish.ToStringFormula(realProps._finishProps);
            return String.Concat(startString, ":", finishString);
        }

        protected override byte[] GetHashData()
        {
            var bytes = new byte[GRID_REFERENCE_HASH_SIZE + 4 + 4];
            GetBaseHashData(bytes);

            RowColumnIndexToBytes(_start.RowIndex, bytes, GRID_REFERENCE_HASH_SIZE);
            RowColumnIndexToBytes(_start.ColumnIndex, bytes, GRID_REFERENCE_HASH_SIZE + 2);

            RowColumnIndexToBytes(_finish.RowIndex, bytes, GRID_REFERENCE_HASH_SIZE + 4);
            RowColumnIndexToBytes(_finish.ColumnIndex, bytes, GRID_REFERENCE_HASH_SIZE + 6);

            return bytes;
        }

        private class CellRangeGridOps : GridOperationsBase
        {
            private readonly CellRangeReference _owner;

            public CellRangeGridOps(CellRangeReference owner)
            {
                _owner = owner;
            }

            public override ReferenceOperationResultType OnColumnsInserted(int insertAt, int count)
            {
                ReferenceOperationResultType startAffected = _owner._start.GridOps.OnColumnsInserted(insertAt, count);
                ReferenceOperationResultType finishAffected = _owner._finish.GridOps.OnColumnsInserted(insertAt, count);

                return startAffected | finishAffected;
            }

            public override ReferenceOperationResultType OnColumnsRemoved(int removeAt, int count)
            {
                Point result = HandleRangeRemoved(_owner._start.Column, _owner._finish.Column, removeAt, count);

                if (result.IsEmpty)
                {
                    return ReferenceOperationResultType.Invalidated;
                }
                bool startAffected = result.X != _owner._start.Column;
                bool finishAffected = result.Y != _owner._finish.Column;

                _owner._start.SetColumn(result.X);
                _owner._finish.SetColumn(result.Y);

                return Affected2Enum(startAffected | finishAffected);
            }

            public override ReferenceOperationResultType OnRangeMoved(SheetReference source, SheetReference dest)
            {
                return HandleRangeMoved(_owner, source, dest, OnSetMovedRangeResult);
            }

            private void OnSetMovedRangeResult(Rectangle result)
            {
                _owner.SetRange(result);
            }

            public override ReferenceOperationResultType OnRowsInserted(int insertAt, int count)
            {
                ReferenceOperationResultType startAffected = _owner._start.GridOps.OnRowsInserted(insertAt, count);
                ReferenceOperationResultType finishAffected = _owner._finish.GridOps.OnRowsInserted(insertAt, count);

                return startAffected | finishAffected;
            }

            public override ReferenceOperationResultType OnRowsRemoved(int removeAt, int count)
            {
                Point result = HandleRangeRemoved(_owner._start.Row, _owner._finish.Row, removeAt, count);

                if (result.IsEmpty)
                {
                    return ReferenceOperationResultType.Invalidated;
                }
                bool startAffected = result.X != _owner._start.Row;
                bool finishAffected = result.Y != _owner._finish.Row;

                _owner._start.SetRow(result.X);
                _owner._finish.SetRow(result.Y);
                return Affected2Enum(startAffected | finishAffected);
            }
        }

        [Serializable]
        private class Properties : GridReferenceProperties
        {
            public ReferenceProperties _finishProps;
            public ReferenceProperties _startProps;

            protected override void InitializeClone(ReferenceProperties clone)
            {
                var cloneProps = (Properties)clone;
                cloneProps._startProps = (ReferenceProperties)_startProps.Clone();
                cloneProps._finishProps = (ReferenceProperties)_finishProps.Clone();
            }
        }
    }
}