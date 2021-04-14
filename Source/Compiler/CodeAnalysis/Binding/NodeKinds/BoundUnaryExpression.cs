using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.CodeAnalysis.Binding.NodeKinds
{
    //Basically, this is just for if someone does like -1, or --1
    //Or some cursed shit like +---+-++----1
    //If you do math like this, you don't deserve to exist.
    //Yes I'm looking at you
    internal sealed class BoundUnaryExpression : BoundExpression
    {
        public BoundUnaryExpression(BoundUnaryOperator op, BoundExpression operand)
        {
            Op = op;
            Operand = operand;
        }

        public BoundUnaryOperator Op { get; }
        public BoundExpression Operand { get; }

        public override Type Type => Op.ResultType;
        public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression; 
    }
}
