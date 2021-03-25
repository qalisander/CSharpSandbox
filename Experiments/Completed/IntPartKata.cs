using System.Collections.Generic;
using System.Linq;

namespace Experiments.Completed
{
    public class IntPartKata
    {
        public static string Part(long n)
        {
            var memory = new HashSet<int>[n + 1];
            var ans = PartRec((int)n).OrderBy(x => x).ToList();

            return $"Range: {ans.Max() - ans.Min()} "
                   + $"Average: {ans.Average():.00} "
                   + $"Median: {Median(ans):.00}";

            IEnumerable<int> PartRec(int arg)
            {
                if (memory[arg] is null)
                {
                    memory[arg] = new HashSet<int> { arg };
        
                    for (var i = 1; i <= arg/2; i++)
                        memory[arg].UnionWith(PartRec(arg - i).Select(x => x * i));
                }

                return memory[arg];
            }

            static double Median(IList<int> list) => list.Count % 2 == 0
                ? ((double) list[list.Count / 2 - 1] + list[list.Count / 2]) / 2
                : list[list.Count / 2];
        }
    }
}
