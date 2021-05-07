using System.Collections.Generic;
using System.Linq;

namespace Experiments.Completed
{
    public class IntPartitions
    {
        public static string Part(long n)
        {
            var ans = PartRec((int)n, new int[n + 1][]).OrderBy(x => x).ToArray();

            return $"Range: {ans.Max() - ans.Min()} Average: {ans.Average():.00} Median: {Median(ans):.00}";

            static IEnumerable<int> PartRec(int arg, int [][] memory) => 
                memory[arg] ?? (memory[arg] = Enumerable.Range(arg, 1)
                    .Union(Enumerable.Range(1, arg / 2)
                        .SelectMany(i => PartRec(arg - i, memory).Select(x => x * i))
                        .Distinct())
                    .ToArray());

            static double Median(IList<int> list) => list.Count % 2 == 0
                ? ((double)list[list.Count / 2 - 1] + list[list.Count / 2]) / 2
                : list[list.Count / 2];
        }
    }
}
