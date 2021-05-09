using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranium.CodeAnalysis.Symbols;
using Uranium.CodeAnalysis.Syntax;

namespace Uranium.CodeAnalysis.Binding.NodeKinds
{
    internal sealed class BoundUnaryOperator
    {
        public BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, TypeSymbol operandTypeSymbol)
            : this(syntaxKind, kind, operandTypeSymbol, operandTypeSymbol)
        { }

        public BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, TypeSymbol operandTypeSymbol, TypeSymbol resultTypeSymbol)
        {
            SyntaxKind = syntaxKind;
            Kind = kind;
            OperandType = operandTypeSymbol;
            ResultType = resultTypeSymbol;
        }

        public SyntaxKind SyntaxKind { get; }
        public BoundUnaryOperatorKind Kind { get; }
        public TypeSymbol OperandType { get; }
        public TypeSymbol ResultType { get; }

        //Just an array of possible operators, as the title would suggest
        private readonly static BoundUnaryOperator[] _operators =
        {
            new(SyntaxKind.Plus, BoundUnaryOperatorKind.Identity, TypeSymbol.Int),
            new(SyntaxKind.Minus, BoundUnaryOperatorKind.Negation, TypeSymbol.Int),

            new(SyntaxKind.Bang, BoundUnaryOperatorKind.LogicalNegation, TypeSymbol.Bool),
            new(SyntaxKind.Tilde, BoundUnaryOperatorKind.BitwiseNegation, TypeSymbol.Int)
        };

        public static BoundUnaryOperator? Bind(SyntaxKind syntaxKind, TypeSymbol operandTypeSymbol)
        {
            foreach(var op in _operators)
            {
                if(op.SyntaxKind == syntaxKind && op.OperandType == operandTypeSymbol)
                {
                    return op;
                }
            }
            return null;
        }
    }
}
