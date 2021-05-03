using Uranium.CodeAnalysis.Syntax;

namespace Uranium.Tests.CodeAnalysis.Parsing
{
    internal static class ParserTestCases
    {
        public const string CaseOne = @"
    for(var i = 0; i <= 10; i += 1)
    {
        i;
    }
";
        public const string CaseTwo = @"
    for(int a = 0; a <= 100; a += 1)
    {
        a;
    }
";
        public const string CaseThree = @"
    for(var i = 0; i <= 1000; i += 1)
    {
        i += 1;
    }
";
        public const string CaseFour = @"
    for(var i = 0; i != 1000; i += 1)
    {
        i += 1;
        var a = i * 2;
    }
";
        public const string CaseFive = @"
    for(float f = 0; f < 100; f += 1)
    {
        var b = f + b;
    }
";
        public const string CaseSix = @"
    for(var i = 100; i >= 10; i -= 1)
    {
        i;
    }
";
        public const string CaseSeven = @"
    for(int a = 0; a >= 100; a -= 1)
    {
        a;
    }
";
        public const string CaseEight = @"
        for(int a = 0; a >= 100; a -= 1)
        {
        }
";
        public const string CaseNine = @"
    for(var i = 2000; i != 1000; i -= 1)
    {
        i += 1;
        var a = i * 2;
    }
";
        public const string CaseTen = @"
    for(var i = 2000; i >= 1000; i -= 1) 
    {
        var a = 1;
    }
";
        public const string CaseEleven = @"
    for(float f = 200; f > 100; f -= 1)
    {
        var b = f + b;
    }
";
        public const string CaseTwelve = "int i = 10 + 1";
        public const string CaseThirteen = "double d = 10.10 + 20.20";
        public const string CaseForteen = "float f = 10.3 + 10.3";
        public const string CaseFifteen = "long l = 15 + 10";
        public const string CaseSixteen = "int i = 10 - 1";
        public const string CaseSeventeen = "double d = 10.1001 - 10";
        public const string CaseEighteen = "float f = 10.3 - 10.3";
        public const string CaseNineteen = "long l = 15 - 10";
        public const string CaseTwenty = "int i = 10 ** 2";
        public const string CaseTwentyOne = "long l = 100 ** 2";
        public const string CaseTwentyTwo = "double d = 10 / 100";
        public static string MakeForLoop
            (
                SyntaxKind identifierToken, string identifier, string initializer, 
                SyntaxKind condition, int conditionNumber, 
                SyntaxKind incrementationOperator, string incrementationAmount
            )
        {
            var identifierTokenText = SyntaxFacts.GetText(identifierToken);
            var conditionalText = SyntaxFacts.GetText(condition);
            var incrementOpText = SyntaxFacts.GetText(incrementationOperator);
            var result = $@"
    for({identifierTokenText} {identifier} = {initializer}; {identifier} {conditionalText} {conditionNumber}; {identifier} {incrementOpText} {incrementationAmount})" + @"
    {
        var fkjwaghjkaw = 1;
    }
";
            return result;
        }
    }
}
