using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.CodeAnalysis.Syntax;

namespace Compiler.CodeAnalysis.Binding.NodeKinds
{
    internal sealed class BoundVariableExpression : BoundExpression
    {
        public BoundVariableExpression(string name, Type type)
        {
            Name = name; 
            Type = type;
        }

        public string Name { get; }
        public override Type Type { get; }

        public override BoundNodeKind Kind => BoundNodeKind.VariableExpression;
    }
}
