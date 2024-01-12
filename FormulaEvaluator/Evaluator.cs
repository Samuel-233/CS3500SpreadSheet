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
        
        static BraceTracker? brace = null;



        /// <summary>
        /// To evaluate the String as a expression, and get a int result.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="variableEvaluator"></param>
        /// <returns>a int as the result for the expression</returns>
        public static int Evaluate(String expression,
                                   Lookup variableEvaluator)
        {
            
            List<String> tokens = ProcessString.StringToTokens(
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
                    Calculate(operators.GetOp1st(), tokens, variableEvaluator);
                }

                //then,calculate the + and -
                while (!operators.Op2Empty())
                {
                    //use operators to locate other numbers and calculate.
                    Calculate(operators.GetOp2nd(), tokens, variableEvaluator);
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
        public static void Calculate(int operatorIndex, List<String> tokens, Lookup variableEvaluator)
        {
            String @operator = tokens[operatorIndex];

            //To loop to left until find a int or variable, then get it and remove it.
            int index = 0;
            while (tokens[operatorIndex - ++index] == " ") { }
            int left = 0;
            String variable = tokens[operatorIndex - index];
            if (!int.TryParse(variable, out left)){
                try
                {
                    left = variableEvaluator(variable);
                }
                catch
                {
                    throw new Exception($"no such variable {variable} in data");
                }
            }
            tokens[operatorIndex - index] = " ";

            //To loop to right until find a int or variable, then get it and remove it.
            index = 0;
            while (tokens[operatorIndex + ++index] == " ") { }
            int right = 0;
            variable = tokens[operatorIndex + index];
            if (!int.TryParse(variable, out right))
            {
                try
                {
                    right = variableEvaluator(variable);
                }
                catch
                {
                    throw new Exception($"no such variable {variable} in data");
                }
            }
            tokens[operatorIndex + index] = " ";

            //Compute the result and put it back
            int answer = 0;
            if (@operator.Equals("+")) { answer = left + right; }
            else if (@operator.Equals("-")) { answer = left - right; }
            else if (@operator.Equals("*")) { answer = left * right; }
            else if (@operator.Equals("/")) { answer = left / right; }

            tokens[operatorIndex] = answer.ToString();
        }


 




    }
}
