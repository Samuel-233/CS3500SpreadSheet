using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormulaEvaluator
{
    public delegate double Cal(double x, double y);

    internal class DelicateTesting
    {
        static void Main() {
            Test(Add);
            Test(Dec);
        }
        
        public static double Add (double x, double y) { return x + y; }
        public static double Dec (double x, double y) { return x - y; }

        static void Test(Cal f)
        {
            double x = 1.0;
            double y = 2.0;
            Console.WriteLine(f(x, y));
        }
    }


}
