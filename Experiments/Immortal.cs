using System;
using System.Diagnostics;

namespace Experiments
{
    // https://www.codewars.com/kata/59568be9cc15b57637000054
    public static class Immortal
    {
        //TODO: use int extensions methods for addition

        /// set true to enable debug
        public static bool Debug = false;

        public static long ElderAge(long N, long M, long deduction, long mod)
        {
            return EvaluateSumRec(0, Math.Max(N, M), Math.Min(N, M));

            long EvaluateSumRec(long initial, long x, long y)
            {
                if (x == 1 && y == 1)
                    return initial;

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

                    newInit = xPow2 ^ yPow2 + initial;

                    ans = SumRange(initial, xPow2 - 1, deduction, mod) * (xPow2 - 1)
                          + SumRange(initial + xPow2, initial + xPow2 * 2 - 1, deduction, mod) * (newX + 1)
                          + SumRange(initial + xPow2, initial + xPow2 * 2 - 1, deduction, mod) * ((newY -= yPow2) + 1);
                }
                else
                {
                    newX = x - xPow2;
                    newY = y;

                    newInit = xPow2 + initial;

                    ans = SumRange(initial, initial + xPow2 - 1, deduction, mod) * (newY + 1);
                }

                return ans % mod + EvaluateSumRec(
                    newInit,
                    Math.Max(newX, newY),
                    Math.Min(newX, newY));
            }

            [DebuggerStepThrough]
            static long Pow2(int pow) => 1L << pow;
        }
        public static long SumRange(long @from, long count, long deduction, long mod)
        {
            if (@from < 0 || count < 0 || deduction < 0 || mod < 0)
                throw new ArgumentException("Negative argument");

            return (long)SumRangeInternal(Deduct((ulong)@from, (ulong)deduction), Deduct((ulong)count, (ulong)deduction), (ulong)mod);

            static ulong SumRangeInternal(ulong from, ulong to, ulong mod) =>
                (to - from - 1) * (from + to) / 2 % mod;

            static ulong Deduct(ulong num, ulong delta) => num >= delta ? num - delta : 0;
        }
    }
}
