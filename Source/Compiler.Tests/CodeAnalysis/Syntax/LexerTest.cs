using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Xunit;
using Compiler.CodeAnalysis.Syntax;

namespace Compiler.Tests.CodeAnalysis.Syntax
{
    public class LexerTest
    {
        //This is horrendous, but I couldn't figure out how to make it any better
        private static readonly IEnumerable<(SyntaxKind kind, string text)> _testCases;
        private static readonly IEnumerable<(SyntaxKind kindLeft, string textLeft, SyntaxKind kindRight, string textRight)> _pairTestCases = GetTokenPairs();

        //Constructor because
        // private static IEnumerable<(SyntaxKind kind, string text)> _testCases = GetOperatorTokens().Concat(GetSyntacticSymbols()).Concat(GetNumbers()).Concat(GetKeywords());
        // Is awful
        static LexerTest()
        {
            //Made a method so that it looks cleaner
            _testCases = Concatenate(GetSoloOperators(), GetCompoundOperators(), GetSyntacticSymbols(), GetNumbers(), GetKeywords());
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
            ( SyntaxKind kindLeft, string textLeft,
              SyntaxKind kindRight, string textRight )
        {
            var text = textLeft + textRight;
            var tokens = SyntaxTree.LexTokens(text).ToArray();

            if(tokens[1].Text != textRight)
                foreach(var t in tokens) { Debug.WriteLine(t); }
            
            Assert.Equal(2, tokens.Length);

            Assert.Equal(tokens[0].Kind, kindLeft);
            Assert.Equal(tokens[1].Kind, kindRight);

            Assert.Equal(tokens[0].Text, textLeft);
            Assert.Equal(tokens[1].Text, textRight);
        }


        private static bool RequiresSeparator(SyntaxKind kindLeft, SyntaxKind kindRight)
        {
            var tokensThatRequire = Concatenate(GetSoloOperators(), GetKeywords(), GetNumbers());
            foreach (var (kind, _) in tokensThatRequire)
            {
                if(kind == kindLeft || kind == kindRight)
                {
                    return true;
                }
            }
            return false;
        }

        public static IEnumerable<object[]> GetTokensData()
        {
            foreach(var (kind, text) in _testCases)
            {
                yield return new object[] { kind, text };
            }
        }

        public static IEnumerable<object[]> GetTokenPairsData()
        {
            foreach(var (kindLeft, textLeft, kindRight, textRight) in _pairTestCases)
            {
                yield return new object[] { kindLeft, textLeft, kindRight, textRight };
            }
        }

        private static IEnumerable<(SyntaxKind kind, string text)> GetSoloOperators()
        {
            return new[]
            {
                (SyntaxKind.Equals, "="),
                (SyntaxKind.Plus, "+"),
                (SyntaxKind.Minus, "-"),
                (SyntaxKind.Divide, "/"),
                (SyntaxKind.Multiply, "*"),
                (SyntaxKind.Percent, "%"),
                (SyntaxKind.Ampersand, "&"),
                (SyntaxKind.Pipe, "|"),
                (SyntaxKind.Hat, "^"),
                (SyntaxKind.GreaterThan, ">"), 
                (SyntaxKind.LesserThan, "<"),
                (SyntaxKind.Bang, "!"), 
            };
        }

        private static IEnumerable<(SyntaxKind kind, string text)> GetCompoundOperators()
        {
            return new[]
            {
                (SyntaxKind.DoubleEquals, "=="),
                (SyntaxKind.PlusPlus, "++"),
                (SyntaxKind.PlusEquals, "+="),
                (SyntaxKind.MinusMinus, "--"),
                (SyntaxKind.MinusEquals, "-="),
                (SyntaxKind.DivideEquals, "/="),
                (SyntaxKind.MultiplyEquals, "*="),
                (SyntaxKind.Pow, "**"),
                (SyntaxKind.PercentEquals, "%="),
                (SyntaxKind.DoubleAmpersand, "&&"),
                (SyntaxKind.DoublePipe, "||"),
                (SyntaxKind.HatEquals, "^="), 
                (SyntaxKind.GreaterThanEquals, ">="),
                (SyntaxKind.LesserThanEquals, "<="),
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

        //Bottom of the file because it does not need to be revisited
        private static IEnumerable<(SyntaxKind kindLeft, string textLeft, SyntaxKind kindRight, string textRight)> GetTokenPairs()
        {
            foreach(var (kind, text) in _testCases)
            {
                foreach(var (kindRight, textRight) in _testCases)
                {
                    if(!RequiresSeparator(kind, kindRight))
                    {
                        yield return new(kind, text, kindRight, textRight);    
                    }
                }
            }    
        }
        
        //^^
        private static IEnumerable<T> Concatenate<T>(params IEnumerable<T>[] list)
        {
            foreach(var element in list)
            {
                foreach(T subElement in element)
                {
                    yield return subElement;
                }
            }
        }

    }
}
