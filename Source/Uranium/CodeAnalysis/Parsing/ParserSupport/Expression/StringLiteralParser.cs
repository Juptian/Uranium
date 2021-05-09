using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Syntax.Expression;

namespace Uranium.CodeAnalysis.Parsing.ParserSupport
{
    internal static class StringLiteralParser 
    {
        public static ExpressionSyntax Parse(Parser parser)
        {
            var stringToken = parser.MatchToken(SyntaxKind.StringToken);
            if(stringToken.Value is null)
            {
                return new LiteralExpressionSyntax(stringToken);
            }
            return new LiteralExpressionSyntax(stringToken, stringToken.Value!, stringToken.Value!.GetType());
        }
    }
}
