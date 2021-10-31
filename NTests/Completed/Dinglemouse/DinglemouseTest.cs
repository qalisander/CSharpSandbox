using System.IO;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace NTests.Completed.Dinglemouse
{
    [TestFixture]
    public class DinglemouseTest
    {
        [Test]
        [TestCase(3, "trains_SimpleCircle.txt", "Aaaa", 2, "bbbbbbbbbbB", 28, 1000)]
        [TestCase(5, "trains_SimpleCrossing.txt", "aaaA", 31, "Bbbbbbbbbbb", 7, 1000)]
        [TestCase(13, "trains_SimpleStation.txt", "Aaaa", 6, "bbbbbbbbbbbbbB", 36, 1000)]
        [TestCase(516, "trains.txt", "Aaaa", 147, "Bbbbbbbbbbb", 288, 1000)]
        [TestCase(-1, "trains_NearMissSelf.txt", "aA", 10, "oooooooooooooooooooooO", 70, 200)]
        [TestCase(-1, "trains_NoCrashOTricky.txt", "aaaA", 15, "bbbB", 5, 100)]
        [TestCase(0, "trains_Random.txt", "xxxxxxX", 11, "Cccccccccc", 42, 2000)]
        [TestCase(0, "trains_CrashBeforeStarted.txt", "oO", 10, "oO", 10, 100)]
        [TestCase(16, "trains_Limits.txt", "aaaA", 22, "bbbbB", 0, 16)]
        [TestCase(-1, "trains_NoCrash0Tricky_2.txt", "aaaA", 15, "bbbB", 5, 1000)]
        public void Example(
            int expected, string TestFile, string aTrain, int aTrainPos, string bTrain, int bTrainPos, int limit)
        {
            Problems.Completed.Dinglemouse.HasPrint = true;
            string track = File.ReadAllText(Path.Combine(GetCurrentFileDir(), TestFile));

            Assert.AreEqual(expected, Problems.Completed.Dinglemouse.TrainCrash(track, aTrain, aTrainPos, bTrain, bTrainPos, limit));
        }

        private string GetCurrentFileDir([CallerFilePath] string path = null) => Path.GetDirectoryName(path);
    }
}
