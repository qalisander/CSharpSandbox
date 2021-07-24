using System.IO;
using System.Runtime.CompilerServices;
using Experiments.Completed;
using NUnit.Framework;

namespace NTests
{
    [TestFixture]
    public class DinglemouseTest
    {
        [Test]
        [TestCase(3, "KataTrains_SimpleCircle.txt", "Aaaa", 0, "Bbbbbbbbbbb", 0, 1000)]
        [TestCase(5, "KataTrains_SimpleCrossing.txt", "Aaaa", 147, "Bbbbbbbbbbb", 288, 1000)]
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
