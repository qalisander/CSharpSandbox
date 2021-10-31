using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Problems.Completed
{
    public static class Boolfuck
    {
        // https://www.codewars.com/kata/5861487fdb20cff3ab000030/train/csharp
        public static string Interpret(string code, string input = "") =>
            Encoding.GetEncoding("ISO-8859-1")
                    .GetString(new BitArray(InterpretInner(code, input).ToArray()).ToByteArray());

        private static IEnumerable<bool> InterpretInner(string code, string input)
        {
            // NOTE: Convertion from bytes to bits array https://stackoverflow.com/questions/40541433/how-to-get-bit-values-from-byte
            var inputEnumerator = new BitArray(Encoding.GetEncoding("ISO-8859-1").GetBytes(input)).GetEnumerator();

            var field = new Dictionary<int, bool> { { 0, false } };
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
                        yield return field[current];
                        break;
                    case ',':
                        if (!inputEnumerator.MoveNext())
                        {
                            field[current] = false;

                            yield break;
                        }
                        field[current] = (bool) inputEnumerator.Current;
                        break;
                }
            }
        }

        private static IEnumerable<KeyValuePair<int, int>> ProcessBrackets(string code)
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

        private static byte[] ToByteArray(this BitArray bits)
        {
            if (bits.Length == 0)
                return new byte[0];

            byte[] tmp = new byte[(bits.Length - 1) / 8 + 1]; // Math.Max(1, bits.Length / 8)
            bits.CopyTo(tmp, 0);

            return tmp;
        }
    }
}
