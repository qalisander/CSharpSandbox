using System.Linq;
using Experiments.Completed;
using Newtonsoft.Json;
using NUnit.Framework;

namespace NTests.Completed
{
    [TestFixture]
    public class SumOfDividedTests
    {

        [Test]
        public void Test1()
        {
            int[] lst = new int[] { 12, 15 };
            Assert.AreEqual("(2 12)(3 27)(5 15)", SumOfDivided.GetSumOfDivided(lst));
        }

        [Test]
        public void Test2()
        {
            var primeNumbers = new[] { 1, 2, 3, 5, 7, 11, 13, 17, 19 };
            var ans = SumOfDivided.GeneratePrimeNumbers(20).ToArray();

            Assert.AreEqual(primeNumbers, ans);

            Assert.IsTrue(
                primeNumbers.SequenceEqual(ans),
                $"Ans: {string.Join('\t', ans)} \n"
                + $"not equal to result {string.Join('\t', primeNumbers)}");
        }

        [Test]
        public void Test3()
        {
            var primeNumbers = new[] { 1, 2, 3, 5, 7, 11, 13, 17, 19 };
            var ans = SumOfDivided.GeneratePrimeNumbers(30).ToArray();

            Assert.AreEqual(
                JsonConvert.SerializeObject(primeNumbers),
                JsonConvert.SerializeObject(ans));
        }
    }
}
