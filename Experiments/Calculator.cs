using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace NTests
{
    public class Calculator
    {
        public static int LastDigit(int[] array)
        {
            if (array.Length == 0)
                return 1;

            // var periods = GetPeriods();
            // var pow = array.Last();
            //
            // foreach(var lastDigit in array.Reverse().Skip(1))
            // {
            //     pow = DigitPower(lastDigit, pow);
            // }
            //
            // return pow;
            //
            // int DigitPower(int digit, int pow)
            // {
            //     var periodicNums = periods[digit % 10];
            //     var period = periodicNums.Length;
            //
            //     return pow == 0 ? 1 : periodicNums[(period + pow - 1) % period];
            // }

            var ans = 1;

            var periods10 = GetPeriods(10);
            var periodicNums10 = periods10[array[0] % 10];

            for (int i = 1; i < array.Length; i++)
            {
                var periods = GetPeriods(periodicNums10.Length);
            }

            return ans;
        }

        public static int[][] GetPeriods(int mod) =>
            Enumerable.Range(0, mod).Select(num => GetPeriod(num, mod).ToArray()).ToArray();

        private static IEnumerable<int> GetPeriod(int num, int mod)
        {
            var digit = num;
            do
            {
                yield return digit;
                digit = digit * num % mod;
            } while (digit != num && digit != 0); // when mod % num == 0
        }

        // TODO: prlly first nuber should be at the end

        public static int[][] GetPeriods() =>
            Enumerable.Range(0, 10).Select(num => GetPeriod(num).ToArray()).ToArray();

        private static IEnumerable<int> GetPeriod(int num)
        {
            var digit = num % 10;
            do
            {
                yield return digit;
                digit = digit * num % 10;
            } while (digit != num);
        }

        public static int[][] GetPeriodsStraightforward()
        {
            var periods = new int[10][];

            for (var i = 0; i < periods.Length; i++)
            {
                var nums = new List<int>();

                var num = i;

                do
                {
                    nums.Add(num);
                    num = num * i % 10;
                } while (num != i);

                periods[i] = nums.ToArray();
            }

            return periods;
        }

        public static int[][] GetPeriodsIter() =>
            Enumerable.Range(0, 10).Select(num => GetPeriodIter(num).ToArray()).ToArray();

        private static IEnumerable<int> GetPeriodIter(int num)
        {
            yield return num;

            for (int i = num * num % 10; i != num; i = i * num % 10)
                yield return i;
        }

        public static int[][] GetPeriodsIterRec() =>
            Enumerable.Range(0, 10).Select(num => GetPeriodIterRec(num).ToArray()).ToArray();

        private static IEnumerable<int> GetPeriodIterRec(int num, int? i = null)
        {
            yield return i ??= num;

            if ((i = i * num % 10) != num)
                foreach (var period in GetPeriodIterRec(num, i))
                    yield return period;
        }
    }
}
