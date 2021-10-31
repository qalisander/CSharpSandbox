using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Problems.Completed;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace XTests
{
    public class NextBiggerNumberTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public NextBiggerNumberTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData(21, 12)]
        [InlineData(531, 513)]
        [InlineData(2_071, 2_017)]
        [InlineData(441, 414)]
        [InlineData(414, 144)]
        [InlineData(654_592_366, 654_569_632)]
        [InlineData(-1, 987654321)]
        [InlineData(-1, 84)]
        [InlineData(5988484849_83558, 5988484849_58853)]
        [InlineData(9000000001, 1900000000)]
        [InlineData(598848484_83559, 598848484_59853)] //But was:  59884848493558
        public void NextBiggerNumberTest(long expected, long param)
        {
            Assert.Equal(expected, NextBiggerNumberKata.NextBiggerNumber(param));
        }

        [Theory]
        [InlineData(122344)]
        [InlineData(12345555555)]
        [InlineData(12345553242345555)]
        public void GetDigitsTest(long number)
        {
            var numbers = Convert.ToString(number).ToCharArray().Select(ch => (int) char.GetNumericValue(ch)).ToArray();

            Assert.Equal(
                JsonConvert.SerializeObject(numbers), 
                JsonConvert.SerializeObject(number.GetDigits().Reverse().ToArray()));
        }

        [Theory]
        [InlineData(654321, new[]{1, 2, 3, 4, 5, 6})]
        [InlineData(987654321, new[]{1, 2, 3, 4, 5, 6, 7, 8, 9})]
        [InlineData(987604301, new[]{1, 0, 3, 4, 0, 6, 7, 8, 9})]
        public void GetLongFromDigits(int expected, int[] digits)
        {
            // public static long GetLongFromDigits(this IEnumerable<int> digits) =>
            //     digits.Aggregate(
            //         (0, 0L),
            //         (accml, dgt) => (accml.Item1 + 1, dgt * (long) Math.Pow(10, accml.Item1) + accml.Item2)).Item2;

            Assert.Equal(expected, digits.GetLongFromDigits());
        }
    }
}
