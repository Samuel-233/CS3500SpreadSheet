using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Author:    Shu Chen
    /// Partner:   None
    /// Date:      2024/1/26
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
    /// This class's main function Evaluate can evaluate a formula in the form of string.
    /// </summary>
    public static class Evaluator
    {
        public delegate int Lookup(string v);

        public static object Evaluate(ReadOnlyCollection<string> tokens, Func<string, double> variableEvaluator)
        {
            Stack<double> values = new Stack<double>();
            Stack<string> operators = new Stack<string>();
            FormulaError error;

            double currentToken = 0;

            for (int i = 0; i < tokens.Count(); i++)
            {
                //Push to stack if it is number
                if (CheckToken.IsNumber(tokens[i], out currentToken))
                {
                    if (values.Count == 0)
                    {
                        values.Push(currentToken);
                    }
                    else
                    {
                        if (PeekAndEval(values, operators, currentToken, out error)) return error;
                    }
                }

                //Turn variable to value
                else if (CheckToken.IsVariable(tokens[i]))
                {
                    string variable = tokens[i];
                    try
                    {
                        currentToken = variableEvaluator(variable);
                    }
                    catch (Exception e)
                    {
                        return new FormulaError(e.Message);
                    }

                    if (PeekAndEval(values, operators, currentToken, out error)) return error;
                }

                // deal with * & /
                else if (tokens[i] == "/" || tokens[i] == "*")
                {
                    operators.Push(tokens[i]);
                }

                //deal with + & -
                else if (tokens[i] == "+" || tokens[i] == "-")
                {
                    if (values.Count() >= 2 && operators.Count() >= 1 && (operators.Peek() == "+" || operators.Peek() == "-"))
                    {
                        if (CalAndPush(values, operators, out error)) return error;
                    }
                    operators.Push(tokens[i]);
                }

                //Deal with left Parenthesis
                else if (tokens[i] == "(")
                {
                    operators.Push(tokens[i]);
                }
                //Deal with right Parenthesis
                else if (tokens[i] == ")")
                {
                    if (CalParenthesis(values, operators, tokens, i, out error)) return error;
                }
            }

            //Cal if there is something left
            if (values.Count == 2 && operators.Count == 1)
            {
                if (CalAndPush(values, operators, out error)) return error;
                return values.Pop();
            }

            //Return the final result.
            else if (values.Count == 1 && operators.Count == 0)
            {
                return values.Pop();
            }
            else { return new FormulaError("Input Formula encounter some unknown problem, please check it."); }
        }

        /// <summary>
        /// Evaluate the formula inside parenthesis.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="operators"></param>
        /// <param name="tokenArray"></param>
        /// <param name="i"></param>
        /// <exception cref="ArgumentException"></exception>
        private static bool CalParenthesis(Stack<double> values, Stack<string> operators, ReadOnlyCollection<string> tokenArray, int i, out FormulaError error)
        {
            //if left operator is not ( then cal it.
            if (values.Count > 1 && operators.Count > 0 && (operators.Peek() != "("))
            {
                if (CalAndPush(values, operators, out error)) return true;
            }

            //pop out the  (
            operators.Pop();

            //cal the * / before the (
            if (values.Count > 1 && operators.Count > 0 && (operators.Peek() == "*" || operators.Peek() == "/"))
            {
                if (CalAndPush(values, operators, out error)) return true;
            }

            error = new FormulaError();
            return false;
        }

        /// <summary>
        /// calculate the equation by given operator.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="operators"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>return true if met an error</returns>
        private static bool CalAndPush(Stack<double> values, Stack<string> operators, out FormulaError error)
        {
            string @operator = operators.Pop();
            double right = values.Pop();
            double left = values.Pop();

            switch (@operator)
            {
                case "*":
                    values.Push(left * right);
                    break;

                case "/":
                    if (right == 0)
                    {
                        error = new FormulaError("Divide by zero error");
                        return true;
                    }
                    values.Push(left / right);
                    break;

                case "+":
                    values.Push(left + right);
                    break;

                case "-":
                    values.Push(left - right);
                    break;

                default:
                    break;
            }
            error = new FormulaError();
            return false;
        }

        /// <summary>
        /// When we pushing value, need to check if the first op is * | / , if it is, then eval it.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="operators"></param>
        /// <param name="currentToken"></param>
        private static bool PeekAndEval(Stack<double> values, Stack<string> operators, double currentToken, out FormulaError error)
        {
            if (operators.Count != 0 && values.Count != 0)
            {
                if (operators.Peek() == "/" || operators.Peek() == "*")
                {
                    if (DivAndMult(values, operators, currentToken, out error)) return true;
                }
                else values.Push(currentToken);
            }
            else { values.Push(currentToken); }
            error = new();
            return false;
        }

        /// <summary>
        /// Start to evaluate the * or /.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="operators"></param>
        /// <param name="currentToken"></param>
        private static bool DivAndMult(Stack<double> values, Stack<string> operators, double currentToken, out FormulaError error)
        {
            if (operators.Peek() == "/" && currentToken == 0)
            {
                error = new FormulaError("Can not divide a number by zero");
                return true;
            }

            string @operator = operators.Pop();

            if (@operator == "*") { values.Push(values.Pop() * currentToken); }
            else { values.Push(values.Pop() / currentToken); }

            error = new();
            return false;
        }

        /// <summary>
        /// Check if a token is a number or variable.
        /// </summary>
        internal class CheckToken
        {
            public static bool IsNumber(string token, out double number)
            { return double.TryParse(token, out number); }

            public static bool IsVariable(string token)
            { return Regex.IsMatch(token, @"[a-zA-Z_](?: [a-zA-Z_]|\d)*"); }
        }
    }
}