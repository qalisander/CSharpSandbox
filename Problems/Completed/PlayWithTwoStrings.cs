using System;
using System.Linq;

namespace Problems.Completed
{
    // https://www.codewars.com/kata/56c30ad8585d9ab99b000c54/train/csharp
    public class PlayWithTwoStrings
    {
        public string WorkOnStrings(string a, string b) => ProcessStr(a, b) + ProcessStr(b, a);

        private static string ProcessStr(string first, string second) => String.Concat(first.GroupJoin(second,
                ch1 => char.ToUpper(ch1),
                ch2 => char.ToUpper(ch2),
                (ch1, ch2s) => ch2s.Count() % 2 != 0 ? SwitchCase(ch1) : ch1));

        private static char SwitchCase(char ch) => char.IsUpper(ch) ? char.ToLower(ch) : char.ToUpper(ch);
    }
}
