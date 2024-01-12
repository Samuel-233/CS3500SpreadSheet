using FormulaEvaluator;
Console.WriteLine(Evaluator.Evaluate("1+1", Evaluator.LookUp));
Console.WriteLine(Evaluator.Evaluate("1 + (( 1    +2) *3)/4", Evaluator.LookUp));
Console.WriteLine(Evaluator.Evaluate("1+(1+1)*9/3", Evaluator.LookUp));
Console.WriteLine(Evaluator.Evaluate("a+(1+1)*9/3", Evaluator.LookUp));