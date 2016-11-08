using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.FormulaRererences
{
    [Serializable]
    internal class ColumnReference : RowColumnReference
    {
        private static readonly Regex _regex = new Regex(string.Format("^{0}{1}:{1}$", SHEET_REGEX, COLUMN_REGEX));

        public ColumnReference(int start, int finish)
            : base(start, finish)
        {
        }

        private ColumnReference(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override Rectangle Range
        {
            get
            {
                int h = Sheet.RowCount;
                int w = Finish - Start + 1;
                return new Rectangle(Start, 1, w, h);
            }
        }

        protected override void ValidateIndex(int index)
        {
            ValidateColumnIndex(index);
        }

        public static bool IsValidString(string s)
        {
            return _regex.IsMatch(s);
        }

        public static ColumnReference FromString(string image)
        {
            image = PrepareParseString(image);
            string[] parts = image.Split(':');
            int start = PartToColumnIndex(parts[0]);
            int finish = PartToColumnIndex(parts[1]);

            return new ColumnReference(start, finish);
        }

        private static int PartToColumnIndex(string part)
        {
            return part.Length == 1 ? ColumnLabel2Index(part[0]) :
                ColumnLabel2Index(part[0], part[1]);
        }

        protected override int GetCopyOffset(int rowOffset, int colOffset)
        {
            return colOffset;
        }

        protected override ReferenceOperationResultType OnColumnsInserted(int insertAt, int count, RowColumnGridOps ops)
        {
            return ops.OnRowColumnInsert(insertAt, count);
        }

        protected override ReferenceOperationResultType OnColumnsRemoved(int removeAt, int count, RowColumnGridOps ops)
        {
            return ops.OnRowColumnDelete(removeAt, count);
        }

        protected override ReferenceOperationResultType OnRowsInserted(int insertAt, int count, RowColumnGridOps ops)
        {
            return ReferenceOperationResultType.Affected;
        }

        protected override ReferenceOperationResultType OnRowsRemoved(int removeAt, int count, RowColumnGridOps ops)
        {
            return ReferenceOperationResultType.Affected;
        }

        protected override string FormatIndex(int index)
        {
            return ColumnIndex2Label(index);
        }

        protected override int GetStartIndex(Rectangle rect)
        {
            return rect.Left;
        }

        protected override int GetFinishIndex(Rectangle rect)
        {
            return rect.Right;
        }
    }
}