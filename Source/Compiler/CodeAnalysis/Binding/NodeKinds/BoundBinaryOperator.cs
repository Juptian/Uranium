using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.CodeAnalysis.Syntax;

namespace Compiler.CodeAnalysis.Binding.NodeKinds
{
    internal sealed class BoundBinaryOperator
    {
        public BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, Type type)
            : this(syntaxKind, kind, type, type, type)
        { }

        public BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, Type operandType, Type resultType)
            : this(syntaxKind, kind, operandType, operandType, resultType)
        { }


        public BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, Type leftType, Type rightType, Type resultType)
        {
            SyntaxKind = syntaxKind;
            Kind = kind;
            LeftType = leftType;
            RightType = rightType;
            ResultType = resultType;
        }

        public SyntaxKind SyntaxKind { get; }
        public BoundBinaryOperatorKind Kind { get; }
        public Type LeftType { get; }
        public Type RightType { get; }
        public Type ResultType { get; }

        //Just an array of potential operators
        private readonly static BoundBinaryOperator[] _operators =
        {
            new BoundBinaryOperator(SyntaxKind.Plus, BoundBinaryOperatorKind.Addition, typeof(int)),
            new BoundBinaryOperator(SyntaxKind.Minus, BoundBinaryOperatorKind.Subtraction, typeof(int)),
            new BoundBinaryOperator(SyntaxKind.Multiply, BoundBinaryOperatorKind.Multiplication, typeof(int)),
            new BoundBinaryOperator(SyntaxKind.Divide, BoundBinaryOperatorKind.Division, typeof(int)),
            new BoundBinaryOperator(SyntaxKind.Pow, BoundBinaryOperatorKind.Pow, typeof(int), typeof(int)),
            new BoundBinaryOperator(SyntaxKind.DoubleEquals, BoundBinaryOperatorKind.LogicalEquals, typeof(int), typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.BangEquals, BoundBinaryOperatorKind.NotEquals, typeof(int), typeof(bool)),

            new BoundBinaryOperator(SyntaxKind.DoubleAmpersand, BoundBinaryOperatorKind.LogicalAND, typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.DoublePipe, BoundBinaryOperatorKind.LogicalOR, typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.DoubleEquals, BoundBinaryOperatorKind.LogicalEquals, typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.BangEquals, BoundBinaryOperatorKind.NotEquals, typeof(bool)),
        };

        public static BoundBinaryOperator? Bind(SyntaxKind syntaxKind, Type leftType, Type rightType)
        { 
            foreach(var op in _operators)
            {
                if(op.SyntaxKind == syntaxKind && op.LeftType == leftType && op.RightType == rightType)
                {
                    return op;
                }
            }
            return null;
        }
    }
}
