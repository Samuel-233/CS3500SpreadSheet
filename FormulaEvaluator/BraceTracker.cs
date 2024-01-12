namespace FormulaEvaluator
{
    public class BraceTracker
    {
        private Stack<int> frontBrace = new Stack<int>();
        private Stack<int> backBrace = new Stack<int>();

        public BraceTracker() { }

        public BraceTracker(List<String> expression)
        {
            frontBrace = new Stack<int>();
            backBrace = new Stack<int>();
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

        public int GetFrontBrace()
        {
            return frontBrace.Pop();
        }

        public int GetBackBrace()
        {
            return backBrace.Pop();
        }

        public int PeekFrontBrace() { return frontBrace.Peek(); }
        public int PeekBackBrace() { return backBrace.Peek(); }

        public bool NoBrace() { return frontBrace.Count == 0; }

    }
}