using Uranium.CodeAnalysis.Text;

namespace Uranium.CodeAnalysis.Binding.Statements
{
    internal sealed class BoundLabelStatement : BoundStatement
    {
        public BoundLabelStatement(LabelSymbol symbol)
        {
            Symbol = symbol;
        }

        public LabelSymbol Symbol { get; }

        public override BoundNodeKind Kind => BoundNodeKind.LabelStatement; 
    }
}
