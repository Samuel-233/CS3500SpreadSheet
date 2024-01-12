namespace FormulaEvaluator
{
    public class OperatorTracker
    {
        private Stack<int> Operator1st = new Stack<int>();
        private Stack<int> Operator2nd = new Stack<int>();

        public OperatorTracker() { }

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

        public int GetOp1st() { return Operator1st.Pop(); }
        public int GetOp2nd() { return Operator2nd.Pop(); }

        public bool Empty() { return Op1Empty() && Op2Empty(); }
        public bool Op1Empty() { return Operator1st.Count == 0; }
        public bool Op2Empty() { return Operator2nd.Count == 0; }
    }
}