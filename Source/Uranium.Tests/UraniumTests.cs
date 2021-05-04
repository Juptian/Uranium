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
            Assert.True(Uranium.Emit(text));

            text[0] = @"
{
    int a = 10.10;
    var b = 100;
    for(; a < b; a += 1)
    {
        b -= 1;
    }
}";
            Assert.True(Uranium.Emit(text));

            Assert.False(Uranium.Emit(Array.Empty<string>()));
        }
    }
}
