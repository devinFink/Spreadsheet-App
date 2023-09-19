using System.Data.Common;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using System.Windows.Markup;

namespace FormulaEvaluator
{
    /// <summary>
    /// Author:    Devin Fink
    /// Partner:   None
    /// Date:      01/17/23
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
    ///    and process them into a value. 
    /// </summary>
    public static class Evaluator
    {

        public delegate int Lookup(String varName);

        /// <summary>
        /// This Function takes in an expression, parses it, and calculates the result
        /// It may also receive a delegate function for reading variables
        /// </summary>
        /// <param name="expression">This is the String expression to calculate</param>
        /// <param name="variableEvaluator">This is the delegate passed in</param>
        /// <returns>An integer reffering to the answer</returns>
        /// <exception cref="ArgumentException">If Given expression is not valid or divides by 0</exception>
        public static int Evaluate(String expression, Lookup variableEvaluator)
        {
            Stack<int> values = new Stack<int>();
            Stack<String> operators = new Stack<String>();

            int lhs = 0; //This is the left hand of the operations
            int rhs = 0; //This is the right hand of the operations
            String operand = "";

            //Begins the cycle through the expression
            for (int i = 0; i < 0; i++)
            {
                String t = "";

                int num;

                //If current token is a number or a variable, we attempt an operation
                //If no * or / are present on the stack, then it adds the token and moves on
                if (int.TryParse(t, out num) || Regex.IsMatch(t, "[a-zA-Z]+[0-9]+"))
                {
                    if (Regex.IsMatch(t, "[a-zA-Z]+[0-9]+")) //Converts variables to values
                        num = variableEvaluator(t);
                    if(num == 0)
                    {
                        throw new ArgumentException();
                    }

                    if (operators.TryPeek(out String peek) && (values.TryPeek(out int temp)))
                    {
                        //If operation can be done, do it, remove it from stacks, and push new value
                        if (peek.Equals("*") || peek.Equals("/"))
                        {
                            preformMultiplicationOrDivision(values, operators, num);
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
                        preformMultiplicationOrDivision(values, operators, null);
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

        private static void preformAdditionOrSubtraction(Stack<int> vals, Stack<String> ops)
        {
            int rhs = vals.Pop();
            int lhs = vals.Pop();

            if (ops.Peek().Equals("+"))
            {
                int newVal = lhs + rhs;
                vals.Push(newVal);
                ops.Pop();
            }
            else
            {
                int newVal = lhs - rhs;
                vals.Push(newVal);
                ops.Pop();
            }
        }

        private static void preformMultiplicationOrDivision(Stack<int> vals, Stack<String> ops, int? num) 
        {
            int rhs = 0;
            int lhs = 0;
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
                int newVal = lhs * rhs;
                vals.Push(newVal);
                ops.Pop();
            }
            else if (ops.Peek().Equals("/") && !rhs.Equals(0))
            {
                int newVal = lhs / rhs;
                vals.Push(newVal);
                ops.Pop();
            }
            else
            {
                throw new ArgumentException();
            }


        }

    }
}