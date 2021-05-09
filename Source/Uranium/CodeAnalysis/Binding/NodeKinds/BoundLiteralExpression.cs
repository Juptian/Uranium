using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranium.CodeAnalysis.Symbols;

namespace Uranium.CodeAnalysis.Binding.NodeKinds
{
    //A literal expression, sweet and simple
    //It holds a value, and that's literally it.
    internal sealed class BoundLiteralExpression : BoundExpression
    {
        public BoundLiteralExpression(object value)
        {
            Value = value;
            if (value is int)
            {
                Type = TypeSymbol.Int;
            }
            else if (value is float)
            {
                Type = TypeSymbol.Float;
            }
            else if (value is long)
            {
                Type = TypeSymbol.Long;
            }
            else if (value is double)
            {
                Type = TypeSymbol.Double;
            }
            else if (value is char)
            {
                Type = TypeSymbol.Char;
            }
            else if (value is string)
            {
                Type = TypeSymbol.String;
            }
            else if (value is bool)
            {
                Type = TypeSymbol.Bool;
            }
            else
            {
                throw new($"Unexpected literal '{value}' of type {value.GetType().ToString()[7..]}");
            }
        }


        public object Value { get; }

        public override TypeSymbol Type { get; } 

        public override BoundNodeKind Kind => BoundNodeKind.LiteralExpression;
    }
}
