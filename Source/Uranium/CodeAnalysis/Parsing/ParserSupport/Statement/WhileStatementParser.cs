using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Syntax.Statement;

namespace Uranium.CodeAnalysis.Parsing.ParserSupport.Statement
{
    internal static class WhileStatementParser
    {
        public static StatementSyntax ParseWhileStatement(Parser parser)
        {
            var keyword = parser.MatchToken(SyntaxKind.WhileKeyword);
            var openParen = parser.MatchToken(SyntaxKind.OpenParenthesis);
            var condition = Parser.ParseExpression(parser);
            var closeParen = parser.MatchToken(SyntaxKind.CloseParenthesis);
            var body = BlockStatementParser.ParseBlockStatement(parser);

            return new WhileStatementSyntax(keyword, openParen, condition, closeParen, body);
        }

    }
}
