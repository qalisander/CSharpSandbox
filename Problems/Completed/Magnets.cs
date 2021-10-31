using System;

namespace Problems.Completed
{
    public class Magnets
    {
        public static double Doubles(int maxk, int maxn)
        {
            double ans = 0;

            for (var k = 0; k < maxk; k++)
            for (var n = 0; n < maxn; n++)
                ans += 1 / ((k + 1) * Math.Pow(n + 1 + 1, 2 * (k + 1)));

            return ans;
        }
    }
}
