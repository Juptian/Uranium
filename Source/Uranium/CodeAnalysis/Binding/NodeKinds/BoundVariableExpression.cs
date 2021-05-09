using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranium.CodeAnalysis.Symbols;
using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Text;

namespace Uranium.CodeAnalysis.Binding.NodeKinds
{
    internal sealed class BoundVariableExpression : BoundExpression
    {
        public BoundVariableExpression(VariableSymbol variable)
        {
            Variable = variable;
        }

        public VariableSymbol Variable { get; }
        public override TypeSymbol Type => Variable.Type;

        public override BoundNodeKind Kind => BoundNodeKind.VariableExpression;
    }
}
