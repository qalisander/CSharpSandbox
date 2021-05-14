using Experiments.Completed;
using Newtonsoft.Json;
using NUnit.Framework;

namespace NTests.Completed
{
    // https://swisslife-oss.github.io/snapshooter/

    [TestFixture]
    public class LastDigitOfHugeNumber
    {
        [Test]
        [TestCase(new int [0], 1)]
        [TestCase(new [] { 0, 0 }, 1)]
        [TestCase(new [] { 0, 0, 0 }, 0)]
        [TestCase(new [] { 1, 2 }, 1)]
        [TestCase(new [] { 3, 4, 5 }, 1)]
        [TestCase(new [] { 4, 3, 6 }, 4)]
        [TestCase(new [] { 7, 6, 21 }, 1)]
        [TestCase(new [] { 12, 30, 21 }, 6)]
        [TestCase(new [] { 2, 2, 2, 0 }, 4)]
        [TestCase(new [] { 2, 14, 2, 0 }, 4)]
        [TestCase(new [] { 937640, 767456, 981242 }, 0)]
        [TestCase(new [] { 123232, 694022, 140249 }, 6)]
        [TestCase(new [] { 499942, 898102, 846073 }, 6)]
        [TestCase(new [] { 590492, 221098 }, 4)]
        [TestCase(new [] { 2, 2 }, 4)]
        public void SampleTest(int [] arr, int expected)
        {
            Assert.AreEqual(expected, Experiments.Completed.LastDigitOfHugeNumber.LastDigit(arr));
        }

        [Test]
        [TestCase(new int [0], 1)]
        [TestCase(new [] { 0, 0 }, 1)]
        [TestCase(new [] { 0, 0, 0 }, 0)]
        [TestCase(new [] { 1, 2 }, 1)]
        [TestCase(new [] { 3, 4, 5 }, 1)]
        [TestCase(new [] { 4, 3, 6 }, 4)]
        [TestCase(new [] { 7, 6, 21 }, 1)]
        [TestCase(new [] { 12, 30, 21 }, 6)]
        [TestCase(new [] { 2, 2, 2, 0 }, 4)]
        [TestCase(new [] { 2, 14, 2, 0 }, 4)]
        [TestCase(new [] { 937640, 767456, 981242 }, 0)]
        [TestCase(new [] { 123232, 694022, 140249 }, 6)]
        [TestCase(new [] { 499942, 898102, 846073 }, 6)]
        [TestCase(new [] { 590492, 221098 }, 4)]
        [TestCase(new [] { 2, 2 }, 4)]
        public void SampleTest2(int [] arr, int expected)
        {
            Assert.AreEqual(expected, Calculator2.LastDigit(arr));
        }

        [Test]
        [TestCase(new int [0], 1)]
        [TestCase(new [] { 0, 0 }, 1)]
        [TestCase(new [] { 0, 0, 0 }, 0)]
        [TestCase(new [] { 1, 2 }, 1)]
        [TestCase(new [] { 3, 4, 5 }, 1)]
        [TestCase(new [] { 4, 3, 6 }, 4)]
        [TestCase(new [] { 7, 6, 21 }, 1)]
        [TestCase(new [] { 12, 30, 21 }, 6)]
        [TestCase(new [] { 2, 2, 2, 0 }, 4)]
        [TestCase(new [] { 2, 14, 2, 0 }, 4)]
        [TestCase(new [] { 937640, 767456, 981242 }, 0)]
        [TestCase(new [] { 123232, 694022, 140249 }, 6)]
        [TestCase(new [] { 499942, 898102, 846073 }, 6)]
        [TestCase(new [] { 590492, 221098 }, 4)]
        [TestCase(new [] { 2, 2 }, 4)]
        public void SampleTest3(int [] arr, int expected)
        {
            Assert.AreEqual(expected, Calculator3.LastDigit(arr));
        }

        [Test]
        [TestCase("[[0],[1],[4,8,6,2],[9,7,1,3],[6,4],[5],[6],[9,3,1,7],[4,2,6,8],[1,9]]", 10)]
        //[TestCase("[[0],[1],[6,2,4,8],[1,3,9,7],[6,4],[5],[6],[1,7,9,3],[6,8,4,2],[1,9]]", 10)]
        public void GetPeriodsTest(string expected, int num)
        {
            Assert.AreEqual(expected, 
                JsonConvert.SerializeObject(Experiments.Completed.LastDigitOfHugeNumber.GetPeriods(num)));
        }

        [Test]
        [TestCase("[4,8,6,2]", 2, 10)]
        [TestCase("[4,2,6,8]", 8, 10)]
        [TestCase("[1,3]", 3, 4)]
        [TestCase("[0]", 2, 4)]
        public void GetPeriodSimpleTest(string expected, int num, int mod)
        {
            Assert.AreEqual(expected, JsonConvert.SerializeObject(Experiments.Completed.LastDigitOfHugeNumber.GetPeriod(num, mod)));
        }

        [Test]
        [TestCase("[1,2,3,4,5,6]", "[5,6,1,2,3,4]", -2)]
        [TestCase("[1,2,3,4,5,6]", "[1,2,3,4,5,6]", 0)]
        [TestCase("[1,2,3,4,5,6]", "[4,5,6,1,2,3]", 3)]
        public void ShiftByTest(string arg, string expected, int shift)
        {
            var arr = JsonConvert.DeserializeObject<int[]>(arg);

            Assert.AreEqual(expected, JsonConvert.SerializeObject(arr!.ShiftBy(shift)));
        }

        [Test]
        public void GetPeriodsStraightforwardTest()
        {
            var periods = Experiments.Completed.LastDigitOfHugeNumber.GetPeriodsStraightforward();
        }

        [Test]
        public void GetPeriodsDoWhileTest()
        {
            Assert.AreEqual(
                JsonConvert.SerializeObject(Experiments.Completed.LastDigitOfHugeNumber.GetPeriodsStraightforward()),
                JsonConvert.SerializeObject(Experiments.Completed.LastDigitOfHugeNumber.GetPeriods()));
        }

        [Test]
        public void GetPeriodsIterTest()
        {
            Assert.AreEqual(
                JsonConvert.SerializeObject(Experiments.Completed.LastDigitOfHugeNumber.GetPeriodsStraightforward()),
                JsonConvert.SerializeObject(Experiments.Completed.LastDigitOfHugeNumber.GetPeriodsIter()));
        }

        [Test]
        public void GetPeriodsIterRecTest()
        {
            Assert.AreEqual(
                JsonConvert.SerializeObject(Experiments.Completed.LastDigitOfHugeNumber.GetPeriodsStraightforward()),
                JsonConvert.SerializeObject(Experiments.Completed.LastDigitOfHugeNumber.GetPeriodsIterRec()));
        }

        [Test]
        public void GetPeriodsMod()
        {
            var json = JsonConvert.SerializeObject(Experiments.Completed.LastDigitOfHugeNumber.GetPeriods(10));

            var periods10 = Experiments.Completed.LastDigitOfHugeNumber.GetPeriods(10);
            var periods4 = Experiments.Completed.LastDigitOfHugeNumber.GetPeriods(4);
            var periods3 = Experiments.Completed.LastDigitOfHugeNumber.GetPeriods(3);
            var periods2 = Experiments.Completed.LastDigitOfHugeNumber.GetPeriods(2);
        }
    }
}
