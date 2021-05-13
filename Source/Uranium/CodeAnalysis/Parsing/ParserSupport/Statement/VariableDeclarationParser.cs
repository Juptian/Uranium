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
            var equals = parser.MatchToken(SyntaxKind.Equals);
            var initializer = Parser.ParseExpression(parser);
            var semicolon = parser.MatchToken(SyntaxKind.Semicolon);

            return new VariableDeclarationSyntax(constKeyword, keyword, identifier, equals, initializer, semicolon);

        }
    }
}
