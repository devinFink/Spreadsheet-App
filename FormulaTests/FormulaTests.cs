using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System.Runtime.CompilerServices;

namespace FormulaTests
{
    /// <summary>
    /// This Class is a tester class designed to test the Formula class.
    /// Tests are described in their header comments as well as have relatively descriptive names
    /// </summary>
    [TestClass]
    public class FormulaTests
    {
        /// <summary>
        /// Simple Test case to make sure code exists
        /// </summary>
        [TestMethod]
        public void TestConstructingAddition()
        {
            Formula test = new Formula("1+1");
        }

        /// <summary>
        /// Testing Multiple operators on constructor
        /// </summary>
        [TestMethod]
        public void TestConstructingAllOperators()
        {
            Formula test = new Formula("1+1-1*1/1");
            Assert.IsTrue(test.ToString() == "1+1-1*1/1");
        }

        /// <summary>
        /// Testing Parentheses in the constructor
        /// </summary>
        [TestMethod]
        public void TestConstructingParentheses()
        {
            Formula test = new Formula("1+1-(1+1)");
            Assert.IsTrue(test.ToString() == "1+1-(1+1)");
        }

        /// <summary>
        /// Testing Parentheses in the constructor
        /// </summary>
        [TestMethod]
        public void TestConstructingMultipleParenths()
        {
            Formula test = new Formula("1+1-(1+1)*1+(5+19)");
            Assert.IsTrue(test.ToString() == "1+1-(1+1)*1+(5+19)");
        }

        /// <summary>
        /// Testing an emoty formula
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructingEmptyForm()
        {
            Formula test = new Formula("");
        }

        /// <summary>
        /// Testing Nested Parentheses in the constructor
        /// </summary>
        [TestMethod]
        public void TestConstructingMultpleparenths()
        {
            Formula test = new Formula("(((10+5)+10)+10)");
            Assert.IsTrue(test.ToString() == "(((10+5)+10)+10)");
        }

        /// <summary>
        /// Testing Nested Parentheses in the constructor on the right
        /// </summary>
        [TestMethod]
        public void TestConstructingMultpleparenthsRightSide()
        {
            Formula test = new Formula("(10+(10+(10*10)))");
            Assert.IsTrue(test.ToString() == "(10+(10+(10*10)))");
        }

        /// <summary>
        /// Testing A variable
        /// </summary>
        [TestMethod]
        public void TestConstructingVariables()
        {
            Formula test = new Formula("x1+y2");
            Assert.IsTrue(test.ToString() == "x1+y2");
        }

        /// <summary>
        /// Testing a variable with a normalizer
        /// </summary>
        [TestMethod]
        public void TestConstructingVariablesAndNormalizing()
        {
            Formula test = new Formula("x1+y2", normalize, isValid);
            Assert.IsTrue(test.ToString() == "X1+Y2");
        }

        /// <summary>
        /// Testing variables in a large formula with Complex variables
        /// </summary>
        [TestMethod]
        public void TestConstructingVariablesInALargeFormulaAndComplexVariables()
        {
            Formula test = new Formula("x1+Y2-h222+(100/__)", normalize, isValid);
            Assert.IsTrue(test.ToString() == "X1+Y2-H222+(100/__)");
        }

        /// <summary>
        /// Testing an emoty formula
        /// </summary>
        [TestMethod]
        public void TestConstructingSpaces()
        {
            Formula test = new Formula("1 + 1 + 5 - (810 + (1000+ 50))");
            Assert.IsTrue(test.ToString() == "1+1+5-(810+(1000+50))");
        }

        /// <summary>
        /// Testing a variable not being valid
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructingBadVariable()
        {
            Formula test = new Formula("1s+y2", normalize, isValid);
        }

        /// <summary>
        /// Testing a variable not being valid
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructingBadVariable2()
        {
            Formula test = new Formula("x1+y2", normalize, s => false);
        }

        /// <summary>
        /// Testing a variable not being valid
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructingBadVariable3()
        {
            Formula test = new Formula("x1+y2", s => "SomeInvalidVar-!-", s => true);
        }

        /// <summary>
        /// Testing No Operator after num
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestMissingOp()
        {
            Formula test = new Formula("111 + 65(101 - 200)");
        }

        /// <summary>
        /// Testing Unbalanced LeftParenths
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestUnbalancedParenthsOnLeft()
        {
            Formula test = new Formula("111 + 65((101 - 200)");
        }

        /// <summary>
        /// Testing Bad Ending
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestBadEnd()
        {
            Formula test = new Formula("111 + 65(101 - 200+");
        }

        /// <summary>
        /// Testing Bad Ending
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestBadEnd2()
        {
            Formula test = new Formula("111 + 65 - 101 - 200+");
        }

        /// <summary>
        /// Testing Bad Ending
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInvalidToken()
        {
            Formula test = new Formula("3 | x");
        }

        /// <summary>
        /// Testing Operator after Operator
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInvalidFollowupToOperator()
        {
            Formula test = new Formula("3 + + 8");
        }

        /// <summary>
        /// Testing Unbalanced parenths
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestBadParenths()
        {
            Formula test = new Formula("111 + 65(101 - 200)))");
        }

        /// <summary>
        /// Testing Unbalanced parenths
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestBadParenths2()
        {
            Formula test = new Formula("111 + 65101 - 200)))");
        }

        /// <summary>
        /// Testing Bad Start
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestIllegalStart()
        {
            Formula test = new Formula("-+)");
        }

        /// <summary>
        /// Testing Addition
        /// </summary>
        [TestMethod]
        public void TestBasicCalculation()
        {
            Formula test = new Formula("1+1");
            Assert.IsTrue((double)test.Evaluate(s => 5) == 2);
        }

        /// <summary>
        /// Testing Multiplication
        /// </summary>
        [TestMethod]
        public void TestBasicCalculation2()
        {
            Formula test = new Formula("5*5");
            Assert.IsTrue((double)test.Evaluate(s => 5) == 25);
        }

        /// <summary>
        /// Testing Multiplication W/ parentheses
        /// </summary>
        [TestMethod]
        public void TestBasicCalculation3()
        {
            Formula test = new Formula("5*5+(6-3)");
            Assert.IsTrue((double)test.Evaluate(s => 5) == 28);
        }

        /// <summary>
        /// Testing Multiple left parenths
        /// </summary>
        [TestMethod]
        public void TestComplexNestedParensLeft()
        {
            Formula test = new Formula("((((x1+x2)+x3)+x4)+x5)+x6");
            Assert.AreEqual(12, (double)test.Evaluate(s => 2));
        }

        /// <summary>
        /// Tests equality of a single num
        /// </summary>
        [TestMethod]
        public void TestEqualityOfANumber()
        {
            Formula test = new Formula("2.000");
            Formula test2 = new Formula("2.0");
            Assert.IsTrue(test.Equals(test2));
        }

        /// <summary>
        /// Tests equality of a Formula
        /// </summary>
        [TestMethod]
        public void TestEqualityOfAFormula()
        {
            Formula test = new Formula("2.000 + 6.00000000000 - 3.0");
            Formula test2 = new Formula("2.0 + 6 - 3.00");
            Assert.IsTrue(test.Equals(test2));
        }

        /// <summary>
        /// Tests equality of a Formula
        /// </summary>
        [TestMethod]
        public void TestDivision()
        {
            Formula test = new Formula("(3.000 + 6.00000000000) / 3");
            Assert.AreEqual(3.0, test.Evaluate(s => 0));
        }
        
        /// <summary>
        /// Tests equality of a Formula
        /// </summary>
        [TestMethod]
        public void TestSubtractionFirst()
        {
            Formula test = new Formula("10 - 5 + 3 - 2");
            Assert.AreEqual((double)6, test.Evaluate(s => 0));
        }

        /// <summary>
        /// Tests equality of a single num
        /// </summary>
        [TestMethod]
        public void TestEqualityWithOperator()
        {
            Formula test = new Formula("2.000");
            Formula test2 = new Formula("2.0");
            Assert.IsTrue(test == test2);
        }

        /// <summary>
        /// Tests equality of a Formula
        /// </summary>
        [TestMethod]
        public void TestInequalityWithOperator()
        {
            Formula test = new Formula("2.000 + 6.00000000000 - 3.0");
            Formula test2 = new Formula("2.0 + 6.2 - 3.00");
            Assert.IsTrue(test != test2);
        }

        /// <summary>
        /// Tests equality of a Formula
        /// </summary>
        [TestMethod]
        public void TestInequality()
        {
            Formula test = new Formula("2.000 + 6.00000000000 - 3.0");
            Formula test2 = new Formula("2.01010101 + 6.2 - 3.00");
            Assert.IsTrue(!test.Equals(test2));
        }

        /// <summary>
        /// Tests returning division by 0
        /// </summary>
        [TestMethod]
        public void TestDivisionBy0()
        {
            Formula test = new Formula("10/0");
            Assert.IsTrue(test.Evaluate(s => 1) is FormulaError);
        }

        /// <summary>
        /// Tests GetVariables
        /// </summary>
        [TestMethod]
        public void TestGetVariables()
        {
            Formula test = new Formula("x+X+y");
            List<String> list = test.GetVariables().ToList();
            Assert.IsTrue(list.Contains("x"));
            Assert.IsTrue(list.Contains("X"));
            Assert.IsTrue(list.Contains("y"));
        }

        /// <summary>
        /// Tests Scientific Notation
        /// </summary>
        [TestMethod]
        public void TestScientificNotation()
        {
            Formula test = new Formula("5e-5 + 1");
            Assert.AreEqual((double)1.00005, test.Evaluate(s => 0));
        }

        /// <summary>
        /// Tests Division in parentheses
        /// </summary>
        [TestMethod]
        public void TestDivisionInParenths()
        {
            Formula test = new Formula("(10 - (8/0))");
            Assert.IsTrue(test.Evaluate(s => 0) is FormulaError);
        }

        /// <summary>
        /// Simply makes sure the hashcode does not report an error
        /// </summary>
        [TestMethod]
        public void TestHashCode()
        {
            Formula test = new Formula("(10 - (8/0))");
            test.GetHashCode();
        }

        /// <summary>
        /// Complex Formula borrowed from assignment 1 Tests
        /// </summary>
        [TestMethod]
        public void TestComplexAndParentheses()
        {
            Formula test = new Formula("2+3*5+(3+4*8)*5+2");
            Assert.AreEqual((double)194, test.Evaluate(s => 0));
        }

        /// <summary>
        /// Previous complex method with division added
        /// </summary>
        [TestMethod]
        public void TestComplexAndParenthesesWithDivision()
        {
            Formula test = new Formula("(2+3*5+(3+4*8)*5+2) / 2");
            Assert.AreEqual((double)97, test.Evaluate(s => 0));
        }

        /// <summary>
        /// Previous complex method with division By Zero
        /// </summary>
        [TestMethod]
        public void TestComplexAndParenthesesWithDivisionByZero()
        {
            Formula test = new Formula("(2+3+(3+4/0)*5+2) / 2");
            Assert.IsTrue(test.Evaluate(s => 0) is FormulaError);
        }

        /// <summary>
        /// Previous complex method with division By Zero
        /// </summary>
        [TestMethod]
        public void TestDivisionByZeroBeforeParenths()
        {
            Formula test = new Formula("10 / (3-3)");
            Assert.IsTrue(test.Evaluate(s => 0) is FormulaError);
        }
















        private bool isValid(String var)
        {
            return true;
        }
        private String normalize(String var)
        {
            return var.ToUpper();
        }
    }


}