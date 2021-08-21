using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranium.CodeAnalysis.Binding.Statements
{
    internal sealed class BoundErrorStatement : BoundStatement
    {
        public override BoundNodeKind Kind => BoundNodeKind.ErrorStatement;
    }
}
