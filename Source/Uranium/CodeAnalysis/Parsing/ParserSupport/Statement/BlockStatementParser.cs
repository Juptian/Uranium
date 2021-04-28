using System.Collections.Immutable;
using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Syntax.Statement;

namespace Uranium.CodeAnalysis.Parsing.ParserSupport.Statement
{
    internal static class BlockStatementParser
    {
        public static BlockStatementSyntax ParseBlockStatement(Parser parser)
        {
            var statements = ImmutableArray.CreateBuilder<StatementSyntax>();
            var openBraceToken = parser.MatchToken(SyntaxKind.OpenCurlyBrace);

            while (parser.Current.Kind is not SyntaxKind.EndOfFile &&
                  parser.Current.Kind is not SyntaxKind.CloseCurlyBrace)
            {
                if (parser.Current.Kind is SyntaxKind.LineBreak || parser.Current.Kind is SyntaxKind.Semicolon)
                {
                    parser.Position++;
                    continue;
                }

                var statement = StatementParser.ParseStatement(parser);
                statements.Add(statement);
            }

            var closeBraceToken = parser.MatchToken(SyntaxKind.CloseCurlyBrace);

            return new(openBraceToken, statements.ToImmutable(), closeBraceToken);
        }


    }
}
