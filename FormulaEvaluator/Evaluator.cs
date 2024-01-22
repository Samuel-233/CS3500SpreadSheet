using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    /// <summary>
    /// Author:    Shu Chen
    /// Partner:   None
    /// Date:      2024/1/22
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
    public delegate int Lookup(String variable_name);
    public static class Evaluator
    {

        public delegate int Lookup(String v);
        public static int Evaluate(String expression, Lookup variableEvaluator)
        {
            int leftParenthesis = 0;
            int rightParenthesis = 0;
            bool isFirstToken = true;

            Stack<int> values = new Stack<int>();
            Stack<string> operators = new Stack<string>();
            expression = expression.Trim();

            string[] tokens = Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            int currentToken = 0;

            //Throw error if first token is not valid input
            if (!CheckToken.IsNumber(tokens[0].Trim(), out currentToken)
                && !CheckToken.IsVariable(tokens[0])
                && tokens[0] != "(" && tokens[0] != "")
                throw new ArgumentException("Not correct format, caused by first token");

            for (int i = 0; i < tokens.Count(); i++)
            {


                if (tokens[i].Trim() == "") { continue; }


                //Push to stack if it is number
                if (CheckToken.IsNumber(tokens[i], out currentToken))
                {
                    if (isFirstToken)
                    {
                        values.Push(currentToken);
                        isFirstToken = false;
                    }
                    else if (!isFirstToken)
                    {
                        PeekAndEval(values, operators, currentToken);
                    }
                }

                //Turn variable to value
                else if (CheckToken.IsVariable(tokens[i]))
                {
                    string variable = tokens[i];
                    isFirstToken = false;
                    currentToken = variableEvaluator(variable);
                    PeekAndEval(values, operators, currentToken);
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
                        CalAndPush(values, operators);
                    }
                    operators.Push(tokens[i]);
                }

                //Deal with left Parenthesis 
                else if (tokens[i] == "(")
                {
                    leftParenthesis++;
                    if (leftParenthesis < rightParenthesis) throw new ArgumentException("Right Parenthesis cannot become before than left Parenthesis");
                    operators.Push(tokens[i]);
                }
                //Deal with right Parenthesis 
                else if (tokens[i] == ")")
                {
                    rightParenthesis++;
                    CalParenthesis(values, operators, tokens, i);
                }
            }



            //Cal if there is something left
            if (values.Count == 2 && operators.Count == 1)
            {
                CalAndPush(values, operators);
                return values.Pop();
            }

            //Return the final result.
            else if (values.Count == 1 && operators.Count == 0)
            {
                return values.Pop();
            }

            else { throw new ArgumentException("Input tokens is wrong format"); }
        }




        /// <summary>
        /// Evaluate the formula inside parenthesis.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="operators"></param>
        /// <param name="tokenArray"></param>
        /// <param name="i"></param>
        /// <exception cref="ArgumentException"></exception>
        private static void CalParenthesis(Stack<int> values, Stack<string> operators, string[] tokenArray, int i)
        {
            //if left operator is not ( then cal it.
            if (values.Count > 1 && operators.Count > 0 && (operators.Peek() != "("))
            {
                CalAndPush(values, operators);
            }

            //if left is not (  or no more operators throw error
            if (operators.Count == 0 || operators.Peek() != "(")
            {
                throw new ArgumentException("bad formula format");
            }

            //pop out the  ( 
            operators.Pop();

            //cal the * / before the (
            if (values.Count > 1 && operators.Count > 0 && (operators.Peek() == "*" || operators.Peek() == "/"))
            {
                CalAndPush(values, operators);
            }
        }



        /// <summary>
        /// calculate the equation by given operator.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="operators"></param>
        /// <exception cref="ArgumentException"></exception>
        private static void CalAndPush(Stack<int> values, Stack<string> operators)
        {
            string @operator = operators.Pop();
            int right = values.Pop();
            int left = values.Pop();

            switch (@operator)
            {
                case "*":
                    values.Push(left * right);
                    break;
                case "/":
                    if (right == 0) throw new ArgumentException("Divide by zero error");
                    values.Push(left / right);
                    break;
                case "+":
                    values.Push(left + right);
                    break;
                case "-":
                    values.Push(left - right);
                    break;
                default:
                    Console.WriteLine("bad formula error");
                    break;
            }
        }

        /// <summary>
        /// When we pushing value, need to check if the first op is * | / , if it is, then eval it.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="operators"></param>
        /// <param name="currentToken"></param>
        private static void PeekAndEval(Stack<int> values, Stack<string> operators, int currentToken)
        {
            if (operators.Count != 0 && values.Count != 0)
            {
                if (operators.Peek() == "/" || operators.Peek() == "*")
                    DivAndMult(values, operators, currentToken);
                else values.Push(currentToken);
            }
            else { values.Push(currentToken); }
        }

        /// <summary>
        /// Start to evaluate the * or /.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="operators"></param>
        /// <param name="currentToken"></param>
        /// <exception cref="ArgumentException">Throw error if divide by zero</exception>
        private static void DivAndMult(Stack<int> values, Stack<string> operators, int currentToken)
        {
            if (operators.Peek() == "/" && currentToken == 0) throw new ArgumentException("Can not divide a number by zero");
            String @operator = operators.Pop();

            if (@operator == "*") { values.Push(values.Pop() * currentToken); }
            else { values.Push(values.Pop() / currentToken); }
        }

        /// <summary>
        /// Check if a token is a number or variable.
        /// </summary>
        internal class CheckToken
        {
            public static bool IsNumber(string token, out int number) { return int.TryParse(token, out number); }
            public static bool IsVariable(string token) { return Regex.IsMatch(token, @"[a-zA-Z]\d"); }
        }

    }




}
