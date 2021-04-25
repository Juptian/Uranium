using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranium.CodeAnalysis.Binding.NodeKinds;

namespace Uranium.CodeAnalysis.Binding.Statements
{
    internal sealed class BoundIfStatement : BoundStatement
    {
        public BoundIfStatement(BoundExpression condition, BoundStatement statement, BoundStatement? elseStatement)
        {
            Condition = condition;
            Statement = statement;
            ElseStatement = elseStatement;
        }

        public BoundExpression Condition { get; }
        public BoundStatement Statement { get; }
        public BoundStatement? ElseStatement { get; }

        public override BoundNodeKind Kind => BoundNodeKind.IfStatement;
    }
}
