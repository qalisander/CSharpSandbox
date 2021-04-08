using System;
using Newtonsoft.Json;
using NUnit.Framework;

namespace NTests
{
    [TestFixture]
    public class LastDigitOfHugeNumber
    {
        // [Ignore("Not implemented1")]
        [Test]
        // [TestCase(new int[0], 1)]
        // [TestCase(new int[] { 0, 0 }, 1)]
        // [TestCase(new int[] { 0, 0, 0 }, 0)]
        // [TestCase(new int[] { 1, 2 }, 1)]
        // [TestCase(new int[] { 3, 4, 5 }, 1)]
        // [TestCase(new int[] { 4, 3, 6 }, 4)]
        // [TestCase(new int[] { 7, 6, 21 }, 1)]
        // [TestCase(new int[] { 12, 30, 21 }, 6)]
        // [TestCase(new int[] { 2, 2, 2, 0 }, 4)]
        // [TestCase(new int[] { 937640, 767456, 981242 }, 0)]
        [TestCase(new int[] { 123232, 694022, 140249 }, 6)]
        // [TestCase(new int[] { 499942, 898102, 846073 }, 6)]
        public void SampleTest(int [] arr, int expected)
        {
            Assert.AreEqual(expected, Calculator.LastDigit(arr));
        }

        [Test]
        public void GetPeriodsStraightforwardTest()
        {
            var periods = Calculator.GetPeriodsStraightforward();
        }

        [Test]
        public void GetPeriodsDoWhileTest()
        {
            Assert.AreEqual(
                JsonConvert.SerializeObject(Calculator.GetPeriodsStraightforward()),
                JsonConvert.SerializeObject(Calculator.GetPeriods()));
        }

        [Test]
        public void GetPeriodsIterTest()
        {
            Assert.AreEqual(
                JsonConvert.SerializeObject(Calculator.GetPeriodsStraightforward()),
                JsonConvert.SerializeObject(Calculator.GetPeriodsIter()));
        }

        [Test]
        public void GetPeriodsIterRecTest()
        {
            Assert.AreEqual(
                JsonConvert.SerializeObject(Calculator.GetPeriodsStraightforward()),
                JsonConvert.SerializeObject(Calculator.GetPeriodsIterRec()));
        }

        [Test]
        public void GetPeriodsMod()
        {
            var json = JsonConvert.SerializeObject(Calculator.GetPeriods(10));

            var periods10 = Calculator.GetPeriods(10);
            var periods4 = Calculator.GetPeriods(4);
            var periods3 = Calculator.GetPeriods(3);
            var periods2 = Calculator.GetPeriods(2);
        }
    }
}
