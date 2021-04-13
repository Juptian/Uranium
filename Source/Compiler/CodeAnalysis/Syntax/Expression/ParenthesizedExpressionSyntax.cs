using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax.Expression
{
    internal sealed class ParenthesizedExpressionSyntax : ExpressionSyntax
    {
        public ParenthesizedExpressionSyntax(SyntaxToken openParenthesisToken, ExpressionSyntax expression, SyntaxToken closedParenthesisToken)
        {
            OpenParenthesisToken = openParenthesisToken;
            Expression = expression;
            ClosedParenthesisToken = closedParenthesisToken;
        }
        public SyntaxToken OpenParenthesisToken { get; }
        public ExpressionSyntax Expression { get; }
        public SyntaxToken ClosedParenthesisToken { get; }

        public override SyntaxKind Kind => SyntaxKind.ParenthesizedExpression;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OpenParenthesisToken;
            yield return Expression;
            yield return ClosedParenthesisToken;
        }
    }
}
