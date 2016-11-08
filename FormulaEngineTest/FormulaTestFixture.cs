using System;
using Microsoft.Office.Interop.Excel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// Tests specific to formulas
[TestClass(), CLSCompliant(false)]
public class FormulaEvaluateTestFixture : TestFixtureBase
{

	protected Application MyExcelApplication;
	protected Worksheet MyWorksheet;

	private FormulaEvaluateGrid MyGrid;
	protected override void DoFixtureSetup()
	{
		MyGrid = new FormulaEvaluateGrid();
		MyExcelApplication = new ApplicationClass();
		Workbook wb = MyExcelApplication.Workbooks.Add();
		MyWorksheet = wb.Worksheets[1];
		this.CopyTableToWorkSheet();
	}

	protected override void DoFixtureTearDown()
	{
		MyExcelApplication.Workbooks[1].Saved = true;
		MyExcelApplication.Workbooks.Close();
		MyExcelApplication.Quit();
	}

	private void ClearFormulaEngine()
	{
		MyFormulaEngine.Clear();
		MyFormulaEngine.Sheets.Add(MyGrid);
	}

	private void CopyTableToWorkSheet()
	{
		ISheet grid = MyGrid;

		for (int row = 1; row <= grid.RowCount; row++) {
			for (int col = 1; col <= grid.ColumnCount; col++) {
				object value = grid.GetCellValue(row, col);

				if (value == null) {
					value = value;
				} else if (object.ReferenceEquals(value.GetType(), typeof(ErrorValueWrapper))) {
					value = value.ToString();
				} else if (object.ReferenceEquals(value.GetType(), typeof(string))) {
					value = "'" + value;
				}

				MyWorksheet.Cells[row, col] = value;
			}
		}
	}

	[TestMethod]
	public void TestFormulaEvaluateAgainstExcel()
	{
		this.ProcessScriptTests("ValidTestFormulas.txt", ProcessFormulaEvaluateAgainstExcel);
	}

	[TestMethod]
	public void TestCultureSensitiveParse()
	{
		// Test that we set the decimal point and argument separator based on the current culture
		System.Globalization.CultureInfo oldCi = System.Threading.Thread.CurrentThread.CurrentCulture;
		System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("et-EE");
		System.Threading.Thread.CurrentThread.CurrentCulture = ci;
		Formula.RecreateParser();
		try {
			Formula f = MyFormulaEngine.CreateFormula("13,45 + sum(1;2;3)");
			double result = (double)f.Evaluate();
			Assert.AreEqual(19.45, result);
		} catch (InvalidFormulaException ex) {
			Assert.IsTrue(false);
		} finally {
			// Reset this or excel will give us problems
			System.Threading.Thread.CurrentThread.CurrentCulture = oldCi;
			// Recreate parser in default culture for other tests
			Formula.RecreateParser();
		}
	}

	private void ProcessFormulaEvaluateAgainstExcel(string formula)
	{
		this.ClearFormulaEngine();
		//Console.WriteLine(formula)
		try {
			object formulaResult = MyFormulaEngine.Evaluate(formula);
			object excelResult = this.EvaluateFormulaInExcel(formula);

			this.CompareResults(formulaResult, excelResult, formula);
		} catch (Exception ex) {
			Console.WriteLine("Failed formula: {0}", formula);
			throw ex;
		}
	}

	private object EvaluateFormulaInExcel(string formula)
	{
		// Do it this way so that we get exactly the same results as a user would get
		Range r = MyWorksheet.Range["F9"];

		if (formula.StartsWith("=") == false) {
			formula = "=" + formula;
		}

		r.Formula = formula;
		r.NumberFormat = "General";

		return r.Value;
	}

	private void CompareResults(object formulaResult, object excelResult, string formula)
	{
		if (object.ReferenceEquals(formulaResult.GetType(), typeof(ErrorValueWrapper))) {
			if (!object.ReferenceEquals(excelResult.GetType(), typeof(int))) {
				throw new ArgumentException("Formula returned an error but excel did not");
			}
			// Error value
			ErrorValueWrapper formulaError = (ErrorValueWrapper) formulaResult;
			ErrorValueWrapper excelError = this.ExcelError2FormulaError((int)excelResult);

			Assert.AreEqual(excelError, formulaError, formula);
		} else if ((formulaResult) is ISheetReference) {
			this.CompareRanges((ISheetReference) formulaResult, (Range)excelResult);
		} else if (object.ReferenceEquals(formulaResult.GetType(), typeof(DateTime))) {
			// Sometimes excel gives us a date and sometimes a double
			this.CompareDate(formulaResult, excelResult);
		} else {
			formulaResult = this.NormalizeValue(formulaResult);
			excelResult = this.NormalizeValue(excelResult);
			Assert.AreEqual(excelResult, formulaResult, formula);
		}
	}

	private void CompareDate(object formulaResult, object excelResult)
	{
		DateTime formulaDate = (DateTime)formulaResult;
		DateTime excelDate;

		if (object.ReferenceEquals(excelResult.GetType(), typeof(DateTime))) {
			excelDate = (DateTime)excelResult;
		} else if (object.ReferenceEquals(excelResult.GetType(), typeof(double))) {
			excelDate = DateTime.FromOADate((double)excelResult);
		} else {
			throw new ArgumentException("Formula returned a date but excel did not");
		}

		Assert.AreEqual(excelDate, formulaDate);
	}

	private ErrorValueWrapper ExcelError2FormulaError(int value)
	{
		XlCVError excelErrorValue = (XlCVError) (value & 0xffff);
		ErrorValueType errorValue;

		switch (excelErrorValue) {
			case XlCVError.xlErrDiv0:
				errorValue = ErrorValueType.Div0;
		        break;
			case XlCVError.xlErrNA:
				errorValue = ErrorValueType.NA;
                break;
            case XlCVError.xlErrName:
				errorValue = ErrorValueType.Name;
                break;
            case XlCVError.xlErrNull:
				errorValue = ErrorValueType.Null;
                break;
            case XlCVError.xlErrNum:
				errorValue = ErrorValueType.Num;
                break;
            case XlCVError.xlErrRef:
				errorValue = ErrorValueType.Ref;
                break;
            case XlCVError.xlErrValue:
				errorValue = ErrorValueType.Value;
                break;
            default:
				throw new InvalidOperationException("Unknown error code");
		}

		return FormulaEngine.CreateError(errorValue);
	}

	private object NormalizeValue(object value)
	{
		if (object.ReferenceEquals(value.GetType(), typeof(double))) {
			double d = (double)value;
			d = System.Math.Round(d, 8);
			return d;
		} else {
			return value;
		}
	}

	protected void CompareRanges(ISheetReference @ref, Range range)
	{
        System.Drawing.Rectangle excelRect = ExcelRangeToRectangle(range);
		System.Drawing.Rectangle refRect = @ref.Area;
		Assert.AreEqual(excelRect, refRect);
	}

	protected System.Drawing.Rectangle ExcelRangeToRectangle(Range r)
	{
        System.Drawing.Rectangle rect = new System.Drawing.Rectangle(r.Column, r.Row, r.Columns.Count, r.Rows.Count);
		return rect;
	}
}
