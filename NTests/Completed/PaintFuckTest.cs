using System.Collections.Generic;
using Problems;
using NUnit.Framework;

namespace NTests.Completed
{
    public class PaintFuckTest
    {
        [TestFixture]
        public class Should_work_for_some_example_test_cases
        {
            private static IEnumerable<TestCaseData> testCases
            {
                get
                {
                    yield return new TestCaseData("*e*e*e*es*es*ws*ws*w*w*w*n*n*n*ssss*s*s*s*", 0, 6, 9,
                            "000000\r\n000000\r\n000000\r\n000000\r\n000000\r\n000000\r\n000000\r\n000000\r\n000000")
                        .Returns(
                            "000000\r\n000000\r\n000000\r\n000000\r\n000000\r\n000000\r\n000000\r\n000000\r\n000000")
                        .SetDescription("Your interpreter should initialize all cells in the datagrid to 0");

                    yield return new TestCaseData("*e*e*e*es*es*ws*ws*w*w*w*n*n*n*ssss*s*s*s*", 7, 6, 9,
                            "111100\r\n000000\r\n000000\r\n000000\r\n000000\r\n000000\r\n000000\r\n000000\r\n000000")
                        .Returns(
                            "111100\r\n000000\r\n000000\r\n000000\r\n000000\r\n000000\r\n000000\r\n000000\r\n000000")
                        .SetDescription(
                            "Your interpreter should adhere to the number of iterations specified");

                    yield return new TestCaseData("*e*e*e*es*es*ws*ws*w*w*w*n*n*n*ssss*s*s*s*", 19, 6, 9,
                            "111100\r\n000010\r\n000001\r\n000010\r\n000100\r\n000000\r\n000000\r\n000000\r\n000000")
                        .Returns(
                            "111100\r\n000010\r\n000001\r\n000010\r\n000100\r\n000000\r\n000000\r\n000000\r\n000000")
                        .SetDescription("Your interpreter should traverse the 2D datagrid correctly");

                    yield return new TestCaseData("*e*e*e*es*es*ws*ws*w*w*w*n*n*n*ssss*s*s*s*", 42, 6, 9,
                            "111100\r\n100010\r\n100001\r\n100010\r\n111100\r\n100000\r\n100000\r\n100000\r\n100000")
                        .Returns(
                            "111100\r\n100010\r\n100001\r\n100010\r\n111100\r\n100000\r\n100000\r\n100000\r\n100000")
                        .SetDescription(
                            "Your interpreter should traverse the 2D datagrid correctly for all of the \"n\", \"e\", \"s\" and \"w\" commands");

                    yield return new TestCaseData("*e*e*e*es*es*ws*ws*w*w*w*n*n*n*ssss*s*s*s*", 100, 6, 9,
                            "111100\r\n100010\r\n100001\r\n100010\r\n111100\r\n100000\r\n100000\r\n100000\r\n100000")
                        .Returns(
                            "111100\r\n100010\r\n100001\r\n100010\r\n111100\r\n100000\r\n100000\r\n100000\r\n100000")
                        .SetDescription(
                            "Your interpreter should terminate normally and return a representation of the final state of the 2D datagrid when all commands have been considered from left to right even if the number of iterations specified have not been fully performed");

                    yield return new TestCaseData("*e*e*e*es*h34543es*ws*ws*w*w*435345w*n*n*n*ssss*2342s*s*s*", 100, 6, 9,
                            "111100\r\n100010\r\n100001\r\n100010\r\n111100\r\n100000\r\n100000\r\n100000\r\n100000")
                        .Returns(
                            "111100\r\n100010\r\n100001\r\n100010\r\n111100\r\n100000\r\n100000\r\n100000\r\n100000")
                        .SetDescription(
                            "Same as previou, but there are not grammar symbols");

                    yield return new TestCaseData("*[es*]", 9, 5, 6,
                            "10000\r\n01000\r\n00100\r\n00000\r\n00000\r\n00000")
                        .Returns(
                            "10000\r\n01000\r\n00100\r\n00000\r\n00000\r\n00000")
                        .SetDescription(
                            "Should_work_for_any_combination_of_loops_be_it_simple_or_nested");

                    yield return new TestCaseData("*[s[e]*]", 39, 5, 5,
                            "11000\r\n11000\r\n11000\r\n11000\r\n11000")
                        .Returns(
                            "11000\r\n11000\r\n11000\r\n11000\r\n11000")
                        .SetDescription(
                            "Your interpreter should also work with nested loops");

                    yield return new TestCaseData("*[es*]*", 3000, 5, 6,
                            "11111\r\n11111\r\n11111\r\n11111\r\n11111\r\n11111")
                        .Returns(
                            "11111\r\n11111\r\n11111\r\n11111\r\n11111\r\n11111")
                        .SetDescription(
                            "Your interpreter should exit the loop at the correct conditions");
                }
            }

            [Test, TestCaseSource("testCases")]
            public string Test(string code, int iterations, int width, int height, string expected)
            {
                string actual = PaintFuck.Interpret(code, iterations, width, height);

                // Prints representation of datagrid - 0's are black and 1's are white
                // Note: Only works properly if your interpreter returns a representation of the datagrid in the correct format
                //Setup.DisplayExpected(expected);
                //Setup.DisplayActual(actual);

                return actual;
            }
        }
    }
}
