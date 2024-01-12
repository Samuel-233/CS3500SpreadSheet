namespace FormulaEvaluator{
/// <summary>
/// This is a class to Process Strings, break down it in to other forms that easier to process
/// </summary>
    public class ProcessString {
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