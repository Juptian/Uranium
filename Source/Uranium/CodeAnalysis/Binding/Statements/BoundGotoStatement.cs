using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranium.CodeAnalysis.Text;

namespace Uranium.CodeAnalysis.Binding.Statements
{
    internal sealed class BoundGotoStatement : BoundStatement
    {
        public BoundGotoStatement(BoundLabel label)
        {
            Label = label;
        }

        public BoundLabel Label { get; }
        public override BoundNodeKind Kind => BoundNodeKind.GotoStatement;
    }

}
