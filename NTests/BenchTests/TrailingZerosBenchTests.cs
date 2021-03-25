using System;
using BenchmarkDotNet.Running;
using Experiments.Completed;
using NUnit.Framework;

namespace NTests.BenchTests
{
    [TestFixture]
    public class TrailingZerosBenchTests
    {
        [Test]
        public void TrailingZerosBenchTest()
        {
            var summary = BenchmarkRunner.Run<TrailingZerosKata>();
            
            Console.WriteLine();
            Console.WriteLine(string.Join("; ", summary.BenchmarksCases));
        }
    }
}
