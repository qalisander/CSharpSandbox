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
        [TestCase("0 0 1 2 1\n1 2 1 3 1\n1 2 2 5 2", "(0; 0; 1; 0) + q1* (-2; 1; 0; 0) + q2* (-1; 0; -2; 1)")]
        [TestCase("0 -6 -7 -24 3 27 -28 27/23 17 -25/14", "(0;25/84;0;0;0;0;0;0;0)+q1*(1;0;0;0;0;0;0;0;0)+q2*(0;-7/6;1;0;0;0;0;0;0)+q3*(0;-4;0;1;0;0;0;0;0)+q4*(0;1/2;0;0;1;0;0;0;0)+q5*(0;9/2;0;0;0;1;0;0;0)+q6*(0;-14/3;0;0;0;0;1;0;0)+q7*(0;9/46;0;0;0;0;0;1;0)+q8*(0;17/6;0;0;0;0;0;0;1)")]
        [TestCase("6 -4 11 -30 -14 -18/17 18 -26\n"+
                  "-5 -12/19 -9/14 -25 19 -15 -11/28 10/7\n"+
                  "-1 -15/16 23 6 20 27/19 -18 -5/7\n"+
                  "-9 -8 -16 -17 28 7/5 -29 -19/20", "(-4742056001/2726794875; 4762942192/2726794875; -78265541/779084250; 388177879/1558168500; 0; 0; 0) + q1* (301642916/77908425; 40548128/77908425; -7539122/11129775; -112216/11129775; 1; 0; 0) + q2* (-229127684/238751625; 435035504/213619875; 58006876/579825375; -267927247/579825375; 0; 1; 0) + q3* (-236909687/89038200; -29685562/11129775; 9096257/22259550; 51046759/89038200; 0; 0; 1)")]
        public void SolveTest(string input, string expected)
        {
            LinearSystem ls = new LinearSystem();
            string result = ls.Solve(input);
            Assert.AreEqual("SOL=" + expected.Replace(" ", ""), result.Replace(" ", ""));
        }
    }
}
