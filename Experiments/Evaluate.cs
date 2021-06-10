using System;
using System.Collections.Generic;
using System.Linq;

namespace Experiments
{
    // https://www.codewars.com/kata/564d9ebde30917684f000048/train/csharp
    public class Evaluate
    {
        abstract class Expr
        {
            public abstract double Eval();

            public abstract Expr Create(IEnumerator<string> enumerator);
        }

        private class Number : Expr
        {
            private double Value { get; set; }

            public override double Eval() => Value;
        }

        private abstract class NoneTerminalExpr : Expr
        {
            protected Expr Arg { get; set; }
        }

        private class Brackets : NoneTerminalExpr
        {
            public override double Eval() => Arg.Eval();
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
        }


        public string eval(string expression)
        {
            var strings = expression.Split(new [] {"sin", "tan", "+"}, StringSplitOptions.RemoveEmptyEntries).ToList();

            using IEnumerator<string> enumerator = strings.GetEnumerator();

            //calculated expression (double converted to string) or Errormessage starting with "ERROR" (+ optional Errormessage)
            string result = "0";

            return result;
        }
    }
}
