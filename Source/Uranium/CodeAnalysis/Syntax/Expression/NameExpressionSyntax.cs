using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranium.CodeAnalysis.Syntax.Expression
{
    internal sealed class NameExpressionSyntax : ExpressionSyntax
    {
        public NameExpressionSyntax(SyntaxToken identifierToken)
        {
            IdentifierToken = identifierToken;
        }

        public SyntaxToken IdentifierToken { get; }

        public override SyntaxKind Kind => SyntaxKind.NameExpression;
    }
}
