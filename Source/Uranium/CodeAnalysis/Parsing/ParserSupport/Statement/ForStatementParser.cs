using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Syntax.Statement;

namespace Uranium.CodeAnalysis.Parsing.ParserSupport.Statement
{
    internal static class ForStatementParser
    {
        public static StatementSyntax ParseForStatement(Parser parser)
        {
            var keyword = parser.MatchToken(SyntaxKind.ForKeyword);
            var openParenthesis = parser.MatchToken(SyntaxKind.OpenParenthesis);
            
            var variable = parser.Current.Kind == SyntaxKind.Semicolon ? null : VariableDeclarationParser.ParseVariableDeclaration(parser);
            var initializeSemi = variable is null ? parser.MatchToken(SyntaxKind.Semicolon) : parser.Peek(-1);

            var condition = parser.Current.Kind == SyntaxKind.Semicolon ? null : Parser.ParseExpression(parser);
            var conditionSemi = parser.MatchToken(SyntaxKind.Semicolon);

            var incrementation = parser.Current.Kind == SyntaxKind.CloseParenthesis ? null : Parser.ParseExpression(parser);
            var closeParenthesis = parser.MatchToken(SyntaxKind.CloseParenthesis);
            var block = BlockStatementParser.ParseBlockStatement(parser);
            
            return new ForStatementSyntax
                (
                    keyword, 
                    openParenthesis, 
                        variable, initializeSemi, 
                        condition, conditionSemi, 
                        incrementation, 
                    closeParenthesis,
                    block
                );
        }

    }
}
