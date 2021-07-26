using System.IO;
using System.Runtime.CompilerServices;
using Experiments;
using NUnit.Framework;

namespace NTests
{
    [TestFixture]
    public class DinglemouseTest
    {
        [Test]
        [TestCase(3, "KataTrains_SimpleCircle.txt", "Aaaa", 2, "bbbbbbbbbbB", 28, 1000)]
        [TestCase(5, "KataTrains_SimpleCrossing.txt", "aaaA", 31, "Bbbbbbbbbbb", 7, 1000)]
        [TestCase(13, "KataTrains_SimpleStation.txt", "Aaaa", 6, "bbbbbbbbbbbbbB", 36, 1000)]
        [TestCase(516, "KataTrains.txt", "Aaaa", 147, "Bbbbbbbbbbb", 288, 1000)]
        [TestCase(516, "KataTrains_NoTrainSymbols.txt", "Aaaa", 147, "Bbbbbbbbbbb", 288, 1000)]
        public void Example(
            int expected, string TestFile, string aTrain, int aTrainPos, string bTrain, int bTrainPos, int limit)
        {
            Dinglemouse.HasPrint = true;
            string track = File.ReadAllText(Path.Combine(GetCurrentFileDir(), TestFile));

            Assert.AreEqual(expected, Dinglemouse.TrainCrash(track, aTrain, aTrainPos, bTrain, bTrainPos, limit));
        }

        private string GetCurrentFileDir([CallerFilePath] string path = null) => Path.GetDirectoryName(path);
    }
}
