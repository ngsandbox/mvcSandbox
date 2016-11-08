using FormulaEngineCore;
using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.FormulaTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FormulaEngineTest
{

    /*
     */
    [TestClass]
    public class ConditionsTest : TestBase
    {
        /// <summary>
        /// Денежные поток от операционной деятельности (OCF) > 0
        /// <remarks>частное тождество</remarks>
        /// </summary>
        [TestMethod]
        public void CheckLowerThanOneRef()
        {
            object cellResult = 50;
            _outputsSheet[3, 5] = cellResult;
            const string formula = "=ЕСЛИ(Outputs!E3 > 0;1;0)";
            Formula f = _formulaEngine.CreateFormula(formula);
            Assert.IsInstanceOfType(f.References[0], typeof(CellReference));
            var cell = (CellReference)f.References[0];
            Assert.AreEqual(cell.ColumnIndex, SheetReference.ColumnLabel2Index('E'), "Column number does not equal to 5 (E)");
            Assert.AreEqual(cell.RowIndex, 3, "Row number does not equal to 3 ()");
            Assert.AreEqual(cell.TargetCellValue, cellResult, "Cell value does not equal to " + cellResult);
        }

        /// <summary>
        /// Если(«Коэффициент покрытия долга (DSCR)» в год» <ГОД(СЕГОДНЯ()); 1;0)
        /// <remarks>частное тождество</remarks>
        /// </summary>
        [TestMethod]
        public void CheckLowerThanOneRefToCurrentYear()
        {
            object cellResult = 2011;
            _outputsSheet[12, 3] = cellResult;
            const string formula = "=Если(Outputs!C12 < ГОД(СЕГОДНЯ()); 1;0)";
            Formula f = _formulaEngine.CreateFormula(formula);
            Assert.IsInstanceOfType(f.References[0], typeof(CellReference));
            var cell = (CellReference)f.References[0];
            Assert.AreEqual(cell.ColumnIndex, SheetReference.ColumnLabel2Index('C'), "Column number does not equal to 5 (E)");
            Assert.AreEqual(cell.RowIndex, 12, "Row number does not equal to 3 ()");
            Assert.AreEqual(cell.TargetCellValue, cellResult, "Cell value does not equal to " + cellResult);
        }

        /// <summary>
        /// Если(«Коэффициент покрытия долга (DSCR)» в год» <ГОД(СЕГОДНЯ()); 1;0)
        /// <remarks>частное тождество</remarks>
        /// </summary>
        [TestMethod]
        public void CheckLowerThanTwoRefsToCurrentYear()
        {
            object cellResult1 = 2011;
            _outputsSheet[12, 3] = cellResult1;
            object cellResult2 = 2017;
            _outputsSheet[20, 20] = cellResult2;
            const string formula = "=Если(И(Outputs!C12 < ГОД(СЕГОДНЯ());Outputs!T20 < ГОД(СЕГОДНЯ())); 1;0)";
            Formula f = _formulaEngine.CreateFormula(formula);
            Assert.IsInstanceOfType(f.References[0], typeof(CellReference));
            Assert.IsInstanceOfType(f.References[1], typeof(CellReference));
            var cell1 = (CellReference)f.References[0];
            var cell2 = (CellReference)f.References[1];
            Assert.AreEqual(cell1.ColumnIndex, SheetReference.ColumnLabel2Index('C'));
            Assert.AreEqual(cell2.ColumnIndex, SheetReference.ColumnLabel2Index('T'));
            Assert.AreEqual(cell1.RowIndex, 12);
            Assert.AreEqual(cell2.RowIndex, 20);
        }

        /// <summary>
        /// (Если(«Коэффициент покрытия долга (DSCR)» в год= ГОД(СЕГОДНЯ())» <1; 1;0)+
        /// Если(«Коэффициент покрытия долга (DSCR)» в год= ГОД(СЕГОДНЯ())-1» <1; 1;0)+
        /// Если(«Коэффициент покрытия долга (DSCR)» в год= ГОД(СЕГОДНЯ())-2» <1; 1;0))>=2
        /// <remarks>частное тождество</remarks>
        /// </summary>
        [TestMethod]
        public void CheckSummOfTwoRefsLowerOrEqTo()
        {
            object cellResult1 = 2016;
            _outputsSheet[12, 3] = cellResult1;
            object cellResult2 = 2017;
            _outputsSheet[20, 20] = cellResult2;
            const string formula = "=Если(Outputs!C12 = ГОД(СЕГОДНЯ());0;1) + Если(Outputs!T20 = ГОД(СЕГОДНЯ()) - 1;3;5) >= 6";
            Formula f = _formulaEngine.CreateFormula(formula);
            Assert.IsInstanceOfType(f.References[0], typeof(CellReference));
            Assert.IsInstanceOfType(f.References[1], typeof(CellReference));
            var cell1 = (CellReference)f.References[0];
            //
            var cell2 = (CellReference)f.References[1];
            Assert.AreEqual(cell1.ColumnIndex, SheetReference.ColumnLabel2Index('C'));
            Assert.AreEqual(cell2.ColumnIndex, SheetReference.ColumnLabel2Index('T'));
            Assert.AreEqual(cell1.RowIndex, 12);
            Assert.AreEqual(cell2.RowIndex, 20);


            //Set Param ID
            cell1.SetRow(55);
            foreach (ReferenceProperties property in f.ReferenceProperties)
            {
                var gringProp = property as SheetReference.GridReferenceProperties;
                if (gringProp != null)
                {
                    gringProp.ImplicitSheet = true;
                }
            }

            //Set Year 
            cell1.SetColumn(2015 % 2000);

            //Set Param ID
            cell2.SetRow(75);
            //Set Year 
            cell2.SetColumn(2025 % 2000);
            string resultFormula = f.ToString();
        }
    }
}
