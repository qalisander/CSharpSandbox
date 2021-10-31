using System;
using System.Numerics;

namespace Problems.Completed
{
    // https://www.codewars.com/kata/59568be9cc15b57637000054
    public static class Immortal
    {
        /// set true to enable debug
        public static bool Debug = false;

        public static long ElderAge(long N, long M, long deduction, long mod)
        {
            if (N < 0 || M < 0 || deduction < 0 || mod < 0)
                throw new ArgumentException("Negative argument");

            return (long) (EvaluateSumRec(0, N, M) % mod);

            BigInteger EvaluateSumRec(BigInteger init, BigInteger N, BigInteger M)
            {
                if (N == 0 || M == 0)
                    return 0;

                var maxCount = BigInteger.Max(N, M);
                var minCount = BigInteger.Min(N, M);

                var maxCountPow2 = BigInteger.Pow(2, (int) BigInteger.Log(maxCount, 2));
                var minCountPow2 = BigInteger.Pow(2, (int) BigInteger.Log(minCount, 2));

                var newN = maxCount - maxCountPow2;
                var newM = maxCountPow2 == minCountPow2 ? minCount - maxCountPow2 : minCount;
                var newInit = maxCountPow2 == minCountPow2 ? init : maxCountPow2 + init;
                
                var ans = maxCountPow2 == minCountPow2
                    ? SumRange(init, maxCountPow2, deduction, mod) * maxCountPow2
                      + SumRange(init + maxCountPow2, maxCountPow2, deduction, mod) * (newM + newN)
                    : SumRange(init, maxCountPow2, deduction, mod) * minCount;

                return ans % mod + EvaluateSumRec(newInit, newN, newM);
            }
        }

        public static BigInteger SumRange(BigInteger numFrom, BigInteger count, long deduction, long mod)
        {
            numFrom -= deduction;
            count = numFrom < 0 ? count + numFrom : count;

            return ZeroWhenNeg(count) * (ZeroWhenNeg(numFrom) * 2 + ZeroWhenNeg(count) - 1) / 2 % mod;

            BigInteger ZeroWhenNeg(BigInteger bigInteger) => bigInteger < 0 ? 0 : bigInteger;
        }
    }
    
    // public static class Immortal
    // {
    //     /// set true to enable debug
    //     public static bool Debug = false;
    //
    //     public static long ElderAge(BigInteger m, BigInteger y, BigInteger l, BigInteger t)
    //     {
    //         BigInteger T = 0;
    //
    //         while (y > 0)
    //         {
    //             var Y = y;
    //             var x = m;
    //             y &= y - 1;
    //             while (x > 0)
    //             {
    //                 var X = x;
    //                 x &= x - 1;
    //                 var S = BigInteger.Max(X - x, Y - y);
    //                 var s = BigInteger.Min(X - x, Y - y);
    //                 var h = BigInteger.Max((x ^ y | S - 1) + 1 - l, 0);
    //                 var w = BigInteger.Min(h, S);
    //                 T += s * w * (h + h - w - 1) / 2;
    //             }
    //         }
    //
    //         return (long)(T % t);
    //     }
    // }
}
