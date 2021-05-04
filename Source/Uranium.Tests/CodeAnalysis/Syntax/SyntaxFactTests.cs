using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Xunit;
using Uranium.CodeAnalysis.Syntax;

namespace Uranium.Tests.CodeAnalysis.Syntax
{
    public class SyntaxFactTests
    {
        [Theory]
        [MemberData(nameof(GetSyntaxKindData))]
        public void SyntaxFactGetTextRoundTrips(SyntaxKind kind)
        {
            var text = SyntaxFacts.GetText(kind);

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
            var text = SyntaxFacts.GetText(kind);

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
            Assert.Equal(text, SyntaxFacts.GetText(kind));
            Assert.Equal(kind, SyntaxFacts.GetKind(text));
            Assert.Equal(text, token.Text);
            Assert.Equal(kind, SyntaxFacts.GetKind(token.Text));
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
                    Assert.Equal(t.Text, SyntaxFacts.GetText(t.Kind));
                    Assert.Equal(t.Kind, SyntaxFacts.GetKind(t.Text));
                }
            }
        }

        [Fact]
        public static void TestTypes()
        {
            Assert.True(SyntaxFacts.IsInteger(SyntaxKind.IntKeyword));
            Assert.True(SyntaxFacts.IsInteger(SyntaxKind.LongKeyword));

            Assert.True(SyntaxFacts.IsFloatingPoint(SyntaxKind.FloatKeyword));
            Assert.True(SyntaxFacts.IsFloatingPoint(SyntaxKind.DoubleKeyword));

            Assert.False(SyntaxFacts.IsInteger(SyntaxKind.FloatKeyword));
            Assert.False(SyntaxFacts.IsInteger(SyntaxKind.DoubleKeyword));

            Assert.False(SyntaxFacts.IsFloatingPoint(SyntaxKind.IntKeyword));
            Assert.False(SyntaxFacts.IsFloatingPoint(SyntaxKind.IntKeyword));
        }

        [Theory]
        [MemberData(nameof(GetSyntaxTextData))]
        public void TestKindFetcher(string kind, SyntaxKind expected)
        {
            if(kind.Equals("BadToken", StringComparison.OrdinalIgnoreCase) || expected == SyntaxKind.BadToken)
            {
                return;
            }
            var resultKind = SyntaxFacts.GetKind(kind);
            Assert.Equal(resultKind, expected);
            Assert.Equal(kind, SyntaxFacts.GetText(resultKind));
        }

        [Fact]
        public void CheckReturnTypes()
        {
            Assert.Equal(typeof(int), SyntaxFacts.GetKeywordType("int"));
            Assert.Equal(typeof(int), SyntaxFacts.GetKeywordType(SyntaxKind.IntKeyword));

            Assert.Equal(typeof(long), SyntaxFacts.GetKeywordType("long"));
            Assert.Equal(typeof(long), SyntaxFacts.GetKeywordType(SyntaxKind.LongKeyword));

            Assert.Equal(typeof(double), SyntaxFacts.GetKeywordType("double"));
            Assert.Equal(typeof(double), SyntaxFacts.GetKeywordType(SyntaxKind.DoubleKeyword));

            Assert.Equal(typeof(float), SyntaxFacts.GetKeywordType("float"));
            Assert.Equal(typeof(float), SyntaxFacts.GetKeywordType(SyntaxKind.FloatKeyword));

            Assert.Equal(typeof(char), SyntaxFacts.GetKeywordType("char"));
            Assert.Equal(typeof(char), SyntaxFacts.GetKeywordType(SyntaxKind.CharKeyword));

            Assert.Equal(typeof(string), SyntaxFacts.GetKeywordType("string"));
            Assert.Equal(typeof(string), SyntaxFacts.GetKeywordType(SyntaxKind.StringKeyword));

            Assert.Equal(typeof(bool), SyntaxFacts.GetKeywordType("bool"));
            Assert.Equal(typeof(bool), SyntaxFacts.GetKeywordType(SyntaxKind.BoolKeyword));

            Assert.Null(SyntaxFacts.GetKeywordType("abc"));
            Assert.Null(SyntaxFacts.GetKeywordType(SyntaxKind.IfKeyword));
        }

        [Fact]
        public void GetUnaryOperator()
        {
            var operators = SyntaxFacts.GetUnaryOperators().ToArray();
            for(int i = 0; i < operators.Length; i++)
            {
                Assert.False(SyntaxFacts.GetUnaryOperatorPrecedence(operators[i]) == 0);
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
                yield return new object[] { SyntaxFacts.GetText(syntaxKinds[i]), syntaxKinds[i] };
            }
        }
        public static string AllTokens()
        {
            var syntaxKinds = (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind));
            var result = new StringBuilder();
            for(int i = 0; i < syntaxKinds.Length; i++)
            {
                result.Append(SyntaxFacts.GetText(syntaxKinds[i]) + " ");
            }
            return result.ToString();
        }
    }
}
