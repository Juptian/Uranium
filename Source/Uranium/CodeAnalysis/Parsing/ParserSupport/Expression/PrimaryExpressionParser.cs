using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Syntax.Expression;

namespace Uranium.CodeAnalysis.Parsing.ParserSupport.Expression
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
                _ => NameExpressionParser.Parse(parser),
            };

    }
}
