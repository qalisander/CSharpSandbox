using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using BenchmarkDotNet.Environments;

namespace Experiments
{
    public static class Immortal
    {
        /// set true to enable debug
        public static bool Debug = false;

        public static long ElderAge(long N, long M, long deduction, long mod)
        {
            ModulusLong.Modulus = long.MaxValue;

            return EvaluateSumRec(0, Math.Max(N, M), Math.Min(N, M));

            long EvaluateSumRec(long init, long x, long y)
            {
                if (x == 1 && y == 1)
                    return init;

                var xPow2 = Pow2((int) Math.Log2(x + 1));
                var yPow2 = Pow2((int) Math.Log2(y + 1));

                long newX;
                long newY;
                long newInit;

                long ans;

                if (xPow2 == yPow2)
                {
                    newX = x - xPow2;
                    newY = y - yPow2;

                    newInit = xPow2 ^ yPow2 + init;

                    ans = SumRange(init, xPow2 - 1) * (xPow2 - 1)
                          + SumRange(init + xPow2, init + xPow2 * 2 - 1) * (newX + 1)
                          + SumRange(init + xPow2, init + xPow2 * 2 - 1) * ((newY -= yPow2) + 1);
                }
                else
                {
                    newX = x - xPow2;
                    newY = y;

                    newInit = xPow2 + init;

                    ans = SumRange(init, init + xPow2 - 1) * (newY + 1);
                }

                return ans % mod + EvaluateSumRec(
                    newInit,
                    Math.Max(newX, newY),
                    Math.Min(newX, newY));
            }

            long SumRange(long from, long to) => SumRangeInternal(Deduct(from, deduction), Deduct(to, deduction), mod);

            static long SumRangeInternal(long from, long to, long mod) =>
                (long) (((ulong) from + (ulong) to) % (ulong) mod
                    * ((ulong) to - (ulong) from + 1) % (ulong) mod
                    / 2 % (ulong) mod);

            [DebuggerStepThrough]
            static long Deduct(long num, long deduction) => num >= deduction ? num - deduction : 0;

            [DebuggerStepThrough]
            static long Pow2(int pow) => 1L << pow;

            // ModulusLong SumRange(long from, long to) =>
            //     ModulusLong.SumRange((ModulusLong) from, (ModulusLong) to);
        }

        //TODO: create special struct with operations with module of mod

        public struct ModulusLong : IEquatable<ModulusLong>, IComparable<ModulusLong>
        {
            private readonly long _value;
            private ulong Ulong => (ulong) _value;

            public static long Modulus { get; set; }

            public ModulusLong(long value)
            {
                _value = value >= 0
                    ? value % Modulus
                    : value % Modulus + Modulus;
            }

            public ModulusLong(ulong value)
            {
                _value = (long)(value % (ulong)Modulus);
            }

            public static ModulusLong One => new ModulusLong(1);

            public static ModulusLong Two => new ModulusLong(2);

            public static ModulusLong SumRange(ModulusLong from, ModulusLong to) =>
                new ModulusLong(checked((from.Ulong + to.Ulong) * (to.Ulong - from.Ulong + 1)) / 2);

            public static ModulusLong SumRange(ModulusLong from, long count) =>
                new ModulusLong(checked((from.Ulong * 2 + (ulong)count - 1) * (ulong)count) / 2);

            public ModulusLong Pow(int pow) =>
                Enumerable.Repeat(this, pow).Aggregate((ml1, ml2) => ml1 * ml2);

            #region operators

            public static bool operator <(ModulusLong left, ModulusLong right) => left.CompareTo(right) < 0;
            
            public static bool operator >(ModulusLong left, ModulusLong right) => left.CompareTo(right) > 0;
            
            public static bool operator <=(ModulusLong left, ModulusLong right) => left.CompareTo(right) <= 0;
            
            public static bool operator >=(ModulusLong left, ModulusLong right) => left.CompareTo(right) >= 0;

            public static ModulusLong operator +(ModulusLong left, ModulusLong right) =>
                new ModulusLong(checked(left.Ulong + right.Ulong));

            public static ModulusLong operator -(ModulusLong left, ModulusLong right) =>
                new ModulusLong(left._value - right._value);

            public static ModulusLong operator *(ModulusLong left, ModulusLong right) =>
                new ModulusLong(checked(left.Ulong * right.Ulong));

            public static implicit operator ModulusLong(int num) => new ModulusLong(num);

            public static explicit operator long(ModulusLong ml) => ml._value;

            #endregion

            #region compare equals

            public int CompareTo(ModulusLong other) => _value.CompareTo(other._value);

            public bool Equals(ModulusLong other) => _value == other._value;

            public override bool Equals(object? obj) => obj is ModulusLong other && Equals(other);

            public override int GetHashCode() => _value.GetHashCode();

            public override string ToString() => _value.ToString();

            #endregion
        }
    }
}
