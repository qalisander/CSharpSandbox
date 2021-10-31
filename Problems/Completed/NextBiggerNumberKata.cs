using System;
using System.Collections.Generic;
using System.Linq;

namespace Problems.Completed
{
    public class NextBiggerNumberKata
    {
        public static long NextBiggerNumber(long n)
        {
            var digits = n.GetDigits().ToList();
        
            for (var index = 0; index < digits.Count; index++)
            {
                if (index != 0 && digits[index - 1] > digits[index])
                {
                    var digitToExchange = digits.Take(index).Where(dgt => dgt > digits[index]).Min();
                    var exchangeIndex = digits.IndexOf(digitToExchange);
        
                    digits.Swap(index, exchangeIndex);
                    digits.Sort(0, index, new NextBiggerNumberExt.Comparer());
        
                    break;
                }
            }

            var ans = digits.GetLongFromDigits();
            return ans != n ? ans : -1;
        }
    }

    public static class NextBiggerNumberExt
    {
        public static IEnumerable<int> GetDigits(this long num)
        {
            for (var i = num; i != 0; i /= 10)
                yield return (int) (i % 10);
        }

        public static long GetLongFromDigits(this IEnumerable<int> digits) =>
            digits.Select((dgt, i) => dgt * (long) Math.Pow(10, i)).Sum();

        public class Comparer : IComparer<int>
        {
            public int Compare(int x, int y) => y - x;
        }

        public static void Swap(this List<int> digits, int index, int exchangeIndex)
        {
            var tmp = digits[index];
            digits[index] = digits[exchangeIndex];
            digits[exchangeIndex] = tmp;
        }
    }
}
