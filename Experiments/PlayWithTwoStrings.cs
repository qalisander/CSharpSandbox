using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Experiments
{
    public class PlayWithTwoStrings
    {
        public string WorkOnStrings(string a, string b)
        {
            // rewrite with GroupJoin
            var mapA = a.GroupBy(ch => char.ToLower(ch)).ToDictionary(ch => ch.Key, ch => ch.Count());
            var mapB = b.GroupBy(ch => char.ToLower(ch)).ToDictionary(ch => ch.Key, ch => ch.Count());

            //var ans = new StringBuilder();

            //foreach (var ch in a)
            //{
            //    if (mapB.TryGetValue(char.ToLower(ch), out var counts) && counts % 2 != 0)
            //    {
            //        ans.Append(SwitchCase(ch));
            //    }
            //    else
            //    {
            //        ans.Append(ch);
            //    }
            //}

            //foreach (var ch in b)
            //{
            //    if (mapA.TryGetValue(char.ToLower(ch), out var counts) && counts % 2 != 0)
            //    {
            //        ans.Append(SwitchCase(ch));
            //    }
            //    else
            //    {
            //        ans.Append(ch);
            //    }
            //}

            //return ans.ToString();


            return String.Concat(a.Select(ch =>
                    mapB.TryGetValue(char.ToLower(ch), out var counts1) && counts1 % 2 != 0
                        ? SwitchCase(ch)
                        : ch)
                .Concat(b.Select(ch =>
                    mapA.TryGetValue(char.ToLower(ch), out var counts) && counts % 2 != 0
                        ? SwitchCase(ch)
                        : ch)));
        }

        private static char SwitchCase(char ch) => char.IsUpper(ch) ? char.ToLower(ch) : char.ToUpper(ch);
    }
}
