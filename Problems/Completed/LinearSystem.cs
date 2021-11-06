using System;
using System.Linq;

namespace Problems.Completed
{
    //NOTE: https://www.codewars.com/kata/56464cf3f982b2e10d000015/train/csharp
    public class LinearSystem
    {
        public string Solve(string input)
        {
            var equations = input
                .Split('\n')
                .Select(row => row
                    .Split(" ")
                    .Select(Fraction.FromStr)
                    .ToArray())
                .ToArray();

            //NOTE: cases N > M and N < M
            //NOTE: when array of zeroes, ignore array
            //NOTE: when all elements are zero, just add (0, 3, 5, ...) * q1 * (1, 0, 0, ...)
            //NOTE: case when we have 0 + 0 + 0 + 1 = 9 => return NONE
            // 1 1 ...
            // 0 0 ...
            // 0 0 ...
            // i - column index; j - row index


            if (equations.Select(row => row.Length).Distinct().Count() != 1)
                throw new ArgumentException("Invalid equation's length!");

            var normalizedColumns = new bool[equations.Length];
            for (var (i, j) = (0, 0); j < equations[0].Length && i < equations.Length; j++)
            {
                var processedRowsCount = i;
                 var (_, firstNonZeroIndex) = equations
                    .Select((row, index) => (row, index))
                    .Skip(processedRowsCount)
                    .FirstOrDefault(t => t.row[j] != Fraction.Zero);

                 if (firstNonZeroIndex == -1)
                     continue;

                normalizedColumns[i] = true;
                Swap(ref equations[firstNonZeroIndex], ref equations[j]);
                equations[i] = Multiply(equations[i], Fraction.One / equations[i][j]);

                for (var i1 = 0; i1 < equations.Length; i1++)
                {
                    if (i1 == i || equations[i1][j] == Fraction.Zero)
                        continue;

                    var factor = equations[i1][j];
                    var offsetRow = Multiply(equations[i], factor);
                    equations[i1] = Sub(equations[i1], offsetRow);
                }

                i++;

                // TODO: Enumerable
                Fraction[] Multiply(Fraction[] row, Fraction factor) =>
                    row.Select(fr => fr * factor).ToArray();

                Fraction[] Sub(Fraction[] row1, Fraction[] row2) =>
                    row1.Zip(row2).Select(t => t.First + t.Second).ToArray();

                void Swap(ref Fraction[] row1, ref Fraction[] row2) => (row2, row1) = (row1, row2);
            }

            throw new NotImplementedException();
        }
    }

    // TODO: struct?
    public class Fraction
    {
        private readonly int _num;
        private readonly int _denom;

        public override int GetHashCode() => HashCode.Combine(_num, _denom);

        public Fraction(int numerator, int denominator = 1)
        {
            if (denominator == 0)
                throw new ArgumentException("Denominator cannot be null!");

            if (denominator < 0)
                (numerator, denominator) = (-numerator, -denominator);

            var gcd = Gcd(numerator, denominator);
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

        protected bool Equals(Fraction other) => _num == other._num && _denom == other._denom;
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((Fraction) obj);
        }

        #endregion

        public override string ToString() => _denom == 1 ? $"{_num}" : $"{_num}/{_denom}";

        private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);

        public static Fraction One => new Fraction(1);
        public static Fraction Zero => new Fraction(0);

        public static Fraction FromStr(string str)
        {
            var arr = str.Split("/").ToArray();
            return arr.Length switch
            {
                1 => new Fraction(Convert.ToInt32(arr[0])),
                2 => new Fraction(Convert.ToInt32(arr[0]), Convert.ToInt32(arr[1])),
                _ => throw new ArgumentException($"Invalid fraction: {str}"),
            };
        }
    }
}
