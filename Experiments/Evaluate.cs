using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

// https://www.codewars.com/kata/564d9ebde30917684f000048/train/csharp
// Lexer examples https://github.com/mauriciomoccelin/compiler
namespace Experiments
{
    // TODO: create different token hierarchies: op, func, paren, num, space
    // TODO: print console messages with underlining 
    public enum TokenType
    {
        None,
        LeftParen,
        RightParen,
        Num,
        Func,
        Plus = 10, //NOTE: first digit - priority
        Minus,
        Mult = 20,
        Div,
        Pow = 30,
    }

    public class Token
    {
        public Token(TokenType type, string value, int index) => (Type, Value, Index) = (type, value, index);
        public TokenType Type { get; }
        public string Value { get; }
        public int Index { get; }
        public int Lenght => Value.Length;
        public override string ToString() => $"<{Type} id[{Index}] '{Value}'>";
        public bool IsOperation() => Type >= TokenType.Plus;
        public bool HasLowerPriorityThen(Token token) => (int) Type / 10 < (int) token.Type / 10;
        public bool HasHigherPriorityThen(Token prevToken) => 
            Type == TokenType.Pow || (int) Type / 10 > (int) prevToken.Type / 10;
    }
    public class Evaluate
    {
        public readonly List<(string regexp, TokenType tokenType)> RegexpToToken = new()
        {
            ( /*language=regexp*/ @"\d+(\.?\d+)?([eE]-?\d+)?", TokenType.Num), // TODO: use factory methods
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
                .SelectMany(tpl => Regex.Matches(expr, tpl.regexp)
                                        .Select(match => new Token(tpl.tokenType, match.Value, match.Index)))
                .OrderBy(token => token.Index)
                .ThenByDescending(token => token.Lenght)
                .FilterAndValidate()
                .Where(token => token.Type != TokenType.None);

        public string Eval(string expr)
        {
            var tokens = Scan(expr);

            return EvalRec(tokens.GetEnumerator())?.Eval().ToString()
                   ?? throw new InvalidOperationException("Empty equation");

            static Expr EvalRec(
                IEnumerator<Token> enumerator, 
                Expr prevExpr = null,
                Token prevOp = null) //add previous unaccomplished operation
            {
                if (!enumerator.MoveNext())
                    return prevExpr;

                var token = enumerator.Current; // TODO: make like function or set like variable better throw experience (100 line)

                switch (token.Type)
                {
                    case TokenType.Num:
                        var number = new Number(token.Value);
                        
                        if (!enumerator.MoveNext() || enumerator.Current.Type == TokenType.LeftParen)
                            return number;

                        // TODO: use typed tokens, with Current as Operation
                        if (!enumerator.Current.IsOperation())
                            throw new InvalidOperationException($"Invalid token: {enumerator.Current}");

                        if (enumerator.Current.HasHigherPriorityThen(prevOp))
                        {
                            var expr = EvalRec(enumerator, number, enumerator.Current);

                            return new Binary(number, enumerator.Current.Type, expr);
                        }
                        else
                        {
                            var expr = new Binary(prevExpr, prevOp.Type, number);
                            
                            var nextExpr = EvalRec(enumerator, expr, enumerator.Current);

                            return new Binary(expr, enumerator.Current.Type, nextExpr);
                        }
                    case TokenType.Minus when prevExpr is null: // TODO: prlly coalesce with functions
                        return new Unary(EvalRec(enumerator), TokenType.Minus);
                    case TokenType.Func:
                    case TokenType.LeftParen:
                        string func = enumerator.Current.Type == TokenType.Func
                            ? enumerator.Current.Value
                            : "";

                        if (!enumerator.MoveNext())
                            throw new InvalidOperationException($"Invalid ending on: {token}");

                        if (enumerator.Current.Type != TokenType.LeftParen)
                            throw new InvalidOperationException($"Invalid function syntax: {enumerator.Current}");

                        return EvalRec(enumerator, new Grouping(EvalRec(enumerator), func), prevOp);
                    case TokenType.RightParen:
                        return prevExpr;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
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
        };

        public Grouping(Expr arg, string func = default) : base(arg)
        {
            Function = string.IsNullOrEmpty(func) 
                ? x => x 
                : Funcs[func];
        }
        private Func<double, double> Function { get; }

        public override double Eval() => Function(Arg.Eval());

        public static bool IsFuncExist(string func) => Funcs.ContainsKey(func);
    }

    // TODO: merge unary and grouping
    public class Unary : NTExpr
    {
        public Unary(Expr arg, TokenType opToken) : base(arg)
        {
            Function = opToken == TokenType.Minus
                ? x => -x
                : throw new NotSupportedException();
        }

        private Func<double, double> Function { get; }
        public override double Eval() => Function(Arg.Eval());
    }

    public class Binary : NTExpr
    {
        private static readonly Dictionary<TokenType, Func<double, double, double>> BinaryOps = new()
        {
            { TokenType.Plus, (x, y) => x + y },
            { TokenType.Minus, (x, y) => x - y },
            { TokenType.Mult, (x, y) => x * y },
            { TokenType.Div, (x, y) => x / y },
            { TokenType.Pow, Math.Pow },
        };

        public Binary(Expr arg, TokenType opToken, Expr arg2) : base(arg)
        {
            Function = BinaryOps[opToken];
            Arg2 = arg2;
        }

        private Expr Arg2 { get; }

        private Func<double, double, double> Function { get; }
        public override double Eval() => Function(Arg.Eval(), Arg2.Eval());
    }

    public static class Ext
    {
        public static IEnumerable<Token> FilterAndValidate(this IEnumerable<Token> tokens)
        {
            Token prev = null;

            foreach (var token in tokens)
            {
                var expectedIndex = prev?.Index + prev?.Lenght;

                if (token.Index > expectedIndex)
                {
                    throw new InvalidOperationException(
                        $"Invalid token: {token}");
                }

                if (token.Index < expectedIndex)
                    continue;

                if (token.Type == TokenType.Func && !Grouping.IsFuncExist(token.Value))
                {
                    throw new InvalidOperationException(
                        $"Invalid function: {token}");
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
