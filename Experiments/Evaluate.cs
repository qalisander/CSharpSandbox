#define MYTEST
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
        //NOTE: first digit - priority
        Plus = 10,
        Minus,
        Mult = 20,
        Div,
        Pow = 30,
    }

    public class Token
    {
        protected Token(Match match) => (Value, Index) = (match.Value, match.Index);
        public string Value { get; }
        public int Index { get; }
        public int Lenght => Value.Length;
        public override string ToString() => $"<{GetType().Name} id[{Index}] '{Value}'>";
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
        public static Func<Match, Paren> Create(ParenType type) =>
            match => new Paren(type, match);
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

        public bool HasHigherPriorityThen(Operation prevOp) =>
            Type == OpType.Pow
            || prevOp is null //TODO: prlly remove
            || (int) Type / 10 > (int) prevOp.Type / 10;

        public static Func<Match, Token> Create(OpType type) =>
            match => new Operation(type, match);
    }

    public class Evaluate
    {
        public readonly List<(string regexp, Func<Match, Token> createToken)> RegexpToToken = new()
        {
            ( /*language=regexp*/ @"\d+(\.?\d+)?([eE]-?\d+)?", Num.Create),
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

        // https://docs.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-options
        // https://en.wikipedia.org/wiki/Lexical_grammar
        // https://en.wikipedia.org/wiki/Lexical_analysis
        public IEnumerable<Token> Scan(string expr) =>
            RegexpToToken.SelectMany(tuple => Regex.Matches(expr, tuple.regexp).Select(tuple.createToken))
                         .OrderBy(token => token.Index)
                         .ThenByDescending(token => token.Lenght)
                         .FilterAndValidate() // TODO: string expr pass as arg here
                         .Where(token => !(token is Space));

        public void Print(string expr) => Console.WriteLine(EvalRec(Scan(expr).GetEnumerator()));

        public string Eval(string expr)
        {
            try
            {
                return EvalRec(Scan(expr).GetEnumerator())?.Eval().ToString()
                       ?? throw new InvalidOperationException("Empty equation");
            }
            catch (Exception)
            {
#if DEBUG && MYTEST
                throw;
#endif
                return "ERROR";
            }
        }

        private Expr EvalRec(
            IEnumerator<Token> enumerator,
            Expr prevExpr = null,
            Operation prevOp = null) //previous unaccomplished operation
        {
            if (!enumerator.MoveNext())
                return prevExpr;

            var token = enumerator.Current;

            switch (token)
            {
                case Num:
                    var number = new Number(token.Value);

                    return CreateOp(number);
                case Operation { Type: OpType.Minus }: // TODO: prlly coalesce with functions
                    return new Unary(EvalRec(enumerator));
                case Func:
                case Paren { Type: ParenType.Left}:
                    string func = token is Func
                        ? enumerator.MoveNext()
                            ? token.Value
                            : throw new InvalidTokenException(token)
                        : ""; // TODO: add minus, prlly use switch

                    if (!(enumerator.Current is Paren { Type: ParenType.Left }))
                        throw new InvalidTokenException(enumerator.Current);

                    var grouping = new Grouping(EvalRec(enumerator), func);

                    return CreateOp(grouping);
                case Paren { Type: ParenType.Right}:
                    return prevExpr;
                default:
                    throw new InvalidTokenException(token);
            }

            Expr CreateOp(Expr? ex)
            {
                if (!enumerator.MoveNext() || enumerator.Current is Paren { Type: ParenType.Right})
                    return ex;

                if (!(enumerator.Current is Operation operation))
                    throw new InvalidTokenException(enumerator.Current);

                var expr = operation.HasHigherPriorityThen(prevOp)
                    ? ex
                    : new Binary(prevExpr, prevOp, ex);

                var nextExpr = EvalRec(enumerator, expr, operation);

                return new Binary(expr, operation, nextExpr);
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
        public override string ToString() => Value.ToString();
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
            FunctionStr = func;

            Function = string.IsNullOrEmpty(func)
                ? x => x
                : Funcs[func];
        }
        private Func<double, double> Function { get; }
        private string FunctionStr { get; }

        public override double Eval() => Function(Arg.Eval());

        public static bool IsFuncExist(string func) => Funcs.ContainsKey(func);
        public override string ToString() => $" {FunctionStr}( {Arg} )";
    }

    // TODO: merge unary and grouping
    public class Unary : NTExpr
    {
        public Unary(Expr arg) : base(arg)
        {
            Function = x => -x;
        }

        private Func<double, double> Function { get; }
        public override double Eval() => Function(Arg.Eval());
        public override string ToString() => $"-{Arg}";
    }

    public class Binary : NTExpr
    {
        private static readonly Dictionary<OpType, Func<double, double, double>> BinaryOps = new()
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
        public static IEnumerable<Token> FilterAndValidate(this IEnumerable<Token> tokens)
        {
            Token prev = null;

            foreach (var token in tokens)
            {
                var expectedIndex = prev?.Index + prev?.Lenght;

                if (token.Index > expectedIndex)
                    throw new InvalidTokenException(token);

                if (token.Index < expectedIndex)
                    continue;

                if (token is Func && !Grouping.IsFuncExist(token.Value))
                    throw new InvalidTokenException(token);

                yield return prev = token;
            }
        }
    }

    public class InvalidTokenException : Exception
    {
        public InvalidTokenException(Token token) : base($"Invalid token: {token}") { }

        //TODO: Invalid string between tokens exception, token has context of string
    }
}
