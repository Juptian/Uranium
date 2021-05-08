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
        public BoundGotoStatement(LabelSymbol label)
        {
            Label = label;
        }

        public LabelSymbol Label { get; }
        public override BoundNodeKind Kind => BoundNodeKind.GotoStatement;
    }

}
