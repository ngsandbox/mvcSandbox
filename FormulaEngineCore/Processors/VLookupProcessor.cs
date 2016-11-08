using System.Drawing;
using FormulaEngineCore.Miscellaneous;

namespace FormulaEngineCore.Processors
{
    internal class VLookupProcessor : HVLookupProcessor
    {
        public override bool IsValidIndex(Rectangle refRect, int index)
        {
            return refRect.Left + index <= refRect.Right;
        }

        public override object[] GetLookupVector()
        {
            return Utility.GetTableColumn(_table, 0);
        }

        public override object[] GetResultVector()
        {
            return Utility.GetTableColumn(_table, _index - 1);
        }
    }
}