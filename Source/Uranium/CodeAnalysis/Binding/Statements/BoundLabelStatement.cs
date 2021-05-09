using Uranium.CodeAnalysis.Text;

namespace Uranium.CodeAnalysis.Binding.Statements
{
    internal sealed class BoundLabelStatement : BoundStatement
    {
        public BoundLabelStatement(BoundLabel symbol)
        {
            Symbol = symbol;
        }

        public BoundLabel Symbol { get; }

        public override BoundNodeKind Kind => BoundNodeKind.LabelStatement; 
    }
}
