using System.Collections.Generic;
using System.Linq;

namespace Experiments.Completed
{
    public class IntPartitions
    {
        // private HashSet<int>[] mem { get; }

        // public IntPartitions(long n) => mem = Enumerable.Range(0, (int)n + 1).Select(i => new HashSet<int>() { i }).ToArray(); // TODO: Lookup

        public static string Part(long n)
        {
            var memory = Enumerable.Range(0, (int)n + 1).Select(i => new HashSet<int>() { i }).ToArray(); // TODO: straightforward solution
            var ans = PartRec((int)n).OrderBy(x => x).ToList();

            return $"Range: {ans.Max() - ans.Min()} "
                   + $"Average: {ans.Average():.00} "
                   + $"Median: {Median(ans):.00}";

            IEnumerable<int> PartRec(int arg)
            {
                if (memory[arg].Count == 1)
                {
                    for (var i = 1; i <= arg / 2; i++)
                        memory[arg].UnionWith(PartRec(arg - i).Select(x => x * i)); // TODO: Concat, Distinct
                }

                return memory[arg];
            }

            static double Median(IList<int> list) => list.Count % 2 == 0
                ? ((double)list[list.Count / 2 - 1] + list[list.Count / 2]) / 2
                : list[list.Count / 2];
        }

        // public HashSet<int> this[int i]
        // {
        //     get {
                
        //     };
        // }
    }
}
