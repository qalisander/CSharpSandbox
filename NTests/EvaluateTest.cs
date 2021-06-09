using Experiments;
using NUnit.Framework;

namespace NTests
{
    [TestFixture]
    public class EvaluateTest
    {
        private Evaluate ev = new Evaluate();

        [Test]
        public void Eval_Test1()
        {
            Assert.AreEqual("18", ev.eval("2*3&2"));
        }

        [Test]
        public void Eval_Test2()
        {
            Assert.AreEqual("-240", ev.eval("(-(2 + 3)* (1 + 2)) * 4 & 2"));
        }
    
        [Test]
        public void Eval_Test3()
        {
            Assert.AreEqual("ERROR", (ev.eval("sqrt(-2)*2")+"     ").Substring(0,5).ToUpper());
        }
    
        [Test]
        public void Eval_Test4()
        {
            Assert.AreEqual("ERROR", (ev.eval("2*5/0")+"     ").Substring(0,5).ToUpper());
        }
    
        [Test]
        public void Eval_Test5()
        {
            Assert.AreEqual("-3906251", ev.eval("-5&3&2*2-1"));
        }
    
        [Test]
        public void Eval_Test6()
        {
            Assert.AreEqual("169", ev.eval("abs(-(-1+(2*(4--3)))&2)"));
        }
    }
}
