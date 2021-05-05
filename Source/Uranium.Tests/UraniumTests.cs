using System;
using Uranium;
using Xunit;

namespace Uranium.Tests
{
    public static class UraniumTests
    {
        private static readonly string[] _diagnostics = new string[]
        {
            "_100", "{ int a = 0.10 }", "{ var a = 10; var a = 11; }", "{ const a = 10; a = 11}", "{10 += 1}", "{10 = 1}",
            "{ int a = 10; double b = 11; float c = a + b; }", "{ var a = 10;", "/*", "a = 100", "a", "int a = 10.1", "{ int a = -10; long b = -a; long c = -b }",
            "10.10.10", "10.21474836472147483647"
        };

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

            for(int i = 0; i < _diagnostics.Length; i++)
            {
                var item = new string[] { _diagnostics[i] };
                Uranium.Emit(item);
                Assert.False(Uranium.Diagnostics.IsEmpty);
            }
        }
    }
}
