using Experiments;
using FluentAssertions;
using NUnit.Framework;

namespace NTests
{
    [TestFixture]
    public class ImmortalTest
    {
        // [Ignore("Not Implemented")]
        [Test]
        [TestCase(5, 8, 5, 1, 100)]
        [TestCase(224, 8, 8, 0, 100007)]
        [TestCase(11925, 25, 31, 0, 100007)]
        [TestCase(4323, 5, 45, 3, 1000007)]
        [TestCase(1586, 31, 39, 7, 2345)]
        [TestCase(808451, 545, 435, 342, 1000007)]
        [TestCase(5456283, 28_827_050_410L, 35_165_045_587L, 7109602, 13719506)]
        public void ElderAgeTest(long ans, long N, long M, long deduction, long mod)
        {
            
            Assert.AreEqual(ans, Immortal.ElderAge(N, M, deduction, mod));

            // Assert.AreEqual((long) 5, Immortal.ElderAge(8, 5, 1, 100));
            // Assert.AreEqual((lopng) 224, Immortal.ElderAge(8, 8, 0, 100007));
            // Assert.AreEqual((long) 11925, Immortal.ElderAge(25, 31, 0, 100007));
            // Assert.AreEqual((long) 4323, Immortal.ElderAge(5, 45, 3, 1000007));
            // Assert.AreEqual((long) 1586, Immortal.ElderAge(31, 39, 7, 2345));
            // Assert.AreEqual((long) 808451, Immortal.ElderAge(545, 435, 342, 1000007));
            // Assert.AreEqual((long) 5456283, Immortal.ElderAge(28_827_050_410L, 35_165_045_587L, 7109602, 13719506));
        }

        [Test]
        [TestCase(10, 2, 10, 100500, 1)] // 0 + 1 mod 100500 = 1
        [TestCase(5, 14, 10, 5, 1)]      // 0 ... 0 + 1 + 2 + 3 + 4 + 5 + 6 + 7 + 8 mod 5 = 1   
        [TestCase(5, 14, 11, 7, 0)]      // 0 ... 0 + 1 + 2 + 3 + 4 + 5 + 6 + 7 mod 7 = 0
        [TestCase(0, 8, 1, 100, 21)]      // 0 ... 0 + 1 + 2 + 3 + 4 + 5 + 6 mod 100 = 21
        public void SumRangeTest(long numFrom, long count, long deduction, long mod, long expected)
        {
            Immortal.SumRange((ulong)numFrom, (ulong)count, (ulong)deduction, (ulong)mod).Should().Be((ulong)expected);
        }
    }
}
