using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Syntax.Expression;

namespace Uranium.CodeAnalysis.Parsing.ParserSupport
{
    internal static class PrimaryExpressionParser
    {
        public static ExpressionSyntax Parse(Parser parser)
            =>  parser.Current.Kind switch
            {
                SyntaxKind.OpenParenthesis => ParenthesizedExpressionParser.Parse(parser),
                SyntaxKind.TrueKeyword or 
                SyntaxKind.FalseKeyword => BooleanExpressionParser.Parse(parser),
                SyntaxKind.NumberToken => NumberLiteralParser.Parse(parser),
                SyntaxKind.StringToken => StringLiteralParser.Parse(parser),
                SyntaxKind.CharToken => StringLiteralParser.ParseChar(parser),
                _ => NameOrCallExpressionParser.Parse(parser),
            };

    }
}
