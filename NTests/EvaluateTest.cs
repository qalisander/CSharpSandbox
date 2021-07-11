using System.Linq;
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
        public void Eval_ScanTest()
        {
            var tokens = ev.Scan("abs ( - 1)* 2.3e-4");
            tokens.Should().HaveCount(7);
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
