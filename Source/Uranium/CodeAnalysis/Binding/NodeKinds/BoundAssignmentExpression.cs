using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranium.CodeAnalysis.Text;
using Uranium.CodeAnalysis.Syntax;

namespace Uranium.CodeAnalysis.Binding.NodeKinds
{
    internal sealed class BoundAssignmentExpression : BoundExpression
    {
        public BoundAssignmentExpression(VariableSymbol variable, BoundExpression expression, SyntaxToken? compoundOperator = null, bool isCompound = false)
        {
            Variable = variable;
            Expression = expression;
            CompoundOperator = compoundOperator;
            IsCompound = isCompound;
        }

        public VariableSymbol Variable { get; }
        public BoundExpression Expression { get; }
        public SyntaxToken? CompoundOperator { get; }
        public bool IsCompound { get; }

        public override Type Type => Expression.Type;

        public override BoundNodeKind Kind => BoundNodeKind.AssignmentExpression;
    }
}
