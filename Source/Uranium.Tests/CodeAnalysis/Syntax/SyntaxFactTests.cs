using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Xunit;
using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Symbols;

namespace Uranium.Tests.CodeAnalysis.Syntax
{
    public class SyntaxFactTests
    {
        [Theory]
        [MemberData(nameof(GetSyntaxKindData))]
        public void SyntaxFactGetTextRoundTrips(SyntaxKind kind)
        {
            var text = TextChecker.GetText(kind);

            if(text.Equals("BadToken"))
            {
                return;
            }

            var tokens = SyntaxTree.LexTokens(text);
            var token = Assert.Single(tokens);

            Assert.Equal(token.Kind, kind);
            Assert.Equal(token.Text, text);
        }

        [Theory]
        [MemberData(nameof(GetSyntaxKindData))]
        public void SyntaxFactsBinaryOperatorsRoundTrips(SyntaxKind kind)
        {
            var text = TextChecker.GetText(kind);

            if(text.Equals("BadToken"))
            {
                return;
            }

            var tokens = SyntaxTree.LexTokens(text);
            var token = Assert.Single(tokens);

            Assert.Equal(token.Kind, kind);
            Assert.Equal(token.Text, text);
        }

        [Theory]
        [MemberData(nameof(GetSyntaxTextData))]
        public void SyntaxFactsBinaryOperatorRoundTrips(string text, SyntaxKind kind)
        {
            if(text.Equals("BadToken") || kind is SyntaxKind.BadToken)
            {
                return;
            }

            var tokens = SyntaxTree.LexTokens(text);
            var token = Assert.Single(tokens);
            Assert.Equal(text, TextChecker.GetText(kind));
            Assert.Equal(kind, TextChecker.GetSyntaxKind(text));
            Assert.Equal(text, token.Text);
            Assert.Equal(kind, TextChecker.GetSyntaxKind(token.Text));
        }

        [Fact]
        public static void TestAllRoundTrips()
        {
            var text = AllTokens();
            SyntaxToken[] tokens = SyntaxTree.LexTokens(text).ToArray();
            for(int i = 0; i < tokens.Length; i++)
            {
                var t = tokens[i];
                if(t.Kind is not SyntaxKind.IdentifierToken)
                {
                    Assert.Equal(t.Text, TextChecker.GetText(t.Kind));
                    Assert.Equal(t.Kind, TextChecker.GetSyntaxKind(t.Text));
                }
            }
        }

        [Fact]
        public static void TestTypes()
        {
            Assert.True(TypeChecker.IsInteger(SyntaxKind.IntKeyword));
            Assert.True(TypeChecker.IsInteger(SyntaxKind.LongKeyword));

            Assert.True(TypeChecker.IsFloatingPoint(SyntaxKind.FloatKeyword));
            Assert.True(TypeChecker.IsFloatingPoint(SyntaxKind.DoubleKeyword));

            Assert.False(TypeChecker.IsInteger(SyntaxKind.FloatKeyword));
            Assert.False(TypeChecker.IsInteger(SyntaxKind.DoubleKeyword));

            Assert.False(TypeChecker.IsFloatingPoint(SyntaxKind.IntKeyword));
            Assert.False(TypeChecker.IsFloatingPoint(SyntaxKind.IntKeyword));
        }

        [Theory]
        [MemberData(nameof(GetSyntaxTextData))]
        public void TestKindFetcher(string kind, SyntaxKind expected)
        {
            if(kind.Equals("BadToken", StringComparison.OrdinalIgnoreCase) || expected == SyntaxKind.BadToken)
            {
                return;
            }
            var resultKind = TextChecker.GetSyntaxKind(kind);
            Assert.Equal(resultKind, expected);
            Assert.Equal(kind, TextChecker.GetText(resultKind));
        }

        [Fact]
        public void CheckReturnTypes()
        {
            Assert.Equal(TypeSymbol.Int, TextChecker.GetKeywordType("int"));
            Assert.Equal(TypeSymbol.Int, TextChecker.GetKeywordType(SyntaxKind.IntKeyword));

            Assert.Equal(TypeSymbol.Long, TextChecker.GetKeywordType("long"));
            Assert.Equal(TypeSymbol.Long, TextChecker.GetKeywordType(SyntaxKind.LongKeyword));

            Assert.Equal(TypeSymbol.Double, TextChecker.GetKeywordType("double"));
            Assert.Equal(TypeSymbol.Double, TextChecker.GetKeywordType(SyntaxKind.DoubleKeyword));

            Assert.Equal(TypeSymbol.Float, TextChecker.GetKeywordType("float"));
            Assert.Equal(TypeSymbol.Float, TextChecker.GetKeywordType(SyntaxKind.FloatKeyword));

            Assert.Equal(TypeSymbol.Char, TextChecker.GetKeywordType("char"));
            Assert.Equal(TypeSymbol.Char, TextChecker.GetKeywordType(SyntaxKind.CharKeyword));

            Assert.Equal(TypeSymbol.String, TextChecker.GetKeywordType("string"));
            Assert.Equal(TypeSymbol.String, TextChecker.GetKeywordType(SyntaxKind.StringKeyword));

            Assert.Equal(TypeSymbol.Bool, TextChecker.GetKeywordType("bool"));
            Assert.Equal(TypeSymbol.Bool, TextChecker.GetKeywordType(SyntaxKind.BoolKeyword));

            Assert.Null(TextChecker.GetKeywordType("abc"));
            Assert.Null(TextChecker.GetKeywordType(SyntaxKind.IfKeyword));
        }

        [Fact]
        public void GetUnaryOperator()
        {
            var operators = OperatorChecker.GetUnaryOperators().ToArray();
            for(int i = 0; i < operators.Length; i++)
            {
                Assert.False(OperatorChecker.GetUnaryOperatorPrecedence(operators[i]) == 0);
            }
        }

        public static IEnumerable<object[]> GetSyntaxKindData()
        {
            var syntaxKinds = (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind));

            for(int i = 0; i < syntaxKinds.Length; i++)
            {
                yield return new object[] { syntaxKinds[i] };
            }
        }

        public static IEnumerable<object[]> GetSyntaxTextData()
        {
            var syntaxKinds = (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind));
            for(int i = 0; i < syntaxKinds.Length; i++)
            {
                yield return new object[] { TextChecker.GetText(syntaxKinds[i]), syntaxKinds[i] };
            }
        }
        public static string AllTokens()
        {
            var syntaxKinds = (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind));
            var result = new StringBuilder();
            for(int i = 0; i < syntaxKinds.Length; i++)
            {
                result.Append(TextChecker.GetText(syntaxKinds[i]) + " ");
            }
            return result.ToString();
        }
    }
}
