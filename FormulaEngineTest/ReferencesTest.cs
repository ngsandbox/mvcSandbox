using System;
using FormulaEngineCore;
using FormulaEngineCore.FormulaRererences;
using FormulaEngineCore.Miscellaneous;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FormulaEngineTest
{
    [TestClass]
    public class ReferencesTest : TestBase
    {
        [TestMethod]
        public void ColumnLabelToIndexText()
        {
            Assert.AreEqual(SheetReference.ColumnLabel2Index('A', 'A'), 27);
            Assert.AreEqual(SheetReference.ColumnLabel2Index('A', 'U'), 47);
            Assert.AreEqual(SheetReference.ColumnLabel2Index('A', 'I'), 35);
            Assert.AreEqual(SheetReference.ColumnLabel2Index('B', 'S'), 71);
            Assert.AreEqual(SheetReference.ColumnLabel2Index('C', 'M'), 91);
            Assert.AreEqual(SheetReference.ColumnLabel2Index('I', 'D'), 238);
        }

        [TestMethod]
        public void ColumnIndexToLabel()
        {
            Assert.AreEqual(SheetReference.ColumnIndex2Label(247), "IM");
            Assert.AreEqual(SheetReference.ColumnIndex2Label(233), "HY");
            Assert.AreEqual(SheetReference.ColumnIndex2Label(226), "HR");
            Assert.AreEqual(SheetReference.ColumnIndex2Label(702), "ZZ");
            Assert.AreEqual(SheetReference.ColumnIndex2Label(515), "SU");
            Assert.AreEqual(SheetReference.ColumnIndex2Label(285), "JY");
        }

        [TestMethod]
        public void CheckFormulaReference()
        {
            _formulaEngine.ResetParser();
            Formula f = _formulaEngine.CreateFormula("=IF(Outputs!C3<>0;TRUE;FALSE)");
            var refs = f.References;
            Assert.IsNotNull(refs, "Rererences list is not defined!");
            Assert.AreEqual(refs.Length, 1, "Rererences length is not equal to 1!");
            Assert.AreEqual(refs[0].ToString(), "Outputs!C3", "Reference's value is not equal!");
        }

        [TestMethod]
        public void CheckSheetReference()
        {
            _formulaEngine.ResetParser();
            Formula f = _formulaEngine.CreateFormula("=IF(Outputs!Z50<>0;TRUE;FALSE)");
            CellReference cell = (CellReference)f.References[0];
            Assert.AreEqual(cell.Sheet.Name, "Outputs");
            Assert.AreEqual(SheetReference.ColumnIndex2Label(cell.ColumnIndex), "Z");
            Assert.AreEqual(cell.RowIndex, 50);
        }

        [TestMethod]
        public void TestReferenceParsing()
        {
            // Test our column index parse/format
            TestColumnIndexParseFormat(1);
            TestColumnIndexParseFormat(15);
            TestColumnIndexParseFormat(27);
            TestColumnIndexParseFormat(256);
            TestColumnIndexParseFormat(71);

            _outputsSheet.SetSize(65536, 256);

            // Test that we are properly interpreting the row and column of string cell references
            DoTestReferenceParsing("A1", 1, 1);
            DoTestReferenceParsing("b2", 2, 2);
            DoTestReferenceParsing("o15", 15, 15);
            DoTestReferenceParsing("z133", 133, 26);
            DoTestReferenceParsing("AA155", 155, 27);
            DoTestReferenceParsing("l12", 12, 12);
            DoTestReferenceParsing("O15", 15, 15);
            DoTestReferenceParsing("He13", 13, 213);
            DoTestReferenceParsing("IV100", 100, 256);
            DoTestReferenceParsing("BS6", 6, 71);
            DoTestReferenceParsing("A65535", 65535, 1);
            DoTestReferenceParsing("CH2000", 2000, 86);
            DoTestReferenceParsing("IV35536", 35536, 256);
            DoTestReferenceParsing("IV65535", 65535, 256);
            _outputsSheet.ResetSize();
        }

        private void TestColumnIndexParseFormat(int columnIndex)
        {
            string s = Utility.ColumnIndex2Label(columnIndex);
            int index = Utility.ColumnLabel2Index(s);
            Assert.AreEqual(columnIndex, index);
        }

        private void DoTestReferenceParsing(string image, int expectedRow, int expectedColumn)
        {
            ISheetReference @ref = _formulaEngine.ReferenceFactory.Parse(image);
            Assert.AreEqual(expectedRow, @ref.Row);
            Assert.AreEqual(expectedColumn, @ref.Column);
        }

        [TestMethod]
        public void TestReferenceValidation()
        {
            DoReferenceValidation("aaa90");
            DoReferenceValidation("zz90");
            DoReferenceValidation("a0");
            DoReferenceValidation("a100000");
            DoReferenceValidation("zzz80000");
            DoReferenceValidation("aa");
            DoReferenceValidation("10");
            DoReferenceValidation("a10:zz45");
            DoReferenceValidation("a99999:b4");
            DoReferenceValidation("Sh  et!a1:b4");
            DoReferenceValidation("9sheet!a1:b4");
            DoReferenceValidation("Outputs a1:b4");
        }

        private void DoReferenceValidation(string image)
        {
            bool invalid;
            try
            {
                _formulaEngine.ReferenceFactory.Parse(image);
                invalid = false;
            }
            catch (Exception)
            {
                invalid = true;
            }

            Assert.IsTrue(invalid, image);
        }

    }
}
