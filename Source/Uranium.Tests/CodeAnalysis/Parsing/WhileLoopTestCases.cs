using Uranium.CodeAnalysis.Syntax;

namespace Uranium.Tests.CodeAnalysis.Parsing
{
    internal static class WhileLoopTestCases
    {
        public const string CaseOne = @"
    while(x < 10)
    {
        x += 1;
    }
";
        public const string CaseTwo = @"
    while(x < a)
    {
        a -= 1;
    }
";
        public const string CaseThree = @"
    while(x > a)
    {
        x -= 1;
    }
";
        public const string CaseFour = @"
    while(10 >= b)
    {
        b += 1;
    }
";
        public const string CaseFive = @"
    while (a == a)
    {
        var b = 10;
    }
";
        public const string CaseSix = @"
    while(0 < 10)
    {

    }
";
        public const string CaseSeven = @"
    while(x != y)
    {
    }
";
        public static string MakeWhileLoop(string identifier, SyntaxKind op, string otherIdentifier)
        {
            var text = $@"
    while({identifier} {TextChecker.GetText(op)} {otherIdentifier})
" + @"
    {
        var wakljfglkwa = 100;
    }";
            return text;
        }

        public static string MakeWhileLoop(int identifier, SyntaxKind op, string otherIdentifier)
        {
            var text = $@"
    while({identifier} {TextChecker.GetText(op)} {otherIdentifier})
" + @"
    {
        var akhflak = 100;
    }";
            return text;
        }

        public static string MakeWhileLoop(string identifier, SyntaxKind op, int otherIdentifier)
        {
            var text = $@"
    while({identifier} {TextChecker.GetText(op)} {otherIdentifier})
" + @"
    {
        let aklfwjalkjkl = 20 + 1;
    }";
            return text;
        }
    }
}
