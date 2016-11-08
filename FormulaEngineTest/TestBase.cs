using System;
using System.IO;
using System.Text;
using FormulaEngineCore;
using FormulaEngineCore.FormulaRererences;
using FormulaEngineTest.Grids;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/*
 
 =IF(Sheet1!C3+Sheet1!E3+Sheet1!H3+Sheet1!C9+SUM(Sheet1!C3:J11)>0;TRUE;FALSE)


[3:2013]+[3:2016]+Sheet1!G4+Sheet1!B10+SUM([4:2013]:[4:2016])+IF
 */

namespace FormulaEngineTest
{
    public delegate void LineProcessor(string line);

    [TestClass]
    public abstract class TestBase
    {

        public bool _circularReferenceFlag;
        protected FormulaEngineTestGrid _outputsSheet;
        private FormulaEngineTestGrid _conditionSheets;

        protected const string COMMENT_CHAR = "\'";

        protected const char ARG_DELIMITER = ';';
        protected const string TEST_COMPONENT_DELIMITER = ";#";

        protected const char ELEMENT_DELIMITER = '†';
        protected FormulaEngine _formulaEngine;

        [TestInitialize]
        public void FixtureSetUp()
        {
            _formulaEngine = new FormulaEngine { Settings = { ListSeparator = ";", DecimalSeparator = "," } };
            DoFixtureSetup();
        }

        [TestCleanup]
        public void FixtureTearDown()
        {
            DoFixtureTearDown();
        }


        protected virtual void DoFixtureSetup()
        {
            _outputsSheet = new FormulaEngineTestGrid("Outputs");
            _conditionSheets = new FormulaEngineTestGrid("Conditions");
            _formulaEngine.CircularReferenceDetected += (sender, args) => _circularReferenceFlag = true;
            _formulaEngine.Sheets.Add(_outputsSheet);
            _formulaEngine.Sheets.Add(_conditionSheets);
            _circularReferenceFlag = false;
        }


        protected virtual void DoFixtureTearDown()
        {
        }

        [TestInitialize]
        public void TestSetup()
        {
            DoTestSetup();
        }


        protected virtual void DoTestSetup()
        {
            Clear();
        }

        protected void Clear()
        {
            if (_formulaEngine != null)
            {
                _formulaEngine.Clear();
                _formulaEngine.Sheets.Add(_outputsSheet);
                _formulaEngine.Sheets.Add(_conditionSheets);
                _circularReferenceFlag = false;
            }
        }


        protected Formula CreateFormula(string expression, int row, int col)
        {
            ISheetReference @ref = _formulaEngine.ReferenceFactory.Cell(row, col);
            Formula f = _formulaEngine.AddFormula(expression, @ref);
            return f;
        }

        protected void ProcessScriptTests(string scriptFileName, LineProcessor processor)
        {
            string scriptPath = Path.Combine("TestScripts", scriptFileName);
            var instream = new FileStream(scriptPath, FileMode.Open, FileAccess.Read);

            var sr = new StreamReader(instream);
            string line = sr.ReadLine();

            var errors = new StringBuilder();
            while (line != null)
            {
                if (line.StartsWith(COMMENT_CHAR) == false)
                {
                    try
                    {
                        processor(line);
                    }
                    catch (Exception ex)
                    {
                        errors.Append(ex);
                        errors.AppendLine("==============================================================");
                        errors.AppendLine();
                    }
                }

                line = sr.ReadLine();
            }

            if (errors.Length > 0)
            {
                throw new Exception(errors.ToString());
            }

            instream.Close();
        }

        protected object CreateType(string name)
        {
            return Activator.CreateInstance(Type.GetType("EngineTest.Components." + name, true));
        }
    }
}