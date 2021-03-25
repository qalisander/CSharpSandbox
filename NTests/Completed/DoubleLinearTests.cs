using System;
using System.Diagnostics;
using System.Linq;
using Experiments.Completed;
using NUnit.Framework;

namespace NTests.Completed
{
    [TestFixture]
    public static class DoubleLinearTests
    {
        private static Stopwatch _stopWatch;

        [SetUp]
        public static void Init()
        {
            _stopWatch = Stopwatch.StartNew();
        }

        [TearDown]
        public static void Cleanup()
        {
            _stopWatch.Stop();
            Debug.WriteLine("Excution time for {0} - {1} ms",
                TestContext.CurrentContext.Test.Name,
                _stopWatch.ElapsedMilliseconds);
            // ... add your code here
        }

        private static void testing(int actual, int expected)
        {
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void test1()
        {
            Console.WriteLine("Fixed Tests DblLinear");
            testing(DoubleLinear.DblLinear(10), 22);
            testing(DoubleLinear.DblLinear(20), 57);
            testing(DoubleLinear.DblLinear(30), 91);
            testing(DoubleLinear.DblLinear(50), 175);
            testing(DoubleLinear.DblLinear(60), 237);
            testing(DoubleLinear.DblLinear(70), 271);
            testing(DoubleLinear.DblLinear(80), 334);
            testing(DoubleLinear.DblLinear(90), 379);
            testing(DoubleLinear.DblLinear(100), 447);           //4,5
            testing(DoubleLinear.DblLinear(1000), 8488);         //8,5
            testing(DoubleLinear.DblLinear(10_000), 157_654);    //15
            testing(DoubleLinear.DblLinear(100_000), 2_902_779); //30
            testing(DoubleLinear.DblLinear(10_000_000), 1_031_926_810);
        }

        [Test]
        public static void test1_2()
        {
            Console.WriteLine("Fixed Tests DblLinear");
            testing(DoubleLinear.DblLinear(10), 22);
            testing(DoubleLinear.DblLinear(20), 57);
            testing(DoubleLinear.DblLinear(30), 91);
            testing(DoubleLinear.DblLinear(50), 175);
            testing(DoubleLinear.DblLinear(60), 237);
            testing(DoubleLinear.DblLinear(70), 271);
            testing(DoubleLinear.DblLinear(80), 334);
            testing(DoubleLinear.DblLinear(90), 379);
            testing(DoubleLinear.DblLinear(100), 447);           //4,5
            testing(DoubleLinear.DblLinear(1000), 8488);         //8,5
            testing(DoubleLinear.DblLinear(10_000), 157_654);    //15
            testing(DoubleLinear.DblLinear(100_000), 2_911_582); //30   
            testing(DoubleLinear.DblLinear(10_000_000), 1_031_926_810); 
        }


        [Test]
        public static void test1_3()
        {
            Console.WriteLine("Fixed Tests DblLinear");
            testing(DoubleLinear3.DblLinear(10), 22);
            testing(DoubleLinear3.DblLinear(20), 57);
            testing(DoubleLinear3.DblLinear(30), 91);
            testing(DoubleLinear3.DblLinear(50), 175);
            testing(DoubleLinear3.DblLinear(60), 237);
            testing(DoubleLinear3.DblLinear(70), 271);
            testing(DoubleLinear3.DblLinear(80), 334);
            testing(DoubleLinear3.DblLinear(90), 379);
            testing(DoubleLinear3.DblLinear(100), 447);           //4,5
            testing(DoubleLinear3.DblLinear(1000), 8488);         //8,5
            testing(DoubleLinear3.DblLinear(10_000), 157_654);    //15
            testing(DoubleLinear3.DblLinear(100_000), 2_911_582); //30   
            testing(DoubleLinear3.DblLinear(10_000_000), 1_031_926_810); 
        }

        [Test]
        public static void test2()
        {

            var mult = 1;
            for (int i = 0; i < 2_000_000_000; i++)
            {
                unchecked
                {
                    mult += i*i;
                }
            }
        
            Assert.AreEqual(mult, -801_172_993);
        }

        [Test]
        public static void test3()
        {
            Assert.AreEqual(Enumerable.Range(1, 1_000_000_000).Aggregate(0, (i, i1) => unchecked(i1 + i)), -243_309_312);
        }

        [Test]
        public static void test4()
        {
            var bools = new bool[1_000_000_000];

            for (var i = 0; i < bools.Length; i++)
            {
                bools[i] = true;
            }

            Assert.AreEqual(bools[1_000_000_00], true);
        }
    }
}
