using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Compiler.CodeAnalysis.Syntax;

namespace Compiler.Tests.CodeAnalysis.Syntax
{
    public class LexerTest
    {
        //This is horrendous, but I couldn't figure out how to make it any better
        private static IEnumerable<(SyntaxKind kind, string text)> _testCases;
        private static IEnumerable<(SyntaxKind kind1, string text1, SyntaxKind kind2, string text2)> _pairTestCases = GetTokenPairs();

        //Constructor because
        // private static IEnumerable<(SyntaxKind kind, string text)> _testCases = GetOperatorTokens().Concat(GetSyntacticSymbols()).Concat(GetNumbers()).Concat(GetKeywords());
        // Is awful
        static LexerTest()
        {
            //Made a method so that it looks cleaner
            _testCases = Concatenate(GetOperatorTokens(), GetSyntacticSymbols(), GetNumbers(), GetKeywords());
        }
       

        [Theory]
        [MemberData(nameof(GetTokensData))] //When GetTokensData returns something, it'll be parsed into here
        public void Lexer_Lexes_Token(SyntaxKind kind, string text)
        {
            var tokens = SyntaxTree.LexTokens(text);

            var singleToken = Assert.Single(tokens);
            Assert.Equal(kind, singleToken.Kind);
            Assert.Equal(text, singleToken.Text);
        }

        [Theory]
        [MemberData(nameof(GetTokenPairsData))]
        public void Lexer_Lexes_TokenPairs
            ( SyntaxKind kind1, string text1,
              SyntaxKind kind2, string text2 )
        {
            var text = text1 + " " + text2;
            var tokens = SyntaxTree.LexTokens(text).ToArray();
            
            Assert.Equal(2, tokens.Length);

            Assert.Equal(tokens[0].Kind, kind1);
            Assert.Equal(tokens[0].Text, text1);

            Assert.Equal(tokens[1].Kind, kind2);
            Assert.Equal(tokens[1].Text, text2);
        }


        private static bool RequiresSeparator(SyntaxKind kind1, SyntaxKind kind2)
        {
            var tokensThatRequire = Concatenate(GetKeywords(), GetNumbers());

            foreach(var s in tokensThatRequire)
            {
                if(s.kind == kind1 || s.kind == kind2)
                {
                    return true;
                }
            }
            return false;
        }

        public static IEnumerable<object[]> GetTokensData()
        {
            foreach(var token in _testCases)
            {
                yield return new object[] { token.kind, token.text };
            }
        }

        public static IEnumerable<object[]> GetTokenPairsData()
        {
            foreach(var tokenPair in _pairTestCases)
            {
                yield return new object[] { tokenPair.kind1, tokenPair.text1, tokenPair.kind2, tokenPair.text2 };
            }
        }

        private static IEnumerable<(SyntaxKind kind, string text)> GetOperatorTokens()
        {
            return new[]
            {
                (SyntaxKind.Equals, "="),
                (SyntaxKind.DoubleEquals, "=="),
                (SyntaxKind.Plus, "+"),
                (SyntaxKind.PlusPlus, "++"),
                (SyntaxKind.PlusEquals, "+="),
                (SyntaxKind.Minus, "-"),
                (SyntaxKind.MinusMinus, "--"),
                (SyntaxKind.MinusEquals, "-="),
                (SyntaxKind.Divide, "/"),
                (SyntaxKind.DivideEquals, "/="),
                (SyntaxKind.Multiply, "*"),
                (SyntaxKind.MultiplyEquals, "*="),
                (SyntaxKind.Pow, "**"),
                (SyntaxKind.Percent, "%"),
                (SyntaxKind.PercentEquals, "%="),
                (SyntaxKind.Ampersand, "&"),
                (SyntaxKind.DoubleAmpersand, "&&"),
                (SyntaxKind.Pipe, "|"),
                (SyntaxKind.DoublePipe, "||"),
                (SyntaxKind.Hat, "^"),
                (SyntaxKind.HatEquals, "^="), 
                (SyntaxKind.GreaterThan, ">"), 
                (SyntaxKind.GreaterThanEquals, ">="),
                (SyntaxKind.LesserThan, "<"),
                (SyntaxKind.LesserThanEquals, "<="),
                (SyntaxKind.Bang, "!"), 
                (SyntaxKind.BangEquals, "!="),

            };
        }

        private static IEnumerable<(SyntaxKind kind, string text)> GetSyntacticSymbols()
        {
            return new[]
            {
                (SyntaxKind.Semicolon, ";"),
                (SyntaxKind.Colon, ":"),
                (SyntaxKind.Dot, "."),
                (SyntaxKind.Comma, ","),
                (SyntaxKind.Tilde, "~"),
                (SyntaxKind.OpenParenthesis, "("),
                (SyntaxKind.CloseParenthesis, ")"),
                (SyntaxKind.OpenCurlyBrace, "{"),
                (SyntaxKind.CloseCurlyBrace, "}"),
                (SyntaxKind.OpenBrackets, "["),
                (SyntaxKind.CloseBrackets, "]"),
                
                //We don't actually care about whitespace, or what it contains
                //As long as the parser ignores it, we're good!
                //So it will not be included in the tests.
            };
        }
        private static IEnumerable<(SyntaxKind kind, string text)> GetNumbers()
        {
            return new[]
            {
                (SyntaxKind.NumberToken, "123.4"),
                (SyntaxKind.NumberToken, "1"),
                (SyntaxKind.NumberToken, "10 000"),
            };
        }
        private static IEnumerable<(SyntaxKind kind, string text)> GetKeywords()
        {
            return new[]
            {
                (SyntaxKind.TrueKeyword, "true"),
                (SyntaxKind.FalseKeyword, "false"),
            };
        }

        private static IEnumerable<(SyntaxKind kind1, string text1, SyntaxKind kind2, string text2)> GetTokenPairs()
        {
            foreach(var t1 in _testCases)
            {
                foreach(var t2 in _testCases)
                {
                    if(!RequiresSeparator(t1.kind, t2.kind))
                    {
                        yield return new(t1.kind, t1.text, t2.kind, t2.text);    
                    }
                }
            }    
        }

        //Bottom of the file because it does not need to be revisited
        private static IEnumerable<T> Concatenate<T>(params IEnumerable<T>[] list)
        {
            foreach(var element in list)
            {
                foreach(T subEl in element)
                {
                    yield return subEl;
                }
            }
        }

    }
}
