using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace dotnet {
    class Program {

        static IEnumerable<double> Nums(long n, double x) {
            var exp = 2;
            while (exp <= n) {
                yield return Math.Pow(-1.0, exp) * Math.Pow(x, exp) / exp;
                exp += 1;
            }
        }

        static void Main(string[] args) {
            Console.Write("x: ");
            var x = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine();

            Console.Write("n: ");
            var n = Convert.ToInt64(Console.ReadLine());
            Console.WriteLine();

            var sw = new Stopwatch();

            sw.Start();
            var result = x + Nums(n, x).Sum();
            sw.Stop();

            Console.WriteLine($"Result: {result}");
            Console.WriteLine($"Elapsed time: {sw.ElapsedMilliseconds}");
        }
    }
}
