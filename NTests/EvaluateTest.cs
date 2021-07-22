using System;
using System.Linq;
using System.Text.RegularExpressions;
using Experiments;
using NUnit.Framework;
using FluentAssertions;

namespace NTests
{
    // Fluent assertion: https://fluentassertions.com/introduction
    [TestFixture]
    public class EvaluateTest
    {
        private Evaluate ev = new Evaluate();

        [Test]
        [TestCase("2*3&2", "18")]
        [TestCase("(2*3&2)", "18")]
        [TestCase("-2*3&2 + 1", "-17")]
        [TestCase("-2 + 2", "0")]
        [TestCase("(-(2 + 3)* (1 + 2)) * 4 & 2", "-240")]
        [TestCase("sqrt(-2)*2", "ERROR")]
        [TestCase("2*5/0", "ERROR")]
        [TestCase("-5&3&2*2-1", "-3906251")]
        [TestCase("abs(-(-1+(2*(4--3)))&2)", "169")]
        [TestCase("abs(-(-1+(2*(4--3))))&2)", "ERROR")]
        [TestCase("abs(-2 * 1e-3)", "0.002")]
        [TestCase("-2&2", "-4")]
        public void Eval_Test1(string expr, string expected)
        {
            Console.WriteLine(expr);
            ev.Print(expr);
            ev.Eval(expr).Should().Be(expected);
        }

        [Test]
        [TestCase("abs ( - 1)* 2.3e-4", 7)]
        [TestCase("abs ( -- 1&6        ) * 2e423", 10)]
        public void Eval_ScanTest_Smoke(string equation, int tokenCount)
        {
            var tokens = ev.Scan(equation);
            Console.WriteLine(equation);
            Console.WriteLine(string.Join("\n", tokens));
            tokens.Should().HaveCount(tokenCount);
        }

        [Test]
        [TestCase("abs ( -- 1&6   %#$     ) * 2e423")]
        [TestCase("a bs ( -- 1&6        ) * 2e423")]
        [TestCase("cos cosh cos h")]
        public void Eval_ScanTest_Fail(string equation)
        {
            Action act = () => ev.Scan(equation).ToArray();
            act.Should().Throw<InvalidTokenException>();
        }

        [Ignore("regexp test")]
        [Test]
        public void Regexp_test()
        {
            //language=regexp
            string pattern = @"(?<duplicateWord>\w+)\s\k<duplicateWord>\W(?<nextWord>\w+)|(?<nsw>nsw)";
            string input = "He said that that was the the correct answer.";
            foreach (Match match in Regex.Matches(input, pattern, RegexOptions.IgnoreCase))
                Console.WriteLine("A duplicate '{0}' at position {1} is followed by '{2}'.",
                    match.Groups["duplicateWord"].Value, 
                    match.Groups["duplicateWord"].Index,
                    match.Groups["nextWord"].Value);
        }
    }
}
