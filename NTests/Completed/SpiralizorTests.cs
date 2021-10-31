using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Problems;
using Newtonsoft.Json;
using NUnit.Framework;

namespace NTests
{
    [TestFixture]
    public class SpiralizorTests
    {
        // NUnit parallel tests

        [Test]
        public void Test05()
        {
            int input = 5;
            int[,] expected = new int[,]{
                {1, 1, 1, 1, 1},
                {0, 0, 0, 0, 1},
                {1, 1, 1, 0, 1},
                {1, 0, 0, 0, 1},
                {1, 1, 1, 1, 1}
            };

            var actual = Spiralizor.Spiralize(input);

            Console.WriteLine("Expected:\n" + 
                              ConvertToStr(expected, input));

            Console.WriteLine("Actual:\n" +
                              ConvertToStr(actual, input));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test08()
        {
            int input = 8;
            int[,] expected = new int[,]{
                {1, 1, 1, 1, 1, 1, 1, 1},
                {0, 0, 0, 0, 0, 0, 0, 1},
                {1, 1, 1, 1, 1, 1, 0, 1},
                {1, 0, 0, 0, 0, 1, 0, 1},
                {1, 0, 1, 0, 0, 1, 0, 1},
                {1, 0, 1, 1, 1, 1, 0, 1},
                {1, 0, 0, 0, 0, 0, 0, 1},
                {1, 1, 1, 1, 1, 1, 1, 1},
            };

            int[,] actual = Spiralizor.Spiralize(input);


            Console.WriteLine("Expected:\n" + 
                              ConvertToStr(expected, input));

            Console.WriteLine("Actual:\n" +
                              ConvertToStr(actual, input));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test09()
        {
            int input = 8;
            int[,] expected = new int[,]{
                {1, 1, 1, 1, 1, 1, 1, 1},
                {0, 0, 0, 0, 0, 0, 0, 1},
                {1, 1, 1, 1, 1, 1, 0, 1},
                {1, 0, 0, 0, 0, 1, 0, 1},
                {1, 0, 1, 0, 0, 1, 0, 1},
                {1, 0, 1, 1, 1, 1, 0, 1},
                {1, 0, 0, 0, 0, 0, 0, 1},
                {1, 1, 1, 1, 1, 1, 1, 1},
            };

            int[,] actual = Spiralizor.Spiralize(input);


            Console.WriteLine("Expected:\n" + 
                              ConvertToStr(expected, input));

            Console.WriteLine("Actual:\n" +
                              ConvertToStr(actual, input));

            Assert.AreEqual(expected, actual);
        }

        public static string ConvertToStr(int[,] arr, int size)
        {
            var sb = new StringBuilder(arr.Length + 3);

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                    sb.Append(arr[i, j] + "\t");

                sb.Append("\n");
            }

            return sb.ToString();
        }
    }
}
