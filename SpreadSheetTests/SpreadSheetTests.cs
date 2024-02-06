using System.Collections.Generic;
using SpreadsheetUtilities;
using SS;

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
            Assert.ThrowsException<InvalidNameException>(()=>s.GetCellContents("1A"));
            Assert.ThrowsException<InvalidNameException>(() => s.GetCellContents("1A1A"));
        }

        /// <summary>
        /// Test for null name
        /// </summary>
        [TestMethod]
        public void TestNullName()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.ThrowsException<ArgumentNullException>(() => s.GetCellContents(null));
        }


        /// <summary>
        /// Empty cell should return a empty string
        /// </summary>
        [TestMethod]
        public void TestGetEmptyCell()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual("",s.GetCellContents("A1"));
            Assert.AreEqual("", s.GetCellContents("AA11"));
        }

        /// <summary>
        /// Fill in data in to cell
        /// </summary>
        [TestMethod]
        public void TestChangeCell()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", 1);
            s.SetCellContents("B1", "test");
            s.SetCellContents("C1", new Formula("1+1"));
            s.SetCellContents("D1", "");
            Assert.AreEqual(1.0, s.GetCellContents("A1"));
            Assert.AreEqual("test", s.GetCellContents("B1"));
            Assert.AreEqual(new Formula("1+1"), s.GetCellContents("C1"));
            Assert.IsTrue(Enumerable.SequenceEqual(new List<string> { "A1" ,"B1","C1"},s.GetNamesOfAllNonemptyCells()));
        }

        /// <summary>
        /// Test for dependency
        /// </summary>
        [TestMethod]
        public void TestDependency()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("F1", new Formula("0"));
            s.SetCellContents("A1", new Formula("B1+C1"));
            s.SetCellContents("B1", new Formula("C1+D1"));
            s.SetCellContents("C1", new Formula("D1+2"));
            IEnumerable<string> cellNeedToCal =  s.SetCellContents("D1", 1);
            Assert.IsTrue(Enumerable.SequenceEqual(new List<string> {"D1","C1","B1","A1"},cellNeedToCal));
        }

        /// <summary>
        /// Test for circular dependency, if there is, there shouldn't have data in the last cell that entered
        /// </summary>
        [TestMethod]
        public void TestCircularDependency()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("A1", new Formula("B1+C1"));
            s.SetCellContents("B1", new Formula("C1+1"));
            Assert.ThrowsException<CircularException>(() => s.SetCellContents("C1", new Formula("A1+B1")));

            Assert.AreEqual("", s.GetCellContents("C1"));
        }
    }
}