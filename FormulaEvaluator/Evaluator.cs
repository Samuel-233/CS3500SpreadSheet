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
    /// this is a main part of Evaluate function, it will take a String as an expression (if it is valid) then calculate the result. Variables are also supported.
    /// </summary>
    public delegate int Lookup(String variable_name);
    public static class Evaluator
    {
        static Dictionary<String, int> variables = new Dictionary<string, int>();
        static BraceTracker? brace = null;

        public static void AddVariable(String variable_name, int value){
            variables.Add(variable_name, value);
        }

        public static void RemoveVariable(String variable_name){
        variables.Remove(variable_name);
        }

        /// <summary>
        /// To evaluate the String as a expression, and get a int result.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="variableEvaluator"></param>
        /// <returns>a int as the result for the expression</returns>
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

            //Find the last element left in the token, which will be the result.
            foreach (String token in tokens) { if (token != " ") return Convert.ToInt32(token); }
            return 0;


        }

        /// <summary>
        /// A method to evaluate expression that already remove the bracket
        /// </summary>
        /// <param name="operatorIndex">the position of the operator</param>
        /// <param name="tokens">a list of tokens that store the expression</param>
        /// <exception cref="Exception">throw error if variable don't exist</exception>
        public static void Calculate(int operatorIndex, List<String> tokens)
        {
            String @operator = tokens[operatorIndex];

            //To loop to left until find a int or variable, then get it and remove it.
            int index = 0;
            while (tokens[operatorIndex - ++index] == " ") { }
            int left = 0;
            String variable = tokens[operatorIndex - index];
            if (!int.TryParse(variable, out left)){
                if (variables.TryGetValue(variable, out left))
                {
                }
                else throw new Exception($"no such variable {variable} in data");
            }
            tokens[operatorIndex - index] = " ";

            //To loop to right until find a int or variable, then get it and remove it.
            index = 0;
            while (tokens[operatorIndex + ++index] == " ") { }
            int right = 0;
            variable = tokens[operatorIndex + index];
            if (!int.TryParse(variable, out right))
            {
                if (variables.TryGetValue(variable, out right))
                {
                }
                else throw new Exception($"no such variable {variable} in data");
            }
            tokens[operatorIndex + index] = " ";

            //Compute the result and put it back
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
