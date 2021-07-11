using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

// https://www.codewars.com/kata/564d9ebde30917684f000048/train/csharp
// Lexer examples https://github.com/mauriciomoccelin/compiler
namespace Experiments
{
    public enum TokenType
    {
        LeftParen,
        RightParen,
        Func,
        Mult,
        Div,
        Pow,
        Minus,
        Plus,
        Num,
        None,
    }

    public class Token
    {
        public Token(TokenType type, string value, int index)
        {
            (Type, Value, Index) = (type, value, index);
        }
        public TokenType Type { get; }
        public string Value { get; }
        public int Index { get; }
        public int Lenght => Value.Length;
        public override string ToString() => $"<{Type} '{Value}'>";
    }
    public class Evaluate
    {
        public readonly List<(string regexp, TokenType tokenType)> RegexpToToken = new()
        {
            ( /*language=regexp*/ @"\d+(\.?\d+)?([eE]-?\d+)?", TokenType.Num),
            ( /*language=regexp*/ "[A-Za-z]+", TokenType.Func),
            ( /*language=regexp*/ @"\(", TokenType.LeftParen),
            ( /*language=regexp*/ @"\)", TokenType.RightParen),
            ( /*language=regexp*/ "-", TokenType.Minus),
            ( /*language=regexp*/ @"\+", TokenType.Plus),
            ( /*language=regexp*/ @"\*", TokenType.Mult),
            ( /*language=regexp*/ "/", TokenType.Div),
            ( /*language=regexp*/ "&", TokenType.Pow),
            ( /*language=regexp*/ @"\s+", TokenType.None),
        };

        // https://docs.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-options
        // https://en.wikipedia.org/wiki/Lexical_grammar
        // https://en.wikipedia.org/wiki/Lexical_analysis
        public IEnumerable<Token> Scan(string expr) =>
            RegexpToToken
                .SelectMany(tpl =>
                    Regex.Matches(expr, tpl.regexp)
                         .Select(match => new Token(tpl.tokenType, match.Value, match.Index)))
                .OrderBy(token => token.Index)
                .ThenByDescending(token => token.Lenght)
                .FilterAndValidate()
                .Where(token => token.Type != TokenType.None);

        public string Eval(string expr)
        {
            var tokens = Scan(expr);

            var enumerator = tokens.GetEnumerator();

            if (enumerator.MoveNext())
                return EvalRec(enumerator).Eval().ToString();

            throw new InvalidOperationException("Empty equation");

            static Expr EvalRec(IEnumerator<Token> enumerator)
            {
                var current = enumerator.Current;

                switch (current.Type)
                {
                    case TokenType.Num:
                        if (!enumerator.MoveNext())
                            throw new NotImplementedException("Create num");

                        switch (enumerator.Current.Type)
                        {
                            case TokenType.Div:
                                break;
                            case TokenType.Pow:
                                break;
                            case TokenType.Minus:
                                break;
                            case TokenType.Plus:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;
                    case TokenType.LeftParen:
                        break;
                    case TokenType.RightParen:
                        break;
                    case TokenType.Func:
                        break;
                    case TokenType.Mult:
                        break;
                    case TokenType.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                throw new NotImplementedException();
            }
        }
    }

    public abstract class Expr
    {
        public abstract double Eval();
    }

    public class Number : Expr
    {
        public Number(string number)
        {
            Value = double.Parse(number.AsSpan(), NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint);
        }

        private double Value { get; }

        public override double Eval() => Value;
    }

    public abstract class NTExpr : Expr
    {
        protected NTExpr(Expr arg)
        {
            Arg = arg;
        }
        protected Expr Arg { get; }
    }

    public class Grouping : NTExpr
    {
        private static readonly Dictionary<string, Func<double, double>> Funcs = new(StringComparer.OrdinalIgnoreCase)
        {
            { "sin", Math.Sin },
            { "sinh", Math.Sinh },
            { "asin", Math.Asin },
            { "cos", Math.Cos },
            { "cosh", Math.Cosh },
            { "acos", Math.Acos },
            { "tanh", Math.Tanh },
            { "tan", Math.Tan },
            { "atan", Math.Atan },
            { "exp", Math.Exp },
            { "ln", Math.Log },
            { "log", Math.Log10 },
            { "sqrt", Math.Sqrt },
            { "abs", Math.Abs },
            { "", x => x },
        };

        public Grouping(Expr arg, string func = "") : base(arg)
        {
            Function = Funcs[func];
        }
        public Func<double, double> Function { get; }

        public override double Eval() => Function(Arg.Eval());

        public static bool IsFuncExist(string func) => Funcs.ContainsKey(func);
    }

    public class Unary : NTExpr
    {
        private static readonly Dictionary<string, Func<double, double>> UnaryOps = new()
        {
            { "-", x => -x },
        };

        public Unary(Expr arg, string op) : base(arg)
        {
            Function = UnaryOps[op];
        }

        public Func<double, double> Function { get; set; }
        public override double Eval() => Function(Arg.Eval());
    }

    public class Binary : NTExpr
    {
        private static readonly Dictionary<string, Func<double, double, double>> BinaryOps = new()
        {
            { "+", (x, y) => x + y },
            { "*", (x, y) => x * y },
            { "&", Math.Pow },
            { "-", (x, y) => x - y },
        };

        public Binary(Expr arg, string op, Expr arg2) : base(arg)
        {
            Function = BinaryOps[op];
            Arg2 = arg2;
        }

        public Expr Arg2 { get; set; }
        public Func<double, double, double> Function { get; set; }
        public override double Eval() => Function(Arg.Eval(), Arg2.Eval());
    }

    public static class Ext
    {
        public static int? Priority(this TokenType tokenType) => tokenType switch
        {
            TokenType.Pow => 3,
            TokenType.Mult => 2,
            TokenType.Div => 2,
            TokenType.Minus => 1,
            TokenType.Plus => 1,
            _ => null,
        };

        public static IEnumerable<Token> FilterAndValidate(this IEnumerable<Token> tokens)
        {
            Token prev = null;

            foreach (var token in tokens)
            {
                var expectedIndex = prev?.Index + prev?.Lenght;

                if (token.Index > expectedIndex)
                {
                    throw new InvalidOperationException(
                        $"Can not recognize token between [{expectedIndex}] and [{token.Index}] indexes");
                }

                if (token.Index < expectedIndex)
                    continue;

                if (token.Type == TokenType.Func && !Grouping.IsFuncExist(token.Value))
                {
                    throw new InvalidOperationException(
                        $"Function at index: [{token.Index}] doesn't exist");
                }

                prev = token;

                yield return token;
            }
        }
    }

    // public static IEnumerable<string> SplitInclude(IEnumerable<string> separators, string str)
    // {
    //     var divisionPoints = new SortedSet<int>
    //     {
    //         str.Length,
    //     };
    //
    //     foreach (var spr in separators.Append(" "))
    //     {
    //         for (var i = 0; i + spr.Length < str.Length; i++)
    //         {
    //             if (string.Compare(spr, 0, str, i, spr.Length) == 0)
    //             {
    //                 divisionPoints.Add(i);
    //                 divisionPoints.Add(i + spr.Length);
    //             }
    //         }
    //     }
    //
    //     var previous = 0;
    //
    //     foreach (var divisionPoint in divisionPoints)
    //     {
    //         var substr = str.Substring(previous, divisionPoint - previous);
    //
    //         if (!string.IsNullOrWhiteSpace(substr))
    //             yield return substr;
    //
    //         previous = divisionPoint;
    //     }
    // }
}
