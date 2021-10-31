using System;
using System.IO;
using System.Linq;
using Problems.Completed;
using Xunit;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace XTests
{
    public class SumOfDividedTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public SumOfDividedTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Test1()
        {
            int[] lst = new int[] { 12, 15 };
            Assert.Equal("(2 12)(3 27)(5 15)", SumOfDivided.GetSumOfDivided(lst));
        }

        [Fact]
        public void Test2()
        {
            int[] lst = new int[] { 12, 15, 21  };
            Assert.Equal("(2 12)(3 48)(5 15)(7 21)", SumOfDivided.GetSumOfDivided(lst));
        }

        [Fact]
        public void GeneratePrimeNumbersTest1()
        {
            var primeNumbers = new[] { 1, 2, 3, 5, 7, 11, 13, 17, 19, 23, 29 };
            var ans = SumOfDivided.GeneratePrimeNumbers(30).ToArray();

            Assert.Equal(JsonConvert.SerializeObject(primeNumbers), JsonConvert.SerializeObject(ans));
        }

        [Fact(Skip = "slow")]
        public void GeneratePrimeNumbersTest2()
        {
            var ans = SumOfDivided.GeneratePrimeNumbers(10_000_000).ToArray();

            Assert.Equal(ans[0], 1);

            File.WriteAllText(
                Path.Combine(Environment.CurrentDirectory, "output.txt"),
                JsonConvert.SerializeObject(ans));
        }
    }
}
