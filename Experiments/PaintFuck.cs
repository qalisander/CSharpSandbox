using System;
using System.Collections.Generic;
using System.Linq;

namespace Experiments
{
    // TODO: yield return + nullable
    // https://www.codewars.com/kata/5868a68ba44cfc763e00008d/train/csharp
    // Procedural sulution
    public class PaintFuck
    {
        public static string Interpret(string code, int iterations, int width, int height)
        {
            //Console.WriteLine(code + $"\niterations: {iterations} \nwidth: {width} \nhigth: {height}");
            var field = Enumerable.Range(0, height).Select(_ => new int[width]).ToArray();
            var brackets = ProcessBrackets(code);
            int x = 0;
            int y = 0;

            for (var index = 0; index < code.Length && iterations > 0; index++)
            {
                iterations--;

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

            static Dictionary<int, int> ProcessBrackets(string code)
            {
                var bracketsStack = new Stack<int>();
                var bracketsDict = new Dictionary<int, int>();

                for (var index = 0; index < code.Length; index++)
                {
                    if (code[index] == ']')
                    {
                        var firstBracketIndex = bracketsStack.Pop();
                        bracketsDict[index] = firstBracketIndex;
                        bracketsDict[firstBracketIndex] = index;
                    }
                    else if (code[index] == '[')
                    {
                        bracketsStack.Push(index);
                    }
                }

                if (bracketsStack.Count != 0)
                    throw new InvalidOperationException(
                        $"input string has not valid brackets. Brackets stack: {string.Join("\t", bracketsStack)}");

                return bracketsDict;

                //IEnumerable<(int firstIndex, int secondIndex)> ProcessBracketsInternal(string code)
                //{
                //    var bracketsStack = new Stack<int>();

                //    foreach (var (ch, index) in code.Select((ch, Index) => (ch, Index)))
                //        if (ch == ']')
                //            yield return (index, bracketsStack.Pop());
                //        else if (ch == '[')
                //            bracketsStack.Push(index);

                //    if (bracketsStack.Count != 0)
                //        throw new InvalidOperationException(
                //            $"input string has not valid brackets. Brackets stack: {string.Join("\t", bracketsStack)}");
                //}
            }
        }
    }
}
