using System;
using System.Collections.Generic;
using System.Linq;

namespace NTests
{
    // https://www.codewars.com/kata/5518a860a73e708c0a000027/train/csharp

    // TODO: GetPeriodicSequence, generate mapping function by periodic sequence, dicionary under hood
    public static class Calculator
    {
        public static int LastDigit(int[] input)
        {
            if (input.Length == 0)
                return 1;

            ComputeZeroPow(input);

            return LastDigitRec(input, 0, 10);

            int LastDigitRec(int[] nums, int i, int mod)
            {
                if (i == nums.Length || nums[i] == 0)
                    return 0;

                // mod 2 crotch
                if (input[i] % mod == 2 && (i + 1 >= input.Length || input[i + 1] == 1))
                    return 2;

                var periodicDigits = GetPeriod(nums[i] % mod, mod).ShiftBy(-2).ToArray();

                var periodicIndex = periodicDigits.Length == 1
                    ? 0
                    : LastDigitRec(nums, i + 1, periodicDigits.Length);

                return periodicDigits[periodicIndex];
            }

            void ComputeZeroPow(int[] ints)
            {
                for (var i = ints.Length - 1; i > 0; i--)
                    if (ints[i] == 0)
                        ints[i - 1] = 1;
            }
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
