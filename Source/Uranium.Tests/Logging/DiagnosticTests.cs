using System;
using System.Collections;
using System.Collections.Immutable;
using Uranium.Logging;
using Uranium.CodeAnalysis.Syntax;
using Xunit;

namespace Uranium.Tests.Logging
{
    public static class DiagnosticTests
    {
        [Fact]
        public static void TestDiagnostics()
        {
            var text = @"
1
{
    let a = _100;
    double b = avc;
    while(true
    {
    }
    a =  ^100;
    long a = 10.1;
    long b = 100;
    var c = a && b
    100 += 10;
    1 = 2;
}
";
            var tree = SyntaxTree.Parse(text);
            Assert.NotEmpty(tree.Diagnostics);
            tree.Diagnostics.GetEnumerator();
        }
    }
}
