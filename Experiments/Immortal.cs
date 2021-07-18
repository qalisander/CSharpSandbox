using System;
using System.Numerics;

namespace Experiments
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

            var bigIntN = new BigInteger(N);
            var bigIntM = new BigInteger(M);

            return (long) (EvaluateSumRec(0, BigInteger.Max(bigIntN, bigIntM), BigInteger.Min(bigIntN, bigIntM) ) % mod);

            BigInteger EvaluateSumRec(BigInteger init, BigInteger x, BigInteger y)
            {
                if (x == 0 || y == 0)
                    return 0;

                var xPow2 = BigInteger.Pow(2, (int) BigInteger.Log(x, 2));
                var yPow2 = BigInteger.Pow(2, (int) BigInteger.Log(y, 2));

                BigInteger newX;
                BigInteger newY;
                BigInteger newInit;
                BigInteger ans;

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

                return ans % mod + EvaluateSumRec(
                    newInit,
                    BigInteger.Max(newX, newY),
                    BigInteger.Min(newX, newY));
            }
        }

        public static BigInteger SumRange(BigInteger numFrom, BigInteger count, long deduction, long mod)
        {
            numFrom -= deduction;
            count = numFrom < 0 ? count + numFrom : count;
            
            numFrom = numFrom < 0 ? 0 : numFrom;
            count = count < 0 ? 0 : count;
            
            return count * (numFrom * 2 + count - 1) / 2 % mod;
        }
    }
}
