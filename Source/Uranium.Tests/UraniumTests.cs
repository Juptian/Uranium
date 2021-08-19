using System;
using Uranium;
using Xunit;

namespace Uranium.Tests
{
    public static class UraniumTests
    {
        private static readonly string[] _diagnostics = new string[]
        {
            "{ int a = 0.10 }", "{ var a = 10; var a = 11; }", "{ const a = 10; a = 11}", "{10 += 1}", "{10 = 1}",
            "{ int a = 10; double b = 11; float c = a + b; }", "{ var a = 10;", "/*", "a = 100", "a", "int a = 10.1", "{ int a = -10; long b = -a; long c = -b }",
            "10.10.10", "10.2147483647214748364721474836472147483647", " \" ", "var = 10", "var x = print(\"foobar\")", "var x = println(\"foobar\")", "print()", "println()",
            "input(\"foobar\"", "{ long a = 10; double d = 20; var b = a + d; }", "{ const a = 10; a = 20; }", "{ char ch = 'ff'; }", "foobar()", "print(10)", 
        };

        [Fact]
        public static void ItRuns()
        {
            var text = new string[] { 
                "\r\n{\r\n    var a = 10;\r\n    var b = 100;\r\n    for(; a < b; a += 1)\r\n    {\r\n        b -= 1;\r\n    }\r\n    var c = \"ab\\c\"\r\n}", 
                "--showtree", "--boundTree" };
            var shouldHaveDiagnostic = @"
{
    int a = 10.10;
    var b = 100;
    for(; a < b; a += 1)
    {
        b -= 1;
    }
}";
            var forLoop = @"
{
    for(var i = 0; i < 10; i++)
    {}
}
";
            var ifCheck = @"
{
    var a = 10;
    if(a == 10)
    {
        a += 20;
    }
    else
    {
        a = 10;
    }
}
";
            var whileCheck = @"
    var i = 0;
    while(i < 100)
    {
        i += 10;
    }
";
            var shouldFallIntoDefault = "aj";
            Assert.True(Uranium.Emit(text));

            text[0] = shouldHaveDiagnostic;
            text[1] = shouldFallIntoDefault;
            Assert.True(Uranium.Emit(text));
            Assert.False(Uranium.Emit(Array.Empty<string>()));

            text[0] = forLoop;
            Assert.True(Uranium.Emit(text));

            text[0] = ifCheck;
            Assert.True(Uranium.Emit(text));
 
            text[0] = whileCheck;
            Assert.True(Uranium.Emit(text));


            for(int i = 0; i < _diagnostics.Length; i++)
            {
                var item = new string[] { _diagnostics[i] };
                Uranium.Emit(item);
                Assert.False(Uranium.Diagnostics.IsEmpty);
            }

            var mutliLineString = @"

";
            var MLSTestCase = new string[] { $"\"{mutliLineString}" };
            Uranium.Emit(MLSTestCase);
            Assert.False(Uranium.Diagnostics.IsEmpty);

        }
    }
}
