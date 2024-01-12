namespace FormulaEvaluator
{
    public class OperatorTracker
    {
        private Stack<int> frontBrace = new Stack<int>();
        private Stack<int> backBrace = new Stack<int>();
        private Stack<int> Operator1st = new Stack<int>();
        private Stack<int> Operator2nd = new Stack<int>();

        public OperatorTracker() { }  

        public OperatorTracker(String[] expression)
        {
            frontBrace = new Stack<int>();
            backBrace = new Stack<int>();
            Operator1st = new Stack<int>();
            Operator2nd = new Stack<int>();
            

            int index = 0;
            foreach (String token in expression)
            {
                if (token == "(") { frontBrace.Push(index); }
                else if (token == ")") { backBrace.Push(index); }
                else if(token == "*"|| token == "/"){ Operator1st.Push(index); }
                else if(token == "+"|| token =="-"){ Operator2nd.Push(index); }
                index++;
            }
            backBrace = new Stack<int>(backBrace);
        }

        public int GetFrontBrace()
        {
            return frontBrace.Pop();
        }

        public int GetBackBrace()
        {
            return backBrace.Pop();
        }

        public int PeekFrontBrace(){ return frontBrace.Peek(); }
        public int PeekBackBrace(){  return backBrace.Peek(); }

        public bool noBrace() { return frontBrace.Count == 0;}
    }
}