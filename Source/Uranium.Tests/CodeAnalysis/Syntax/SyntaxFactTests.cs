﻿using System;
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
            foreach(var t in tokens)
            {
                if(t.Kind is not SyntaxKind.IdentifierToken)
                {
                    Assert.Equal(t.Text, SyntaxFacts.GetText(t.Kind));
                    Assert.Equal(t.Kind, SyntaxFacts.GetKind(t.Text));
                }
            }
        }

        public static IEnumerable<object[]> GetSyntaxKindData()
        {
            var syntaxKinds = (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind));
            
            foreach(var kind in syntaxKinds)
            {
                yield return new object[] { kind };
            }
        }

        public static IEnumerable<object[]> GetSyntaxTextData()
        {
            var syntaxKinds = (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind));
            foreach(var kind in syntaxKinds)
            {
                yield return new object[] { SyntaxFacts.GetText(kind), kind };
            }
        }
        public static string AllTokens()
        {
            var syntaxKinds = (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind));
            var result = new StringBuilder();
            foreach(var kind in syntaxKinds)
            {
                result.Append(SyntaxFacts.GetText(kind) + " ");
            }
            return result.ToString();
        }
    }
}
