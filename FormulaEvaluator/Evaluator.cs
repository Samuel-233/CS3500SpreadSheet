using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    /// <summary>
    /// Author:    Shu Chen
    /// Partner:   None
    /// Date:      2024/1/14
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

                BraceTracker.BraceIndex.BraceIndexToInts(brace.GetInnerBrace(),out int start,out int end);  

                if (start != -1 && end != tokens.Count)
                {
                    tokens[end] = " ";
                    tokens[start] = " ";
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

            int answer = 0;
            //Find the last element left in the token, which will be the result.
            foreach (String token in tokens) {
                if (token.Equals(" ")) { continue; }
                if (!int.TryParse(token, out answer))
                {
                   answer = EvalVariable(variableEvaluator, token);
                }
            }

            return answer;


        }

        /// <summary>
        /// A method to evaluate expression that already remove the bracket
        /// </summary>
        /// <param name="operatorIndex">the position of the operator</param>
        /// <param name="tokens">a list of tokens that store the expression</param>
        /// <exception cref="Exception">throw error if variable don't exist</exception>
        private static void Calculate(int operatorIndex, List<String> tokens, Lookup variableEvaluator)
        {
            String @operator = tokens[operatorIndex];

            //To loop to left until find a int or variable, then get it and remove it.
            int index = 0;
            while (tokens[operatorIndex - ++index] == " ") { }
            int left = 0;
            String variable = tokens[operatorIndex - index];
            if (!int.TryParse(variable, out left)){
                left = EvalVariable(variableEvaluator, variable);
            }
            tokens[operatorIndex - index] = " ";

            //To loop to right until find a int or variable, then get it and remove it.
            index = 0;
            while (tokens[operatorIndex + ++index] == " ") { }
            int right = 0;
            variable = tokens[operatorIndex + index];
            if (!int.TryParse(variable, out right))
            {
                right = EvalVariable(variableEvaluator, variable);
            }
            tokens[operatorIndex + index] = " ";

            //Compute the result and put it back
            int answer = 0;
            if (@operator.Equals("+")) { answer = left + right; }
            else if (@operator.Equals("-")) { answer = left - right; }
            else if (@operator.Equals("*")) { answer = left * right; }
            else if (@operator.Equals("/")) {
                if (right == 0) throw new InvalidDataException("Can not divide by zeor");
                answer = left / right; 
            }

            tokens[operatorIndex] = answer.ToString();
        }


        /// <summary>
        /// Check if the look up function is null before evaluate the variable.
        /// </summary>
        /// <param name="lookup">lookup function</param>
        /// <param name="variable">variable name</param>
        /// <returns>the value of variable</returns>
        /// <exception cref="Exception">if lookup is null</exception>
        private static int EvalVariable(Lookup lookup,String variable) {
            if(lookup == null) { throw new Exception("please provide a look up function"); }
            return lookup(variable);
        }




    }
}
