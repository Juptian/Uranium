using System;
using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Syntax.Expression;

namespace Uranium.CodeAnalysis.Parsing.ParserSupport
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
                //Declaring bool ahead of time so that we can use a single if statement
                bool isCompound = SyntaxFacts.CheckForCompoundOperator(parser.Next);
                
                if(isCompound ||
                   parser.Next.Kind is SyntaxKind.Equals)
                {
                    //Despite knowing the token, we want to consume it, to avoid loops
                    var identifierToken = parser.NextToken();
                    var operatorToken = parser.NextToken();
                    ExpressionSyntax right;
                    if(operatorToken.Kind != SyntaxKind.PlusPlus &&
                       operatorToken.Kind != SyntaxKind.MinusMinus)
                    {
                        right = Parse(parser);
                    }
                    else
                    {
                        right = new LiteralExpressionSyntax(new(SyntaxKind.NumberToken, operatorToken.Position, "1", 1));
                    }
                    if(isCompound)
                    {
                        var soloOp = SyntaxFacts.GetSoloOperator(operatorToken);
                        return new AssignmentExpressionSyntax(identifierToken, soloOp, right, true, operatorToken);
                    }

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
