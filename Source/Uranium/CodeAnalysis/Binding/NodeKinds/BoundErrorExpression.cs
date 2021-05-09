using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranium.CodeAnalysis.Symbols;

namespace Uranium.CodeAnalysis.Binding.NodeKinds
{
    internal sealed class BoundErrorExpression : BoundExpression
    {
        public override TypeSymbol Type => TypeSymbol.Error;
        public override BoundNodeKind Kind => BoundNodeKind.ErrorExpression; 
    }
}
