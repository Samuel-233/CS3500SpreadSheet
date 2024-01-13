using FormulaEvaluator;
using FormulaEvaluatorTester;

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
/// A tester file to test the code and see if it run correctly.
/// </summary>
VariableStorage.AddVariable("a", 10);
VariableStorage.AddVariable("b", 1024);
Console.WriteLine((1+1)==Evaluator.Evaluate("1+1", VariableStorage.LookUp));
Console.WriteLine((1 + ((1 + 2) * 3) / 4) == Evaluator.Evaluate("1 + (( 1    +2) *3)/4", VariableStorage.LookUp));
Console.WriteLine((1 + (1 + 1) * 9 / 3)==Evaluator.Evaluate("1+(1+1)*9/3", VariableStorage.LookUp));
Console.WriteLine((10+(1+1)*9/3)==Evaluator.Evaluate("a+(1+1)*9/3", VariableStorage.LookUp));
Console.WriteLine((1024 * (10 + (1 / 1) * (9 / 3)) - 1024) == Evaluator.Evaluate("b*(a+(1/1)*(9/3))-b", VariableStorage.LookUp));
Console.WriteLine(((10 + (4) / 2) + (0)) == Evaluator.Evaluate("((10+(4)/2)+(0))", VariableStorage.LookUp));
Console.ReadLine();