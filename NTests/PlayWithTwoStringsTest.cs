using Experiments;
using NUnit.Framework;

namespace NTests
{
    [TestFixture]
    public class PlayWithTwoStringsTest
    {
        [Test]
        public void PlayWithTwoStrings_withoutRandom1()
        {
            Assert.AreEqual("abCCde", new PlayWithTwoStrings().WorkOnStrings("abc","cde"));
        }
        [Test]
        public void PlayWithTwoStrings_withoutRandom2()
        {
            Assert.AreEqual("ABABbababa", new PlayWithTwoStrings().WorkOnStrings("abab", "bababa"));

        }
        [Test]
        public void PlayWithTwoStrings_withoutRandom3()
        {
            Assert.AreEqual("abcDeFGtrzWDEFGgGFhjkWqE", new PlayWithTwoStrings().WorkOnStrings("abcdeFgtrzw", "defgGgfhjkwqe"));
        }
        [Test]
        public void PlayWithTwoStrings_withoutRandom4()
        {
            Assert.AreEqual("abcDEfgDEFGg", new PlayWithTwoStrings().WorkOnStrings("abcdeFg", "defgG"));
        }
    }
}
