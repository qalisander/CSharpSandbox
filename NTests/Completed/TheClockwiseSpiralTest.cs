using NUnit.Framework;
using System;
public class TheClockwiseSpiralTest
{
    [Test]
    public void Test1()
    {
        var expected = new int[,] { { 1 } };
        Assert.AreEqual(expected, TheClockwiseSpiral.CreateSpiral(1));
    }
    [Test]
    public void Test2()
    {
        var expected = new int[,]
        {
            {1, 2},
            {4, 3},
        };
        Assert.AreEqual(expected, TheClockwiseSpiral.CreateSpiral(2));
    }
    [Test]
    public void Test3()
    {
        var expected = new int[,]
        {
            {1, 2, 3},
            {8, 9, 4},
            {7, 6, 5}
        };
        Assert.AreEqual(expected, TheClockwiseSpiral.CreateSpiral(3));
    }
}
