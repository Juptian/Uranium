using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.CodeAnalysis.Binding.NodeKinds
{
    //A binary expression, this holds both a left, and a right node
    //basically just this:
    //   *
    //  / \
    // 1   2
    // so that we can actually evaluate it
    internal sealed class BoundBinaryExpression : BoundExpression
    {
        //Just a bound binary expression, not sure what else to say 
        public BoundBinaryExpression(BoundExpression left, BoundBinaryOperator op, BoundExpression right)
        {
            Left = left;
            Op = op;
            Right = right;
        }

        public BoundExpression Left { get; }
        public BoundBinaryOperator Op { get; }
        public BoundExpression Right { get; }

        public override Type Type => Op.ResultType;
        public override BoundNodeKind Kind => BoundNodeKind.BinaryExpression;
    }
}
