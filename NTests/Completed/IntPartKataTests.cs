using System;
using Experiments.Completed;
using NTests.Base;
using NUnit.Framework;

namespace NTests.Completed
{
    [TestFixture]
    public class IntPartKataTests : StopwatchTests
    {
        [Test, Timeout(12_000)]
        [TestCase("Range: 1 Average: 1.50 Median: 1.50", 2)]
        [TestCase("Range: 2 Average: 2.00 Median: 2.00", 3)]
        [TestCase("Range: 3 Average: 2.50 Median: 2.50", 4)]
        [TestCase("Range: 5 Average: 3.50 Median: 3.50", 5)]
        [TestCase("Range: 1457 Average: 268.11 Median: 152.00", 20)]        //85 ms ->
        [TestCase("Range: 8747 Average: 1126.14 Median: 500.00", 25)]       //2032 ms -> 
        [TestCase("Range: 2125763 Average: 85158.49 Median: 14250.00", 40)]
        [TestCase("Range: 86093441 Average: 1552316.81 Median: 120960.00", 50)]
        public void Test1(string expected, long arg)
        {
            Console.WriteLine("****** Basic Tests Small Numbers");
            Assert.AreEqual(expected, IntPartitions.Part(arg));
        }
    }
}
