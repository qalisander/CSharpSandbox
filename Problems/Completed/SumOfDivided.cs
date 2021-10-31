using System;
using System.Collections.Generic;
using System.Linq;

namespace Problems.Completed
{
    public class SumOfDivided
    {
        public static string GetSumOfDivided(int[] input) => string.Concat(
            GeneratePrimeNumbers(input.Max(Math.Abs))
                .Where(num => num != 1 && input.Any(inp => inp % num == 0))
                .Select(primeNum => $"({primeNum} {input.Where(inp => inp % primeNum == 0).Sum()})"));

        public static IEnumerable<int> GeneratePrimeNumbers(int max)
        {
            var primeNumbers = new List<int> { 1, 2 };

            for (var candidate = 3; candidate <= max; candidate++)
            {
                //Lower numbers will be checked first
                var isPrime = primeNumbers
                    .Where(num => num != 1)
                    .TakeWhile(num => num <= Math.Sqrt(candidate))
                    .All(num => candidate % num != 0);

                if (isPrime)
                    primeNumbers.Add(candidate);
            }

            return primeNumbers;
        }
    }
}
