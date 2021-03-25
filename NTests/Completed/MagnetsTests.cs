using System;
using Experiments.Completed;
using NUnit.Framework;

namespace NTests.Completed
{
    [TestFixture]
    public static class MagnetsTests 
    {

        private static Random rnd = new Random();
        private static void assertFuzzyEquals(double act, double exp)
        {
            bool inrange = Math.Abs(act - exp) <= 1e-6;
            if (inrange == false) 
            {
                string specifier = "#0.000000";
                Console.WriteLine(
                    "At 1e-6: Expected must be " + exp.ToString(specifier) +", but got " + act.ToString(specifier));
            }
            Assert.AreEqual(true, inrange);  
        }

        [Test]
        public static void test1() 
        {
            Console.WriteLine("Fixed Tests: Doubles"); 
            assertFuzzyEquals(Magnets.Doubles(1, 10), 0.5580321939764581);     // 0.5580321939764581
            assertFuzzyEquals(Magnets.Doubles(10, 1000), 0.6921486500921933);  // 0.6921486500921933
            assertFuzzyEquals(Magnets.Doubles(10, 10000), 0.6930471674194457); // 0.6930471674194457
            assertFuzzyEquals(Magnets.Doubles(20, 10000), 0.6930471955575918); // 0.6930471955575918
        }
    }
}
