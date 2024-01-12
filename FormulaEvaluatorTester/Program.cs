using FormulaEvaluator;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
Console.WriteLine(Evaluator.Evaluate("1+1",Evaluator.LookUp));
String[] strings = Evaluator.checkTokenValid(Regex.Split("1 + (( 1    +2) *3)/4", "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)"));
OperatorTracker brace = new OperatorTracker(strings);
Console.WriteLine(brace.GetFrontBrace());
foreach (String s in strings) { 
    Console.Write(s);
}
