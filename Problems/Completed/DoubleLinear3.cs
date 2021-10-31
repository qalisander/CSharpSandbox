using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace Problems.Completed
{
    public class DoubleLinear3
    {
        [Benchmark]
        public static int DblLinear(int n)
        {
            var sequence = new HashSet<int>(n*2);
            sequence.Add(1);

            var counter = 0;

            for (var i = 1; i < int.MaxValue; i++)
            {
                if (IsInSequence(i, 2) || IsInSequence(i, 3))
                {
                    sequence.Add(i);
                    counter++;
                }

                if (counter == n)
                    return i;
            }

            return -1;

            bool IsInSequence(int i, int funcGrowth) => (i-1) % funcGrowth == 0 && sequence.Contains((i - 1) / funcGrowth);
        }
    }
}
