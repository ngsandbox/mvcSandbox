using System;
using FormulaEngineCore.FormulaTypes;

namespace FormulaEngineTest.Grids
{
    public class FormulaEngineTestGrid : ISheet
    {
        private const int ROW_COUNT = 50;
        private const int COL_COUNT = 50;
        private readonly object[,] _cells;

        public FormulaEngineTestGrid(string name)
        {
            _cells = new object[ROW_COUNT, COL_COUNT];
            Name = name;
            ResetSize();
            SetRandomValues();
        }

        private void SetRandomValues()
        {
            var random = new Random();
            for (int i = 0; i < ROW_COUNT; i++)
            {
                for (int j = 0; j < COL_COUNT; j++)
                {
                    _cells[i, j] = random.Next(j, i + j + COL_COUNT);
                }
            }
        }

        public object this[int row, int column]
        {
            get { return _cells[row - 1, column - 1]; }
            set { _cells[row - 1, column - 1] = value; }
        }

        public string Name { get; set; }

        public int RowCount { get; private set; }

        public int ColumnCount { get; private set; }

        public void ResetSize()
        {
            RowCount = ROW_COUNT;
            ColumnCount = COL_COUNT;
        }

        public void SetSize(int rowCount, int colCount)
        {
            RowCount = rowCount;
            ColumnCount = colCount;
        }

        public void ClearCell(int row, int col)
        {
            _cells[row - 1, col - 1] = null;
        }
    }
}