using System;
using Uranium;
using Xunit;

namespace Uranium.Tests
{
    public static class UraniumTests
    {
        [Fact]
        public static void ItRuns()
        {
            var text = new string[] { @"
{
    var a = 10;
    var b = 100;
    for(; a < b; a += 1)
    {
        b -= 1;
    }
}", "--tree" };
            var shouldHaveDiagnostic = @"
{
    int a = 10.10;
    var b = 100;
    for(; a < b; a += 1)
    {
        b -= 1;
    }
}";
            var shouldFallIntoDefault = "aj";
            Assert.True(Uranium.Emit(text));

            text[0] = shouldHaveDiagnostic;
            text[1] = shouldFallIntoDefault;
            Assert.True(Uranium.Emit(text));
            Assert.False(Uranium.Emit(Array.Empty<string>()));
        }
    }
}
