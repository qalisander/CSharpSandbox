using System.Linq;
using System.Text;

namespace Experiments.Completed
{
    // https://www.codewars.com/kata/586dd26a69b6fd46dd0000c0/train/csharp
    public class MiniStringFuck
    {
        public static string MyFirstInterpreter(string code) =>
            code.Aggregate((new StringBuilder(), (byte)0), (acml, ch) => ch switch
            {
                '+' => (acml.Item1, acml.Item2 += 1), // modulo 256 (byte.Max) 
                '.' => (acml.Item1.Append((char)acml.Item2), acml.Item2),
                _ => acml,
            }).Item1.ToString();
    }
}
