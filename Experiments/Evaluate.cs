using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Security.Cryptography;

namespace Experiments
{
    // https://www.codewars.com/kata/564d9ebde30917684f000048/train/csharp
    public class Evaluate
    {
        // delegate double Function(params double[] args);
        //
        // private static Dictionary<string, Function> Operations = new()
        // {
        //     { "sin", args => Math.Sin(args[0]) },
        //     { "tan", x => Math.Tan(x[0]) },
        //     { "exp", x => Math.Exp(x[0]) },
        //     { "-", x => -x[0] },
        // };
        //
        // private static Dictionary<string, Func<double[], double>> Operations = new()
        // {
        //     { "sin", x => Math.Sin(x[0]) },
        //     { "tan", x => Math.Tan(x[0]) },
        //     { "exp", x => Math.Exp(x[0]) },
        //     { "-", x => -x[0] },
        // };

        // Expressions are graph and I need sort it. Topological sorting
        abstract class Expr
        {
            public abstract double Eval();

            public abstract Expr Create(IEnumerator<string> enumerator);
        }

        private class Number : Expr
        {
            private double Value { get; set; }

            public override double Eval() => Value;
            public override Expr Create(IEnumerator<string> enumerator) => throw new NotImplementedException();

            // TODO: when number expression is creating check next expression
        }

        private abstract class NoneTerminalExpr : Expr
        {
            protected Expr Arg { get; set; }
        }

        private class Brackets : NoneTerminalExpr
        {
            public override double Eval() => Arg.Eval();
            public override Expr Create(IEnumerator<string> enumerator) => throw new NotImplementedException();
        }

        private class OneArgFunc : NoneTerminalExpr
        {
            private static Dictionary<string, Func<double, double>> Operations = new()
            {
                { "sin", x => Math.Sin(x) },
                { "tan", x => Math.Tan(x) },
                { "exp", x => Math.Exp(x) },
                { "-", x => -x },
            };

            public Func<double, double> Function { get; set; }
            public override double Eval() => Function(Arg.Eval());
            public override Expr Create(IEnumerator<string> enumerator) => throw new NotImplementedException();
        }

        private class TwoArgFunc : NoneTerminalExpr
        {
            private static Dictionary<string, Func<double, double, double>> Operations = new()
            {
                { "+", (x, y) => x + y },
                { "*", (x, y) => x * y },
                { "&", (x, y) => Math.Pow(x, y) },
            };

            public Expr Arg2 { get; set; }
            public Func<double, double, double> Function { get; set; }
            public override double Eval() => Function(Arg.Eval(), Arg2.Eval());
            public override Expr Create(IEnumerator<string> enumerator) => throw new NotImplementedException();
        }

        // public IEnumerable<string> ParseExpression(string expression)
        // {
        //     for (int start = 0, len = 1; start < expression.Length;)
        //     {
        //         var span = expression.AsSpan(start, len);
        //         
        //     }
        // }

        public string Eval(string expression)
        {

            //calculated expression (double converted to string) or Errormessage starting with "ERROR" (+ optional Errormessage)
            string result = "0";

            return result;
        }

        public static IEnumerable<string> SplitInclude(IEnumerable<string> separators, string str)
        {
            var divisionPoints = new SortedSet<int>(){str.Length};

            foreach (var spr in separators.Append(" "))
            {
                for (var i = 0; i + spr.Length < str.Length; i++)
                {
                    if (string.Compare(spr, 0, str, i, spr.Length) == 0)
                    {
                        divisionPoints.Add(i);
                        divisionPoints.Add(i + spr.Length);
                    }
                }
            }

            var previous = 0;
            foreach (var divisionPoint in divisionPoints)
            {
                var substr = str.Substring(previous, divisionPoint - previous);

                if (!string.IsNullOrWhiteSpace(substr))
                    yield return substr;

                previous = divisionPoint;
            }
        }
    }
}
