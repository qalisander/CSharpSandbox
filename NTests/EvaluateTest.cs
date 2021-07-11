using System;
using System.ComponentModel.DataAnnotations;
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
        public void Eval_Test1()
        {
            Assert.AreEqual("18", ev.Eval("2*3&2"));
        }

        [Test]
        public void Eval_Test2()
        {
            Assert.AreEqual("-240", ev.Eval("(-(2 + 3)* (1 + 2)) * 4 & 2"));
        }
    
        [Test]
        public void Eval_Test3()
        {
            Assert.AreEqual("ERROR", (ev.Eval("sqrt(-2)*2")+"     ").Substring(0,5).ToUpper());
        }
    
        [Test]
        public void Eval_Test4()
        {
            Assert.AreEqual("ERROR", (ev.Eval("2*5/0")+"     ").Substring(0,5).ToUpper());
        }
    
        [Test]
        public void Eval_Test5()
        {
            Assert.AreEqual("-3906251", ev.Eval("-5&3&2*2-1"));
        }
    
        [Test]
        public void Eval_Test6()
        {
            Assert.AreEqual("169", ev.Eval("abs(-(-1+(2*(4--3)))&2)"));
        }

        [Test]
        [TestCase("abs ( - 1)* 2.3e-4", 7)]
        [TestCase("abs ( -- 1&6        ) * 2e423", 10)]
        [TestCase("cos cosh cos h", 10)]
        public void Eval_ScanTest_Smoke(string equation, int tokenCount)
        {
            var tokens = ev.Scan(equation);
            Console.WriteLine(equation);
            Console.WriteLine(string.Join("; ", tokens));
            tokens.Should().HaveCount(tokenCount);
        }

        [Test]
        [TestCase("abs ( -- 1&6   %     ) * 2e423")]
        [TestCase("a bs ( -- 1&6        ) * 2e423")]
        public void Eval_ScanTest_Fail(string equation)
        {
            Action act = () => ev.Scan(equation).ToArray();
            act.Should().Throw<InvalidOperationException>();
        }

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

        // [Test]
        // [TestCase(new [] { ")", "[]", "var", }, "[] dfvf dfvardf))",
        //     new [] { "[]", "dfvf", "df", "var", "df", ")", ")" })]  
        // [TestCase(new [] { ")", "[]", "vardf", }, "[] dfvf dfvardf))",
        //     new [] { "[]", "dfvf", "df", "vardf", ")", ")" })]  
        // [TestCase(new [] { "12345", "3456789" }, "123456789",
        //     new [] { "12", "345", "6789"})] 
        // [TestCase(new [] { "cos", "cosh", "+" }, "cosh+1223",
        //     new [] { "cosh", "+", "1223"})]
        //
        // public void SplitIncludeTest(string[] separators, string str, string[] ans)
        // {
        //     Evaluate.SplitInclude(separators, str).Should()
        //             .BeEquivalentTo(ans, options => options.WithoutStrictOrdering());
        // }
    }
}
