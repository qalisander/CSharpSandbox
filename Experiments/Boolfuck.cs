using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public class Boolfuck
{
    // https://www.codewars.com/kata/search/my-languages?q=Esolang%20Interpreters&&beta=false
    // https://www.codewars.com/kata/5861487fdb20cff3ab000030/train/csharp

    // C# String To Byte Array
    // https://www.c-sharpcorner.com/article/c-sharp-string-to-byte-array/
    // https://social.msdn.microsoft.com/Forums/en-US/e2403fd6-61ce-4487-b11a-fddcef40c87f/using-bitarray-with-binarywriter-streams?forum=Vsexpressvcs

    // TODO: Convertion from bytes to bits array https://stackoverflow.com/questions/40541433/how-to-get-bit-values-from-byte
    public static string Interpret(string code, string input)
    { // TODO: prbl use spans and create Benchmarks, spans and
        var inputEnumerator = new BitArray(Encoding.UTF8.GetBytes(input)).GetEnumerator();
        // TODO: rewrite with char yield return
        var output = new List<bool>();

        var field = new Dictionary<int, bool>{{0, false}};
        var brackets = new Dictionary<int, int>(ProcessBrackets(code));
        var current = 0;

        for (var index = 0; index < code.Length; index++)
        {
            switch (code[index])
            {
                case '<':
                    current--;
                    field.TryAdd(current, false);
                    break;
                case '>':
                    current++;
                    field.TryAdd(current, false);
                    break;
                case '+':
                    field[current] ^= true;
                    break;
                case '[':
                    if (field[current] == false)
                        index = brackets[index];
                    break;
                case ']':
                    if (field[current] == true)
                        index = brackets[index];
                    break;
                case ';':
                    output.Add(field[current]);
                    break;
                case ',':
                    // TODO: use yield return
                    if (!inputEnumerator.MoveNext())
                        goto endFor;

                    field[current] = (bool)inputEnumerator.Current;
                    break;
            }
        }
        endFor:

        byte[] arr = new byte[0];
        new BitArray(output.ToArray()).CopyTo(arr, 0);

        // https://stackoverflow.com/questions/3917086/convert-bitarray-to-string
        return Encoding.ASCII.GetString(arr);

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
