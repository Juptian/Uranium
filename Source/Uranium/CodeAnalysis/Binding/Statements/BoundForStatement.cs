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
        public BoundForStatement(BoundVariableDeclaration? variableDeclaration, BoundBinaryExpression? condition, BoundAssignmentExpression? increment, BoundBlockStatement body)
        {
            VariableDeclaration = variableDeclaration;
            Condition = condition;
            Increment = increment;
            Body = body;
        }

        public BoundVariableDeclaration? VariableDeclaration { get; }
        public BoundBinaryExpression? Condition { get; }
        public BoundAssignmentExpression? Increment { get; }
        public BoundBlockStatement Body { get; }

        public override BoundNodeKind Kind => BoundNodeKind.ForStatement;
    }
}
