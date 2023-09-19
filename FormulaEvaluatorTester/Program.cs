using FormulaEvaluator;

/// <summary>
/// Author:    Devin Fink
/// Partner:   None
/// Date:      01/19/23
/// Course:    CS 3500, University of Utah, School of Computing
/// Copyright: CS 3500 and Devin Fink - This work may not 
///            be copied for use in Academic Coursework.
///
/// I, Devin Fink, certify that I wrote this code from scratch and
/// did not copy it in part or whole from another source.  All 
/// references used in the completion of the assignments are cited 
/// in my README file.
///
/// File Contents
///
///    This File tests the previously written Evaluator class and allows it to be ran through the console as an application
/// </summary>
internal class Program
{
    /// <summary>
    /// Puts all tests together and runs them all
    /// </summary>
    /// <param name="args">N/A for this method</param>
    private static void Main(string[] args)
    {
        firstTest();
        SecondTest();
        ThirdTest();
        FourthTest();
        FifthTest();
        SixthTest();
        VariableTest();
        VariableTest2();
        nestedParenths();
        tripleParenths();
        dividebyZero();
        brokenForm();
        Console.ReadLine();
    }

    //The following 6 tests are designed to test basic functionalities of the program
    //Including but not limited to the 4 basic operations (+-*/) and Order of operations
    /// <summary>
    /// Computes a basic multiplication
    /// </summary>
    private static void firstTest()
    {
        if (Evaluator.Evaluate("5*5", null) == 25)
        {
            Console.WriteLine("1 Success");
        }
    }
    /// <summary>
    /// Computes a basic order of operations
    /// </summary>
    private static void SecondTest()
    {
        if (Evaluator.Evaluate("5 * 5 + 5", null) == 30)
        {
            Console.WriteLine("2 Success");
        }
        else { Console.WriteLine(Evaluator.Evaluate("5 * 5 + 5", null)); }
    }

    /// <summary>
    /// Computes basic parentheses
    /// </summary>
    private static void ThirdTest()
    {
        if (Evaluator.Evaluate("5 * (5 + 5)", null) == 50)
        {
            Console.WriteLine("3 Success");
        }
        else { Console.WriteLine(Evaluator.Evaluate("5 * (5 + 5)", null)); }
    }

    /// <summary>
    /// Computes parentheses with multiple other operators
    /// </summary>
    private static void FourthTest()
    {
        if (Evaluator.Evaluate("5 * (5 + 5) / 10", null) == 5)
        {
            Console.WriteLine("4 Success");
        }
        else { Console.WriteLine(Evaluator.Evaluate("5 * (5 + 5) / 10", null)); }
    }

    /// <summary>
    /// Computes a subtraction in a larger formula
    /// </summary>
    private static void FifthTest()
    {
        if (Evaluator.Evaluate("5 * (10-4) / 10", null) == 3)
        {
            Console.WriteLine("5 Success");
        }
        else { Console.WriteLine(Evaluator.Evaluate("5 * (10 - 4) / 10", null)); }
    }

    /// <summary>
    /// Computes a long equation, with almost all operators to test everything together
    /// </summary>
    private static void SixthTest()
    {
        if (Evaluator.Evaluate("1 + 2 * 4 - (2 + 10) * 51 + (8 + 2) - 60 * 101", null) == -6653)
        {
            Console.WriteLine("6 Success");
        }
        else { Console.WriteLine(Evaluator.Evaluate("1 + 2 * 4 - (2 + 10) * 51 + (8 + 2) - 60 * 101", null)); }
    }

    //The next 2 tests are designed to test edge cases in regards to parentheses
    /// <summary>
    /// Tests nested parentheses
    /// </summary>
    private static void nestedParenths()
    {
        if (Evaluator.Evaluate("5 + (5*(5+5))", null) == 55)
        {
            Console.WriteLine("Nested Parentheses Success");
        }
    }

    /// <summary>
    /// Tests multiple operators in the same set of parentheses
    /// </summary>
    private static void tripleParenths()
    {
        if (Evaluator.Evaluate("5 + (5 / 5 + 1)", null) == 7)
        {
            Console.WriteLine("Triple Parentheses Success");
        }
    }

    //The next 2 tests test functionality in regards to variables in equations
    /// <summary>
    /// Tests basic Variable functionality
    /// </summary>
    private static void VariableTest()
    {
        if (Evaluator.Evaluate("5 + A1", (x)=>5) == 10)
        {
            Console.WriteLine("7 Success");
        }
        else { Console.WriteLine(Evaluator.Evaluate("5 + A1", (x)=>5)); }
    }

    /// <summary>
    /// Tests basic variable functionality on a larger, more complex variable
    /// </summary>
    private static void VariableTest2()
    {
        if (Evaluator.Evaluate("5 + hello99", (x) => 5) == 10)
        {
            Console.WriteLine("8 Success");
        }
        else { Console.WriteLine(Evaluator.Evaluate("5 + hello99", (x) => 5)); }
    }

    //The next two edgecases test for broken equations or invalid operations
    /// <summary>
    /// Tests for a correct exception when a division by 0 is expected
    /// </summary>
    private static void dividebyZero()
    {
        try
        {
            Evaluator.Evaluate("5 / 0", null);
        }
        catch(ArgumentException)
        {
            Console.WriteLine("Dividing by Zero Success (Or failure, depending how u look at it)");
        }
    }

    /// <summary>
    /// Tests proper exception throwing when given equation is wrong
    /// </summary>
    private static void brokenForm()
    {
        try
        {
            Evaluator.Evaluate("5 *", null);
        }
        catch (ArgumentException)
        {
            Console.WriteLine("BrokenForm Broke (Thats a success)");
        }
    }






}