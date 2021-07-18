using System;
using System.Linq;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace Experiments.Completed
{
    [SimpleJob(RunStrategy.ColdStart, targetCount: 5)]
    [MinColumn, MaxColumn, MeanColumn, MedianColumn]
    public class TrailingZerosKata 
    {
        [Params(1_000_000)] public int prm;

        [Benchmark]
        public void For() => TrailingZerosFor(prm);

        [Benchmark]
        public void ForMethodImplAnd() => TrailingZerosForMethodImpl(prm);

        [Benchmark]
        public void Range() => TrailingZerosRange(prm);

        [Benchmark]
        public void RangeMethodImplAnd() => TrailingZerosRangeMethodImpl(prm);

        [Benchmark]
        public void Basic() => TrailingZeros(prm);

        public static int TrailingZerosFor(int n)
        {
            var ans = 0;

            for (int i = 0; i <= n; i++)
            {
                ans += CountFives(i);
            }

            return ans;

            static int CountFives(int num)
            {
                int ans = 0;

                for (var i = num; i > 0 && i % 5 == 0; i /= 5)
                    if (i % 5 == 0)
                        ans++;

                return ans;
            }
        }

        public static int TrailingZerosForMethodImpl(int n)
        {
            var ans = 0;

            // 2500 ms -> 1099 ms
            for (var i = 0; i <= n && i >= 0; i+=5)
            {
                ans += CountFives(i);
            }

            return ans;

            // [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static int CountFives(int num)
            {
                int ans = 0;

                for (var i = num; i > 0 && i % 5 == 0; i /= 5)
                    if (i % 5 == 0)
                        ans++;

                return ans;
            }
        }

        public static int TrailingZerosRange(int n)
        {
            return Enumerable.Range(1, n).Sum(CountFives);

            static int CountFives(int num)
            {
                int ans = 0;

                for (var i = num; i > 0 && i % 5 == 0; i /= 5)
                    if (i % 5 == 0)
                        ans++;

                return ans;
            }
        }

        public static int TrailingZerosRangeMethodImpl(int n)
        {
            return Enumerable.Range(1, n).Sum(CountFives);

            // [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static int CountFives(int num)
            {
                int ans = 0;

                for (var i = num; i > 0 && i % 5 == 0; i /= 5)
                    if (i % 5 == 0)
                        ans++;

                return ans;
            }
        }

        public static int TrailingZeros(int n)
        {
            var ans = 0;

            for (var i = 1;; i++)
            {
                var powOfFive = (int) Math.Pow(5, i);

                if (powOfFive > 0 && powOfFive <= n)
                    ans += n / (int) powOfFive;
                else
                    break;
            }

            return ans;
        }
    }
}
