using NUnit.Framework;
using Compiler.Lexing;
using System;

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
            Assert.AreEqual(TokenType.CloseCurlyBrace, lexer.Lex(text1[0]) );
            Assert.AreEqual(TokenType.LineBreak, lexer.Lex(text1[1]));

            Console.WriteLine("Test1 passed!");
        }
    }
}