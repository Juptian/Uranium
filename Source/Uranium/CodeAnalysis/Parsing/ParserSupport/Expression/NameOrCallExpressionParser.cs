using System.Collections.Immutable;
using Uranium.CodeAnalysis.Parsing.ParserSupport.Expression;
using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Syntax.Expression;

namespace Uranium.CodeAnalysis.Parsing.ParserSupport
{
    internal static class NameOrCallExpressionParser
    {
        public static ExpressionSyntax Parse(Parser parser)
        {
            var identifierToken = parser.MatchToken(SyntaxKind.IdentifierToken);
            if (parser.Current.Kind == SyntaxKind.OpenParenthesis) 
            {
                return CallExpression(identifierToken, parser);
            }
            return new NameExpressionSyntax(identifierToken);
        }

        private static ExpressionSyntax CallExpression(SyntaxToken identifierToken, Parser parser)
        {
            var openParen = parser.MatchToken(SyntaxKind.OpenParenthesis);
            var args = ParseArgs(parser);
            var closeParen = parser.MatchToken(SyntaxKind.CloseParenthesis);
            return new CallExpressionSyntax(identifierToken, openParen, args, closeParen);
        }

        private static SeparatedSyntaxList<ExpressionSyntax> ParseArgs(Parser parser)
        {
            var builder = ImmutableArray.CreateBuilder<SyntaxNode>();
            while(parser.Current.Kind != SyntaxKind.CloseParenthesis && parser.Current.Kind != SyntaxKind.EndOfFile)
            {
                var currentArg = PrimaryExpressionParser.Parse(parser);
                builder.Add(currentArg);
            }
            return new(builder.ToImmutable());
        }
    }
}
