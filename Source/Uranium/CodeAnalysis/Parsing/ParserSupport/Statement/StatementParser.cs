using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Syntax.Statement;

namespace Uranium.CodeAnalysis.Parsing.ParserSupport
{
    internal static class StatementParser
    {
        //If it doesn't fit any of our current conditions, we take it as an expression
        public static StatementSyntax Parse(Parser parser)
            => parser.Current.Kind switch
            {
                SyntaxKind.OpenCurlyBrace => BlockStatementParser.Parse(parser),
                SyntaxKind.LetConstKeyword or
                SyntaxKind.ConstKeyword or
                SyntaxKind.VarKeyword or 
                SyntaxKind.IntKeyword or
                SyntaxKind.LongKeyword or 
                SyntaxKind.DoubleKeyword or
                SyntaxKind.FloatKeyword or
                SyntaxKind.StringKeyword or
                SyntaxKind.CharKeyword => VariableDeclarationParser.Parse(parser),
                SyntaxKind.IfKeyword => IfStatementParser.Parse(parser),
                SyntaxKind.WhileKeyword => WhileStatementParser.Parse(parser),
                SyntaxKind.ForKeyword => ForStatementParser.Parse(parser),
                _ => Parser.ParseExpressionStatement(parser),
            };

    }
}
