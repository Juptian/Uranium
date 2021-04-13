using NUnit.Framework;
using Compiler.CodeAnalysis.Lexing;
using Compiler.CodeAnalysis.Syntax;
using System;

namespace Compiler.Tests
{
    public class Tests
    {
       /* private Lexer lexer;


        [SetUp]
        public void Setup()
        {
            RunTests();
        }

        public void RunTests()
        {
            TestSyntax();
            TestCompoundOperators();
            TestNumbers();
        }

        [Test]
        public void TestSyntax()
        {
            lexer = new Lexer(@"} 
");

            Assert.AreEqual(SyntaxKind.CloseCurlyBrace, lexer.LexTokens('}') );
           

            Console.WriteLine("Test 1 passed!");
        }

        public void TestCompoundOperators()
        {
            lexer = new Lexer("/=");
            Assert.AreEqual(SyntaxKind.DivideEquals, lexer.LexTokens('/'));

            Console.WriteLine("Test 2 passed!");

        }

        public void TestNumbers()
        {
            lexer = new Lexer("123.45");
            Assert.AreEqual(SyntaxKind.NumberToken, lexer.LexTokens('1'));
            Assert.AreEqual(123.45f, lexer._Current);

            lexer = new Lexer("3.141592653589");

            Assert.AreEqual(SyntaxKind.NumberToken, lexer.LexTokens('3'));
            Assert.AreEqual(3.14159274f, lexer._Current);

            Console.WriteLine("Test 3 passed");
        }*/
    }
}