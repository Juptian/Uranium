using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Xunit;
using Uranium.CodeAnalysis.Syntax;

namespace Uranium.Tests.CodeAnalysis.Lexing
{
    public class LexerTests
    {
        //This is horrendous, but I couldn't figure out how to make it any better
        private static readonly IEnumerable<(SyntaxKind kind, string text)> _testCases;
        private static readonly IEnumerable<(TupleContainer left, TupleContainer right)> _pairTestCases = GetTokenPairs();

        //Constructor because
        // private static IEnumerable<(SyntaxKind kind, string text)> _testCases = GetOperatorTokens().Concat(GetSyntacticSymbols()).Concat(GetNumbers()).Concat(GetKeywords());
        // Is awful
        static LexerTests()
        {
            //Made a method so that it looks cleaner
            _testCases = Concatenate(GetSoloOperators(), GetCompoundOperators(), GetSyntacticSymbols(), GetNumbers(), GetKeywords());
        }
       
        [Theory]
        [MemberData(nameof(GetTokensData))] //When GetTokensData returns something, it'll be parsed into here
        public void LexerLexesToken(SyntaxKind kind, string text)
        {
            var tokens = SyntaxTree.LexTokens(text);

            var singleToken = Assert.Single(tokens);
            Assert.Equal(kind, singleToken.Kind);
            Assert.Equal(text, singleToken.Text);
        }

        [Theory]
        [MemberData(nameof(GetTokenPairsData))]
        public void LexerLexesTokenPairs(TupleContainer left, TupleContainer right)
        {
            var leftText = left.Text;
            var rightText = right.Text;

            var leftKind = left.Kind;
            var rightKind = right.Kind;

            var text = leftText + rightText;
            var tokens = SyntaxTree.LexTokens(text).ToArray();
            
            Assert.Equal(2, tokens.Length);

            Assert.Equal(leftKind, tokens[0].Kind);
            Assert.Equal(rightKind, tokens[1].Kind);

            Assert.Equal(leftText, tokens[0].Text);
            Assert.Equal(rightText, tokens[1].Text);
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
            foreach(var (left, right) in _pairTestCases)
            {
                yield return new[] { left, right };
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
                (SyntaxKind.NumberToken, "10000"),
            };
        }

        private static IEnumerable<(SyntaxKind kind, string text)> GetKeywords()
        {
            return new[]
            {
                (SyntaxKind.TrueKeyword, "true"),
                (SyntaxKind.FalseKeyword, "false"),
                (SyntaxKind.DoubleKeyword, "double"),
                (SyntaxKind.StringKeyword, "string"),
                (SyntaxKind.FloatKeyword, "float"),
                (SyntaxKind.LongKeyword, "long"),
                (SyntaxKind.BoolKeyword, "bool"),
                (SyntaxKind.VarKeyword, "var"),
                (SyntaxKind.StructKeyword, "struct"),
                (SyntaxKind.ClassKeyword, "class"),
                (SyntaxKind.NamespaceKeyword, "namespace"),
                (SyntaxKind.EnumKeywrod, "enum"),
                (SyntaxKind.TypeDefKeyword, "typedef"),
                (SyntaxKind.Null, "null"),
            };
        }

        //Bottom of the file because it does not need to be revisited very often
        private static bool RequiresSeparator(SyntaxKind leftKind, SyntaxKind rightKind)
        {
            var tokensThatRequire = Concatenate(GetSoloOperators(), GetKeywords(), GetNumbers());
            foreach (var (kind, _) in tokensThatRequire)
            {
                if(kind == leftKind || kind == rightKind)
                {
                    return true;
                }
            }
            return false;
        }

        //^^
        private static IEnumerable<(TupleContainer left, TupleContainer Right)> GetTokenPairs()
        {
            foreach(var (kind, text) in _testCases)
            {
                foreach(var (rightKind, rightText) in _testCases)
                {
                    if(!RequiresSeparator(kind, rightKind))
                    {
                        var left = new TupleContainer(kind, text);
                        var right = new TupleContainer(rightKind, rightText);
                        yield return new(left, right);    
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
