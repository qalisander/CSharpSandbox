#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

// https://www.codewars.com/kata/564d9ebde30917684f000048/train/csharp
// Lexer examples https://github.com/mauriciomoccelin/compiler
namespace Experiments
{
    public enum ParenType
    {
        Left,
        Right,
    }

    public enum OpType
    {
        Plus,
        Minus,
        Mult,
        Div,
        Pow,
    }

    public class Token
    {
        protected Token(Match match) => (Value, Index) = (match.Value, match.Index);
        public string Value { get; }
        public int Index { get; }
        public string? InitialStr { get; set; }
        public int Length => Value.Length;
        public int LastIndex => Index + Length;

        public string HighlightedStr => string.IsNullOrEmpty(InitialStr)
            ? ""
            : HighlightArea(InitialStr, Index, Length);

        public override string ToString() => $"<{GetType().Name} id[{Index}] '{Value}'>";
        public string HighlightAfter(Token? token) =>
            HighlightArea(InitialStr ?? "", token?.LastIndex ?? 0, Index - (token?.LastIndex ?? 0));
        private static string HighlightArea(string expression, int index, int length) =>
            expression + "\n" + (new string(' ', index) + new string('^', length)).PadRight(expression.Length);
    }

    internal class Space : Token
    {
        private Space(Match match) : base(match) { }
        public static Space Create(Match match) => new Space(match);
    }

    internal class Paren : Token
    {
        private Paren(ParenType parenType, Match match) : base(match) => Type = parenType;
        public ParenType Type { get; }
        public static Func<Match, Paren> Create(ParenType type) => match => new Paren(type, match);
    }

    internal class Num : Token
    {
        private Num(Match match) : base(match) { }
        public static Num Create(Match match) => new Num(match);
    }

    public class Func : Token
    {
        private Func(Match match) : base(match) { }
        public static Func Create(Match match) => new Func(match);
    }

    public class Operation : Token
    {
        private Operation(OpType type, Match match) : base(match) => Type = type;
        public OpType Type { get; }
        public static Func<Match, Token> Create(OpType type) => match => new Operation(type, match);
    }

    public class Evaluate
    {
        public readonly List<(string regexp, Func<Match, Token> createToken)> RegexpToToken =
            new List<(string regexp, Func<Match, Token> createToken)>
            {
                ( /*language=regexp*/ @"\d+(\.?\d+)?([eE]-?\+?\d+)?", Num.Create),
                ( /*language=regexp*/ "[A-Za-z]+", Func.Create),
                ( /*language=regexp*/ @"\(", Paren.Create(ParenType.Left)),
                ( /*language=regexp*/ @"\)", Paren.Create(ParenType.Right)),
                ( /*language=regexp*/ "-", Operation.Create(OpType.Minus)),
                ( /*language=regexp*/ @"\+", Operation.Create(OpType.Plus)),
                ( /*language=regexp*/ @"\*", Operation.Create(OpType.Mult)),
                ( /*language=regexp*/ "/", Operation.Create(OpType.Div)),
                ( /*language=regexp*/ "&", Operation.Create(OpType.Pow)),
                ( /*language=regexp*/ @"\s+", Space.Create),
            };

        public IEnumerable<Token> Scan(string expr) =>
            RegexpToToken.SelectMany(pair => Regex.Matches(expr, pair.regexp).Select(pair.createToken))
                         .OrderBy(token => token.Index)
                         .ThenByDescending(token => token.Length)
                         .ProcessTokens(expr)
                         .Where(token => !(token is Space));

        public void Print(string expr) => Console.WriteLine(CreateExpr(Scan(expr).GetEnumerator()));

        public string? Eval(string expr)
        {
            try
            {
                return CreateExpr(Scan(expr).GetEnumerator())?.Eval() switch
                {
                    null => throw new InvalidTokenException("Empty equation"),
                    var dbl when !double.IsFinite(dbl.Value) => throw new InvalidTokenException("Is infinite"),
                    var eval => eval.ToString(),
                };
            }
            catch (InvalidTokenException)
            {
                // throw;
                return "ERROR";
            }
        }

        // expression     → term ;
        // term           → factor ( ( "-" | "+" ) factor )* ;
        // factor         → unary ( ( "/" | "*" ) unary )* ;
        // unary          → "-" unary | pow ;
        // pow            → primary "&" pow | primary;
        // primary        → func? "(" term ")" | number;
        private Expr CreateExpr(IEnumerator<Token> enumerator)
        {
            if (!enumerator.MoveNext())
                throw new InvalidTokenException();

            return Term() switch
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                var expr when enumerator.Current is null => expr,
                _ => throw new InvalidTokenException(),
            };

            Expr Term(Expr? prevExpr = null) =>
                (prevExpr ??= Factor()) is var expr
                && enumerator.Current is Operation op
                && (op.Type == OpType.Plus || op.Type == OpType.Minus)
                    ? enumerator.MoveNext()
                        ? Term(new Binary(expr, op, Factor()))
                        : throw new InvalidTokenException()
                    : expr;

            Expr Factor(Expr? prevExpr = null) =>
                (prevExpr ??= Unary()) is var expr
                && enumerator.Current is Operation op
                && (op.Type == OpType.Mult || op.Type == OpType.Div)
                    ? enumerator.MoveNext()
                        ? Factor(new Binary(expr, op, Unary()))
                        : throw new InvalidTokenException(op)
                    : expr;

            Expr Unary() =>
                enumerator.Current is Operation { Type: OpType.Minus }
                    ? enumerator.MoveNext()
                        ? new Unary(Unary(), "-")
                        : throw new InvalidTokenException()
                    : Pow();

            Expr Pow() =>
                Primary() is var expr
                && enumerator.MoveNext()
                && enumerator.Current is Operation { Type: OpType.Pow } op
                    ? enumerator.MoveNext()
                        ? new Binary(expr, op, Pow())
                        : throw new InvalidTokenException()
                    : expr;

            Expr Primary()
            {
                string funcStr = enumerator.Current switch
                {
                    Func func when enumerator.MoveNext() => func.Value,
                    _ => "",
                };

                return enumerator.Current switch
                {
                    Paren { Type: ParenType.Left } when enumerator.MoveNext() => Term() switch
                    {
                        var term when enumerator.Current is Paren { Type: ParenType.Right } =>
                            new Unary(term, funcStr),
                        _ => throw new InvalidTokenException(enumerator.Current),
                    },
                    Num num => new Number(num.Value),
                    _ => throw new InvalidTokenException(enumerator.Current),
                };
            }
        }
    }

    public abstract class Expr
    {
        public abstract double Eval();
    }

    public class Number : Expr
    {
        public Number(string number) => 
            Value = double.Parse(number.AsSpan(), NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint);

        private double Value { get; }
        public override double Eval() => Value;
        public override string ToString() => Value.ToString();
    }

    public abstract class NtExpr : Expr
    {
        protected NtExpr(Expr arg) => Arg = arg;
        protected Expr Arg { get; }
    }

    public class Unary : NtExpr
    {
        private static readonly Dictionary<string, Func<double, double>> Funcs =
            new Dictionary<string, Func<double, double>>(StringComparer.OrdinalIgnoreCase)
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

        public Unary(Expr arg, string func = "") : base(arg) => FunctionStr = func;
        
        private string FunctionStr { get; }

        public override double Eval() => Calculate(Arg.Eval());
        private double Calculate(double num) => FunctionStr switch
        {
            "" => num,
            "-" => -num,
            _ => Funcs[FunctionStr](num),
        };
        public static bool IsFuncExist(string func) => Funcs.ContainsKey(func);
        public override string ToString() => $" {FunctionStr}( {Arg} )";
    }

    public class Binary : NtExpr
    {
        private static readonly Dictionary<OpType, Func<double, double, double>> BinaryOps =
            new Dictionary<OpType, Func<double, double, double>>
            {
                { OpType.Plus, (x, y) => x + y },
                { OpType.Minus, (x, y) => x - y },
                { OpType.Mult, (x, y) => x * y },
                { OpType.Div, (x, y) => x / y },
                { OpType.Pow, Math.Pow },
            };

        public Binary(Expr arg, Operation op, Expr arg2) : base(arg)
        {
            OpStr = op.Value;
            Function = BinaryOps[op.Type];
            Arg2 = arg2;
        }

        private Expr Arg2 { get; }
        private string OpStr { get; }
        private Func<double, double, double> Function { get; }

        public override double Eval() => Function(Arg.Eval(), Arg2.Eval());
        public override string ToString() => $"[ {Arg} {OpStr} {Arg2} ]";
    }

    public static class Ext
    {
        public static IEnumerable<Token> ProcessTokens(this IEnumerable<Token> tokens, string initialString)
        {
            Token? prev = null;

            foreach (var token in tokens)
            {
                token.InitialStr = initialString;

                var expectedIndex = prev?.Index + prev?.Length;

                if (token.Index > expectedIndex)
                    throw new InvalidTokenException(prev, token);

                if (token.Index < expectedIndex)
                    continue;

                if (token is Func && !Unary.IsFuncExist(token.Value))
                    throw new InvalidTokenException(token);

                yield return prev = token;
            }
        }
    }

    public class InvalidTokenException : Exception
    {
        public InvalidTokenException(string message) : base(message) { }
        public InvalidTokenException(Token? token = null) : base(InvalidTokenStr(token)) { }
        public InvalidTokenException(Token? token1, Token token2)
            : base($"Invalid token!\n{token2.HighlightAfter(token1)}")
        {
        }
        private static string InvalidTokenStr(Token? token) => token is null
            ? "Invalid expression's ending"
            : $"Invalid token: {token}\n{token.HighlightedStr}";
    }
}
