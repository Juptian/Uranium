using System;
using System.Collections.Generic;
using Xunit;
using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Text;
using Uranium.CodeAnalysis;
using Uranium.CodeAnalysis.Symbols;

namespace Uranium.Tests.CodeAnalysis.Expression
{
    public class EvaluatorTests
    {
        private const int fourPowEight = 65536;
        [Theory]
        [InlineData("1", 1)]
        [InlineData("+1", 1)]
        [InlineData("-1", -1)]
        [InlineData("~1", ~1)]
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

        [InlineData("1 && true", true)]
        [InlineData("11 && false", false)]
        [InlineData("10.1 || false", true)]
        [InlineData("0 && false", false)]
        [InlineData("0 && true", false)]
        [InlineData("0 || true", true)]

        [InlineData("true && 1", true)]
        [InlineData("false && 11", false)]
        [InlineData("false || 10.1", true)]
        [InlineData("false && 0", false)]
        [InlineData("false && 1", false)]
        [InlineData("false || 1", true)]

        [InlineData("1 && 1", true)]
        [InlineData("0 && 11", false)]
        [InlineData("0 || 10.1", true)]
        [InlineData("10 && 0", false)]
        [InlineData("0 && 1", false)]
        [InlineData("0 || 1", true)]
        
        [InlineData("1 + false", 1)]
        [InlineData("false + 1", 1)]
        [InlineData("1 + true", 2)]
        [InlineData("true + 1", 2)]

        [InlineData("1 | 2", (1 | 3))]
        [InlineData("1 | 0", (1 | 0))]
        [InlineData("1 & 2", (1 & 2))]
        [InlineData("1 & 0", (1 & 0))]
        [InlineData("1 ^ 0", (1 ^ 0))]
        [InlineData("1 ^ 3", (1 ^ 3))]

        [InlineData("false | false", false)]
        [InlineData("true | true", true)]
        [InlineData("false | true", true)]
        [InlineData("true | false", true)]

        [InlineData("false & false", false)]
        [InlineData("true & true", true)]
        [InlineData("false & true", false)]
        [InlineData("true & false", false)]

        [InlineData("false ^ false", false)]
        [InlineData("true ^ true", false)]
        [InlineData("false ^ true", true)]
        [InlineData("true ^ false", true)]

        [InlineData("true == false", false)]
        [InlineData("true != false", true)]
        [InlineData("false != true", true)]
        [InlineData("false == false", true)]
        [InlineData("!false", true)]
        [InlineData("!true", false)]
        [InlineData("let a = 10 * 10;", 100)]
        [InlineData("var a = 10 * 10;", 100)]
        [InlineData("const a = 10 * 10;", 100)]
        [InlineData("3 == true", true)]
        [InlineData("100 == false", false)]
        [InlineData("0 == false", true)]
        [InlineData("0 != false", false)]
        [InlineData("false == 0", true)]
        [InlineData("true == 1", true)]
        [InlineData("var a = 0.2147483;", (double)0.2147483)]
        [InlineData("{ var a = 10; a--; }", 9)]
        [InlineData("{ var a = 10; a++; }", 11)]
        [InlineData(@"
{ 
    var a = 10; 
    if(a == 10) 
    {
        a = 100;
    }
}", 100)]
        [InlineData(@"
{ 
    var a = 10;
    if (a == 5)
    {
        a = 19;
    }
}", 10)]
        [InlineData(@"
{ 
    var a = 10;
    if (a == 10) 
    {
        a = 100; 
    }
    else
    {
        a = 0;
    }
}", 100)]
        [InlineData(@"
{ 
    var a = 10;
    if(a == 5)
    {
        a = 100;
    }    
    else 
    {
        a = 0;
    }
}", 0)]
        [InlineData(@"
{ 
    var i = 10;
    var result = 0;
    while (i > 0) 
    {
        result = result + i;
        i = i - 1;
    }
    result;
}", 55)]
        [InlineData(@"
{
    for(var i = 0; i <= 10; i = i + 1)
    {
        i;
    }
}
", 11)]
        [InlineData(@"
{
    var a = 10;
    a += 10;
}", 20)]
        [InlineData(@"
{
    var a = 10;
    a -= 10;
}", 0)]
        [InlineData(@"
{
    var a = 10;
    a *= 10;
}", 100)]
        [InlineData(@"
{
    var a = 10;
    a /= 10;
}", 1)]
        [InlineData(@"
{
    var a = 10;
    a **= 5;
}", 100000)]
        [InlineData(@"
{
    var a = 10;
    var b = 1 + 2;
    a **= a - 10 + (b - 1);
}", 100)]
        [InlineData("{ var a = 10; { var b = a; } }", 10)]
        [InlineData("4**8", fourPowEight)]
        [InlineData("4**9", 262144)]
        [MemberData(nameof(TestCases))]
        public void EvaluatorTestCases(string text, object expectedResult)
        {
            var syntaxTree = SyntaxTree.Parse(text);
            var compilation = new Compilation(syntaxTree);
            var variables = new Dictionary<VariableSymbol, object>();
            var actualResult = compilation.Evaluate(variables);

            if (text.Equals("BadToken"))
            {
                return;
            }
            Assert.Empty(actualResult.Diagnostics);
            Assert.Equal(expectedResult, actualResult.Value);
        }
        public static IEnumerable<object[]> TestCases()
        {
            //Low numbers
            //Unrolled for effieciency sake
            for (int i = 1; i <= 50; i++)
            {
                yield return new object[] { $"{i} + {i}", i << 1 };
                yield return new object[] { $"{i} + -{i}/2", i + -(i >> 1) };
                yield return new object[] { $"{i} * {i}", i * i };
                yield return new object[] { $"{i} * 2 / {i}", 2 };
                yield return new object[] { $"var a = {i};", i };
                yield return new object[] { $"let a = {i};", i };
                yield return new object[] { $"const a = {i};", i };
                yield return new object[] { $"{i} <= {i + 1}", true };
                yield return new object[] { $"{i} < {i}", false };
                yield return new object[] { $"{i + 1} > {i}", true };
                yield return new object[] { $"{i - 1} >= {i}", false };
            }

            for (int i = 1; i <= 50; i++)
            {
                yield return new object[] { "{ " + $"var a = {i}; a += {i}" + " }", i << 1 };
                yield return new object[] { "{ " + $"var a = {i}; a -= {i}" + " }", 0 };
                yield return new object[] { "{ " + $"var a = {i}; a *= {i}" + " }", i * i };
                yield return new object[] { "{ " + $"var a = {i}; a /= {i}" + " }", 1 };
                yield return new object[] { "{ " + $"var a = {i}; a **= ({i}-{i}+1) + ({i}/{i} + 1)" + " }", i * i * i };
            }
            for (float i = 0.01f; i <= 2; i += 0.01f)
            {
                yield return new object[] { "{ " + $"float i = {i};" + " }", (float)i };
                yield return new object[] { "{ " + $"double i = {(double)i};" + " }", (double)i };
            }
            for (int i = 0; i <= 50; i++)
            {
                yield return new object[] { "{ " + $"int i = {i * 100};" + " }", i * 100 };
                yield return new object[] { "{ " + $"long i = {i * 2000};" + " }", (long)i * 2000 };
            }
            for (int i = 0; i < 50; i++)
            {
                yield return new object[] { $"{i} + -{i}", 0 };
                yield return new object[] { $"{i} + +{i}", i << 1 };
            }
            for(int i = 1; i <= 3; i++)
            {
                //Compound operators
                yield return new object[] { "{ " + $"int i = {i}; i += {i}" + " }", i << 1 };
                yield return new object[] { "{ " + $"int i = {i}; i -= {i}" + " }", 0 };
                yield return new object[] { "{ " + $"int i = {i}; i /= {i}" + " }", 1 };
                yield return new object[] { "{ " + $"int i = {i}; i *= {i}" + " }", i * i };
                yield return new object[] { "{ " + $"int i = 1; i **= {i}" + " }", 1 };
                yield return new object[] { "{ " + $"int i = {i}; i++;" + " }", i + 1 };
                yield return new object[] { "{ " + $"int i = {i}; i--;" + " }", i - 1 };
                
                yield return new object[] { "{ " + $"long i = {i}; i += {i}" + " }", (long)(i << 1) };
                yield return new object[] { "{ " + $"long i = {i}; i -= {i}" + " }", (long)0 };
                yield return new object[] { "{ " + $"long i = {i}; i /= {i}" + " }", (long)1 };
                yield return new object[] { "{ " + $"long i = 1; int b = {i}; i **= b" + " }", (long)1 };
                yield return new object[] { "{ " + $"long i = 1; long  b = {i}; i **= b" + " }", (long)1 };
                yield return new object[] { "{ " + $"long i = {i}; i *= {i}" + " }", (long)(i * i) };
                yield return new object[] { "{ " + $"long i = {i}; i++;" + " }", (long)(i + 1) };
                yield return new object[] { "{ " + $"long i = {i}; i--;" + " }", (long)(i - 1) };

                yield return new object[] { "{ " + $"float i = {i}; i += {i}" + " }", (float)(i << 1) };
                yield return new object[] { "{ " + $"float i = {i}; i -= {i}" + " }", (float)0 };
                yield return new object[] { "{ " + $"float i = {i}; i /= {i}" + " }", (float)1 };
                yield return new object[] { "{ " + $"float i = {i}; i *= {i}" + " }", (float)(i * i) };
                yield return new object[] { "{ " + $"float i = 1; i **= {i}" + " }", (float)1 };
                yield return new object[] { "{ " + $"float i = {i}; i++;" + " }", (float)(i + 1) };
                yield return new object[] { "{ " + $"float i = {i}; i--;" + " }", (float)(i - 1) };

                yield return new object[] { "{ " + $"double i = {i}; i += {i}" + " }", (double)(i << 1) };
                yield return new object[] { "{ " + $"double i = {i}; i -= {i}" + " }", (double)(0) };
                yield return new object[] { "{ " + $"double i = {i}; i /= {i}" + " }", (double)(1) };
                yield return new object[] { "{ " + $"double i = {i}; i *= {i}" + " }", (double)(i * i) };

                yield return new object[] { "{ " + $"double i = 1; i **= {i}" + " }", (double)(1) };
                yield return new object[] { "{ " + $"double i = {i}; i++;" + " }", (double)(i + 1) };
                yield return new object[] { "{ " + $"double i = {i}; i--;" + " }", (double)(i - 1) };

                yield return new object[] { "{ " + $"long i = {i}; i < i" + " }", false };
                yield return new object[] { "{ " + $"long i = {i}; i <= i" + " }", true };
                yield return new object[] { "{ " + $"long i = {i}; i > i" + " }", false };
                yield return new object[] { "{ " + $"long i = {i}; i >= i" + " }", true };

                yield return new object[] { "{ " + $"float i = {i}; i < i" + " }", false };
                yield return new object[] { "{ " + $"float i = {i}; i <= i" + " }", true };
                yield return new object[] { "{ " + $"float i = {i}; i > i" + " }", false };
                yield return new object[] { "{ " + $"float i = {i}; i >= i" + " }", true };

                yield return new object[] { "{ " + $"double i = {i}; i < i" + " }", false };
                yield return new object[] { "{ " + $"double i = {i}; i <= i" + " }", true };
                yield return new object[] { "{ " + $"double i = {i}; i > i" + " }", false };
                yield return new object[] { "{ " + $"double i = {i}; i >= i" + " }", true };
                
                yield return new object[] { "{ " + $"int i = {i}; float f = {i}; i += f;" + " }", i << 1 };
                yield return new object[] { "{ " + $"int i = {i}; float f = {i}; i -= f;" + " }", 0 };
                yield return new object[] { "{ " + $"int i = {i}; float f = {i}; i *= f;" + " }", i * i };
                yield return new object[] { "{ " + $"int i = {i}; float f = {i}; i /= f;" + " }", 1 };
                yield return new object[] { "{ " + $"int i = {i}; float f = {i}; i < f;" + " }", false };
                yield return new object[] { "{ " + $"int i = {i}; float f = {i}; i <= f;" + " }", true };
                yield return new object[] { "{ " + $"int i = {i}; float f = {i}; i > f;" + " }", false };
                yield return new object[] { "{ " + $"int i = {i}; float f = {i}; i >= f;" + " }", true };
                yield return new object[] { "{ " + $"int i = 1; float f = {i}; i ** f;" + " }", 1 };
                
                yield return new object[] { "{ " + $"int i = {i}; long f = {i}; i += f;" + " }", i << 1 };
                yield return new object[] { "{ " + $"int i = {i}; long f = {i}; i -= f;" + " }", 0 };
                yield return new object[] { "{ " + $"int i = {i}; long f = {i}; i *= f;" + " }", i * i };
                yield return new object[] { "{ " + $"int i = {i}; long f = {i}; i /= f;" + " }", 1 };
                yield return new object[] { "{ " + $"int i = {i}; long f = {i}; i < f;" + " }", false };
                yield return new object[] { "{ " + $"int i = {i}; long f = {i}; i <= f;" + " }", true };
                yield return new object[] { "{ " + $"int i = {i}; long f = {i}; i > f;" + " }", false };
                yield return new object[] { "{ " + $"int i = {i}; long f = {i}; i >= f;" + " }", true };

                yield return new object[] { "{ " + $"int i = {i}; double f = {i}; i += f;" + " }", i << 1 };
                yield return new object[] { "{ " + $"int i = {i}; double f = {i}; i -= f;" + " }", 0 };
                yield return new object[] { "{ " + $"int i = {i}; double f = {i}; i *= f;" + " }", i * i };
                yield return new object[] { "{ " + $"int i = {i}; double f = {i}; i /= f;" + " }", 1 };
                yield return new object[] { "{ " + $"int i = {i}; double f = {i}; i < f;" + " }", false };
                yield return new object[] { "{ " + $"int i = {i}; double f = {i}; i <= f;" + " }", true };
                yield return new object[] { "{ " + $"int i = {i}; double f = {i}; i > f;" + " }", false };
                yield return new object[] { "{ " + $"int i = {i}; double f = {i}; i >= f;" + " }", true };
                yield return new object[] { "{ " + $"int i = 1; double f = {i}; i ** f;" + " }", 1 };

                yield return new object[] { "{ " + $"float i = {i}; int f = {i}; i += f;" + " }", (float)(i << 1) };
                yield return new object[] { "{ " + $"float i = {i}; int f = {i}; i -= f;" + " }", (float)(0) };
                yield return new object[] { "{ " + $"float i = {i}; int f = {i}; i *= f;" + " }", (float)(i * i) };
                yield return new object[] { "{ " + $"float i = {i}; int f = {i}; i /= f;" + " }", (float)(1) };
                yield return new object[] { "{ " + $"float i = {i}; int f = {i}; i < f;" + " }", false };
                yield return new object[] { "{ " + $"float i = {i}; int f = {i}; i <= f;" + " }", true };
                yield return new object[] { "{ " + $"float i = {i}; int f = {i}; i > f;" + " }", false };
                yield return new object[] { "{ " + $"float i = {i}; int f = {i}; i >= f;" + " }", true };
                yield return new object[] { "{ " + $"float i = 1; int f = {i}; i ** f;" + " }", (float)(1) };

                yield return new object[] { "{ " + $"float i = {i}; double f = {i}; i += f;" + " }", (float)(i << 1) };
                yield return new object[] { "{ " + $"float i = {i}; double f = {i}; i -= f;" + " }", (float)(0) };
                yield return new object[] { "{ " + $"float i = {i}; double f = {i}; i *= f;" + " }", (float)(i * i) };
                yield return new object[] { "{ " + $"float i = {i}; double f = {i}; i /= f;" + " }", (float)(1) };
                yield return new object[] { "{ " + $"float i = {i}; double f = {i}; i < f;" + " }", false };
                yield return new object[] { "{ " + $"float i = {i}; double f = {i}; i <= f;" + " }", true };
                yield return new object[] { "{ " + $"float i = {i}; double f = {i}; i > f;" + " }", false };
                yield return new object[] { "{ " + $"float i = {i}; double f = {i}; i >= f;" + " }", true };
                yield return new object[] { "{ " + $"float i = 1; double f = {i}; i ** f;" + " }", (float)(1) };

                
                yield return new object[] { "{ " + $"double i = {i}; float f = {i}; i += f;" + " }", (double)(i << 1) };
                yield return new object[] { "{ " + $"double i = {i}; float f = {i}; i -= f;" + " }", (double)(0) };
                yield return new object[] { "{ " + $"double i = {i}; float f = {i}; i *= f;" + " }", (double)(i * i) };
                yield return new object[] { "{ " + $"double i = {i}; float f = {i}; i /= f;" + " }", (double)(1) };
                yield return new object[] { "{ " + $"double i = {i}; float f = {i}; i < f;" + " }", false };
                yield return new object[] { "{ " + $"double i = {i}; float f = {i}; i <= f;" + " }", true };
                yield return new object[] { "{ " + $"double i = {i}; float f = {i}; i > f;" + " }", false };
                yield return new object[] { "{ " + $"double i = {i}; float f = {i}; i >= f;" + " }", true };
                yield return new object[] { "{ " + $"double i = 1; float f = {i}; i ** f;" + " }", (double)(1) };
                
                yield return new object[] { "{ " + $"double i = {i}; int f = {i}; i += f;" + " }", (double)(i << 1) };
                yield return new object[] { "{ " + $"double i = {i}; int f = {i}; i -= f;" + " }", (double)(0) };
                yield return new object[] { "{ " + $"double i = {i}; int f = {i}; i *= f;" + " }", (double)(i * i) };
                yield return new object[] { "{ " + $"double i = {i}; int f = {i}; i /= f;" + " }", (double)(1) };
                yield return new object[] { "{ " + $"double i = {i}; int f = {i}; i < f;" + " }", false };
                yield return new object[] { "{ " + $"double i = {i}; int f = {i}; i <= f;" + " }", true };
                yield return new object[] { "{ " + $"double i = {i}; int f = {i}; i > f;" + " }", false };
                yield return new object[] { "{ " + $"double i = {i}; int f = {i}; i >= f;" + " }", true };
                yield return new object[] { "{ " + $"double i = 1; int f = {i}; i ** f;" + " }", (double)(1) };
                
                yield return new object[] { "{ " + $"long i = {i}; int f = {i}; i += f;" + " }", (long)(i << 1) };
                yield return new object[] { "{ " + $"long i = {i}; int f = {i}; i -= f;" + " }", (long)(0) };
                yield return new object[] { "{ " + $"long i = {i}; int f = {i}; i *= f;" + " }", (long)(i * i) };
                yield return new object[] { "{ " + $"long i = {i}; int f = {i}; i /= f;" + " }", (long)1 };
                yield return new object[] { "{ " + $"long i = {i}; int f = {i}; i < f;" + " }", false };
                yield return new object[] { "{ " + $"long i = {i}; int f = {i}; i <= f;" + " }", true };
                yield return new object[] { "{ " + $"long i = {i}; int f = {i}; i > f;" + " }", false };
                yield return new object[] { "{ " + $"long i = {i}; int f = {i}; i >= f;" + " }", true };
                yield return new object[] { "{ " + $"long i = 1; int f = {i}; i ** f;" + " }", (long)1 };

            }

            //Testing for high numbers
            //To cover most cases
            for(int i = 999_950; i < 1_000_000; i++)
            {
                yield return new object[] { $"{i} + {i}", i << 1 };
                yield return new object[] { $"{i} + -{i}/2", i + -(i >> 1) };
                yield return new object[] { $"{i} * {i}", i * i };
                yield return new object[] { $"{i} * 2 / {i}", 2 };
                yield return new object[] { $"var a = {i};", i };
                yield return new object[] { $"let a = {i};", i };
                yield return new object[] { $"const a = {i};", i };
                yield return new object[] { $"{i} <= {i + 1}", true };
                yield return new object[] { $"{i} < {i}", false };
                yield return new object[] { $"{i + 1} > {i}", true };
                yield return new object[] { $"{i - 1} >= {i}", false };
            }

            for(int i = 999_950; i < 1_000_000; i++)
            {
                yield return new object[] { $"{i} + -{i}", 0 };
                yield return new object[] { $"{i} + +{i}", i << 1 };
            }

            for(int i = 999_950; i < 1_000_000; i++)
            {
                yield return new object[] { "{ " + $"int i = {i}; i += {i}" + " }", i << 1 };
                yield return new object[] { "{ " + $"int i = {i}; i -= {i}" + " }", 0 };
                yield return new object[] { "{ " + $"int i = {i}; i /= {i}" + " }", 1 };
                yield return new object[] { "{ " + $"int i = 1; i **= {i}" + " }", 1 };
                yield return new object[] { "{ " + $"int i = {i}; i++;" + " }", i + 1 };
                yield return new object[] { "{ " + $"int i = {i}; i--;" + " }", i - 1 };
            }
        }
    }
}
