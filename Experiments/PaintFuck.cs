using System;
using System.Linq;

namespace Experiments
{
    // https://www.codewars.com/kata/5868a68ba44cfc763e00008d/train/csharp
    public class PaintFuck
    {
        public static string Interpret(string code, int iterations, int width, int height)
        {
            var field = Enumerable.Range(0, height).Select(_ => new int[width]).ToArray();
            int x = 0;
            int y = 0;

            foreach (var ch in code.Take(iterations))
            {
                if (ch == '*')
                    field[y][x] = 1;

                (x, y) = ch switch
                {
                    'n' => (x, (y + 1) % height),
                    's' => (x, (height + y - 1) % height),
                    'e' => ((x + 1) % width, y),
                    'w' => ((width + x - 1) % width, y),
                    '*' => (x, y),
                };
            }

            return String.Join("\r\n", field.Select(arr => String.Concat(arr)));
        }
    }
}
