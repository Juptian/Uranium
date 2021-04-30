using System;
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

            if(parser.Current.Kind is SyntaxKind.IdentifierToken)
            {
                //Two ifs here to keep it clean
                //Also bool initialization to avoid two ifs
                if(parser.Next.Kind == SyntaxKind.Equals && SyntaxFacts.CheckForCompoundOperator(parser.Next) is var isCompound)
                {
                    //Despite knowing the token, we want to consume it, to avoid loops
                    var identifierToken = parser.NextToken();
                    var operatorToken = parser.NextToken();
                    var right = Parse(parser);
                    
                    //Something something cleaner than other implementations
                    operatorToken = isCompound ? SyntaxFacts.GetSoloOperator(operatorToken) : operatorToken;

                    return new AssignmentExpressionSyntax(identifierToken, operatorToken, right);
                }
            }

            StopInfiniteLoops(parser);
            return BinaryExpressionParser.Parse(parser);
        }

        //The parser can get stuck in infinite loops with completely unexpected and unhandled tokens
        //So, I've decided to add a function as to try to keep the code clean
        //This way tokens are always handled.
        private static void StopInfiniteLoops(Parser parser)
        {
            if(SyntaxFacts.CheckForCompoundOperator(parser.Current))
            {
                parser._diagnostics.ReportInvalidCompoundOperator(parser.Current.Span, parser.Current);
                parser.NextToken();
            } 
            else if(parser.Current.Kind is SyntaxKind.Equals)
            {
                parser._diagnostics.ReportInvalidEqualsToken(parser.Current.Span);
                parser.NextToken();
            }
        }
    }
}
