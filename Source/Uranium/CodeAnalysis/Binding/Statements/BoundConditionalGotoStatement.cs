using Uranium.CodeAnalysis.Binding.NodeKinds;
using Uranium.CodeAnalysis.Text;

namespace Uranium.CodeAnalysis.Binding.Statements
{
    internal sealed class BoundConditionalGotoStatement : BoundStatement
    {
        public BoundConditionalGotoStatement(BoundLabel label, BoundExpression condition, bool jumpIfFalse)
        {
            Label = label;
            Condition = condition;
            JumpIfFalse = jumpIfFalse;
        }

        public BoundLabel Label { get; }
        public BoundExpression Condition { get; }
        public bool JumpIfFalse { get; }

        public override BoundNodeKind Kind => BoundNodeKind.ConditionalGotoStatement;
    }

}
