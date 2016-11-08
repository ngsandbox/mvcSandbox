using System.Globalization;
using System.Threading;
using FormulaEngineCore;
using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;
using FormulaEngineTest.Grids;
using Microsoft.Office.Interop.Excel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rectangle = System.Drawing.Rectangle;

// Tests specific to formulas
namespace FormulaEngineTest
{
    [TestClass]
    public class FormulaTest : TestBase
    {
        protected Application _excelApplication;

        private FormulaEvaluateGrid _grid;
        protected Worksheet _worksheet;

        protected override void DoFixtureSetup()
        {
            _grid = new FormulaEvaluateGrid();
            _excelApplication = new Application();
            Workbook wb = _excelApplication.Workbooks.Add();
            _worksheet = wb.Worksheets[1];
            CopyTableToWorkSheet();
        }

        protected override void DoFixtureTearDown()
        {
            _excelApplication.Workbooks[1].Saved = true;
            _excelApplication.Workbooks.Close();
            _excelApplication.Quit();
        }

        private void CopyTableToWorkSheet()
        {
            ISheet grid = _grid;

            for (int row = 1; row <= grid.RowCount; row++)
            {
                for (int col = 1; col <= grid.ColumnCount; col++)
                {
                    object value = grid[row, col];

                    if (value != null)
                    {
                        if (ReferenceEquals(value.GetType(), typeof(ErrorValueWrapper)))
                        {
                            value = value.ToString();
                        }
                        else if (ReferenceEquals(value.GetType(), typeof(string)))
                        {
                            value = "'" + value;
                        }
                    }

                    _worksheet.Cells[row, col] = value;
                }
            }
        }

        [TestMethod]
        public void TestCultureSensitiveParse()
        {
            // Test that we set the decimal point and argument separator based on the current culture
            CultureInfo oldCi = Thread.CurrentThread.CurrentCulture;
            var ci = new CultureInfo("et-EE");
            Thread.CurrentThread.CurrentCulture = ci;
            _formulaEngine.Settings.ListSeparator = ";";
            _formulaEngine.Settings.DecimalSeparator = ",";
            _formulaEngine.ResetParser();
            try
            {
                Formula f = _formulaEngine.CreateFormula("13,45 + sum(1;2;3)");
                var result = (double)f.Evaluate();
                Assert.AreEqual(19.45, result);
            }
            catch (InvalidFormulaException)
            {
                Assert.IsTrue(false);
            }
            finally
            {
                // Reset this or excel will give us problems
                Thread.CurrentThread.CurrentCulture = oldCi;
                // Recreate parser in default culture for other tests
                _formulaEngine.ResetParser();
            }
        }
        protected void CompareRanges(ISheetReference @ref, Range range)
        {
            Rectangle excelRect = ExcelRangeToRectangle(range);
            Rectangle refRect = @ref.Area;
            Assert.AreEqual(excelRect, refRect);
        }

        protected Rectangle ExcelRangeToRectangle(Range r)
        {
            var rect = new Rectangle(r.Column, r.Row, r.Columns.Count, r.Rows.Count);
            return rect;
        }
    }
}