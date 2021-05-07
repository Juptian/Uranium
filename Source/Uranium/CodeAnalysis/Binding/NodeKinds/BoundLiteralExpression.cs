using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranium.CodeAnalysis.Binding.NodeKinds
{
    //A literal expression, sweet and simple
    //It holds a value, and that's literally it.
    internal sealed class BoundLiteralExpression : BoundExpression
    {
        public BoundLiteralExpression(object value, Type type)
        {
            Value = value;
            _type = type;
        }

        private readonly Type _type;

        public object Value { get; }

        public override Type Type => _type; 

        public override BoundNodeKind Kind => BoundNodeKind.LiteralExpression;
    }
}
