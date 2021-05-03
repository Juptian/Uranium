using System;
using System.Collections.Generic;
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
        [InlineData("let a = 10 * 10;", 100)]
        [InlineData("var a = 10 * 10;", 100)]
        [InlineData("const a = 10 * 10;", 100)]
        [InlineData("3 == true", true)]
        [InlineData("100 == false", false)]
        [InlineData("0 == false", true)]
        [InlineData("0 != false", false)]
        [InlineData("false == 0", true)]
        [InlineData("true == 1", true)]
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
", 10)]
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
        [MemberData(nameof(TestCases))]
        public void SyntaxFactGetTextRoundTrips(string text, object expectedResult)
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
            for (int i = 1; i <= 100; i += 10)
            {
                yield return new object[] { $"{i} + {i}", i + i };
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
            
                yield return new object[] { $"{i + 1} + {i + 1}", (i + 1) << 1};
                yield return new object[] { $"{i + 1} + -{i + 1}/2", i + 1 + -((i + 1) >> 1) };
                yield return new object[] { $"{i + 1} * {i + 1}", (i + 1) * (i + 1) };
                yield return new object[] { $"{i + 1} * 2 / {i + 1}", 2 };
                yield return new object[] { $"var a = {i + 1};", i + 1 };
                yield return new object[] { $"let a = {i + 1};", i + 1};
                yield return new object[] { $"const a = {i + 1};", i + 1};
                yield return new object[] { $"{i + 1} <= {i + 2}", true };
                yield return new object[] { $"{i + 1} < {i + 1}", false };
                yield return new object[] { $"{i + 2} > {i + 1}", true };
                yield return new object[] { $"{i} >= {i + 1}", false };
           
                yield return new object[] { $"{i + 2} + {i + 2}", (i + 2) << 1 };
                yield return new object[] { $"{i + 2} + -{i + 2}/2", i + 2 + -((i + 2) >> 1) };
                yield return new object[] { $"{i + 2} * {i + 2}", (i + 2) * (i + 2) };
                yield return new object[] { $"{i + 2} * 2 / {i + 2}", 2 };
                yield return new object[] { $"var a = {i + 2};", i + 2 };
                yield return new object[] { $"let a = {i + 2};", i + 2};
                yield return new object[] { $"const a = {i + 2};", i + 2};
                yield return new object[] { $"{i + 2} <= {i + 3}", true };
                yield return new object[] { $"{i + 2} < {i + 2}", false };
                yield return new object[] { $"{i + 3} > {i + 2}", true };
                yield return new object[] { $"{i + 1} >= {i + 2}", false };

                yield return new object[] { $"{i + 3} + {i + 3}", (i + 3) << 1 };
                yield return new object[] { $"{i + 3} + -{i + 3}/2", i + 3 + -((i + 3) >> 1) };
                yield return new object[] { $"{i + 3} * {i + 3}", (i + 3) * (i + 3) };
                yield return new object[] { $"{i + 3} * 2 / {i + 3}", 2 };
                yield return new object[] { $"var a = {i + 3};", i + 3 };
                yield return new object[] { $"let a = {i + 3};", i + 3};
                yield return new object[] { $"const a = {i + 3};", i + 3};
                yield return new object[] { $"{i + 3} <= {i + 4}", true };
                yield return new object[] { $"{i + 3} < {i + 3}", false };
                yield return new object[] { $"{i + 4} > {i + 3}", true };
                yield return new object[] { $"{i + 2} >= {i + 3}", false };
           
                yield return new object[] { $"{i + 4} + {i + 4}", (i + 4) << 1 };
                yield return new object[] { $"{i + 4} + -{i + 4}/2", i + 4 + -((i + 4) >> 1) };
                yield return new object[] { $"{i + 4} * {i + 4}", (i + 4) * (i + 4) };
                yield return new object[] { $"{i + 4} * 2 / {i + 4}", 2 };
                yield return new object[] { $"var a = {i + 4};", i + 4 };
                yield return new object[] { $"let a = {i + 4};", i + 4};
                yield return new object[] { $"const a = {i + 4};", i + 4};
                yield return new object[] { $"{i + 4} <= {i + 5}", true };
                yield return new object[] { $"{i + 4} < {i + 4}", false };
                yield return new object[] { $"{i + 5} > {i + 4}", true };
                yield return new object[] { $"{i + 3} >= {i + 4}", false };

                yield return new object[] { $"{i + 5} + {i + 5}", (i + 5) << 1};
                yield return new object[] { $"{i + 5} + -{i + 5}/2", i + 5 + -((i + 5) >> 1) };
                yield return new object[] { $"{i + 5} * {i + 5}", (i + 5) * (i + 5) };
                yield return new object[] { $"{i + 5} * 2 / {i + 5}", 2 };
                yield return new object[] { $"var a = {i + 5};", i + 5 };
                yield return new object[] { $"let a = {i + 5};", i + 5 };
                yield return new object[] { $"const a = {i + 5};", i + 5 };
                yield return new object[] { $"{i + 5} <= {i + 6}", true };
                yield return new object[] { $"{i + 5} < {i + 5}", false };
                yield return new object[] { $"{i + 6} > {i + 5}", true };
                yield return new object[] { $"{i + 4} >= {i + 5}", false };
           
                yield return new object[] { $"{i + 6} + {i + 6}", (i + 6) << 1 };
                yield return new object[] { $"{i + 6} + -{i + 6}/2", i + 6 + -((i + 6) >> 1) };
                yield return new object[] { $"{i + 6} * {i + 6}", (i + 6) * (i + 6) };
                yield return new object[] { $"{i + 6} * 2 / {i + 6}", 2 };
                yield return new object[] { $"var a = {i + 6};", i + 6 };
                yield return new object[] { $"let a = {i + 6};", i + 6 };
                yield return new object[] { $"const a = {i + 6};", i + 6 };
                yield return new object[] { $"{i + 6} <= {i + 7}", true };
                yield return new object[] { $"{i + 6} < {i + 6}", false };
                yield return new object[] { $"{i + 7} > {i + 6}", true };
                yield return new object[] { $"{i + 5} >= {i + 6}", false };

                yield return new object[] { $"{i + 7} + {i + 7}", (i + 7) << 1 };
                yield return new object[] { $"{i + 7} + -{i + 7}/2", i + 7 + -((i + 7) >> 1) };
                yield return new object[] { $"{i + 7} * {i + 7}", (i + 7) * (i + 7) };
                yield return new object[] { $"{i + 7} * 2 / {i + 7}", 2 };
                yield return new object[] { $"var a = {i + 7};", i + 7 };
                yield return new object[] { $"let a = {i + 7};", i + 7 };
                yield return new object[] { $"const a = {i + 7};", i + 7 };
                yield return new object[] { $"{i + 7} <= {i + 8}", true };
                yield return new object[] { $"{i + 7} < {i + 7}", false };
                yield return new object[] { $"{i + 8} > {i + 7}", true };
                yield return new object[] { $"{i + 6} >= {i + 7}", false };
           
                yield return new object[] { $"{i + 8} + {i + 8}", (i + 8) << 1 };
                yield return new object[] { $"{i + 8} + -{i + 8}/2", i + 8 + -((i + 8) >> 1) };
                yield return new object[] { $"{i + 8} * {i + 8}", (i + 8) * (i + 8) };
                yield return new object[] { $"{i + 8} * 2 / {i + 8}", 2 };
                yield return new object[] { $"var a = {i + 8};", i + 8 };
                yield return new object[] { $"let a = {i + 8};", i + 8 };
                yield return new object[] { $"const a = {i + 8};", i + 8 };
                yield return new object[] { $"{i + 8} <= {i + 9}", true };
                yield return new object[] { $"{i + 8} < {i + 8}", false };
                yield return new object[] { $"{i + 9} > {i + 8}", true };
                yield return new object[] { $"{i + 7} >= {i + 8}", false };
           
                yield return new object[] { $"{i + 9} + {i + 9}", (i + 9) << 1 };
                yield return new object[] { $"{i + 9} + -{i + 9}/2", i + 9 + -((i + 9) >> 1) };
                yield return new object[] { $"{i + 9} * {i + 9}", (i + 9) * (i + 9) };
                yield return new object[] { $"{i + 9} * 2 / {i + 9}", 2 };
                yield return new object[] { $"var a = {i + 9};", i + 9 };
                yield return new object[] { $"let a = {i + 9};", i + 9 };
                yield return new object[] { $"const a = {i + 9};", i + 9 };
                yield return new object[] { $"{i + 9} <= {i + 10}", true };
                yield return new object[] { $"{i + 9} < {i + 9}", false };
                yield return new object[] { $"{i + 10} > {i + 9}", true };
                yield return new object[] { $"{i + 8} >= {i + 9}", false };
            }

            for (int i = 1; i <= 50; i++)
            {
                yield return new object[] { "{ " + $"var a = {i}; a += {i}" + " }", i + i };
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
            for (int i = 0; i <= 200; i++)
            {
                yield return new object[] { "{ " + $"int i = {i * 100};" + " }", i * 100 };
                yield return new object[] { "{ " + $"long i = {i * 2000};" + " }", (long)i * 2000 };
            }
            for (int i = 0; i < 100; i++)
            {
                yield return new object[] { $"{i} + -{i}", 0 };
                yield return new object[] { $"{i} + +{i}", i + i };
            }
            for (int i = 1; i <= 100; i++)
            {
                yield return new object[] { "{ " + $"int i = {i}; i += {i}" + " }", i + i };
                yield return new object[] { "{ " + $"int i = {i}; i -= {i}" + " }", 0 };
                yield return new object[] { "{ " + $"int i = {i}; i /= {i}" + " }", 1 };
                yield return new object[] { "{ " + $"int i = 1; i **= {i}" + " }", 1 };
            }
            for (int i = 1; i <= 100; i++)
            {
                yield return new object[] { "{ " + $"int i = {i}; i *= {i}" + " }", i * i };
            }

            //Testing for high numbers
            //To cover most cases
            for(int i = 999_900; i < 1_000_000; i += 10)
            {
                yield return new object[] { $"{i} + {i}", i + i };
                yield return new object[] { $"{i} + -{i}/2", i + -i / 2 };
                yield return new object[] { $"{i} * {i}", i * i };
                yield return new object[] { $"{i} * 2 / {i}", 2 };
                yield return new object[] { $"var a = {i};", i };
                yield return new object[] { $"let a = {i};", i };
                yield return new object[] { $"const a = {i};", i };
                yield return new object[] { $"{i} <= {i + 1}", true };
                yield return new object[] { $"{i} < {i}", false };
                yield return new object[] { $"{i + 1} > {i}", true };
                yield return new object[] { $"{i - 1} >= {i}", false };
            
                yield return new object[] { $"{i + 1} + {i + 1}", (i + 1) << 1};
                yield return new object[] { $"{i + 1} + -{i + 1}/2", i + 1 + -((i + 1) >> 1) };
                yield return new object[] { $"{i + 1} * {i + 1}", (i + 1) * (i + 1) };
                yield return new object[] { $"{i + 1} * 2 / {i + 1}", 2 };
                yield return new object[] { $"var a = {i + 1};", i + 1 };
                yield return new object[] { $"let a = {i + 1};", i + 1};
                yield return new object[] { $"const a = {i + 1};", i + 1};
                yield return new object[] { $"{i + 1} <= {i + 2}", true };
                yield return new object[] { $"{i + 1} < {i + 1}", false };
                yield return new object[] { $"{i + 2} > {i + 1}", true };
                yield return new object[] { $"{i} >= {i + 1}", false };
           
                yield return new object[] { $"{i + 2} + {i + 2}", (i + 2) << 1 };
                yield return new object[] { $"{i + 2} + -{i + 2}/2", i + 2 + -((i + 2) >> 1) };
                yield return new object[] { $"{i + 2} * {i + 2}", (i + 2) * (i + 2) };
                yield return new object[] { $"{i + 2} * 2 / {i + 2}", 2 };
                yield return new object[] { $"var a = {i + 2};", i + 2 };
                yield return new object[] { $"let a = {i + 2};", i + 2};
                yield return new object[] { $"const a = {i + 2};", i + 2};
                yield return new object[] { $"{i + 2} <= {i + 3}", true };
                yield return new object[] { $"{i + 2} < {i + 2}", false };
                yield return new object[] { $"{i + 3} > {i + 2}", true };
                yield return new object[] { $"{i + 1} >= {i + 2}", false };

                yield return new object[] { $"{i + 3} + {i + 3}", (i + 3) << 1 };
                yield return new object[] { $"{i + 3} + -{i + 3}/2", i + 3 + -((i + 3) >> 1) };
                yield return new object[] { $"{i + 3} * {i + 3}", (i + 3) * (i + 3) };
                yield return new object[] { $"{i + 3} * 2 / {i + 3}", 2 };
                yield return new object[] { $"var a = {i + 3};", i + 3 };
                yield return new object[] { $"let a = {i + 3};", i + 3};
                yield return new object[] { $"const a = {i + 3};", i + 3};
                yield return new object[] { $"{i + 3} <= {i + 4}", true };
                yield return new object[] { $"{i + 3} < {i + 3}", false };
                yield return new object[] { $"{i + 4} > {i + 3}", true };
                yield return new object[] { $"{i + 2} >= {i + 3}", false };
           
                yield return new object[] { $"{i + 4} + {i + 4}", (i + 4) << 1 };
                yield return new object[] { $"{i + 4} + -{i + 4}/2", i + 4 + -((i + 4) >> 1) };
                yield return new object[] { $"{i + 4} * {i + 4}", (i + 4) * (i + 4) };
                yield return new object[] { $"{i + 4} * 2 / {i + 4}", 2 };
                yield return new object[] { $"var a = {i + 4};", i + 4 };
                yield return new object[] { $"let a = {i + 4};", i + 4};
                yield return new object[] { $"const a = {i + 4};", i + 4};
                yield return new object[] { $"{i + 4} <= {i + 5}", true };
                yield return new object[] { $"{i + 4} < {i + 4}", false };
                yield return new object[] { $"{i + 5} > {i + 4}", true };
                yield return new object[] { $"{i + 3} >= {i + 4}", false };

                yield return new object[] { $"{i + 5} + {i + 5}", (i + 5) << 1};
                yield return new object[] { $"{i + 5} + -{i + 5}/2", i + 5 + -((i + 5) >> 1) };
                yield return new object[] { $"{i + 5} * {i + 5}", (i + 5) * (i + 5) };
                yield return new object[] { $"{i + 5} * 2 / {i + 5}", 2 };
                yield return new object[] { $"var a = {i + 5};", i + 5 };
                yield return new object[] { $"let a = {i + 5};", i + 5 };
                yield return new object[] { $"const a = {i + 5};", i + 5 };
                yield return new object[] { $"{i + 5} <= {i + 6}", true };
                yield return new object[] { $"{i + 5} < {i + 5}", false };
                yield return new object[] { $"{i + 6} > {i + 5}", true };
                yield return new object[] { $"{i + 4} >= {i + 5}", false };
           
                yield return new object[] { $"{i + 6} + {i + 6}", (i + 6) << 1 };
                yield return new object[] { $"{i + 6} + -{i + 6}/2", i + 6 + -((i + 6) >> 1) };
                yield return new object[] { $"{i + 6} * {i + 6}", (i + 6) * (i + 6) };
                yield return new object[] { $"{i + 6} * 2 / {i + 6}", 2 };
                yield return new object[] { $"var a = {i + 6};", i + 6 };
                yield return new object[] { $"let a = {i + 6};", i + 6 };
                yield return new object[] { $"const a = {i + 6};", i + 6 };
                yield return new object[] { $"{i + 6} <= {i + 7}", true };
                yield return new object[] { $"{i + 6} < {i + 6}", false };
                yield return new object[] { $"{i + 7} > {i + 6}", true };
                yield return new object[] { $"{i + 5} >= {i + 6}", false };

                yield return new object[] { $"{i + 7} + {i + 7}", (i + 7) << 1 };
                yield return new object[] { $"{i + 7} + -{i + 7}/2", i + 7 + -((i + 7) >> 1) };
                yield return new object[] { $"{i + 7} * {i + 7}", (i + 7) * (i + 7) };
                yield return new object[] { $"{i + 7} * 2 / {i + 7}", 2 };
                yield return new object[] { $"var a = {i + 7};", i + 7 };
                yield return new object[] { $"let a = {i + 7};", i + 7 };
                yield return new object[] { $"const a = {i + 7};", i + 7 };
                yield return new object[] { $"{i + 7} <= {i + 8}", true };
                yield return new object[] { $"{i + 7} < {i + 7}", false };
                yield return new object[] { $"{i + 8} > {i + 7}", true };
                yield return new object[] { $"{i + 6} >= {i + 7}", false };
           
                yield return new object[] { $"{i + 8} + {i + 8}", (i + 8) << 1 };
                yield return new object[] { $"{i + 8} + -{i + 8}/2", i + 8 + -((i + 8) >> 1) };
                yield return new object[] { $"{i + 8} * {i + 8}", (i + 8) * (i + 8) };
                yield return new object[] { $"{i + 8} * 2 / {i + 8}", 2 };
                yield return new object[] { $"var a = {i + 8};", i + 8 };
                yield return new object[] { $"let a = {i + 8};", i + 8 };
                yield return new object[] { $"const a = {i + 8};", i + 8 };
                yield return new object[] { $"{i + 8} <= {i + 9}", true };
                yield return new object[] { $"{i + 8} < {i + 8}", false };
                yield return new object[] { $"{i + 9} > {i + 8}", true };
                yield return new object[] { $"{i + 7} >= {i + 8}", false };
           
                yield return new object[] { $"{i + 9} + {i + 9}", (i + 9) << 1 };
                yield return new object[] { $"{i + 9} + -{i + 9}/2", i + 9 + -((i + 9) >> 1) };
                yield return new object[] { $"{i + 9} * {i + 9}", (i + 9) * (i + 9) };
                yield return new object[] { $"{i + 9} * 2 / {i + 9}", 2 };
                yield return new object[] { $"var a = {i + 9};", i + 9 };
                yield return new object[] { $"let a = {i + 9};", i + 9 };
                yield return new object[] { $"const a = {i + 9};", i + 9 };
                yield return new object[] { $"{i + 9} <= {i + 10}", true };
                yield return new object[] { $"{i + 9} < {i + 9}", false };
                yield return new object[] { $"{i + 10} > {i + 9}", true };
                yield return new object[] { $"{i + 8} >= {i + 9}", false };
            }

            for(int i = 999_900; i < 1_000_000; i += 10)
            {
                yield return new object[] { $"{i} + -{i}", 0 };
                yield return new object[] { $"{i} + +{i}", i << 1 };

                yield return new object[] { $"{i + 1} + -{i + 1}", 0 };
                yield return new object[] { $"{i + 1} + +{i + 1}", (i + 1) << 1};

                yield return new object[] { $"{i + 2} + -{i + 2}", 0 };
                yield return new object[] { $"{i + 2} + +{i + 2}", (i + 2) << 1};

                yield return new object[] { $"{i + 3} + -{i + 3}", 0 };
                yield return new object[] { $"{i + 3} + +{i + 3}", (i + 3) << 1};

                yield return new object[] { $"{i + 4} + -{i + 4}", 0 };
                yield return new object[] { $"{i + 4} + +{i + 4}", (i + 4) << 1};

                yield return new object[] { $"{i + 5} + -{i + 5}", 0 };
                yield return new object[] { $"{i + 5} + +{i + 5}", (i + 5) << 1};

                yield return new object[] { $"{i + 6} + -{i + 6}", 0 };
                yield return new object[] { $"{i + 6} + +{i + 6}", (i + 6) << 1};

                yield return new object[] { $"{i + 7} + -{i + 7}", 0 };
                yield return new object[] { $"{i + 7} + +{i + 7}", (i + 7) << 1};

                yield return new object[] { $"{i + 8} + -{i + 8}", 0 };
                yield return new object[] { $"{i + 8} + +{i + 8}", (i + 8) << 1};

                yield return new object[] { $"{i + 9} + -{i + 9}", 0 };
                yield return new object[] { $"{i + 9} + +{i + 9}", (i + 9) << 1};
            }


            for(int i = 999_900; i < 1_000_000; i += 10)
            {
                yield return new object[] { "{ " + $"int i = {i}; i += {i}" + " }", i << 1 };
                yield return new object[] { "{ " + $"int i = {i}; i -= {i}" + " }", 0 };
                yield return new object[] { "{ " + $"int i = {i}; i /= {i}" + " }", 1 };
                yield return new object[] { "{ " + $"int i = 1; i **= {i}" + " }", 1 };

                yield return new object[] { "{ " + $"int i = {i + 1}; i += {i + 1}" + " }", (i + 1) << 1 };
                yield return new object[] { "{ " + $"int i = {i + 1}; i -= {i + 1}" + " }", 0 };
                yield return new object[] { "{ " + $"int i = {i + 1}; i /= {i + 1}" + " }", 1 };
                yield return new object[] { "{ " + $"int i = 1; i **= {i + 1}" + " }", 1 };

                yield return new object[] { "{ " + $"int i = {i + 2}; i += {i + 2}" + " }", (i + 2) << 1 };
                yield return new object[] { "{ " + $"int i = {i + 2}; i -= {i + 2}" + " }", 0 };
                yield return new object[] { "{ " + $"int i = {i + 2}; i /= {i + 2}" + " }", 1 };
                yield return new object[] { "{ " + $"int i = 1; i **= {i + 2}" + " }", 1 };

                yield return new object[] { "{ " + $"int i = {i + 3}; i += {i + 3}" + " }", (i + 3) << 1 };
                yield return new object[] { "{ " + $"int i = {i + 3}; i -= {i + 3}" + " }", 0 };
                yield return new object[] { "{ " + $"int i = {i + 3}; i /= {i + 3}" + " }", 1 };
                yield return new object[] { "{ " + $"int i = 1; i **= {i + 3}" + " }", 1 };

                yield return new object[] { "{ " + $"int i = {i + 4}; i += {i + 4}" + " }", (i + 4) << 1 };
                yield return new object[] { "{ " + $"int i = {i + 4}; i -= {i + 4}" + " }", 0 };
                yield return new object[] { "{ " + $"int i = {i + 4}; i /= {i + 4}" + " }", 1 };
                yield return new object[] { "{ " + $"int i = 1; i **= {i + 4}" + " }", 1 };

                yield return new object[] { "{ " + $"int i = {i + 5}; i += {i + 5}" + " }", (i + 5) << 1 };
                yield return new object[] { "{ " + $"int i = {i + 5}; i -= {i + 5}" + " }", 0 };
                yield return new object[] { "{ " + $"int i = {i + 5}; i /= {i + 5}" + " }", 1 };
                yield return new object[] { "{ " + $"int i = 1; i **= {i + 5}" + " }", 1 };

                yield return new object[] { "{ " + $"int i = {i + 6}; i += {i + 6}" + " }", (i + 6) << 1 };
                yield return new object[] { "{ " + $"int i = {i + 6}; i -= {i + 6}" + " }", 0 };
                yield return new object[] { "{ " + $"int i = {i + 6}; i /= {i + 6}" + " }", 1 };
                yield return new object[] { "{ " + $"int i = 1; i **= {i + 6}" + " }", 1 };

                yield return new object[] { "{ " + $"int i = {i + 7}; i += {i + 7}" + " }", (i + 7) << 1 };
                yield return new object[] { "{ " + $"int i = {i + 7}; i -= {i + 7}" + " }", 0 };
                yield return new object[] { "{ " + $"int i = {i + 7}; i /= {i + 7}" + " }", 1 };
                yield return new object[] { "{ " + $"int i = 1; i **= {i + 7}" + " }", 1 };

                yield return new object[] { "{ " + $"int i = {i + 8}; i += {i + 8}" + " }", (i + 8) << 1 };
                yield return new object[] { "{ " + $"int i = {i + 8}; i -= {i + 8}" + " }", 0 };
                yield return new object[] { "{ " + $"int i = {i + 8}; i /= {i + 8}" + " }", 1 };
                yield return new object[] { "{ " + $"int i = 1; i **= {i + 8}" + " }", 1 };

                yield return new object[] { "{ " + $"int i = {i + 9}; i += {i + 9}" + " }", (i + 9) << 1 };
                yield return new object[] { "{ " + $"int i = {i + 9}; i -= {i + 9}" + " }", 0 };
                yield return new object[] { "{ " + $"int i = {i + 9}; i /= {i + 9}" + " }", 1 };
                yield return new object[] { "{ " + $"int i = 1; i **= {i + 9}" + " }", 1 };
            }
        }
    }
}
