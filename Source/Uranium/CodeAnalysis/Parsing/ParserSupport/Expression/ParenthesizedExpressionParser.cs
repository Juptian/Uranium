using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Syntax.Expression;

namespace Uranium.CodeAnalysis.Parsing.ParserSupport
{
    internal static class ParenthesizedExpressionParser
    {
        public static ExpressionSyntax Parse(Parser parser)
        {
            var left = parser.MatchToken(SyntaxKind.OpenParenthesis);
            var expression = Parser.ParseExpression(parser);
            var right = parser.MatchToken(SyntaxKind.CloseParenthesis);
            return new ParenthesizedExpressionSyntax(left, expression, right);
        }
    }
}
