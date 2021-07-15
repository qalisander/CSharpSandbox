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
        public Token(TokenType type, string value, int index)
        {
            (Type, Value, Index) = (type, value, index);
        }
        public Token(string value, int index)
        {
            (Type, Value, Index) = (TokenType.None, value, index);
        }
        public TokenType Type { get; }
        public string Value { get; }
        public int Index { get; }
        public int Lenght => Value.Length;
        public override string ToString() => $"<{Type} id[{Index}] '{Value}'>";
        public bool IsOperation() => Type >= TokenType.Plus;
        public bool HasHigherPriorityThen(Token prevToken) =>
            Type == TokenType.Pow
            || prevToken is null
            || (int) Type / 10 > (int) prevToken.Type / 10;
    }
    class Paren : Token
    {
        public enum ParenType
        {
            Left,
            Right,
        }

        public Paren(ParenType parenType, string value, int index) : base(value, index) => Type = parenType;
        public ParenType Type { get; }
        public Paren CreateLeft(Match match) => new Paren(ParenType.Left, match.Value, match.Index);
    }
    class Num : Token
    {
        public Num(string value, int index) : base(value, index) {}
        public Num CreateNum(Match match) => new Num(match.Value, match.Index);
    }

    public class Func : Token
    {
        public Func(string value, int index) : base(value, index){}
        public Func CreateFunc(Match match) => new Func(match.Value, match.Index);
    }
    
    public class Operation : Token
    {
        public enum OpType
        {
            Plus = 10, //NOTE: first digit - priority
            Minus,
            Mult = 20,
            Div,
            Pow = 30,
        }
        private Operation(OpType type, string value, int index) : base(value, index) => this.Type = type;
        public OpType Type { get; }
        
        public Operation CreatePlus(Match match) => new Operation(OpType.Plus, match.Value, match.Index);
        public Operation CreateMult(Match match) => new Operation(OpType.Mult, match.Value, match.Index);
        public Operation CreateMinus(Match match) => new Operation(OpType.Minus, match.Value, match.Index);
        public Operation CreateDiv(Match match) => new Operation(OpType.Div, match.Value, match.Index);
        public Operation CreatePow(Match match) => new Operation(OpType.Pow, match.Value, match.Index);
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
                .FilterAndValidate() // TODO: string expr pass as arg here
                .Where(token => token.Type != TokenType.None);

        public void Print(string expr)
        {
            Console.WriteLine(EvalRec(Scan(expr).GetEnumerator()));
        }
        
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
            Token prevOp = null) //add previous unaccomplished operation
        {
            if (!enumerator.MoveNext())
                return prevExpr;

            var token = enumerator.Current;

            switch (token.Type)
            {
                case TokenType.Num:
                    var number = new Number(token.Value);

                    return CreateOp(number);
                case TokenType.Minus: // TODO: prlly coalesce with functions
                    return new Unary(EvalRec(enumerator), TokenType.Minus);
                case TokenType.Func:
                case TokenType.LeftParen:
                    string func = token.Type == TokenType.Func
                        ? enumerator.MoveNext()
                            ? token.Value
                            : throw new InvalidTokenException(token)
                        : ""; // TODO: add minus, prlly use switch

                    if (enumerator.Current.Type != TokenType.LeftParen)
                        throw new InvalidTokenException(enumerator.Current);

                    var grouping = new Grouping(EvalRec(enumerator), func);

                    return CreateOp(grouping);
                case TokenType.RightParen:
                    return prevExpr;
                default:
                    throw new InvalidTokenException(token);
            }

            Expr CreateOp(Expr? ex)
            {
                if (!enumerator.MoveNext() || enumerator.Current.Type == TokenType.RightParen)
                    return ex;

                // TODO: use typed tokens, with Current as Operation
                if (!enumerator.Current.IsOperation())
                    throw new InvalidTokenException(enumerator.Current);

                var operation = enumerator.Current;

                var expr = operation.HasHigherPriorityThen(prevOp)
                    ? ex
                    : new Binary(prevExpr, prevOp, ex);

                var nextExpr = EvalRec(enumerator, expr, enumerator.Current);

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
        private string FunctionStr { get; set; }

        public override double Eval() => Function(Arg.Eval());

        public static bool IsFuncExist(string func) => Funcs.ContainsKey(func);
        public override string ToString() => $" {FunctionStr}( {Arg} )";
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
        public override string ToString() => $"-{Arg}";
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

        public Binary(Expr arg, Token opToken, Expr arg2) : base(arg)
        {
            OpStr = opToken.Value;
            Function = BinaryOps[opToken.Type];
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

                if (token.Type == TokenType.Func && !Grouping.IsFuncExist(token.Value))
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
