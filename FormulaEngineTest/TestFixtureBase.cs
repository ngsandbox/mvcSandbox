using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

public abstract class TestFixtureBase
{

	protected const string COMMENT_CHAR = "\'";

	protected FormulaEngine MyFormulaEngine;
	protected const char ARG_DELIMITER = ';';
	protected const char TEST_COMPONENT_DELIMITER = '�';

	protected const char ELEMENT_DELIMITER = '�';
	[ClassInitialize]
	public void FixtureSetUp()
	{
		MyFormulaEngine = new FormulaEngine();
		this.DoFixtureSetup();
	}

	[ClassCleanup]
	public void FixtureTearDown()
	{
		this.DoFixtureTearDown();
	}


	protected virtual void DoFixtureSetup()
	{
	}


	protected virtual void DoFixtureTearDown()
	{
	}

	[TestInitialize]
	public void TestSetup()
	{
		this.DoTestSetup();
	}


	protected virtual void DoTestSetup()
	{
	}

	protected Formula CreateFormula(string expression, int row, int col)
	{
		ISheetReference @ref = MyFormulaEngine.ReferenceFactory.Cell(row, col);
		Formula f = MyFormulaEngine.AddFormula(expression, @ref);
		return f;
	}

	protected void ProcessScriptTests(string scriptFileName, LineProcessor processor)
	{
		string scriptPath = Path.Combine("../TestScripts", scriptFileName);
		FileStream instream = new FileStream(scriptPath, FileMode.Open, FileAccess.Read);

		StreamReader sr = new StreamReader(instream);
		string line = sr.ReadLine();

		while (line != null) {
			if (line.StartsWith(COMMENT_CHAR) == false) {
				processor(line);
			}

			line = sr.ReadLine();
		}

		instream.Close();
	}

	protected object CreateType(string name)
	{
		return Activator.CreateInstance(Type.GetType("Tests." + name, true));
	}
}
