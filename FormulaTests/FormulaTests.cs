using SpreadsheetUtilities;

namespace FormulaTests
{

    
    [TestClass]
    public class FormulaTests
    {
        [TestMethod]
        public void TestBasicCal(){

        }

        [TestMethod]
        public void TestBasicCalWithVariables()
        {
            
        }


        [TestMethod]
        public void TestFormatRules()
        {
            //Rule1
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("1^3", s => s, s => true));
            //Rule2
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("", s => s, s => true));
            //Rule3
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("(3))", s => s, s => true));
            //Rule4
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("((4)", s => s, s => true));
            //Rule5
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("+1+1", s => s, s => true));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("-1+1", s => s, s => true));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("*1+1", s => s, s => true));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("/1+1", s => s, s => true));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula(")1+1", s => s, s => true));
            //Rule6
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("1+1(", s => s, s => true));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("1+1+", s => s, s => true));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("1+1-", s => s, s => true));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("1+1*", s => s, s => true));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("1+1/", s => s, s => true));
            //Rule7
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("()1+1", s => s, s => true));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("(1+1", s => s, s => true));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("(+1+1", s => s, s => true));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("(-1+1", s => s, s => true));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("(*1+1", s => s, s => true));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("(/1+1", s => s, s => true));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("**1+1", s => s, s => true));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("-*1+1", s => s, s => true));
            //Rule8
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("1 1", s => s, s => true));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("1 A1", s => s, s => true));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("1 (", s => s, s => true));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("A1 (", s => s, s => true));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("A1 1", s => s, s => true));

            //Check is Valid Func
            try{
                new Formula("1+1", s => s, s => false);
            }catch(Exception ex){
                Assert.Fail("Exception:" + ex);
            }//Even Valid Func return false, but there is no variable in formula, so it does not throw error

            Assert.ThrowsException<FormulaFormatException>(() => new Formula("1+A1", s => s, s => false));
        }
    }
}