using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Uranium.CodeAnalysis.Text;

namespace Uranium.Tests.CodeAnalysis.Text
{
    public class SourceTextTest
    {
        [Theory]
        [InlineData(".", 1)]
        [InlineData(".\r\n", 2)]
        [InlineData(".\r\n\r\n", 3)]
        public void SourceTextIncludesLastLine(string text, int expectedLineCount)
        {
            var sourceText = SourceText.From(text);
            Assert.Equal(expectedLineCount, sourceText.Lines.Length);
        }
    }
}
