using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranium.CodeAnalysis.Syntax;

namespace Uranium.CodeAnalysis.Binding.NodeKinds
{
    internal sealed class BoundBinaryOperator
    {
        public BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, Type type, bool isCompound = false)
            : this(syntaxKind, kind, type, type, type, isCompound)
        {
        }

        public BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, Type operandType, Type resultType, bool isCompound = false)
            : this(syntaxKind, kind, operandType, operandType, resultType, isCompound)
        { }


        public BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, Type leftType, Type rightType, Type resultType, bool isCompound = false)
        {
            SyntaxKind = syntaxKind;
            Kind = kind;
            LeftType = leftType;
            RightType = rightType;
            ResultType = resultType;
            IsCompound = isCompound;
        }

        public SyntaxKind SyntaxKind { get; }
        public BoundBinaryOperatorKind Kind { get; }
        public Type LeftType { get; }
        public Type RightType { get; }
        public Type ResultType { get; }
        public bool IsCompound { get; }

        //Just an array of potential operators
        private readonly static BoundBinaryOperator[] _operators =
        {
            new(SyntaxKind.Plus, BoundBinaryOperatorKind.Addition, typeof(int)),
            new(SyntaxKind.Minus, BoundBinaryOperatorKind.Subtraction, typeof(int)),
            new(SyntaxKind.Multiply, BoundBinaryOperatorKind.Multiplication, typeof(int)),
            new(SyntaxKind.Divide, BoundBinaryOperatorKind.Division, typeof(int)),
            new(SyntaxKind.Pow, BoundBinaryOperatorKind.Pow, typeof(int), typeof(int)),
            new(SyntaxKind.DoubleEquals, BoundBinaryOperatorKind.LogicalEquals, typeof(int), typeof(bool)),
            new(SyntaxKind.BangEquals, BoundBinaryOperatorKind.NotEquals, typeof(int), typeof(bool)),

            new(SyntaxKind.PlusEquals, BoundBinaryOperatorKind.AdditionEquals, typeof(int), true),
            new(SyntaxKind.MinusEquals, BoundBinaryOperatorKind.SubtractionEquals, typeof(int), true),
            new(SyntaxKind.MultiplyEquals, BoundBinaryOperatorKind.MultiplicationEquals, typeof(int), true),
            new(SyntaxKind.DivideEquals, BoundBinaryOperatorKind.DivisionEquals, typeof(int), true),

            new(SyntaxKind.PlusPlus, BoundBinaryOperatorKind.AdditionAddition, typeof(int), true),
            new(SyntaxKind.MinusMinus, BoundBinaryOperatorKind.SubtractionSubtraction, typeof(int), true),



            new(SyntaxKind.LesserThan, BoundBinaryOperatorKind.LesserThan, typeof(int), typeof(bool)),
            new(SyntaxKind.LesserThanEquals, BoundBinaryOperatorKind.LesserThanEquals, typeof(int), typeof(bool)),
            new(SyntaxKind.GreaterThan, BoundBinaryOperatorKind.GreaterThan, typeof(int), typeof(bool)),
            new(SyntaxKind.GreaterThanEquals, BoundBinaryOperatorKind.GreaterThanEquals, typeof(int), typeof(bool)),

            new(SyntaxKind.DoubleAmpersand, BoundBinaryOperatorKind.LogicalAND, typeof(bool)),
            new(SyntaxKind.DoublePipe, BoundBinaryOperatorKind.LogicalOR, typeof(bool)),

            new(SyntaxKind.DoubleEquals, BoundBinaryOperatorKind.LogicalEquals, typeof(bool)),
            new(SyntaxKind.BangEquals, BoundBinaryOperatorKind.NotEquals, typeof(bool)),

            new(SyntaxKind.DoubleEquals, BoundBinaryOperatorKind.LogicalEquals, typeof(int), typeof(bool), typeof(bool)),
            new(SyntaxKind.BangEquals, BoundBinaryOperatorKind.NotEquals, typeof(int), typeof(bool), typeof(bool)),

            new(SyntaxKind.DoubleEquals, BoundBinaryOperatorKind.LogicalEquals, typeof(bool), typeof(int), typeof(bool)),
            new(SyntaxKind.BangEquals, BoundBinaryOperatorKind.NotEquals, typeof(bool), typeof(int), typeof(bool)),

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
