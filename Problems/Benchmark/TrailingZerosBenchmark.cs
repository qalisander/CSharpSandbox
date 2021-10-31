namespace Problems.Benchmark
{
    // https: //benchmarkdotnet.org/articles/guides/choosing-run-strategy.html
    // [SimpleJob(RunStrategy.ColdStart, targetCount: 5)]
    // [MinColumn, MaxColumn, MeanColumn, MedianColumn]
    // public class TrailingZerosBenchmark
    // {
    //     [Params(1_000_000)] public int prm;
    //
    //     [Benchmark]
    //     public void For()
    //     {
    //         TrailingZerosKata.TrailingZerosFor(prm);
    //     }
    //
    //     [Benchmark]
    //     public void ForMethodImplAnd()
    //     {
    //         TrailingZerosKata.TrailingZerosForMethodImpl(prm);
    //     }
    //
    //     [Benchmark]
    //     public void Range()
    //     {
    //         TrailingZerosKata.TrailingZerosRange(prm);
    //     }
    //
    //     [Benchmark]
    //     public void RangeMethodImpl()
    //     {
    //         TrailingZerosKata.TrailingZerosRangeMethodImpl(prm);
    //     }
    // }
}
