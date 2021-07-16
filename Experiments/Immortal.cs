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
    }
}
