using System;
using System.Drawing;
using System.Runtime.Serialization;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineCore.Miscellaneous;

namespace FormulaEngineCore.FormulaRererences
{
    [Serializable]
    internal abstract class RowColumnReference : SheetReference
    {
        private int _finish;
        private int _start;

        protected RowColumnReference(int start, int finish)
        {
            ValidateIndex(start);
            ValidateIndex(finish);

            if (start > finish)
            {
                int temp = start;
                start = finish;
                finish = temp;
            }

            _start = start;
            _finish = finish;
        }

        protected RowColumnReference(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _start = info.GetInt32("Start");
            _finish = info.GetInt32("Finish");
            ComputeHashCode();
        }

        public override bool CanRangeLink
        {
            get { return true; }
        }

        protected int Start
        {
            get { return _start; }
            set
            {
                _start = value;
                _start = System.Convert.ToInt32(Value);
            }
        }

        protected int Finish
        {
            get { return _finish; }
            set
            {
                _finish = value;
                _finish = System.Convert.ToInt32(Value);
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Start", _start);
            info.AddValue("Finish", _finish);
        }

        protected abstract void ValidateIndex(int index);

        protected override GridOperationsBase CreateGridOps()
        {
            return new RowColumnGridOps(this);
        }

        public static ReferenceProperties CreateProperties(bool implicitSheet, string image)
        {
            var props = new Properties();
            GetProperties(implicitSheet, props);
            props.StartAbsolute = image.StartsWith("$");
            props.FinishAbsolute = image.IndexOf("$", 1, StringComparison.Ordinal) != -1;
            return props;
        }

        protected override bool EqualsGridReference(SheetReference @ref)
        {
            var realRef = (RowColumnReference)@ref;
            return _start == realRef._start && _finish == realRef._finish;
        }

        public override bool IsReferenceEqualForCircularReference(Reference @ref)
        {
            return Intersects(@ref);
        }

        protected abstract ReferenceOperationResultType OnColumnsInserted(int insertAt, int count, RowColumnGridOps ops);
        protected abstract ReferenceOperationResultType OnRowsInserted(int insertAt, int count, RowColumnGridOps ops);
        protected abstract ReferenceOperationResultType OnColumnsRemoved(int removeAt, int count, RowColumnGridOps ops);
        protected abstract ReferenceOperationResultType OnRowsRemoved(int removeAt, int count, RowColumnGridOps ops);

        protected abstract int GetStartIndex(Rectangle rect);
        protected abstract int GetFinishIndex(Rectangle rect);

        protected abstract string FormatIndex(int index);

        protected override string FormatInternal()
        {
            string startString = FormatIndex(_start);
            string finishString = FormatIndex(_finish);
            return string.Concat(startString, ":", finishString);
        }

        protected override string FormatWithPropsInternal(ReferenceProperties props)
        {
            var realProps = (Properties)props;

            string startString = FormatIndex(_start);
            string finishString = FormatIndex(_finish);
            string startAbsolute = GetAbsoluteString(realProps.StartAbsolute);
            string finishAbsolute = GetAbsoluteString(realProps.FinishAbsolute);

            return string.Concat(startAbsolute, startString, ":", finishAbsolute, finishString);
        }

        protected abstract int GetCopyOffset(int rowOffset, int colOffset);

        protected override void OnCopyInternal(int rowOffset, int colOffset, ISheet destsheet, ReferenceProperties props)
        {
            int offset = GetCopyOffset(rowOffset, colOffset);
            var realProps = (Properties)props;
            _start = OffsetIndex(_start, offset, realProps.StartAbsolute);
            _finish = OffsetIndex(_finish, offset, realProps.FinishAbsolute);

            if (_start > _finish)
            {
                int tempIndex = _start;
                _start = _finish;
                _finish = tempIndex;

                bool tempBool = realProps.StartAbsolute;
                realProps.StartAbsolute = realProps.FinishAbsolute;
                realProps.FinishAbsolute = tempBool;
            }
        }

        protected override byte[] GetHashData()
        {
            var bytes = new byte[GRID_REFERENCE_HASH_SIZE + 2 + 2];
            GetBaseHashData(bytes);
            RowColumnIndexToBytes(_start, bytes, GRID_REFERENCE_HASH_SIZE);
            RowColumnIndexToBytes(_finish, bytes, GRID_REFERENCE_HASH_SIZE + 2);
            return bytes;
        }

        [Serializable]
        private class Properties : GridReferenceProperties
        {
            public bool StartAbsolute;
            public bool FinishAbsolute;
        }

        protected class RowColumnGridOps : GridOperationsBase
        {
            private readonly RowColumnReference _owner;

            public RowColumnGridOps(RowColumnReference owner)
            {
                _owner = owner;
            }

            public ReferenceOperationResultType OnRowColumnInsert(int insertAt, int count)
            {
                Point result = HandleRangeInserted(_owner.Start, _owner.Finish, insertAt, count);
                bool startAffected = _owner.Start != result.X;
                bool finishAffected = _owner.Finish != result.Y;

                _owner.Start = result.X;
                _owner.Finish = result.Y;

                return Affected2Enum(startAffected || finishAffected);
            }

            public ReferenceOperationResultType OnRowColumnDelete(int removeAt, int count)
            {
                Point result = HandleRangeRemoved(_owner.Start, _owner.Finish, removeAt, count);

                if (result.IsEmpty)
                {
                    return ReferenceOperationResultType.Invalidated;
                }
                bool startAffected = _owner.Start != result.X;
                bool finishAffected = _owner.Finish != result.Y;

                _owner.Start = result.X;
                _owner.Finish = result.Y;

                return Affected2Enum(startAffected || finishAffected);
            }

            public override ReferenceOperationResultType OnColumnsInserted(int insertAt, int count)
            {
                return _owner.OnColumnsInserted(insertAt, count, this);
            }

            public override ReferenceOperationResultType OnColumnsRemoved(int removeAt, int count)
            {
                return _owner.OnColumnsRemoved(removeAt, count, this);
            }

            public override ReferenceOperationResultType OnRangeMoved(SheetReference source, SheetReference dest)
            {
                return HandleRangeMoved(_owner, source, dest, OnSetMovedRangeResult);
            }

            private void OnSetMovedRangeResult(Rectangle result)
            {
                _owner._start = _owner.GetStartIndex(result);
                _owner._finish = _owner.GetFinishIndex(result) - 1;
            }

            public override ReferenceOperationResultType OnRowsInserted(int insertAt, int count)
            {
                return _owner.OnRowsInserted(insertAt, count, this);
            }

            public override ReferenceOperationResultType OnRowsRemoved(int removeAt, int count)
            {
                return _owner.OnRowsRemoved(removeAt, count, this);
            }
        }
    }
}