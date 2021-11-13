using BenchmarkDotNet.Running;
using Problems.Completed;

namespace Problems.Benchmark
{
    public class Benchmark
    {
        //// https://benchmarkdotnet.org/articles/guides/getting-started.html
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<LinearSystem>();
        }
    }
}
