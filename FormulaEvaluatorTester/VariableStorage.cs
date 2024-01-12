
namespace FormulaEvaluatorTester
{
    public class VariableStorage
    {
        static Dictionary<String, int> variables = new Dictionary<string, int>();
        public static int LookUp(String variableName)
        {
            int result;
            variables.TryGetValue(variableName, out result);
            return result;
        }


        public static void AddVariable(String variable_name, int value)
        {
            variables.Add(variable_name, value);
        }

        public static void RemoveVariable(String variable_name)
        {
            variables.Remove(variable_name);
        }
    }
}