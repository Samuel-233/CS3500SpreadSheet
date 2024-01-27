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
        public void TestEquals()
        {
            Assert.IsTrue(new Formula("1", s => s, s => false).Equals(new Formula("1", s => s, s => false)));
            Assert.IsTrue(new Formula("1+1", s => s, s => false).Equals(new Formula("1+1", s => s, s => false)));
            Assert.IsTrue(new Formula("1.0", s => s, s => false).Equals(new Formula("1.0", s => s, s => false)));
            Assert.IsTrue(new Formula("1.0+1.0", s => s, s => false).Equals(new Formula("1+1", s => s, s => false)));
            Assert.IsTrue(new Formula("1.0+1", s => s, s => false).Equals(new Formula("1+1.0", s => s, s => false)));
            Assert.IsTrue(new Formula("1.0000", s => s, s => false).Equals(new Formula("1", s => s, s => false)));
            Assert.IsTrue(new Formula("1.000+1", s => s, s => false).Equals(new Formula("1e0+1.0000", s => s, s => false)));
            Assert.IsTrue(new Formula("10.000+1", s => s, s => false).Equals(new Formula("1e1+1.0000", s => s, s => false)));
            Assert.IsTrue(new Formula("a1+1.0", s => s, s => true).Equals(new Formula("a1+1", s => s, s => true)));
            Assert.IsTrue(new Formula("x1+y2", s => s.ToUpper(), s => true).Equals(new Formula("X1+y2", s => s.ToUpper(), s => true)));
            Assert.IsTrue(new Formula("x1+y2", s => s.ToUpper(), s => true).Equals(new Formula("X1+Y2", s => s.ToUpper(), s => true)));


            Assert.IsFalse(new Formula("a1+1.0", s => s, s => true).Equals(new Formula("a1", s => s, s => true)));
            Assert.IsFalse(new Formula("x1+y2", s => s.ToUpper(), s => true).Equals(new Formula("Y2+X1", s => s.ToUpper(), s => true)));
            Assert.IsFalse(new Formula("1+1.0", s => s, s => true).Equals(new Formula("1+2.0", s => s, s => true)));

            Assert.IsFalse(new Formula("a1+1.0", s => s, s => true).Equals(new List<string> {"a1","1.0"}));

        }

        [TestMethod]
        public void TestFormat(){
            try
            {
                new Formula("1.0+1.1", s => s, s => false);
                new Formula("1+1.3", s => s, s => false);
                new Formula("10e5+0.0+0.000", s => s, s => false);
                new Formula("10e5*9", s => s, s => false);
                new Formula("(10e5+A1)/3.0", s => s, s => true);
            }
            catch (Exception ex)
            {
                Assert.Fail("Exception:" + ex);
            }
        }

        [TestMethod]
        public void TestFormatRules()
        {
            //Rule1
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("3!", s => s, s => true));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("!3", s => s, s => true));
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
            //Rule6 - can't reach this exception
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("1((", s => s, s => true));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("1+(", s => s, s => true));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("1-(", s => s, s => true));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("1**", s => s, s => true));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("1//", s => s, s => true));
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
            try
            {
                new Formula("1+1", s => s, s => false);
            }
            catch (Exception ex)
            {
                Assert.Fail("Exception:" + ex);
            }//Even Valid Func return false, but there is no variable in formula, so it does not throw error

            Assert.ThrowsException<FormulaFormatException>(() => new Formula("1+A1", s => s, s => false));
        }
    }
}