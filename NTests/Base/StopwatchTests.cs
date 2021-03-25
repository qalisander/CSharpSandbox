using System;
using System.Diagnostics;
using NUnit.Framework;

namespace NTests.Base
{
    [TestFixture]
    public class StopwatchTests
    {
        private Stopwatch _stopwatch;

        [SetUp]
        public void Init()
        {
            _stopwatch = Stopwatch.StartNew();
        }

        [TearDown]
        public void Cleanup()
        {
            _stopwatch.Stop();

            Console.WriteLine(
                "Execution time for {0} - {1} ms",
                TestContext.CurrentContext.Test.Name,
                _stopwatch.ElapsedMilliseconds);
        }
    }
}
