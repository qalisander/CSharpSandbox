using System.Collections.Generic;
using System.Linq;

namespace NTests
{
    public class Calculator
    {
        public static int LastDigit(int[] array)
        {
            var ans = 1;

            for (var i = 1; i < array.Length; i++)
            {
                var lastDigit = array[i - 1] % 10;
                var power = array[i];
            }

            return ans;
        }

        public static int[][] GetPeriods()
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

        public static int[][] GetPeriods2() =>
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

        public static int[][] GetPeriods3() =>
            Enumerable.Range(0, 10).Select(num => GetPeriod3(num).ToArray()).ToArray();

        private static IEnumerable<int> GetPeriod3(int num)
        {
            var digit = num % 10;
            do
            {
                yield return digit;
                digit = digit * num % 10;
            } while (digit != num);
        }
    }
}
