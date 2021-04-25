using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranium.CodeAnalysis.Binding.NodeKinds;

namespace Uranium.CodeAnalysis.Binding.Statements
{
    internal sealed class BoundForStatement : BoundStatement
    {
        public BoundForStatement(BoundStatement? variableDeclaration, BoundExpression? condition, BoundExpression? increment, BoundBlockStatement body)
        {
            VariableDeclaration = variableDeclaration;
            Condition = condition;
            Increment = increment;
            Body = body;
        }

        public BoundStatement? VariableDeclaration { get; }
        public BoundExpression? Condition { get; }
        public BoundExpression? Increment { get; }
        public BoundBlockStatement Body { get; }

        public override BoundNodeKind Kind => BoundNodeKind.ForStatement;
    }
}
