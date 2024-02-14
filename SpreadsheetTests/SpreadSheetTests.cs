using SpreadsheetUtilities;
using SS;

/// <summary>
/// Author:    Shu Chen
/// Partner:   None
/// Date:      2024/2/14
/// Course:    CS 3500, University of Utah, School of Computing
/// Copyright: CS 3500 and Shu Chen - This work may not
///            be copied for use in Academic Coursework.
///
/// I, Shu Chen, certify that I wrote this code from scratch and
/// did not copy it in part or whole from another source.  All
/// references used in the completion of the assignments are cited
/// in my README file.
///
/// File Contents
///
/// This is a tester for the spread Sheet
/// </summary>
namespace SpreadSheetTests
{
    [TestClass]
    public class SpreadSheetTests
    {
        /// <summary>
        /// Test for wrong format
        /// </summary>
        [TestMethod]
        public void TestWrongFormat()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.ThrowsException<InvalidNameException>(() => s.GetCellContents("1A"));
            Assert.ThrowsException<InvalidNameException>(() => s.GetCellContents("1A1A"));
        }

        /// <summary>
        /// Test for null name
        /// </summary>
        [TestMethod]
        public void TestNullName()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.ThrowsException<InvalidNameException>(() => s.GetCellContents(null));
        }

        /// <summary>
        /// Empty cell should return a empty string
        /// </summary>
        [TestMethod]
        public void TestGetEmptyCell()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual("", s.GetCellContents("A1"));
            Assert.AreEqual("", s.GetCellContents("AA11"));
        }

        /// <summary>
        /// Fill in data in to cell
        /// </summary>
        [TestMethod]
        public void TestChangeCell()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "1");
            s.SetContentsOfCell("B1", "test");
            s.SetContentsOfCell("C1", "=1+1");
            s.SetContentsOfCell("D1", "");
            s.SetContentsOfCell("D1", null);
            Assert.AreEqual(1.0, s.GetCellContents("A1"));
            Assert.AreEqual("test", s.GetCellContents("B1"));
            Assert.AreEqual(new Formula("1+1"), s.GetCellContents("C1"));
            Assert.IsTrue(Enumerable.SequenceEqual(new List<string> { "A1", "B1", "C1" }, s.GetNamesOfAllNonemptyCells()));
        }

        /// <summary>
        /// Test for dependency
        /// </summary>
        [TestMethod]
        public void TestDependency()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("F1", "0");
            s.SetContentsOfCell("A1", "=B1+C1");
            s.SetContentsOfCell("B1", "=C1+D1");
            s.SetContentsOfCell("C1", "=D1+2");
            IEnumerable<string> cellNeedToCal = s.SetContentsOfCell("D1", "1");
            Assert.IsTrue(Enumerable.SequenceEqual(new List<string> { "D1", "C1", "B1", "A1" }, cellNeedToCal));
        }

        /// <summary>
        /// Test for dependency
        /// </summary>
        [TestMethod]
        public void TestDependency2()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "1");
            s.SetContentsOfCell("B1", "7");
            s.SetContentsOfCell("C1", "=A1+B1");
            s.SetContentsOfCell("D1", "=A1*C1");
            IEnumerable<string> cellNeedToCal = s.SetContentsOfCell("B1", "1");
            Assert.IsTrue(Enumerable.SequenceEqual(new List<string> { "B1", "C1", "D1" }, cellNeedToCal));
        }

        /// <summary>
        /// Test for circular dependency, if there is, there shouldn't have data in the last cell that entered
        /// </summary>
        [TestMethod]
        public void TestCircularDependency()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=B1+C1");
            s.SetContentsOfCell("B1", "=C1+1");
            Assert.ThrowsException<CircularException>(() => s.SetContentsOfCell("C1", "=A1+B1"));

            Assert.AreEqual("", s.GetCellContents("C1"));
        }

        /// <summary>
        /// Test to cal cell value
        /// </summary>
        [TestMethod]
        public void TestCellValueEasy()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "1");
            s.SetContentsOfCell("B1", "test");
            s.SetContentsOfCell("C1", "=1+1");
            s.SetContentsOfCell("D1", "");
            Assert.AreEqual(1.0, s.GetCellValue("A1"));
            Assert.AreEqual("test", s.GetCellValue("B1"));
            Assert.AreEqual(2.0, s.GetCellValue("C1"));
        }

        /// <summary>
        /// Test to cal cell value
        /// </summary>
        [TestMethod]
        public void TestCellValueWithDep()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "1");
            s.SetContentsOfCell("B1", "test");
            s.SetContentsOfCell("C1", "=A1+1");
            s.SetContentsOfCell("D1", "=A1+C1");
            s.SetContentsOfCell("E1", "=C1+D1");
            s.SetContentsOfCell("F1", "=A1+B1");
            Assert.AreEqual(1.0, s.GetCellValue("A1"));
            Assert.AreEqual("test", s.GetCellValue("B1"));
            Assert.AreEqual(2.0, s.GetCellValue("C1"));
            Assert.AreEqual(3.0, s.GetCellValue("D1"));
            Assert.AreEqual(5.0, s.GetCellValue("E1"));
            Assert.IsTrue(s.GetCellValue("F1") is FormulaError);
            s.SetContentsOfCell("A1", "2");
            Assert.AreEqual(2.0, s.GetCellValue("A1"));
            Assert.AreEqual(3.0, s.GetCellValue("C1"));
            Assert.AreEqual(5.0, s.GetCellValue("D1"));
            Assert.AreEqual(8.0, s.GetCellValue("E1"));
            s.SetContentsOfCell("A1", "=2/0");
            Assert.IsTrue(s.GetCellValue("F1") is FormulaError);
            Assert.IsTrue(s.GetCellValue("C1") is FormulaError);
        }

        /// <summary>
        /// Test Save and Read
        /// </summary>
        [TestMethod]
        public void TestSaveAndRead()
        {
            Spreadsheet s = SmallSpreadSheet();

            s.Save("saveTestSimple.txt");
            s.Save("saveTestSimple.txt");
            s = new Spreadsheet("saveTestSimple.txt", s => true, s => s, "default");

            Assert.AreEqual(1.0, s.GetCellValue("A1"));
            Assert.AreEqual(2.0, s.GetCellValue("B1"));
            Assert.AreEqual(3.0, s.GetCellValue("C1"));
            Assert.AreEqual("string", s.GetCellValue("F1"));
        }

        /// <summary>
        /// Test all error cases for read and write
        /// </summary>
        [TestMethod]
        [TestCategory("RWE")]
        public void TestReadAndWriteException()
        {
            //Test Missing File
            Assert.ThrowsException<SpreadsheetReadWriteException>(() => new Spreadsheet("missingFile", s => true, s => s, "default"));
            Spreadsheet s = SmallSpreadSheet();
            s.Save("RWE.txt");

            //Test Wrong version
            Assert.ThrowsException<SpreadsheetReadWriteException>(() => new Spreadsheet("RWE.txt", s => true, s => s, "wrongVersion"));

            //Test wrong format in version
            using (StreamWriter outputFile = new StreamWriter("wrongFormatXML.txt"))
            {
                outputFile.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<WRONG version=\"default\">\r\n </WRONG>");
            }
            Assert.ThrowsException<SpreadsheetReadWriteException>(() => new Spreadsheet("wrongFormatXML.txt", s => true, s => s, "default"));

            //Test wrong format in data
            using (StreamWriter outputFile = new StreamWriter("wrongFormatXML1.txt"))
            {
                outputFile.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<spreadsheet version=\"default\">\r\n  <Wrong>\r\n    <name>A1</name>\r\n    <content>1</content>\r\n  </cell>\r\n  </spreadsheet>");
            }
            Assert.ThrowsException<SpreadsheetReadWriteException>(() => new Spreadsheet("wrongFormatXML1.txt", s => true, s => s, "default"));

            //Test wrong saving format
            s.SetContentsOfCell("A1", "1");
            Assert.ThrowsException<SpreadsheetReadWriteException>(() => s.Save("*()_+.exe"));
        }

        /// <summary>
        /// A method to create a spreadsheet with number, string, formula error
        /// </summary>
        /// <returns></returns>
        private Spreadsheet SmallSpreadSheet()
        {
            Spreadsheet s = new();
            s.SetContentsOfCell("A1", "1");
            s.SetContentsOfCell("B1", "2");
            s.SetContentsOfCell("C1", "=A1+B1");
            s.SetContentsOfCell("D1", "=1/0");
            s.SetContentsOfCell("E1", "=D1+C1");
            s.SetContentsOfCell("F1", "string");
            return s;
        }
    }
}