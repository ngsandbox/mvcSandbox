using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineCore.FormulaRererences
{
    [Serializable]
    internal class RowReference : RowColumnReference
    {
        private static readonly Regex _regex = new Regex(String.Format("^{0}{1}:{1}$", SHEET_REGEX, ROW_REGEX));

        public RowReference(int start, int finish)
            : base(start, finish)
        {
        }

        private RowReference(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override Rectangle Range
        {
            get
            {
                int h = Finish - Start + 1;
                int w = Sheet.ColumnCount;
                return new Rectangle(1, Start, w, h);
            }
        }

        protected override void ValidateIndex(int index)
        {
            ValidateRowIndex(index);
        }

        public static bool IsValidString(string s)
        {
            return _regex.IsMatch(s);
        }

        public static RowReference FromString(string image)
        {
            image = PrepareParseString(image);
            string[] parts = image.Split(':');
            int start = int.Parse(parts[0]);
            int finish = int.Parse(parts[1]);

            return new RowReference(start, finish);
        }

        protected override ReferenceOperationResultType OnColumnsInserted(int insertAt, int count, RowColumnGridOps ops)
        {
            return ReferenceOperationResultType.Affected;
        }

        protected override ReferenceOperationResultType OnColumnsRemoved(int removeAt, int count, RowColumnGridOps ops)
        {
            return ReferenceOperationResultType.Affected;
        }

        protected override int GetStartIndex(Rectangle rect)
        {
            return rect.Top;
        }

        protected override int GetFinishIndex(Rectangle rect)
        {
            return rect.Bottom;
        }

        protected override ReferenceOperationResultType OnRowsInserted(int insertAt, int count, RowColumnGridOps ops)
        {
            return ops.OnRowColumnInsert(insertAt, count);
        }

        protected override ReferenceOperationResultType OnRowsRemoved(int removeAt, int count, RowColumnGridOps ops)
        {
            return ops.OnRowColumnDelete(removeAt, count);
        }

        protected override string FormatIndex(int index)
        {
            return index.ToString(CultureInfo.InvariantCulture);
        }

        protected override int GetCopyOffset(int rowOffset, int colOffset)
        {
            return rowOffset;
        }
    }
}