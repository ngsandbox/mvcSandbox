using System;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass()]
public class FormulaEngineTestFixture : TestFixtureBase
{

	private FormulaEngineTestGrid MySheet1;
	private FormulaEngineTestGrid MySheet2;

	public bool MyCircularReferenceFlag;

	public FormulaEngineTestFixture()
	{
	}

	protected override void DoFixtureSetup()
	{
		MySheet1 = new FormulaEngineTestGrid("Sheet1");
		MySheet2 = new FormulaEngineTestGrid("Sheet2");
		MyFormulaEngine.CircularReferenceDetected += OnCircularReferenceDetected;
	}

	protected override void DoTestSetup()
	{
		this.Clear();
	}

	private void Clear()
	{
		MyFormulaEngine.Clear();
		MyFormulaEngine.Sheets.Add(MySheet1);
		MyFormulaEngine.Sheets.Add(MySheet2);
		MyCircularReferenceFlag = false;
	}

	public void OnCircularReferenceDetected(object sender, EventArgs eventArgs)
	{
		MyCircularReferenceFlag = true;
	}

	[TestMethod, ExpectedException(typeof(ArgumentException))]
	public void TestAddOverlappingFormula()
	{
		// Add a formula at the same location as an existing one
		this.CreateFormula("1+1", 1, 1);
		this.CreateFormula("2*56", 1, 1);
	}

	[TestMethod]
	public void TestAddFormula()
	{
		// Null formula
		this.DoAddFormula(null, MyFormulaEngine.ReferenceFactory.Parse("a1"));
		Formula f = MyFormulaEngine.CreateFormula("1+1");
		// Null ref
		this.DoAddFormula(f, null);
		// Invalid references
		this.DoAddFormula(f, MyFormulaEngine.ReferenceFactory.Cells(2, 2, 4, 4));
		this.DoAddFormula(f, MyFormulaEngine.ReferenceFactory.Columns(3, 4));
		this.DoAddFormula(f, MyFormulaEngine.ReferenceFactory.Rows(5, 6));

		// Add with duplicate reference
		MyFormulaEngine.AddFormula(f, MyFormulaEngine.ReferenceFactory.Parse("A1"));
		this.DoAddFormula(f, MyFormulaEngine.ReferenceFactory.Parse("A1"));
	}

	private void DoAddFormula(Formula f, IReference @ref)
	{
		bool sawException = false;
		try {
			MyFormulaEngine.AddFormula(f, @ref);
		} catch (Exception ex) {
			sawException = true;
		}

		Assert.IsTrue(sawException);
	}

	[TestMethod]
	public void TestReferenceParsing()
	{
		// Test our column index parse/format
		this.TestColumnIndexParseFormat(1);
		this.TestColumnIndexParseFormat(15);
		this.TestColumnIndexParseFormat(27);
		this.TestColumnIndexParseFormat(256);
		this.TestColumnIndexParseFormat(71);

		MySheet1.SetSize(65536, 256);

		// Test that we are properly interpreting the row and column of string cell references
		this.DoTestReferenceParsing("A1", 1, 1);
		this.DoTestReferenceParsing("b2", 2, 2);
		this.DoTestReferenceParsing("o15", 15, 15);
		this.DoTestReferenceParsing("z133", 133, 26);
		this.DoTestReferenceParsing("AA155", 155, 27);
		this.DoTestReferenceParsing("l12", 12, 12);
		this.DoTestReferenceParsing("O15", 15, 15);
		this.DoTestReferenceParsing("He13", 13, 213);
		this.DoTestReferenceParsing("IV100", 100, 256);
		this.DoTestReferenceParsing("BS6", 6, 71);
		this.DoTestReferenceParsing("A65535", 65535, 1);
		this.DoTestReferenceParsing("CH2000", 2000, 86);
		this.DoTestReferenceParsing("IV35536", 35536, 256);
		this.DoTestReferenceParsing("IV65535", 65535, 256);

		MySheet1.ResetSize();
	}

	private void TestColumnIndexParseFormat(int columnIndex)
	{
		string s = Utility.ColumnIndex2Label(columnIndex);
		int index = Utility.ColumnLabel2Index(s);
		Assert.AreEqual(columnIndex, index);
	}

	private void DoTestReferenceParsing(string image, int expectedRow, int expectedColumn)
	{
		ISheetReference @ref = MyFormulaEngine.ReferenceFactory.Parse(image);
		Assert.AreEqual(expectedRow, @ref.Row);
		Assert.AreEqual(expectedColumn, @ref.Column);
	}

	[TestMethod]
	public void TestReferenceValidation()
	{
		this.DoReferenceValidation("aaa90");
		this.DoReferenceValidation("zz90");
		this.DoReferenceValidation("a0");
		this.DoReferenceValidation("a100000");
		this.DoReferenceValidation("zzz80000");
		this.DoReferenceValidation("aa");
		this.DoReferenceValidation("10");
		this.DoReferenceValidation("a10:zz45");
		this.DoReferenceValidation("a99999:b4");
		this.DoReferenceValidation("Sh  et!a1:b4");
		this.DoReferenceValidation("9sheet!a1:b4");
		this.DoReferenceValidation("Sheet1 a1:b4");
	}

	private void DoReferenceValidation(string image)
	{
		bool invalid;

		try {
			MyFormulaEngine.ReferenceFactory.Parse(image);
			invalid = false;
		} catch (Exception ex) {
			invalid = true;
		}

		Assert.IsTrue(invalid, image);
	}

	private void ProcessTestScriptLine(string line)
	{
		//Console.WriteLine(line)
		TestComponentInfo[] infos = this.CreateTestComponentInfos(line);
		this.RunTest(infos, line);
	}

	private TestComponentInfo[] CreateTestComponentInfos(string line)
	{
		string[] strings = line.Split(TestFixtureBase.TEST_COMPONENT_DELIMITER);
		TestComponentInfo[] infos = new TestComponentInfo[strings.Length - 2];

		for (int i = 0; i <= strings.Length - 1; i++) {
			string[] strings2 = strings[i].Split(TestFixtureBase.ELEMENT_DELIMITER);
			TestComponentInfo info;
			info.Component = (TestComponent) this.CreateType(strings2[0]);
			info.Args = strings2[1].Split(TestFixtureBase.ARG_DELIMITER);
			infos[i] = info;
		}

		return infos;
	}

	private void RunTest(TestComponentInfo[] infos, string line)
	{
		IDictionary state = new Hashtable();
		state.Add(typeof(FormulaEngineTestFixture), this);

		this.Clear();

		for (int i = 0; i <= infos.Length - 1; i++) {
			TestComponentInfo info;
			info = infos[i];
			TestComponent component = info.Component;
			component.AcceptParameters(info.Args, MyFormulaEngine);
			this.ExecuteTestComponent(component, state, line);
		}
	}

	private void ExecuteTestComponent(TestComponent component, IDictionary state, string line)
	{
		try {
			component.Execute(MyFormulaEngine, state);
		} catch (Exception ex) {
			string[] arr = new string[3 - 2];
			arr[0] = line;
			arr[1] = ex.ToString();
		    arr[2] = new string('-', 80);
			string msg = string.Join(System.Environment.NewLine, arr);
			Assert.Fail(msg);
		}
	}

	[TestMethod]
	public void TestMiscellaneous()
	{
		this.ProcessScriptTests("MiscellaneousTests.txt", ProcessTestScriptLine);
	}

	[TestMethod]
	public void TestReferenceAdjusts()
	{
		this.ProcessScriptTests("ReferenceTests.txt", ProcessTestScriptLine);
	}

	[TestMethod]
	public void TestDependencies()
	{
		this.ProcessScriptTests("DependencyTests.txt", ProcessTestScriptLine);
	}

	[TestMethod]
	public void TestCalculationOrder()
	{
		this.ProcessScriptTests("CalculationOrderTests.txt", ProcessTestScriptLine);
	}

	[TestMethod]
	public void TestCircularReferenceHandling()
	{
		this.ProcessScriptTests("CircularReferenceTests.txt", ProcessTestScriptLine);
	}
}
