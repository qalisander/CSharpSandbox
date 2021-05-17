namespace Solution
{
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class BoolfuckTest
    {
        //[Test]
        //public void testEmpty () {
        //    Assert.AreEqual ("", Boolfuck.Interpret ("", ""));
        //    Assert.AreEqual ("", Boolfuck.Interpret (Brainfuck.toBoolfuck (""), ""));
        //}
        //[Test]
        //public void testSingleCommands () {
        //    Assert.AreEqual ("", Boolfuck.Interpret ("<", ""));
        //    Assert.AreEqual ("", Boolfuck.Interpret (">", ""));
        //    Assert.AreEqual ("", Boolfuck.Interpret ("+", ""));
        //    Assert.AreEqual ("", Boolfuck.Interpret (".", ""));
        //    Assert.AreEqual ("\u0000", Boolfuck.Interpret (";", ""));
        //}
        //[Test]
        //public void testIO () {
        //    Assert.AreEqual ("*", Boolfuck.Interpret (Brainfuck.toBoolfuck (",."), "*"));
        //}
        [Test]
        public void TestHelloWorld()
        {
            Assert.AreEqual("Hello, world!\n",
                Boolfuck.Interpret(
                    ";;;+;+;;+;+;+;+;+;+;;+;;+;;;+;;+;+;;+;;;+;;+;+;;+;+;;;;+;+;;+;;;+;;+;+;+;;;;;;;+;+;;+;;;+;+;;;+;+;;;;+;+;;+;;+;+;;+;;;+;;;+;;+;+;;+;;;+;+;;+;;+;+;+;;;;+;+;;;+;+;+;",
                    ""));
        }

        [Test]
        [TestCase(
            ">,>,>,>,>,>,>,>,<<<<<<<[>]+<[+<]>>>>>>>>>[+]+<<<<<<<<+[>+]<[<]>>>>>>>>>[+<<<<<<<<[>]+<[+<]>>>>>>>>>+<<<<<<<<+[>+]<[<]>>>>>>>>>[+]<<<<<<<<;>;>;>;>;>;>;>;<<<<<<<,>,>,>,>,>,>,>,<<<<<<<[>]+<[+<]>>>>>>>>>[+]+<<<<<<<<+[>+]<[<]>>>>>>>>>]<[+<]",
            "Codewars\u00ff",
            "Codewars")] // expected
        [TestCase(
            ">,>,>,>,>,>,>,>,>+<<<<<<<<+[>+]<[<]>>>>>>>>>[+<<<<<<<<[>]+<[+<]>;>;>;>;>;>;>;>;>+<<<<<<<<+[>+]<[<]>>>>>>>>>[+<<<<<<<<[>]+<[+<]>>>>>>>>>+<<<<<<<<+[>+]<[<]>>>>>>>>>[+]+<<<<<<<<+[>+]<[<]>>>>>>>>>]<[+<]>,>,>,>,>,>,>,>,>+<<<<<<<<+[>+]<[<]>>>>>>>>>]<[+<]",
            "Codewars",
            "Codewars")] // expected
        [TestCase(
            ">,>,>,>,>,>,>,>,>>,>,>,>,>,>,>,>,<<<<<<<<+<<<<<<<<+[>+]<[<]>>>>>>>>>[+<<<<<<<<[>]+<[+<]>>>>>>>>>>>>>>>>>>+<<<<<<<<+[>+]<[<]>>>>>>>>>[+<<<<<<<<[>]+<[+<]>>>>>>>>>+<<<<<<<<+[>+]<[<]>>>>>>>>>[+]>[>]+<[+<]>>>>>>>>>[+]>[>]+<[+<]>>>>>>>>>[+]<<<<<<<<<<<<<<<<<<+<<<<<<<<+[>+]<[<]>>>>>>>>>]<[+<]>>>>>>>>>>>>>>>>>>>>>>>>>>>+<<<<<<<<+[>+]<[<]>>>>>>>>>[+<<<<<<<<[>]+<[+<]>>>>>>>>>+<<<<<<<<+[>+]<[<]>>>>>>>>>[+]<<<<<<<<<<<<<<<<<<<<<<<<<<[>]+<[+<]>>>>>>>>>[+]>>>>>>>>>>>>>>>>>>+<<<<<<<<+[>+]<[<]>>>>>>>>>]<[+<]<<<<<<<<<<<<<<<<<<+<<<<<<<<+[>+]<[<]>>>>>>>>>[+]+<<<<<<<<+[>+]<[<]>>>>>>>>>]<[+<]>>>>>>>>>>>>>>>>>>>;>;>;>;>;>;>;>;<<<<<<<<",
            "\u0008\u0009",
            "\u0048")] // expected
        public void TestBasic(string code, string input, string expected)
        {
            Assert.AreEqual(expected, Boolfuck.Interpret(code, input));
        }
    }
}
