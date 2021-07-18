using System;
using System.Diagnostics;

namespace Experiments
{
    // https://www.codewars.com/kata/59568be9cc15b57637000054
    public static class Immortal
    {
        /// set true to enable debug
        public static bool Debug = false;

        public static long ElderAge(long N, long M, long k, long newp)
        {
            if (N < 0 || M < 0 || k < 0 || newp < 0)
                throw new ArgumentException("Negative argument");

            var deduction = (ulong) k;
            var mod = (ulong) newp;

            return (long) EvaluateSumRec(0, (ulong) Math.Max(N, M), (ulong) Math.Min(N, M));

            ulong EvaluateSumRec(ulong init, ulong x, ulong y)
            {
                if (x == 0 || y == 0)
                    return 0;

                var xPow2 = Pow2((int) Math.Log2(x));
                var yPow2 = Pow2((int) Math.Log2(y));

                ulong newX;
                ulong newY;
                ulong newInit;
                ulong ans;

                if (xPow2 == yPow2)
                {
                    newX = x - xPow2;
                    newY = y - xPow2;
                    newInit = init;

                    var sumInRow = SumRange(init + xPow2, xPow2, deduction, mod);
                    ans = SumRange(init, xPow2, deduction, mod) * xPow2 + sumInRow * newX + sumInRow * newY;
                }
                else
                {
                    newX = x - xPow2;
                    newY = y;
                    newInit = xPow2 + init;

                    ans = SumRange(init, xPow2, deduction, mod) * y;
                }

                return (ans % mod + EvaluateSumRec(
                    newInit,
                    Math.Max(newX, newY),
                    Math.Min(newX, newY))) % mod;
            }

            [DebuggerStepThrough]
            static ulong Pow2(int pow) => (ulong) 1 << pow;
        }

        public static ulong SumRange(ulong numFrom, ulong count, ulong deduction, ulong mod)
        {
            if (numFrom >= deduction)
                numFrom -= deduction;
            else
            {
                count -= deduction - numFrom;
                numFrom = 0;
            }

            return count * (numFrom * 2 + count - 1) / 2 % mod;
        }
    }
}
