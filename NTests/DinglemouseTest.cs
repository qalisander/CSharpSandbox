using System.IO;
using Experiments.Completed;
using Microsoft.VisualBasic;
using NUnit.Framework;

namespace NTests
{
    [TestFixture]
    public class DinglemouseTest
    {
        public const string TEST_FILES_DIR =
            @"C:\Users\kosha\source\repos\CodeLab\NTests\TestFiles";

        [Test]
        [TestCase("KataTrains.txt", "Aaaa", 147, "Bbbbbbbbbbb", 288, 1000)]
        public void Example(
            string TestFile, string aTrain, int aTrainPos, string bTrain, int bTrainPos, int limit)
        {
            string track = File.ReadAllText(Path.Combine(TEST_FILES_DIR, TestFile));

            Assert.AreEqual(
                516,
                Dinglemouse.TrainCrash(track, aTrain, aTrainPos, bTrain, bTrainPos, limit));
        }
    }
}
