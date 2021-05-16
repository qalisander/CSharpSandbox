using System;
using System.Collections.Generic;
using System.Linq;

namespace Experiments
{
    // https://www.codewars.com/kata/5868a68ba44cfc763e00008d/train/csharp
    // Procedural sulution
    public class PaintFuck
    {
        public static string Interpret(string code, int iterations, int width, int height)
        {
            var field = Enumerable.Range(0, height).Select(_ => new int[width]).ToArray();
            var brackets = new Dictionary<int, int>(ProcessBrackets(code));
            int x = 0;
            int y = 0;

            for (var index = 0; index < code.Length && iterations > 0; index++, iterations--)
            {
                switch (code[index])
                {
                    case 's':
                        y = (y + 1) % height;
                        break;
                    case 'n':
                        y = (height + y - 1) % height;
                        break;
                    case 'e':
                        x = (x + 1) % width;
                        break;
                    case 'w':
                        x = (width + x - 1) % width;
                        break;
                    case '*':
                        field[y][x] ^= 1;
                        break;
                    case '[':
                        if (field[y][x] == 0)
                            index = brackets[index];
                        break;
                    case ']':
                        if (field[y][x] == 1)
                            index = brackets[index];
                        break;
                    default:
                        iterations++;
                        break;
                }
            }

            return String.Join("\r\n", field.Select(arr => String.Concat(arr)));

            static IEnumerable<KeyValuePair<int, int>> ProcessBrackets(string code)
            {
                var bracketsStack = new Stack<int>();

                foreach (var (ch, index) in code.Select((ch, index) => (ch, index)))
                    if (ch == ']')
                    {
                        yield return KeyValuePair.Create(index, bracketsStack.Peek());
                        yield return KeyValuePair.Create(bracketsStack.Pop(), index);
                    }
                    else if (ch == '[')
                        bracketsStack.Push(index);

                if (bracketsStack.Count != 0)
                    throw new InvalidOperationException(
                        $"input string has not valid brackets. Brackets stack: {string.Join("\t", bracketsStack)}");
            }
        }
    }
}
