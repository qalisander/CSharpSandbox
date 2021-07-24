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
        [TestCase(3, "KataTrains_SimpleCircle.txt", "aaaA", -1, "Bbbbbbbbbbb", -1, 1000)]
        [TestCase(5, "KataTrains_SimpleCrossing.txt", "Aaaa", -1, "bbbbbbbbbbB", -1, 1000)]
        [TestCase(13, "KataTrains_SimpleStation.txt", "aaaA", -1, "Bbbbbbbbbbbbbb", -1, 1000)]
        [TestCase(516, "KataTrains.txt", "Aaaa", 147, "Bbbbbbbbbbb", 288, 1000)]
        public void Example(
            int expected, string TestFile, string aTrain, int aTrainPos, string bTrain, int bTrainPos, int limit)
        {
            string track = File.ReadAllText(Path.Combine(GetCurrentFileDir(), TestFile));

            Assert.AreEqual(expected, Dinglemouse.TrainCrash(track, aTrain, aTrainPos, bTrain, bTrainPos, limit));
        }

        private string GetCurrentFileDir([CallerFilePath] string path = null) => Path.GetDirectoryName(path);
    }
}
