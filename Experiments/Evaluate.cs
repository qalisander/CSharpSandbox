using System;
using System.Collections.Generic;
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
        public List<(string regexp, TokenType tokenType)> RegexpToToken = new()
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

        public IEnumerable<Token> Scan(string expr)
        {
            // https://docs.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-options
            //https://en.wikipedia.org/wiki/Lexical_grammar
            // https://en.wikipedia.org/wiki/Lexical_analysis
            //MatchCollection mc = Regex.Matches("text", @"\bm\S*e\b");

            // NOTE: https://stackoverflow.com/questions/6219454/efficient-way-to-remove-all-whitespace-from-string
            // TODO: prlly remove, there is not need in this check with function validation
            if (Regex.Matches(expr, @"(?<=[A-Za-z])\s+(?=[A-Za-z])").Any())
            {
                throw new InvalidOperationException(
                    $"Not supported space symbol on index: {Regex.Matches(expr, @"(?<=[A-Za-z])\s+(?=[A-Za-z])").First().Index}");
            }

            return RegexpToToken
                   .SelectMany(tpl => 
                       Regex.Matches(expr, tpl.regexp).Select(match => new Token(tpl.tokenType, match.Value, match.Index)))
                   .OrderBy(token => token.Index)
                   .ThenByDescending(token => token.Lenght)
                   .RemoveIntercepted()
                   .Where(token => token.Type != TokenType.None);
        }

        public string Eval(string expr)
        {
            var tokens = Scan(expr);

            var enumerator = tokens.GetEnumerator();
            if (enumerator.MoveNext())
            {
                return EvalRec(enumerator).Eval().ToString();
            }

            throw new InvalidOperationException("Empty equation");
            
            static Expr EvalRec(IEnumerator<Token> enumerator)
            {
                var current = enumerator.Current;

                switch (current.Type)
                {
                    case TokenType.Num:
                        if (!enumerator.MoveNext())
                            throw NotImplementedException("Create num");
                        
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
            }
        }

        public abstract class Expr
        {
            public int Index { get; set; }

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
            // TODO: prefix and funcion

            public override double Eval() => Arg.Eval();
            public override Expr Create(IEnumerator<string> enumerator) => throw new NotImplementedException();
        }

        private class OneArgFunc : NoneTerminalExpr
        {
            // Use as switch, we don't need to enumerate operations
            private static Dictionary<string, Func<double, double>> Operations = new()
            {
                { "sin", x => Math.Sin(x) },
                { "sinh", x => Math.Sinh(x) },
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
        
        // TODO: rename to FilterAndValidate, check function existence
        public static IEnumerable<Token> RemoveIntercepted(this IEnumerable<Token> tokens)
        {
            Token prev = null;
            foreach (var token in tokens)
            {
                var expectedIndex = prev?.Index + prev?.Lenght;

                if (token.Index > expectedIndex)
                    throw new InvalidOperationException(
                        $"Can not recognize token between [{expectedIndex}] and [{token.Index}] indexes");

                if (token.Index < expectedIndex)
                    continue;

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
    // Prblly create mapping string -> token

    // Enum of tokens

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
}
