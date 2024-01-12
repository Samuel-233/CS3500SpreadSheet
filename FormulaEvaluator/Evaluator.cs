using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    /// <summary>
    /// Author:    Shu Chen
    /// Partner:   [Partner Name or None]
    /// Date:      2024/1/10
    /// Course:    CS 3500, University of Utah, School of Computing
    /// Copyright: CS 3500 and [Your Name(s)] - This work may not 
    ///            be copied for use in Academic Coursework.
    ///
    /// I, Shu Chen, certify that I wrote this code from scratch and
    /// did not copy it in part or whole from another source.  All 
    /// references used in the completion of the assignments are cited 
    /// in my README file.
    ///
    /// File Contents
    ///
    ///    [... and of course you should describe the contents of the 
    ///    file in broad terms here ...]
    /// </summary>
    public delegate int Lookup(String variable_name);
    public class Evaluator
    {
        static Dictionary<String, int> variables = new Dictionary<string, int>();
        static BraceTracker? brace = null;

        public static int Evaluate(String expression,
                                   Lookup variableEvaluator)
        {
            
            List<String> tokens = checkTokenValid(
            Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)"));


            brace = new BraceTracker(tokens);

            //First, deal with the most inner brace, like normal calculate
            while (!brace.NoBrace())
            {
                int start = brace.GetFrontBrace();
                int end = brace.GetBackBrace();
                if (start != -1 && end != tokens.Count)
                {
                    tokens[start] = " ";
                    tokens[end] = " ";
                }

                //find all operators in that brace
                OperatorTracker operators = new OperatorTracker(tokens, start, end);

                //first,calculate the * and /
                while (!operators.Op1Empty())
                {
                    //use operators to locate other numbers and calculate.
                    Calculate(operators.GetOp1st(), tokens);
                }

                //then,calculate the + and -
                while (!operators.Op2Empty())
                {
                    //use operators to locate other numbers and calculate.
                    Calculate(operators.GetOp2nd(), tokens);
                }

            }

            foreach (String token in tokens) { if (token != " ") return Convert.ToInt32(token); }
            return 0;


        }

        public static void Calculate(int operatorIndex, List<String> tokens)
        {
            String @operator = tokens[operatorIndex];

            int index = 0;
            while (tokens[operatorIndex - ++index] == " ") { }
            int left = Convert.ToInt32(tokens[operatorIndex - index]);
            tokens[operatorIndex - index] = " ";

            index = 0;
            while (tokens[operatorIndex + ++index] == " ") { }
            int right = Convert.ToInt32(tokens[operatorIndex + index]);
            tokens[operatorIndex + index] = " ";

            int answer = 0;
            if (@operator == "+") { answer = left + right; }
            else if (@operator == "-") { answer = left - right; }
            else if (@operator == "*") { answer = left * right; }
            else if (@operator == "/") { answer = left / right; }

            tokens[operatorIndex] = answer.ToString();
        }

        /// <summary>
        /// Return variable's value if it exist
        /// </summary>
        /// <param name="variable_name"></param>
        /// <returns>Return variable's value if it exist</returns>
        /// <exception cref="Exception"></exception>
        public static int LookUp(String variable_name)
        {
            if (variables.ContainsKey(variable_name))
            {
                return variables[variable_name];
            }
            throw new Exception("Not a valid variable");
        }


        /// <summary>
        /// A helper method to remove all white space
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns>a new List contains no space tokens</returns>
        public static List<String> checkTokenValid(String[] tokens)
        {
            List<String> newTokens = new List<string>();
            char space = ' ';
            foreach (String token in tokens)
            {
                int frontIndex = 0;
                int backIndex = token.Length - 1;
                bool haveSpace = false;
                if (token.Length == 0 || (token.Length == 1 && token[0] == space))
                {
                    continue;
                }
                while (frontIndex < token.Length - 1 && token[frontIndex] == space)
                {
                    haveSpace = true;
                    frontIndex++;
                }
                while (backIndex > 0 && token[backIndex] == space)
                {
                    haveSpace = true;
                    backIndex--;
                }
                if (haveSpace && frontIndex <= backIndex)
                {
                    newTokens.Add(token.Substring(frontIndex, backIndex - frontIndex + 1));
                    continue;
                }
                if (!haveSpace)
                {
                    newTokens.Add(token);
                }
            }
            return newTokens;
        }




    }
}
