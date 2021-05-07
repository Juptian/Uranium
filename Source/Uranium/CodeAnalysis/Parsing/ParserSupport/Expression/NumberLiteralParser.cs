using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Syntax.Expression;

namespace Uranium.CodeAnalysis.Parsing.ParserSupport
{
    internal static class NumberLiteralParser
    {
        public static ExpressionSyntax Parse(Parser parser)
        {
            var numberToken = parser.MatchToken(SyntaxKind.NumberToken);
            if(numberToken.Value is null)
            {
                return new LiteralExpressionSyntax(numberToken);
            }
            return new LiteralExpressionSyntax(numberToken, numberToken.Value!, numberToken.Value!.GetType());
        }
    }
}
