using System;
using System.Collections.Generic;

namespace Uranium.CodeAnalysis.Syntax.Expression
{
    //Just a literal expression, it has a token, and that's it!
    public sealed class LiteralExpressionSyntax : ExpressionSyntax
    {

        public LiteralExpressionSyntax(SyntaxToken literalToken)
            : this(literalToken, literalToken.Value, typeof(int))
        { }

        public LiteralExpressionSyntax(SyntaxToken literalToken, object? value, Type type)
        {
            LiteralToken = literalToken;
            Value = value;
            Type = type;
        }

        public override SyntaxKind Kind => SyntaxKind.LiteralExpression;

        public SyntaxToken LiteralToken { get; }
        public object? Value { get; }
        public Type Type { get; }
    }
}
