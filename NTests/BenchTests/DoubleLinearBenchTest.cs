using BenchmarkDotNet.Running;
using Experiments.Completed;
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
