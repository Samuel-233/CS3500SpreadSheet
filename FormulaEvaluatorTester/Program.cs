using FormulaEvaluator;
using FormulaEvaluatorTester;


VariableStorage.AddVariable("a", 10);
Console.WriteLine(Evaluator.Evaluate("1+1", VariableStorage.LookUp));
Console.WriteLine(Evaluator.Evaluate("1 + (( 1    +2) *3)/4", VariableStorage.LookUp));
Console.WriteLine(Evaluator.Evaluate("1+(1+1)*9/3", VariableStorage.LookUp));
Console.WriteLine(Evaluator.Evaluate("a+(1+1)*9/3", VariableStorage.LookUp));
Console.ReadLine();