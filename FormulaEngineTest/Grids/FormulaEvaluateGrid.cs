using FormulaEngineCore;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineTest.Grids
{
    internal class FormulaEvaluateGrid : ISheet
    {
        private const int ROW_COUNT = 7;
        private const int COL_COUNT = 5;

        private readonly object[,] _table;

        public FormulaEvaluateGrid()
        {
            _table = new object[ROW_COUNT, COL_COUNT];
            FillTable();
        }

        public string Name
        {
            get { return "Sheet1"; }
        }

        public int RowCount
        {
            get { return ROW_COUNT; }
        }

        public int ColumnCount
        {
            get { return COL_COUNT; }
        }

        public object this[int row, int column]
        {
            get { return _table[row - 1, column - 1]; }
            set { _table[row - 1, column - 1] = value; }
        }

        private void FillTable()
        {
            // Numbers
            _table[0, 0] = 123.34;
            _table[0, 1] = (double)13;
            _table[0, 2] = (double)-5;
            _table[0, 3] = 1000.23;
            _table[0, 4] = 1300;
            // Integer

            // More numbers
            _table[1, 0] = 0.56;
            _table[1, 1] = (double)100;
            _table[1, 2] = (double)0;
            _table[1, 3] = 3.45;
            _table[1, 4] = 155;
            //Integer

            // Numbers as strings
            _table[2, 0] = "123";
            _table[2, 1] = "45.23";
            _table[2, 2] = "0";
            _table[2, 3] = "-11";
            _table[2, 4] = string.Empty;

            // Random strings
            _table[3, 0] = "eugene";
            _table[3, 1] = "not a number";
            _table[3, 2] = "****";
            _table[3, 3] = "()()^^^";
            _table[3, 4] = "S";

            // Errors
            _table[4, 0] = FormulaEngine.CreateError(ErrorValueType.Div0);
            _table[4, 1] = FormulaEngine.CreateError(ErrorValueType.NA);
            _table[4, 2] = FormulaEngine.CreateError(ErrorValueType.Name);
            _table[4, 3] = FormulaEngine.CreateError(ErrorValueType.Null);
            _table[4, 4] = FormulaEngine.CreateError(ErrorValueType.Num);

            // Empty cells
            _table[5, 0] = null;
            _table[5, 1] = null;
            _table[5, 2] = null;
            _table[5, 3] = null;
            _table[5, 4] = null;

            // Booleans
            _table[6, 0] = true;
            _table[6, 1] = false;
            _table[6, 2] = false;
            _table[6, 3] = true;
            _table[6, 4] = true;
        }
    }
}