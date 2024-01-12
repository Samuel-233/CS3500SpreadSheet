namespace FormulaEvaluator
{
    /// <summary>
    /// A class to track the position of every brackets.
    /// </summary>
    public class BraceTracker
    {
        private Stack<int> frontBrace = new Stack<int>();
        private Stack<int> backBrace = new Stack<int>();

        public BraceTracker() { }

        /// <summary>
        /// Update the index of each brackets in to stack, most inner brackets is at the top of the stack
        /// </summary>
        /// <param name="expression">a list of token to compute</param>
        public BraceTracker(List<String> expression)
        {
            frontBrace = new Stack<int>();
            backBrace = new Stack<int>();
            //need to add an extra bracket on the most outside, to make sure the Calculate() also compute the number outside the bracket
            frontBrace.Push(-1);

            int index = 0;
            foreach (String token in expression)
            {
                if (token == "(") { frontBrace.Push(index); }
                else if (token == ")") { backBrace.Push(index); }
                index++;
            }
            backBrace.Push(expression.Count);
            backBrace = new Stack<int>(backBrace);
        }

        /// <summary>
        /// Getter method for Front Brace stack
        /// </summary>
        /// <returns>the most inner front brace's index</returns>
        public int GetFrontBrace(){return frontBrace.Pop();}

        /// <summary>
        /// Getter method for Back Brace stack
        /// </summary>
        /// <returns>the most inner back brace's index</returns>
        public int GetBackBrace(){return backBrace.Pop();}

        /// <summary>
        /// Peek function for Front Braces
        /// </summary>
        /// <returns>the index of the inner front brace</returns>
        public int PeekFrontBrace() { return frontBrace.Peek(); }

        /// <summary>
        /// Peek function for Back Braces
        /// </summary>
        /// <returns>the index of the inner back brace</returns>
        public int PeekBackBrace() { return backBrace.Peek(); }

        /// <summary>
        /// Check if there is no more braces in the stack.
        /// </summary>
        /// <returns>a boolean</returns>
        public bool NoBrace() { return frontBrace.Count == 0; }

    }
}