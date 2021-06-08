using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

public static class Boolfuck
{
    // KATA: https://www.codewars.com/kata/5861487fdb20cff3ab000030/train/csharp

    // https://www.codewars.com/kata/search/my-languages?q=Esolang%20Interpreters&&beta=false

    // C# String To Byte Array
    // https://www.c-sharpcorner.com/article/c-sharp-string-to-byte-array/
    // https://social.msdn.microsoft.com/Forums/en-US/e2403fd6-61ce-4487-b11a-fddcef40c87f/using-bitarray-with-binarywriter-streams?forum=Vsexpressvcs

    // NOTE: Convertion from bytes to bits array https://stackoverflow.com/questions/40541433/how-to-get-bit-values-from-byte

    // https://stackoverflow.com/questions/3917086/convert-bitarray-to-string

    // TODO: prbl use spans and create Benchmarks, spans and

    // TODO: prbl: use ISO_8859_1
    public static string Interpret(string code, string input = "")
    {
        //Console.WriteLine($"code: {code}\n" + $"input: {string.Join('\t', input.ToCharArray().Select(ch => (int)ch))}");

        //if (input == "\u000d\u000c")
        //    return "\u009c";

        var inputEnumerator = new BitArray(Encoding.Latin1.GetBytes(input)).GetEnumerator();
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
                    {
                        field[current] = false;
                        goto endFor;
                    }

                    field[current] = (bool)inputEnumerator.Current;
                    break;
            }
        }
        endFor:

        return Encoding.Latin1.GetString(new BitArray(output.ToArray()).ToByteArray());

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

    private static byte[] ToByteArray(this BitArray bits)
    {
        if (bits.Length == 0)
            return new byte[0];

        // Math.Max(1, bits.Length / 8) // (bits.Length - 1) / 8 + 1
        byte[] tmp = new byte[(bits.Length - 1) / 8 + 1];
        bits.CopyTo(tmp, 0);

        return tmp;
    }
}
