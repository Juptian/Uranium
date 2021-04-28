using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Syntax.Expression;

namespace Uranium.CodeAnalysis.Parsing.ParserSupport.Expression
{
    internal static class AssignmentExpressionParser
    {
        public static ExpressionSyntax Parse(Parser parser)
        {
            //Assignments will be done like this:
            //
            //a = b = 5
            //
            //
            //   =
            //  / \
            // a   =
            //    / \
            //    b  5
            //Checking for an identifier token, and a double equals
            //this way we can actually assign two identifiers

            //If it's an identifier, we parse it as one, if not, we go to our next level
            //Binary expressions

            if(parser.Current.Kind is SyntaxKind.IdentifierToken)
            {
                if(parser.Next.Kind == SyntaxKind.Equals)
                {
                    //Despite knowing the token, we want to consume it, to avoid loops
                    var identifierToken = parser.NextToken();
                    var operatorToken = parser.NextToken();
                    var right = Parse(parser);

                    return new AssignmentExpressionSyntax(identifierToken, operatorToken, right);
                }
                else if(SyntaxFacts.CheckForCompoundOperator(parser.Next))
                {
                    var identifierToken = parser.NextToken();
                    var operatorToken = parser.NextToken();
                    var other = Parse(parser);
                    return new AssignmentExpressionSyntax(identifierToken, SyntaxFacts.GetSoloOperator(operatorToken), other!, true, operatorToken);
                }
            }
            return BinaryExpressionParser.Parse(parser);
        }
    }
}
