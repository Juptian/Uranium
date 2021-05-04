
namespace Uranium.Tests.CodeAnalysis.Parsing
{
    internal static class ParserTestCases
    {
        public const string CaseOne = "int i = 10 + 1";
        public const string CaseTwo = "double d = 10.10 + 20.20";
        public const string CaseThree = "float f = 10.3 + 10.3";
        public const string CaseFour = "long l = 15 + 10";
        public const string CaseFive = "int i = 10 - 1";
        public const string CaseSix = "double d = 10.1001 - 10";
        public const string CaseSeven = "float f = 10.3 - 10.3";
        public const string CaseEight = "long l = 15 - 10";
        public const string CaseNine = "int i = 10 ** 2";
        public const string CaseTen = "long l = 100 ** 2";
        public const string CaseEleven = "double d = 10 / 100";
    }
}
