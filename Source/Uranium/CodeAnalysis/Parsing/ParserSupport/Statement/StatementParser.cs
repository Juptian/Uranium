using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Syntax.Statement;

namespace Uranium.CodeAnalysis.Parsing.ParserSupport.Statement
{
    internal static class StatementParser
    {
        //If it doesn't fit any of our current conditions, we take it as an expression
        public static StatementSyntax ParseStatement(Parser parser)
            => parser.Current.Kind switch
            {
                SyntaxKind.OpenCurlyBrace => BlockStatementParser.ParseBlockStatement(parser),
                SyntaxKind.LetConstKeyword or
                SyntaxKind.ConstKeyword or
                SyntaxKind.VarKeyword => VariableDeclarationParser.ParseVariableDeclaration(parser),
                SyntaxKind.IfKeyword => IfStatementParser.ParseIfStatement(parser),
                SyntaxKind.WhileKeyword => WhileStatementParser.ParseWhileStatement(parser),
                SyntaxKind.ForKeyword => ForStatementParser.ParseForStatement(parser),
                _ => Parser.ParseExpressionStatement(parser),
            };

    }
}
