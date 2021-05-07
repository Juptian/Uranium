using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Syntax.Expression;

namespace Uranium.CodeAnalysis.Parsing.ParserSupport
{
    internal static class BooleanExpressionParser
    {
        public static ExpressionSyntax Parse(Parser parser)
        {
            var isTrue = parser.Current.Kind == SyntaxKind.TrueKeyword;
            var keywordToken = isTrue ? parser.MatchToken(SyntaxKind.TrueKeyword) : parser.MatchToken(SyntaxKind.FalseKeyword);
            return new LiteralExpressionSyntax(keywordToken, isTrue, typeof(bool));
        }
    }
}
