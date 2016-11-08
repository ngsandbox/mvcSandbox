using System.Drawing;
using FormulaEngineCore.Miscellaneous;

namespace FormulaEngineCore.Processors
{
    internal class HLookupProcessor : HVLookupProcessor
    {
        public override bool IsValidIndex(Rectangle refRect, int index)
        {
            return refRect.Top + index <= refRect.Bottom;
        }

        public override object[] GetLookupVector()
        {
            return Utility.GetTableRow(_table, 0);
        }

        public override object[] GetResultVector()
        {
            return Utility.GetTableRow(_table, _index - 1);
        }
    }
}