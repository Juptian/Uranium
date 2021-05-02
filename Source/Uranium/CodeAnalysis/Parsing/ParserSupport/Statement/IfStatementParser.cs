using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Syntax.Statement;

namespace Uranium.CodeAnalysis.Parsing.ParserSupport
{
    internal static class IfStatementParser
    {
        public static StatementSyntax Parse(Parser parser)
        {
            var keyword = parser.MatchToken(SyntaxKind.IfKeyword);
            var openParen = parser.MatchToken(SyntaxKind.OpenParenthesis);

            var condition = Parser.ParseExpression(parser);
            
            var closeParen = parser.MatchToken(SyntaxKind.CloseParenthesis);
            var body = BlockStatementParser.Parse(parser);
            var elseClause = ParseElseClause(parser);

            return new IfStatementSyntax(keyword, openParen, condition, closeParen, body, elseClause);
        }

        private static ElseClauseSyntax? ParseElseClause(Parser parser)
        {
            if(parser.Current.Kind != SyntaxKind.ElseKeyword)
            {
                return null;
            }
            var keyword = parser.NextToken();
            var statement = StatementParser.Parse(parser);

            return new(keyword, statement);
        }

    }
}
