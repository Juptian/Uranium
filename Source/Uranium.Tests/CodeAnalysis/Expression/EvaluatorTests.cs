using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Text;
using Uranium.CodeAnalysis;

namespace Uranium.Tests.CodeAnalysis.Expression
{
    public class EvaluatorTests
    {
        [Theory]
        [InlineData("1", 1)]
        [InlineData("+1", 1)]
        [InlineData("-1", -1)]
        [InlineData("1 - 2", -1)]
        [InlineData("1 * 25", 25)]
        [InlineData("5 ** 2", 25)]
        [InlineData("(1 / 1) * 2 / ( 1 + 1)", 1)]
        [InlineData("10**2", 100)]
        [InlineData("(10)", 10)]
        [InlineData("(10**2)**2", 10_000)]
        [InlineData("10 000/2", 5_000)]
        [InlineData("12 == 3", false)]
        [InlineData("3 == 3", true)]
        [InlineData("3 != 3", false)]
        [InlineData("false", false)]
        [InlineData("true", true)]
        [InlineData("true == false", false)]
        [InlineData("true != false", true)]
        [InlineData("false != true", true)]
        [InlineData("false == false", true)]
        [InlineData("!false", true)]
        [InlineData("!true", false)]
        [InlineData("(a = 10) * 10", 100)]
        [MemberData(nameof(TestCases))]
        public void SyntaxFactGetTextRoundTrips(string text, object expectedResult)
        {
            var syntaxTree = SyntaxTree.Parse(text);
            var compilation = new Compilation(syntaxTree);
            var variables = new Dictionary<VariableSymbol, object>(); 
            var actualResult = compilation.Evaluate(variables);

            if(text.Equals("BadToken"))
            {
                return;
            }
            
            Assert.Empty(actualResult.Diagnostics);
            Assert.Equal(expectedResult, actualResult.Value);
        }
        public static IEnumerable<object[]> TestCases()
        {
            for(int i = 1; i <= 1_000_000; i++)
            {
                i = i == 100 ? 999_900 : i;
                yield return new object[] { $"{i} + {i}", i + i };
                yield return new object[] { $"{i} + -{i}/2", i + -i / 2 };
                yield return new object[] { $"{i} * {i}", i * i };
                yield return new object[] { $"{i} * 2 / {i}", 2 };
                yield return new object[] { $"a = {i}", i };
            }
        }
    }
}
