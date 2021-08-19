using Uranium.CodeAnalysis.Symbols;

namespace Uranium.CodeAnalysis.Binding.NodeKinds
{
    internal sealed class BoundConversionExpression : BoundExpression
    {
        public BoundConversionExpression(TypeSymbol type, BoundExpression expression)
        {
            Type = type;
            Expression = expression;
        }
        public override TypeSymbol Type { get; }
        public BoundExpression Expression { get; }

        public override BoundNodeKind Kind => BoundNodeKind.ConversionExpression; 
    }
}
