using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Syntax.Expression;

namespace Uranium.CodeAnalysis.Parsing.ParserSupport.Expression
{
    internal static class NameExpressionParser
    {
        public static ExpressionSyntax Parse(Parser parser)
        {
            var identifierToken = parser.MatchToken(SyntaxKind.IdentifierToken);
            return new NameExpressionSyntax(identifierToken);
        }
    }
}
