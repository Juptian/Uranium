using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranium.CodeAnalysis.Binding.Statements
{
    internal sealed class BoundBlockStatement : BoundStatement
    {
        public BoundBlockStatement(ImmutableArray<BoundStatement> statements)
        {
            Statements = statements;
        }

        public ImmutableArray<BoundStatement> Statements { get; }

        public override BoundNodeKind Kind => BoundNodeKind.BlockStatement;
    }
}
