using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Problems.Completed
{
    // https://www.codewars.com/kata/5518a860a73e708c0a000027/train/csharp
    // https://www.codewars.com/kata/5511b2f550906349a70004e1/train/rust
    public static class LastDigitOfHugeNumber
    {
        public static int LastDigit(int[] input) =>
            input.Length == 0 ? 1 : LastDigitRec(input.ComputeZeroPow(), 0, 10);

        private static int LastDigitRec(int[] nums, int i, int mod)
        {
            if (i == nums.Length)
                return 1;

            if (nums[i] == 0)
                return 0;

            // mod 2 and period lenth 4 crotch
            if (nums[i] % mod == 2 && (i + 1 >= nums.Length || nums[i + 1] == 1))
                return 2;

            var periodicDigits = GetPeriod(nums[i] % mod, mod).ShiftBy(-2).ToArray();

            var periodicIndex = periodicDigits.Length == 1
                ? 0
                : LastDigitRec(nums, i + 1, periodicDigits.Length);

            return periodicDigits[periodicIndex];
        }

        private static int[] ComputeZeroPow(this int[] input) =>
            input.Reverse().ComputeZeroPow().Reverse().ToArray();

        private static IEnumerable<int> ComputeZeroPow(this IEnumerable<int> nums)
        {
            int prev = -1;

            foreach (var num in nums)
                yield return prev = (prev == 0 ? 1 : num);
        }

        public static IEnumerable<int> GetPeriod(int num, int mod)
        {
            var digit = num;

            do
            {
                yield return digit = digit * num % mod;
            } while (digit != num && digit != 0);
        }

        public static IEnumerable<T> ShiftBy<T>(this IEnumerable<T> enumerable, int delta)
        {
            var arr = enumerable.ToArray();
            var len = arr.Length;

            for (var i = 0; i < len; i++)
                yield return arr[(i + len + delta % len) % len];
        }

        //--------------------------------------------------------------------------------------

        public static int[][] GetPeriods(int mod) =>
            Enumerable.Range(0, mod).Select(num => GetPeriod1(num, mod).ToArray()).ToArray();

        private static IEnumerable<int> GetPeriod1(int num, int mod)
        {
            var digit = num;

            do
            {
                digit = digit * num % mod;

                yield return digit;
            } while (digit != num && digit != 0); // when mod % num == 0
        }

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

    //------------------------------------------------------------------------------

    public class Calculator2
    {
        public static int LastDigit(int[] array)
        {
            if (array.Length == 0)
            {
                return 1;
            }

            int number = array.Last();

            foreach (int i in array.Reverse().Skip(1))
            {
                int power = number;

                switch (power)
                {
                    case 0:
                        number = 1;

                        break;
                    case 1:
                        number = i;

                        break;
                    case 2:
                        number = i * i;

                        break;
                    default:
                        power = (power - 3) % 4 + 3;
                        int n = i < 3 ? i : (i - 3) % 20 + 3;
                        number = (int) Math.Pow(n, power);

                        break;
                }
            }

            return number % 10;
        }
    }

    //----------------------------------------------------------------

    public class Calculator3
    {
        public static int LastDigit(int[] array) =>
            (int) (array.Reverse().Aggregate(1L, (e, m) =>
                (long) Math.Pow(m > 20 ? m % 20 + 20 : m, e > 4 ? e % 4 + 4 : e)) % 10);
    }

    //------------------------------------------

    public class Calculator4
    {
        public static int LastDigit(int[] array)
        {
            BigInteger n = 1; //Initialize as a long (larged signed) integer as 1 

            if (array == null || array.Length == 0)
            {
                return 1;
            } //Default to 1 for empty array

            Array.Reverse(array);

            foreach (var x in array)
            {
                //Loop through the array for all other values 
                n = BigInteger.Pow(x, (int) (n >= 4 ? n % 4 + 4 : n));
            }

            return (int) (n % 10);
        }
    }

// If your language doesn't have native big integers, this solution might not work.
//    def last_digit(lst):
//      n = 1
//      for x in reversed(lst):
//      # If you write x ** (n% 4 + 4) directly, the result will be wrong when n is 0
//      n = x ** (n if n < 4 else n % 4 + 4) 
//      return n % 10
}
