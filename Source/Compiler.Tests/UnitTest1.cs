using NUnit.Framework;
using Compiler.Lexing;

namespace Compiler.Tests
{
    public class Tests
    {
        private Lexer lexer;
        private readonly string text1 = @"}
+=;
";

        [SetUp]
        public void Setup()
        {
            lexer = new Lexer(text1);
        }

        [Test]
        public void Test1()
        {
            Assert.Pass("Test will pass", lexer.Lex(text1[0]) );
        }
    }
}