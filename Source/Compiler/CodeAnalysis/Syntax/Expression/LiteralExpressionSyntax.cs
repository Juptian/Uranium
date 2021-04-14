using System.Collections.Generic;

namespace Compiler.CodeAnalysis.Syntax.Expression
{
    //Just a literal expression, it has a token, and that's it!
    internal sealed class LiteralExpressionSyntax : ExpressionSyntax
    {

        public LiteralExpressionSyntax(SyntaxToken literalToken)
            : this(literalToken, literalToken.Value ?? 0)
        { }

        public LiteralExpressionSyntax(SyntaxToken literalToken, object value)
        {
            LiteralToken = literalToken;
            Value = value;
        }

        public override SyntaxKind Kind => SyntaxKind.LiteralExpression;
        public SyntaxToken LiteralToken { get; }
        public object Value { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return LiteralToken;
        }
    }
}
