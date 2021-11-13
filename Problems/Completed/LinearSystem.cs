using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

//NOTE: https://www.codewars.com/kata/56464cf3f982b2e10d000015/train/csharp
namespace Problems.Completed
{
    [SimpleJob(RunStrategy.ColdStart, targetCount: 5000)]
    [MinColumn, MaxColumn, MeanColumn, MedianColumn]
    public class LinearSystem
    {
        [Benchmark]
        public void SolveBenchmark() => Solve("6 -4 11 -30 -14 -18/17 18 -26\n" +
                                              "-5 -12/19 -9/14 -25 19 -15 -11/28 10/7\n" +
                                              "-1 -15/16 23 6 20 27/19 -18 -5/7\n" +
                                              "-9 -8 -16 -17 28 7/5 -29 -19/20");

        public string Solve(string input)
        {
            var equations = ParseEquations(input);
            if (equations.Select(row => row.Length).Distinct().Count() != 1)
                throw new ArgumentException("Invalid equation's length!");

            var variablesCount = equations[0].Length - 1;
            var normalizedColumns = new int?[variablesCount];

            // i - row's index; j - column's index
            for (var (i, j) = (0, 0); i < equations.Length; j++, i++)
            {
                if (j == variablesCount)
                {
                    if (equations[i][j] != Fraction.Zero)
                        return "SOL=NONE";

                    j--;
                    continue;
                }

                if (!TryGetFirstNonZero(out var firstNonZeroIndex))
                {
                    i--;
                    continue;
                }

                normalizedColumns[j] = i;
                Swap(ref equations[firstNonZeroIndex], ref equations[i]);
                equations[i] = Multiply(equations[i], Fraction.One / equations[i][j]);

                for (var i1 = 0; i1 < equations.Length; i1++)
                {
                    if (i1 == i || equations[i1][j] == Fraction.Zero)
                        continue;

                    var factor = equations[i1][j];
                    var offsetRow = Multiply(equations[i], factor);
                    equations[i1] = Sub(equations[i1], offsetRow);
                }

                Fraction[] Multiply(Fraction[] row, Fraction factor) =>
                    row.Select(fr => fr * factor).ToArray();

                Fraction[] Sub(Fraction[] row1, Fraction[] row2) =>
                    row1.Zip(row2).Select(t => t.First - t.Second).ToArray();

                void Swap(ref Fraction[] row1, ref Fraction[] row2) => (row2, row1) = (row1, row2);

                bool TryGetFirstNonZero(out int firstNonZeroIndx)
                {
                    var firstNonZero = equations
                        .Select((row, index) => (row, index))
                        .Skip(i)
                        .FirstOrDefault(t => t.row[j] != Fraction.Zero);

                    firstNonZeroIndx = firstNonZero.index;
                    return firstNonZero.row != null;
                }
            }

            var solutions = ExtractSolutions(equations, normalizedColumns);
            return FormatSolutions(solutions);
        }
        private static string FormatSolutions(IEnumerable<Fraction[]> solutions)
        {
            var separator = " + ";
            var sb = new StringBuilder("SOL=");
            foreach (var (solution, i) in solutions.Select((sl, i) => (sl, i)))
            {
                if (i != 0)
                    sb.Append("q").Append(i).Append("* ");

                sb.Append("(").AppendJoin("; ", solution.Select(fr => fr.ToString())).Append(")").Append(separator);
            }

            return sb.Remove(sb.Length - separator.Length, separator.Length).ToString();
        }

        private static IEnumerable<Fraction[]> ExtractSolutions(Fraction[][] equations, int?[] normalizedColumns)
        {
            var constSolutions = normalizedColumns
                .Select(iNull => iNull is int i ? equations[i][^1] : Fraction.Zero).ToArray();
            var parametrizedSolutions = normalizedColumns
                .Select((iNull, j) => (iNull, j))
                .Where(parameter => parameter.iNull is null)
                .Select(parameter => normalizedColumns.Select((iNull, j) => iNull switch
                {
                    _ when j == parameter.j => Fraction.One,
                    int i => -equations[i][parameter.j],
                    null => Fraction.Zero,
                }).ToArray());

            return parametrizedSolutions.Prepend(constSolutions);
        }

        private static Fraction[][] ParseEquations(string input) => input
            .Split('\n')
            .Select(row => row
                .Split(" ")
                .Select(Fraction.FromStr)
                .ToArray())
            .ToArray();
    }

    public struct Fraction : IEquatable<Fraction>
    {
        private readonly BigInteger _num;
        private readonly BigInteger _denom;

        public Fraction(BigInteger numerator) : this(numerator, BigInteger.One) { }

        public Fraction(BigInteger numerator, BigInteger denominator)
        {
            if (denominator == 0)
                throw new ArgumentException("Denominator cannot be null!");

            if (denominator < 0)
                (numerator, denominator) = (-numerator, -denominator);

            var gcd = Gcd(BigInteger.Abs(numerator), denominator);
            _num = numerator / gcd;
            _denom = denominator / gcd;
        }

        #region operators stuff

        public static bool operator ==(Fraction? left, Fraction? right) => Equals(left, right);

        public static bool operator !=(Fraction? left, Fraction? right) => !Equals(left, right);

        public static Fraction operator +(Fraction left, Fraction right) =>
            new Fraction(left._num * right._denom + right._num * left._denom, left._denom * right._denom);

        public static Fraction operator -(Fraction fr) => new Fraction(-fr._num, fr._denom);

        public static Fraction operator -(Fraction left, Fraction right) => left + (-right);

        public static Fraction operator *(Fraction left, Fraction right) =>
            new Fraction(left._num * right._num, left._denom * right._denom);

        public static Fraction operator /(Fraction left, Fraction right) =>
            new Fraction(left._num * right._denom, left._denom * right._num);

        #endregion

        #region equality stuff

        public bool Equals(Fraction other)
        {
            return _num.Equals(other._num) && _denom.Equals(other._denom);
        }
        public override bool Equals(object? obj)
        {
            return obj is Fraction other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_num, _denom);
        }

        #endregion

        public override string ToString() => _denom == 1 ? $"{_num}" : $"{_num}/{_denom}";

        private static BigInteger Gcd(BigInteger a, BigInteger b) => b == 0 ? a : Gcd(b, a % b);

        public static Fraction One => new Fraction(1);
        public static Fraction Zero => new Fraction(0);

        public static Fraction FromStr(string str)
        {
            var arr = str.Split("/").ToArray();
            return arr.Length switch
            {
                1 => new Fraction(BigInteger.Parse(arr[0])),
                2 => new Fraction(BigInteger.Parse(arr[0]), BigInteger.Parse(arr[1])),
                _ => throw new ArgumentException($"Invalid fraction: {str}"),
            };
        }
    }
}
