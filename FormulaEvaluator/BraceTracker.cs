namespace FormulaEvaluator
{
    /// <summary>
    /// Author:    Shu Chen
    /// Partner:   None
    /// Date:      2024/1/12
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
    /// A class to track the position of every brackets, so that the evaluator know it should do what first
    /// </summary>
    public class BraceTracker
    {
        private List<BraceIndex> brace = new List<BraceIndex>();

        public BraceTracker() { }

        /// <summary>
        /// Update the index of each brackets in to stack, most inner brackets is at the top of the stack
        /// </summary>
        /// <param name="expression">a list of token to compute</param>
        public BraceTracker(List<String> expression)
        {

            brace = new List<BraceIndex>();
            //need to add an extra bracket on the most outside, to make sure the Calculate() also compute the number outside the bracket
            brace.Add(new BraceIndex(-1, expression.Count));
            
            int index = 0;
            foreach (String token in expression)
            {
                if (token == "(") {brace.Add(new BraceIndex(index)); }
                else if (token == ")") {FeedBackBrace(index);}
                index++;
            }

        }

        public BraceIndex GetInnerBrace() {
            BraceIndex index = brace[brace.Count - 1];
            brace.RemoveAt(brace.Count - 1); 
            return index;
        }


        /// <summary>
        /// Check if there is no more braces in the stack.
        /// </summary>
        /// <returns>a boolean</returns>
        public bool NoBrace() { return brace.Count == 0; }

        /// <summary>
        /// Loop over the brace Index, find and fill in the back brace index
        /// </summary>
        /// <param name="backBraceIndex"></param>
        public void FeedBackBrace(int backBraceIndex)
        {
            int index = brace.Count;
            while(brace[--index].backBraceIndex!=-2){}
            brace[index].backBraceIndex = backBraceIndex;
        }


        /// <summary>
        /// A helper class to store two int at same time.
        /// </summary>
        public class BraceIndex
        {
            public int frontBraceIndex = -2;
            public int backBraceIndex = -2;
            public BraceIndex(int frontBrace)
            {
                this.frontBraceIndex = frontBrace;
            }

            public BraceIndex(int frontBrace, int backBrace) { 
                this.frontBraceIndex = frontBrace;
                this.backBraceIndex= backBrace;
            }

            /// <summary>
            /// Break down two int in to two separate int 
            /// </summary>
            /// <param name="braceIndex"></param>
            /// <param name="frontBraceIndex"></param>
            /// <param name="backBraceIndex"></param>
            public static void BraceIndexToInts(BraceIndex braceIndex, out int frontBraceIndex, out int backBraceIndex) {
                frontBraceIndex = braceIndex.frontBraceIndex;
                backBraceIndex = braceIndex.backBraceIndex;
            }

        }
    }


}