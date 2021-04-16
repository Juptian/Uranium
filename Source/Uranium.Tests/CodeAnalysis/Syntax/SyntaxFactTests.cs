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
        public void SyntaxtFactGetTextRoundTrips(SyntaxKind kind)
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

        public static IEnumerable<object[]> GetSyntaxKindData()
        {
            var syntaxKinds = (SyntaxKind[]) Enum.GetValues(typeof(SyntaxKind));
            
            foreach(var kind in syntaxKinds)
            {
                yield return new object[] { kind };
            }
        }
    }
}
