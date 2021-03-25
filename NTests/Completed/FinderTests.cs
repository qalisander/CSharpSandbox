using Experiments;
using NUnit.Framework;

namespace NTests
{
    [TestFixture]
    public class FinderTests
    {
        const string a = ".W.\n" +
                         ".W.\n" +
                         "...";
        
        const string b = ".W.\n" +
                         ".W.\n" +
                         "W..";
        
        const string c = "......\n" +
                         "......\n" +
                         "......\n" +
                         "......\n" +
                         "......\n" +
                         "......";
        
        const string d = "......\n" +
                         "......\n" +
                         "......\n" +
                         "......\n" +
                         ".....W\n" +
                         "....W.";

        private const string e = ".W...\n" +
                                 ".W...\n" +
                                 ".W.W.\n" +
                                 "...W.\n" +
                                 "...W.";

        private const string one = ".";


        [Test]
        [TestCase(true, a)]
        [TestCase(false, b)]
        [TestCase(true, c)]
        [TestCase(false, d)]
        [TestCase(true, e)]
        [TestCase(true, one)]
        public void SampleTests(bool expected, string arg)
        {
            Assert.AreEqual(expected, Finder.PathFinder(arg));
        }
    }
}
