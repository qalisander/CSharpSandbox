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

        public static long ElderAge(long N, long M, long k, long newp)
        {
            if (N < 0 || M < 0 || k < 0 || newp < 0)
                throw new ArgumentException("Negative argument");

            ulong deduction = (ulong)k;
            ulong mod = (ulong)newp;
            
            return (long)EvaluateSumRec(0, (ulong)Math.Max(N, M), (ulong)Math.Min(N, M));

            ulong EvaluateSumRec(ulong init, ulong x, ulong y)
            {
                if (x == 0 || y == 0)
                    return 0;

                if (x == 1 && y == 1)
                    return init;

                // TODO: lowerPow -> pow2
                ulong xLowerPow2 = Pow2((int) Math.Log2(x + 1)); // TODO: remove 1
                ulong yLowerPow2 = Pow2((int) Math.Log2(y + 1));

                ulong newX;
                ulong newY;
                ulong newInit;
                ulong ans;

                if (xLowerPow2 == yLowerPow2)
                {
                    newX = x - xLowerPow2;
                    newY = y - xLowerPow2;
                    newInit = xLowerPow2 ^ yLowerPow2 + init; // NOTE: 64 + 16 = 80

                    ans = SumRange(init, xLowerPow2, deduction, mod) * xLowerPow2
                          + SumRange(init + xLowerPow2, newX, deduction, mod) * xLowerPow2
                          + SumRange(init + xLowerPow2, newY, deduction, mod) * xLowerPow2;
                }
                else 
                {
                    newX = x - xLowerPow2;
                    newY = y;
                    newInit = xLowerPow2 + init;

                    ans = SumRange(init, xLowerPow2, deduction, mod) * y;
                }

                return ans % mod + EvaluateSumRec(
                    newInit,
                    Math.Max(newX, newY),
                    Math.Min(newX, newY));
            }

            [DebuggerStepThrough]
            static ulong Pow2(int pow) => (ulong)1L << pow;
        }
        
        public static ulong SumRange(ulong numFrom, ulong count, ulong deduction, ulong mod)
        {
            if (numFrom >= deduction)
            {
                numFrom -= deduction;
            }
            else
            {
                count -= deduction - numFrom;
                numFrom = 0;
            }

            return count * (numFrom * 2 + count - 1) / 2 % mod;
        }
    }
}
