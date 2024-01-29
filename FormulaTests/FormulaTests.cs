using SpreadsheetUtilities;

namespace FormulaTests
{
/// <summary>
/// A test class for Formula
/// </summary>
    
    [TestClass]
    public class FormulaTests
    {
    /// <summary>
    /// Test basic calculation
    /// </summary>
        [TestMethod]
        public void TestBasicCal(){
            Assert.AreEqual(2.0, new Formula("1+1", s => s, s => true).Evaluate(s=>0));
            Assert.AreEqual(3.0, new Formula("9.0/3.0", s => s, s => true).Evaluate(s => 0));
            Assert.AreEqual(8.2, new Formula("2*4.1", s => s, s => true).Evaluate(s => 0));
            Assert.AreEqual(0.0, new Formula("1-1", s => s, s => true).Evaluate(s => 0));
            Assert.AreEqual(2.0, new Formula("((1+1))", s => s, s => true).Evaluate(s => 0));
            Assert.AreEqual(9.0, new Formula("3*((20+1)/7)", s => s, s => true).Evaluate(s => 0));
        }

        /// <summary>
        /// Test basic calculation with variable
        /// </summary>
        [TestMethod]
        public void TestBasicCalWithVariables()
        {
            Assert.AreEqual(2.0, new Formula("1+A1", s => s, s => true).Evaluate(s => 1));
            Assert.AreEqual(1.0, new Formula("A1/A1", s => s, s => true).Evaluate(s => 9));
            Assert.AreEqual(6.0, new Formula("a1+A1", s => s.ToUpper(), s => true).Evaluate(s => 3));
            Assert.AreEqual(3.0, new Formula("A1/b1", s => s, s => true).Evaluate(s => s.Equals("A1")? 9.0:3.0));
            Assert.AreEqual(2.5, new Formula("A1/b1", s => s, s => true).Evaluate(s => s.Equals("A1") ? 5 : 2));
        }

        /// <summary>
        /// Test to see if it can return formula Error
        /// </summary>
        /// <exception cref="Exception"></exception>
        [TestMethod]
        public void TestFormulaError()
        {
            Assert.IsTrue(new Formula("0/0", s => s, s => true)
                .Evaluate(s => throw new Exception($"can't find the variable {s}")) is FormulaError);
            Assert.IsTrue(new Formula("0/0.00000000000000", s => s, s => true)
                .Evaluate(s => throw new Exception($"can't find the variable {s}")) is FormulaError);
            Assert.IsTrue(new Formula("A1/10", s => s, s => true)
                .Evaluate(s => throw new Exception($"can't find the variable {s}")) is FormulaError);
        }

        /// <summary>
        /// Test Operators 
        /// </summary>
        [TestMethod]
        public void TestOperators(){
            Assert.IsTrue(new Formula("1", s => s, s => false)==(new Formula("1", s => s, s => false)));
            Assert.IsTrue(new Formula("1+1", s => s, s => false)==(new Formula("1+1", s => s, s => false)));
            Assert.IsTrue(new Formula("1.0", s => s, s => false)==(new Formula("1.0", s => s, s => false)));
            Assert.IsFalse(new Formula("1.0+1.0", s => s, s => false)!=(new Formula("1+1", s => s, s => false)));
            Assert.IsFalse(new Formula("1.0+1", s => s, s => false) != (new Formula("1+1.0", s => s, s => false)));


        }

        /// <summary>
        /// Test hash code
        /// </summary>
        [TestMethod]
        public void TestHashCode(){
            Assert.AreEqual(new Formula("10.000+1", s => s, s => false).GetHashCode(),
                new Formula("1e1+1.0000", s => s, s => false).GetHashCode());
            Assert.AreEqual(new Formula("a1+1.0", s => s, s => true).GetHashCode(),
                new Formula("a1+1", s => s, s => true).GetHashCode());
            Assert.AreEqual(new Formula("x1+y2", s => s.ToUpper(), s => true).GetHashCode(),
                new Formula("X1+y2", s => s.ToUpper(), s => true).GetHashCode());
            Assert.AreEqual(new Formula("x1+y2", s => s.ToUpper(), s => true).GetHashCode(),
                new Formula("X1+Y2", s => s.ToUpper(), s => true).GetHashCode());
        }

        /// <summary>
        /// Test variables
        /// </summary>
        [TestMethod]
        public void TestGetVariables(){
            Formula formula = new Formula("a1+b1+C1+DD1", s => s, s => true);

            IEnumerator<string> e = formula.GetVariables().GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a1", e.Current);
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b1", e.Current); 
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("C1", e.Current);
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("DD1", e.Current);


            formula = new Formula("a1+a1+b1+b1", s => s, s => true);

            e = formula.GetVariables().GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a1", e.Current);
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b1", e.Current); 
            
            formula = new Formula("a1+b1+A1+B1", s => s.ToUpper(), s => true);
            e = formula.GetVariables().GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("A1", e.Current);
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("B1", e.Current);
        }


        /// <summary>
        /// Test to see each formula is equal or not
        /// </summary>
        [TestMethod]
        public void TestEquals()
        {
            Assert.IsTrue(new Formula("1", s => s, s => false).Equals(new Formula("1")));
            Assert.IsTrue(new Formula("1+1", s => s, s => false).Equals(new Formula("1+1")));
            Assert.IsTrue(new Formula("1.0", s => s, s => false).Equals(new Formula("1.0")));
            Assert.IsTrue(new Formula("1.0+1.0").Equals(new Formula("1+1", s => s, s => false)));
            Assert.IsTrue(new Formula("1.0+1", s => s, s => false).Equals(new Formula("1+1.0")));
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

        /// <summary>
        /// Test the "compile" stage of the Formula class
        /// </summary>
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

        /// <summary>
        /// Test the "compile" stage of the Formula class - each format rules
        /// </summary>
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
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("3*((20+1)/7)*(8)8", s => s, s => true));

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