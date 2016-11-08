using System.Drawing;

namespace FormulaEngineCore.Processors
{
    internal abstract class HVLookupProcessor : LookupProcessor
    {
        protected int _index;

        protected object[,] _table;

        public void Initialize(object[,] table, int index)
        {
            _table = table;
            _index = index;
        }

        public abstract bool IsValidIndex(Rectangle refRect, int index);
    }
}