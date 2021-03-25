using BenchmarkDotNet.Running;
using Experiments.Completed;

namespace Experiments.Benchmark
{
    public class Benchmark
    {
        // https://benchmarkdotnet.org/articles/guides/getting-started.html
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<TrailingZerosKata>();
        }
    }
}
