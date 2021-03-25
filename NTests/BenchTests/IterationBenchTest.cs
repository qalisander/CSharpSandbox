using BenchmarkDotNet.Running;
using Experiments.Benchmark;
using NUnit.Framework;

namespace NTests.BenchTests
{
    [TestFixture]
    public class IterationBenchTest
    {

        [Test]
        public void IterationBenchmarkTest()
        {
            BenchmarkRunner.Run<IterationBenchmark>();
        }
    }
}
