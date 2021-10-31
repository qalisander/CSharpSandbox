using BenchmarkDotNet.Running;
using Problems.Completed;
using NUnit.Framework;

namespace NTests.BenchTests
{
    [TestFixture]
    public class DoubleLinearBenchTest
    {
        [Test]
        public void Bench()
        {
            var summary = BenchmarkRunner.Run<DoubleLinear>();
        }
    }
}
