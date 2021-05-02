using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Syntax.Statement;

namespace Uranium.CodeAnalysis.Parsing.ParserSupport
{
    internal static class VariableDeclarationParser
    {
        public static StatementSyntax Parse(Parser parser)
        {
            var keyword = parser.NextToken();
            var identifier = parser.MatchToken(SyntaxKind.IdentifierToken);
            var equals = parser.MatchToken(SyntaxKind.Equals);
            var initializer = Parser.ParseExpression(parser);
            var semicolon = parser.MatchToken(SyntaxKind.Semicolon);

            return new VariableDeclarationSyntax(keyword, identifier, equals, initializer, semicolon);
        }

    }
}
