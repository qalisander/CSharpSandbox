using Problems.Completed;
using NTests.Base;
using NUnit.Framework;

namespace NTests.Completed
{
    //https://fluentassertions.com/introduction
    [TestFixture]
    [Timeout(10000)]
    public class TrailingZeros : StopwatchTests
    {
        [Test]
        [TestCase(1, 5)]
        [TestCase(2, 12)]
        [TestCase(6, 25)]
        [TestCase(131, 531)]
        [TestCase(0, -20)]
        [TestCase(249_999_998, 1_000_000_000)]
        [TestCase(499999997, 2_000_000_000)]
        [TestCase(536870902, int.MaxValue)]
        public void ForMethodImplTests(int expected, int arg)
        {
            Assert.That(TrailingZerosKata.TrailingZerosForMethodImpl(arg), Is.EqualTo(expected));
            // Assert.AreEqual(expected, TrailingZerosKata.TrailingZerosForMethodImpl(arg));
            // TrailingZerosKata.TrailingZerosForMethodImpl(arg).Should().Be(expected);
        }

        [Test]
        [TestCase(1, 5)]
        [TestCase(2, 12)]
        [TestCase(6, 25)]
        [TestCase(131, 531)]
        [TestCase(0, -20)]
        [TestCase(249_999_998, 1_000_000_000)]
        [TestCase(499999997, 2_000_000_000)]
        [TestCase(536870902, int.MaxValue)]
        public void RangeTests(int expected, int arg)
        {
            Assert.That(TrailingZerosKata.TrailingZerosRange(arg), Is.EqualTo(expected));
            // Assert.AreEqual(expected, TrailingZerosKata.TrailingZerosFor(arg));
            // TrailingZerosKata.TrailingZerosRange(arg).Should().Be(expected);
        }

        [Test]
        [TestCase(1, 5)]
        [TestCase(2, 12)]
        [TestCase(6, 25)]
        [TestCase(131, 531)]
        [TestCase(0, -20)]
        [TestCase(249_999_998, 1_000_000_000)]
        [TestCase(499999997, 2_000_000_000)]
        [TestCase(536870902, int.MaxValue)]
        public void TrailingZerosTests(int expected, int arg)
        {
            Assert.That(TrailingZerosKata.TrailingZeros(arg), Is.EqualTo(expected));
            // Assert.AreEqual(expected, TrailingZerosKata.TrailingZerosFor(arg));
            // TrailingZerosKata.TrailingZerosRange(arg).Should().Be(expected);
        }
    }
}
