using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.CodeAnalysis.Syntax;

namespace Compiler.CodeAnalysis.Binding.NodeKinds
{
    internal sealed class BoundUnaryOperator
    {
        public BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, Type operandType)
            : this(syntaxKind, kind, operandType, operandType)
        { }

        public BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, Type operandType, Type resultType)
        {
            SyntaxKind = syntaxKind;
            Kind = kind;
            OperandType = operandType;
            ResultType = resultType;
        }

        public SyntaxKind SyntaxKind { get; }
        public BoundUnaryOperatorKind Kind { get; }
        public Type OperandType { get; }
        public Type ResultType { get; }

        //Just an array of possible operators, as the title would suggest
        private readonly static BoundUnaryOperator[] _operators =
        {
            new BoundUnaryOperator(SyntaxKind.Plus, BoundUnaryOperatorKind.Identity, typeof(int)),
            new BoundUnaryOperator(SyntaxKind.Minus, BoundUnaryOperatorKind.Negation, typeof(int)),

            new BoundUnaryOperator(SyntaxKind.Bang, BoundUnaryOperatorKind.LogicalNegation, typeof(bool)),
        };

        public static BoundUnaryOperator? Bind(SyntaxKind syntaxKind, Type operandType)
        {
            foreach(var op in _operators)
            {
                if(op.SyntaxKind == syntaxKind && op.OperandType == operandType)
                {
                    return op;
                }
            }
            return null;
        }
    }
}
