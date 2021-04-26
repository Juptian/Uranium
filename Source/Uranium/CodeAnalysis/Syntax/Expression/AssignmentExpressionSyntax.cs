using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranium.CodeAnalysis.Syntax.Expression
{
    internal sealed class AssignmentExpressionSyntax : ExpressionSyntax
    {
        public AssignmentExpressionSyntax(SyntaxToken identifierToken, SyntaxToken equalsToken, ExpressionSyntax expression, bool isCompound = false)
            : this(identifierToken, equalsToken, expression, isCompound, null)
        { }

        public AssignmentExpressionSyntax(SyntaxToken identifierToken, SyntaxToken equalsToken, ExpressionSyntax expression, bool isCompound, SyntaxToken? compoundOperator)
        {
            IdentifierToken = identifierToken;
            EqualsToken = equalsToken;
            Expression = expression;
            IsCompound = isCompound;
            CompoundOperator = compoundOperator;
        }


        public override SyntaxKind Kind => SyntaxKind.AssignmentExpression;

        public SyntaxToken IdentifierToken { get; }
        public SyntaxToken EqualsToken { get; }
        public ExpressionSyntax Expression { get; }
        public bool IsCompound { get; }
        public SyntaxToken? CompoundOperator { get; }
    }
}
