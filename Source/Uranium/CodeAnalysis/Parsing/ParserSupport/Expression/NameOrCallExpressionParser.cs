using System;
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
            if((TypeChecker.IsTypeKeyword(parser.Current) || parser.Current.Kind == SyntaxKind.IdentifierToken) && parser.Next.Kind == SyntaxKind.OpenParenthesis)
            {
                return CallExpression(parser);
            }
            return NameExpression(parser);
        }

        private static ExpressionSyntax NameExpression(Parser parser)
        {
            var identifierToken = parser.NextToken();
            return new NameExpressionSyntax(identifierToken);
        }

        private static ExpressionSyntax CallExpression(Parser parser)
        {
            var identifierToken = parser.NextToken();
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
                if(parser.Current.Kind != SyntaxKind.CloseParenthesis)
                {
                    var comma = parser.MatchToken(SyntaxKind.Comma);
                    builder.Add(comma);
                }
            }
            return new(builder.ToImmutable());
        }
    }
}
