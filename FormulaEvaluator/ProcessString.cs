namespace FormulaEvaluator
{
    /// <summary>
    /// Author:    Shu Chen
    /// Partner:   None
    /// Date:      2024/1/11
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
    /// This is a class to Process Strings, break down it in to other forms that easier to process
    /// </summary>
    public class ProcessString
    {
        /// <summary>
        /// A helper method to remove all white space
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns>a new List contains no space tokens</returns>
        public static List<String> StringToTokens(String[] tokens)
        {
            List<String> newTokens = new List<string>();
            char space = ' ';
            foreach (String token in tokens)
            {
                int frontIndex = 0;
                int backIndex = token.Length - 1;
                bool haveSpace = false;
                //If token is empty or just one space, ignore that
                if (token.Length == 0 || (token.Length == 1 && token[0] == space))
                {
                    continue;
                }
                //remove spaces before the useful data
                while (frontIndex < token.Length - 1 && token[frontIndex] == space)
                {
                    haveSpace = true;
                    frontIndex++;
                }
                //remove spaces behind the useful data
                while (backIndex > 0 && token[backIndex] == space)
                {
                    haveSpace = true;
                    backIndex--;
                }
                //if there is a space and not every char of token is space, add this token to list
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