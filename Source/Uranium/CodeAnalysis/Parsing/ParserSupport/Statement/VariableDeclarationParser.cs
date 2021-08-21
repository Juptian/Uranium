using System;
using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Syntax.Statement;

namespace Uranium.CodeAnalysis.Parsing.ParserSupport
{
    internal static class VariableDeclarationParser
    {
        public static StatementSyntax Parse(Parser parser)
            => ParseConstVariableDeclaration(parser);
        private static StatementSyntax ParseConstVariableDeclaration(Parser parser)
        {
            if(parser.Current.Kind == SyntaxKind.ConstKeyword)
            {
                if(parser.Next.Kind == SyntaxKind.IdentifierToken)
                {
                    parser._diagnostics.ReportInvalidConstKeyword(parser.Current.Span);
                }
                var constKeyword = parser.NextToken();
                return ParseVariableDeclaration(parser, constKeyword);
            }
            return ParseVariableDeclaration(parser, null);
        }

        private static StatementSyntax ParseVariableDeclaration(Parser parser, SyntaxToken? constKeyword)
        {
            var keyword = parser.NextToken();
            var identifier = parser.MatchToken(SyntaxKind.IdentifierToken);
            SyntaxToken semicolon;
            if(parser.Current.Kind == SyntaxKind.Equals)
            {
                var equals = parser.MatchToken(SyntaxKind.Equals);
                var initializer = Parser.ParseExpression(parser);
                semicolon = parser.MatchToken(SyntaxKind.Semicolon);

                return new VariableDeclarationSyntax(constKeyword, keyword, identifier, equals, initializer, semicolon);
            }
            else if(parser.Current.Kind == SyntaxKind.Semicolon)
            {
                semicolon = parser.NextToken();
                return new VariableDeclarationSyntax(constKeyword, keyword, identifier, null, null, semicolon);
            }

            semicolon = new SyntaxToken(SyntaxKind.Semicolon, parser.Current.Position, ";", null);

            var isCommaValid = IsNextTokenTypeKeyword(parser);
            if((parser.Current.Kind != SyntaxKind.Comma && isCommaValid) && parser.Peek(-1).Kind != SyntaxKind.Comma)
            {
                parser._diagnostics.ReportInvalidToken(identifier.Span, parser.Current, SyntaxKind.Equals, SyntaxKind.Semicolon, SyntaxKind.Comma);
            }
            return new VariableDeclarationSyntax(constKeyword, keyword, identifier, null, null, semicolon);
        }

        private static bool IsNextTokenTypeKeyword(Parser parser)
            => parser.Next.Kind switch
            {
                SyntaxKind.IntKeyword or
                SyntaxKind.BoolKeyword or
                SyntaxKind.LongKeyword or
                SyntaxKind.FloatKeyword or
                SyntaxKind.DoubleKeyword or
                SyntaxKind.CharKeyword or
                SyntaxKind.StringKeyword => true,
                _ => false,
            };
    }
}
