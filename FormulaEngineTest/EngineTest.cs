using System;
using FormulaEngineCore;
using FormulaEngineCore.FormulaRererences;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FormulaEngineTest
{
    [TestClass]
    public class EngineTest : TestBase
    {

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void TestAddOverlappingFormula()
        {
            // Add a formula at the same location as an existing one
            CreateFormula("1+1", 1, 1);
            CreateFormula("2*56", 1, 1);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void AddNullFormula()
        {
            _formulaEngine.AddFormula((Formula)null, _formulaEngine.ReferenceFactory.Parse("a1"));
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void CheckNullRef()
        {
            Formula f = _formulaEngine.CreateFormula("1+1");
            _formulaEngine.AddFormula(f, null);
        }

        [TestMethod]
        public void CheckInvalidRefs()
        {
            Formula f = _formulaEngine.CreateFormula("1+1");
            DoAddFormula(f, _formulaEngine.ReferenceFactory.Cells(2, 2, 4, 4));
            DoAddFormula(f, _formulaEngine.ReferenceFactory.Columns(3, 4));
            DoAddFormula(f, _formulaEngine.ReferenceFactory.Rows(5, 6));
        }


        [TestMethod]
        public void CheckDuplicateRef()
        {
            Formula f = _formulaEngine.CreateFormula("1+1");
            _formulaEngine.AddFormula(f, _formulaEngine.ReferenceFactory.Parse("A1"));
            DoAddFormula(f, _formulaEngine.ReferenceFactory.Parse("A1"));
        }

        private void DoAddFormula(Formula f, IReference @ref)
        {
            bool sawException = false;
            try
            {
                _formulaEngine.AddFormula(f, @ref);
            }
            catch (Exception)
            {
                sawException = true;
            }

            Assert.IsTrue(sawException);
        }

    }
}