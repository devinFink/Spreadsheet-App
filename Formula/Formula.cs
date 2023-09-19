// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens


using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities


    {/// <summary>
     /// Author:    Devin Fink
     /// Partner:   None
     /// Date:      02/02/23
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
     ///    This File provides the functions to properly recieve in-fix expressions
     ///    and process them into a value. Full API as follows:
     /// </summary>
     /// <summary>
     /// Represents formulas written in standard infix notation using standard precedence
     /// rules.  The allowed symbols are non-negative numbers written using double-precision 
     /// floating-point syntax (without unary preceeding '-' or '+'); 
     /// variables that consist of a letter or underscore followed by 
     /// zero or more letters, underscores, or digits; parentheses; and the four operator 
     /// symbols +, -, *, and /.  
     /// 
     /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
     /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
     /// and "x 23" consists of a variable "x" and a number "23".
     /// 
     /// Associated with every formula are two delegates:  a normalizer and a validator.  The
     /// normalizer is used to convert variables into a canonical form, and the validator is used
     /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
     /// that it consist of a letter or underscore followed by zero or more letters, underscores,
     /// or digits.)  Their use is described in detail in the constructor and method comments.
     /// </summary>
    public class Formula
    {
        private StringBuilder formulaString;
        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) : this(formula, s => s, s => true) { }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        /// <param name="formula">String to be evaluated</param>
        /// <param name="isValid">Delegate to check validity of Variables</param>
        /// <param name="normalize">Delegate to unify state of all variables</param>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            FormulaSyntaxCheck(formula, normalize, isValid);
        }

        /// <summary>
        /// This method contains all of the logic in order to build a Formula, and checks each token 
        /// for proper syntax. After this method is ran a formula is set in stone and cannot be changed 
        /// without creating a new one, so it is guarenteed to be syntactically correct.
        /// </summary>
        /// <param name = "formula" > String to be evaluated</param>
        /// <param name="isValid">Delegate to check validity of Variables</param>
        /// <param name="normalize">Delegate to unify state of all variables</param>
        /// <exception cref="FormulaFormatException"></exception>
        private void FormulaSyntaxCheck(string formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            formulaString = new StringBuilder();
            List<String> substrings = GetTokens(formula).ToList<string>();

            int leftParenths = 0;
            int rightParenths = 0;
            bool hasToken = false;

            //Initial Starting checks
            //Normalizes All variables and checks for validity
            for (int i = 0; i < substrings.Count; i++)
            {
                if (double.TryParse(substrings[i], out double notUsed))
                    continue;
                if (isVariable(substrings[i]))
                {
                    substrings[i] = normalize(substrings[i]);
                    if (!isValid(substrings[i]))
                    {
                        throw new FormulaFormatException("All Variables must be valid");
                    }

                    if (!Regex.IsMatch(substrings[i], "^[a-zA-Z_]([a-zA-Z_]|\\d)*$"))
                    {
                        throw new FormulaFormatException("All Variables must be valid");
                    }
                }
            }

            //Begins the loop through the formula to check each token individually
            for (int i = 0; i < substrings.Count; i++)
            {
                //If token is first, it must be a number, variable, or left parentheses
                String token = substrings[i];


                if (i == 0)
                {
                    if ((!double.TryParse(token, out double num) && !isVariable(token) && token != "("))
                    {
                        throw new FormulaFormatException("Starting Token must be valid");
                    }
                }

                //If token is last, must be a number, variable, or right parentheses
                if (i == substrings.Count - 1)
                {
                    if ((!double.TryParse(token, out double num) && !isVariable(token) && token != ")"))
                    {
                        throw new FormulaFormatException("Ending Token must be Valid");
                    }
                }

                //If token is not a valid token
                if ((token != "(" && token != ")" && token != "+" && token != "-" && token != "*" && token != "/" && !isVariable(token) && !double.TryParse(token, out double val)))
                    throw new FormulaFormatException($"{token} is an invalid token");
                else
                    hasToken = true;


                //This series of statements tracks parentheses and confirms that they stay balanced and kept even
                if (token == "(")
                    leftParenths++;
                else if (token == ")")
                {
                    rightParenths++;
                    if (rightParenths > leftParenths)
                        throw new FormulaFormatException("Unbalanced Parentheses");
                }

                //Depending on previous token, operator or value, the following token must be the opposite. 
                //Only runs for non-starting tokens
                if (i > 0)
                {
                    if (substrings[i - 1] == "(" || substrings[i - 1] == "+" || substrings[i - 1] == "-" || substrings[i - 1] == "/" || substrings[i - 1] == "*")
                    {
                        if ((!isVariable(token) && !double.TryParse(token, out val) && token != "("))
                            throw new FormulaFormatException("Must have valid value after operator");
                    }

                    if (substrings[i - 1] == ")" || isVariable(substrings[i - 1]) || double.TryParse(substrings[i - 1], out val))
                    {
                        if (token != ")" && token != "+" && token != "-" && token != "*" && token != "/")
                            throw new FormulaFormatException("Must have valid operator after value");
                    }
                }
                //Adds to the stringbuilder to simplify equality checks later
                if (double.TryParse(token, out double temp))
                    formulaString.Append(temp.ToString());
                else { formulaString.Append(normalize(token)); }
            }

            //Basic final checks for validity
            if (hasToken == false)
                throw new FormulaFormatException("Must have valid Token");
            if (leftParenths != rightParenths)
                throw new FormulaFormatException("Unbalanced Parentheses");
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// <paramref name="lookup"/> Delegate used to find value of variables
        /// <return> Object, either Double result of the formula or an error</return>
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            Stack<double> values = new Stack<double>();
            Stack<String> operators = new Stack<String>();

            double lhs = 0; //This is the left hand of the operations
            double rhs = 0; //This is the right hand of the operations
            String operand = "";
            List<String> substrings = GetTokens(this.ToString()).ToList<string>();

            //Begins the cycle through the expression
            for (int i = 0; i < substrings.Count; i++)
            {
                String t = substrings[i]; //Current Token

                double num; //To Store value if current token is a number

                //If current token is a number or a variable, we attempt an operation
                //If no * or / are present on the stack, then it adds the token and moves on
                if (double.TryParse(t, out num) || Regex.IsMatch(t, "[a-zA-Z_](?: [a-zA-Z_]|\\d)*"))
                {
                    if (Regex.IsMatch(t, "^[a-zA-Z_]([a-zA-Z_]|\\d)*$"))//Converts variables to values
                    {
                        try
                        {
                            num = lookup(t);
                        }
                        catch (ArgumentException e)
                        {
                            return new FormulaError("Invalid Variable value");
                        }
                    }

                    if (operators.TryPeek(out String peek) && (values.TryPeek(out double temp)))
                    {
                        //If operation can be done, do it, remove it from stacks, and push new value
                        if (peek.Equals("*") || peek.Equals("/"))
                        {
                            preformMultiplicationOrDivision(values, operators, num, out bool DivisionBy0);
                            if (DivisionBy0)
                                return new FormulaError("Division By 0 Occured");
                        }
                        else
                        {
                            values.Push(num);
                        }
                    }

                    else
                    {
                        values.Push(num);
                    }
                }

                //If current token is an addition or subtraction operator,
                //we check to see if all multipliers and dividers have been processed.
                //If so, we preform the operator from the stack and add the token
                //If not, we just add the token
                else if (t.Equals("+") || t.Equals("-"))
                {
                    if (operators.TryPeek(out String peek) && (values.Count >= 2))
                    {
                        if (peek.Equals("+") || peek.Equals("-"))
                        {
                            preformAdditionOrSubtraction(values, operators);
                        }
                    }
                    operators.Push(t);
                }

                //If Token is any of these operators, we simply push it to the stack
                else if (t.Equals("/") || t.Equals("*") || t.Equals("("))
                {
                    operators.Push(t);
                }

                //If end of parentheses has been found, we process any operators inside
                //Then, we remove the beginning of the parentheses and process any previous tokens
                else if (t.Equals(")"))
                {
                    operators.TryPeek(out String peek);
                    if (peek.Equals("+") || peek.Equals("-") && values.Count() >= 2)
                    {
                        preformAdditionOrSubtraction(values, operators);
                    }

                    //This if statement makes sure the parentheses tokens were processed correctly
                    if (operators.TryPeek(out String s) && s.Equals("("))
                    {
                        operators.Pop();
                    }

                    if (operators.TryPeek(out String op) && (op.Equals("*") || op.Equals("/")))
                    {
                        preformMultiplicationOrDivision(values, operators, null, out bool DivisionBy0);
                        if (DivisionBy0)
                        {
                            return new FormulaError("Division By Zero Occured");
                        }
                    }
                }
            }

            //This code is the final steps, after the array has been processed. 
            //We check for any remaining tokens, process them, and return the last
            //remaining value in the stack as our result
            //If anything is wrong here and the operations do not result in exactly
            //one value on the stack, an Argumentexception is thrown
            if (operators.Count() == 0)
            {
                return values.Pop();

            }
            //Only + or - should be left at this point,
            //anything else is an illegal argument
            else if (operators.Count() == 1)
            {
                if (operators.Peek().Equals("+"))
                {
                    if (values.Count() == 2)
                    {
                        rhs = values.Pop();
                        lhs = values.Pop();
                        return lhs + rhs;
                    }
                }
                if (operators.Peek().Equals("-"))
                {
                    if (values.Count() == 2)
                    {
                        rhs = values.Pop();
                        lhs = values.Pop();
                        return lhs - rhs;
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        /// <returns>Ienumerable of Strings</returns>
        public IEnumerable<String> GetVariables()
        {
            List<String> variables = new List<String>();
            List<String> allTokens = GetTokens(formulaString.ToString()).ToList<String>();
            foreach (var v in allTokens)
            {
                if (Regex.IsMatch(v, @"[a-zA-Z_](?: [a-zA-Z_]|\d)*"))
                {
                    variables.Add(v);
                }
            }

            return variables;
        }

        /// <summary>
        /// Checks whether or not a given token follows the requirements for 
        /// veing a variable
        /// </summary>
        /// <param name="token">String token to be evaluated</param>
        /// <returns>true if variable, false if not</returns>
        public bool isVariable(String token)
        {
            return (Regex.IsMatch(token, @"[a-zA-Z_](?: [a-zA-Z_]|\d)*"));
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            return formulaString.ToString();
        }

        /// <summary>
        ///  <change> make object nullable </change>
        ///
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        /// <param name="obj">Any nullable Object</param>
        /// <returns>true if equal, false if not</returns>
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            else if (!(obj is Formula)) return false;

            return (formulaString.ToString().Equals(obj.ToString()));
        }

        /// <summary>
        ///   <change> We are now using Non-Nullable objects.  Thus neither f1 nor f2 can be null!</change>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// 
        /// </summary>
        /// <param name="f1">Left side to equate</param>
        /// <param name="f2">Right side to equate</param>
        /// <returns>True if equal, false otherwise</returns>
        public static bool operator ==(Formula f1, Formula f2)
        {
            return f1.Equals(f2);
        }

        /// <summary>
        ///   <change> We are now using Non-Nullable objects.  Thus neither f1 nor f2 can be null!</change>
        ///   <change> Note: != should almost always be not ==, if you get my meaning </change>
        ///   Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// </summary>
        /// <param name="f1">Left side to equate</param>
        /// <param name="f2">Right side to equate</param>
        /// <returns>True if not equal, false otherwise</returns>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return !f1.Equals(f2);
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        /// <returns>Hashcode for This Formula</returns>
        public override int GetHashCode()
        {
            return formulaString.ToString().GetHashCode();
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        /// <param name="formula">String formula to represent this object</param>
        /// <returns>An Ienumerable with all of the tokens in it</returns>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }

        /// <summary>
        /// This method, given two stacks from the evaluate method, computes either addition
        /// or subtration based on current state of the stacks
        /// </summary>
        /// <param name="vals">Stack of Values</param>
        /// <param name="ops">Stack of Operators</param>
        private static void preformAdditionOrSubtraction(Stack<double> vals, Stack<String> ops)
        {
            double rhs = vals.Pop();
            double lhs = vals.Pop();

            if (ops.Peek().Equals("+"))
            {
                double newVal = lhs + rhs;
                vals.Push(newVal);
                ops.Pop();
            }
            else
            {
                double newVal = lhs - rhs;
                vals.Push(newVal);
                ops.Pop();
            }
        }

        /// <summary>
        /// This method, given two stacks from the evaluate method, computes either multiplication
        /// or Division based on current state of the stacks. This method must also return a bool based on 
        /// the status of division by 0, and takes in an extra num to identify how many values must be taken 
        /// off the stack to begin the operation.
        /// </summary>
        /// <param name="vals">Stack of Values</param>
        /// <param name="ops">Stack of Operators</param>
        private static void preformMultiplicationOrDivision(Stack<double> vals, Stack<String> ops, double? num, out bool DivisionBy0)
        {
            DivisionBy0 = false;
            double rhs = 0;
            double lhs = 0;
            if (num == null)
            {
                rhs = vals.Pop();
                lhs = vals.Pop();
            }
            else
            {
                rhs = (int)num;
                lhs = vals.Pop();
            }

            if (ops.Peek().Equals("*"))
            {
                double newVal = lhs * rhs;
                vals.Push(newVal);
                ops.Pop();
            }
            else if (ops.Peek().Equals("/") && !rhs.Equals(0))
            {
                double newVal = lhs / rhs;
                vals.Push(newVal);
                ops.Pop();
            }
            else if (rhs.Equals(0))
            {
                DivisionBy0 = true;
            }
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }


    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}


// <change>
//   If you are using Extension methods to deal with common stack operations (e.g., checking for
//   an empty stack before peeking) you will find that the Non-Nullable checking is "biting" you.
//
//   To fix this, you have to use a little special syntax like the following:
//
//       public static bool OnTop<T>(this Stack<T> stack, T element1, T element2) where T : notnull
//
//   Notice that the "where T : notnull" tells the compiler that the Stack can contain any object
//   as long as it doesn't allow nulls!
// </change>
