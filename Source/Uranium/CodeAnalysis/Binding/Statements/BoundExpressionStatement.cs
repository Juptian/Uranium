using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranium.CodeAnalysis.Binding.NodeKinds;

namespace Uranium.CodeAnalysis.Binding.Statements
{
    internal sealed class BoundExpressionStatement : BoundStatement
    {
        public BoundExpressionStatement(BoundExpression expression)
        {
            Expression = expression;
        }

        public override BoundNodeKind Kind => BoundNodeKind.ExpressionStatement;

        public BoundExpression Expression { get; }
    }
}
