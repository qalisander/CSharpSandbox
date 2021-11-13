using NUnit.Framework;
using System;
using Problems.Completed;

namespace NTests.Completed
{
    public class LinearEquationsTest
    {
        [Test]
        public void FractionTest()
        {
            var fr15By10 = new Fraction(15, 10);
            var fr5By2 = new Fraction(5, 2);

            Assert.AreEqual(fr15By10 - fr5By2, new Fraction(-1));
            Assert.AreEqual(fr15By10 * fr5By2, new Fraction(75, 20));
            Assert.AreEqual(fr15By10 / fr5By2, new Fraction(3, 5));
            Assert.AreEqual(fr15By10 * Fraction.Zero, Fraction.Zero);
        }

        [Test]
        [TestCase("1 2 0 0 7\n0 3 4 0 8\n0 0 5 6 9", "(97/15; 4/15; 9/5; 0) + q1* (-16/5; 8/5; -6/5; 1)")]
        [TestCase("1 2 3 4\n6 6 7 8\n9 10 11 12", "(0; -1; 2)")]
        [TestCase("1 2 3 4", "(4; 0; 0) + q1 * (-2; 1; 0) + q2 * (-3; 0; 1)")]
        [TestCase("3/2 1/2 3", "(2; 0) + q1 * (-1/3; 1)")]
        [TestCase("1 2 2\n1 2 2\n2 4 4", "(2; 0) + q1 * (-2; 1)")]
        [TestCase("0 0 0\n0 0 0", "(0; 0) + q1 * (1; 0) + q2 * (0; 1)")]
        [TestCase("0 0 0 0\n0 0 0 0", "(0; 0; 0) + q1 * (1; 0; 0) + q2 * (0; 1; 0) + q3 * (0; 0; 1)")]
        [TestCase("0 0 1 2 1\n1 2 1 3 1\n1 2 2 5 3", "NONE")]
        [TestCase("1/20 -10/3 -10/9 -13\n-29 8 -27/4 0\n-26 -14 25 10/7", "(3343180/9270107; 4197595/1324301; 20461200/9270107)")]
        public void SolveTest(string input, string ans)
        {
            LinearSystem ls = new LinearSystem();
            string result = ls.Solve(input);
            Assert.AreEqual(ans.Replace(" ", ""), result.Replace(" ", ""));
        }
    }
}
