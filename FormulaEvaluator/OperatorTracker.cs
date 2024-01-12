﻿namespace FormulaEvaluator
{
/// <summary>
/// A class to track the position of every operator.
/// </summary>
    public class OperatorTracker
    {
        //two stack that record the operators that with different priority. 
        private Stack<int> Operator1st = new Stack<int>();
        private Stack<int> Operator2nd = new Stack<int>();

        public OperatorTracker() { }
        /// <summary>
        /// Track each operators's position, also separate the order: * / is earlier than + -
        /// </summary>
        /// <param name="expression">a list of tokens</param>
        /// <param name="start">start of tracking region</param>
        /// <param name="end">end of tracking region</param>
        public OperatorTracker(List<String> expression, int start, int end)
        {

            Operator1st = new Stack<int>();
            Operator2nd = new Stack<int>();



            for (int i = start + 1; i < end; i++)
            {
                String token = expression[i];
                if (token == "*" || token == "/") { Operator1st.Push(i); }
                else if (token == "+" || token == "-") { Operator2nd.Push(i); }
            }
        }

        /// <summary>
        /// Get and remove a Operator stack element
        /// </summary>
        /// <returns>the top of the stack (the index of the operator)</returns>
        public int GetOp1st() { return Operator1st.Pop(); }

        /// <summary>
        /// Get and remove a Operator stack element
        /// </summary>
        /// <returns>the top of the stack (the index of the operator)</returns>
        public int GetOp2nd() { return Operator2nd.Pop(); }

        /// <summary>
        /// To check if both stack is empty
        /// </summary>
        /// <returns>a bool</returns>
        public bool Empty() { return Op1Empty() && Op2Empty(); }

        /// <summary>
        /// To check if the 1st priority Operator Stack is empty
        /// </summary>
        /// <returns>a bool</returns>
        public bool Op1Empty() { return Operator1st.Count == 0; }

        /// <summary>
        /// To check if the 2nd priority Operator Stack is empty
        /// </summary>
        /// <returns>a bool</returns>
        public bool Op2Empty() { return Operator2nd.Count == 0; }
    }
}