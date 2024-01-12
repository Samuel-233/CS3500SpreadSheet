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
        static OperatorTracker? @operator = null;

        public static int Evaluate(String expression,
                                   Lookup variableEvaluator)
        {
            // TODO...
            string[] tokens =
            Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            @operator = new OperatorTracker(tokens);
            return EvaluateRec(tokens, 0, tokens.Length-1,LookUp);

        }

        static public int EvaluateRec(String[] tokens, int start, int end,
                                   Lookup variableEvaluator)
        {
            //If there are brace in expression, deal with it first.
            if (!(@operator == null || @operator.noBrace())){
                EvaluateRec(tokens,@operator.GetFrontBrace(),@operator.GetBackBrace(),LookUp);
            }
            
            //Then deal with the Operator that has higher priority
            if()
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
        /// <returns>a new array contains no space tokens</returns>
        public static String[] checkTokenValid(String[] tokens)
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
            return newTokens.ToArray();
        }




    }
}
