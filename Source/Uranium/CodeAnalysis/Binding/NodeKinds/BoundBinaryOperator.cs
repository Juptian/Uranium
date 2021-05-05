using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        static BoundBinaryOperator()
        {
            _operators = CreateOperators().ToArray();
        }

        public SyntaxKind SyntaxKind { get; }
        public BoundBinaryOperatorKind Kind { get; }
        public Type LeftType { get; }
        public Type RightType { get; }
        public Type ResultType { get; }
        public bool IsCompound { get; }

        //Just an numberTypesay of potential operators
        private readonly static BoundBinaryOperator[] _operators;

        private static IEnumerable<BoundBinaryOperator> CreateOperators()
        {
            //Using a for loop here for the sake of readability
            var numberTypes = new Type[] { typeof(int), typeof(long), typeof(double), typeof(float)};
            for(int i = 0; i < numberTypes.Length; i++)
            {
                yield return new(SyntaxKind.Plus, BoundBinaryOperatorKind.Addition, numberTypes[i]);
                yield return new(SyntaxKind.Minus, BoundBinaryOperatorKind.Subtraction, numberTypes[i]);
                yield return new(SyntaxKind.Multiply, BoundBinaryOperatorKind.Multiplication, numberTypes[i]);
                yield return new(SyntaxKind.Divide, BoundBinaryOperatorKind.Division, numberTypes[i]);
                yield return new(SyntaxKind.Pow, BoundBinaryOperatorKind.Pow, numberTypes[i], numberTypes[i]);
                yield return new(SyntaxKind.DoubleEquals, BoundBinaryOperatorKind.LogicalEquals, numberTypes[i], typeof(bool));
                yield return new(SyntaxKind.BangEquals, BoundBinaryOperatorKind.NotEquals, numberTypes[i], typeof(bool));

                yield return new(SyntaxKind.DoubleEquals, BoundBinaryOperatorKind.LogicalEquals, numberTypes[i], typeof(bool), typeof(bool));
                yield return new(SyntaxKind.BangEquals, BoundBinaryOperatorKind.NotEquals, numberTypes[i], typeof(bool), typeof(bool));
                yield return new(SyntaxKind.DoubleEquals, BoundBinaryOperatorKind.LogicalEquals, typeof(bool), numberTypes[i], typeof(bool));
                yield return new(SyntaxKind.BangEquals, BoundBinaryOperatorKind.NotEquals, typeof(bool), numberTypes[i], typeof(bool));

                yield return new(SyntaxKind.PlusEquals, BoundBinaryOperatorKind.AdditionEquals, numberTypes[i], true);
                yield return new(SyntaxKind.MinusEquals, BoundBinaryOperatorKind.SubtractionEquals, numberTypes[i], true);
                yield return new(SyntaxKind.MultiplyEquals, BoundBinaryOperatorKind.MultiplicationEquals, numberTypes[i], true);
                yield return new(SyntaxKind.DivideEquals, BoundBinaryOperatorKind.DivisionEquals, numberTypes[i], true);

                yield return new(SyntaxKind.PlusPlus, BoundBinaryOperatorKind.AdditionAddition, numberTypes[i], true);
                yield return new(SyntaxKind.MinusMinus, BoundBinaryOperatorKind.SubtractionSubtraction, numberTypes[i], true);

                yield return new(SyntaxKind.LesserThan, BoundBinaryOperatorKind.LesserThan, numberTypes[i], typeof(bool));
                yield return new(SyntaxKind.LesserThanEquals, BoundBinaryOperatorKind.LesserThanEquals, numberTypes[i], typeof(bool));

                yield return new(SyntaxKind.GreaterThan, BoundBinaryOperatorKind.GreaterThan, numberTypes[i], typeof(bool));
                yield return new(SyntaxKind.GreaterThanEquals, BoundBinaryOperatorKind.GreaterThanEquals, numberTypes[i], typeof(bool));
                
                yield return new(SyntaxKind.DoubleEquals, BoundBinaryOperatorKind.LogicalEquals, numberTypes[i], typeof(bool), typeof(bool));
                yield return new(SyntaxKind.BangEquals, BoundBinaryOperatorKind.NotEquals, numberTypes[i], typeof(bool), typeof(bool));
                yield return new(SyntaxKind.Ampersand, BoundBinaryOperatorKind.BitwiseAND, numberTypes[i], typeof(bool));
                yield return new(SyntaxKind.Pipe, BoundBinaryOperatorKind.BitwiseOR, numberTypes[i], typeof(bool));
                yield return new(SyntaxKind.Hat, BoundBinaryOperatorKind.BitwiseXOR, numberTypes[i]);

            }
            //These are down here as they do not need to be returned every iteration
            //They're always going to be the same things, so might as well
            yield return new(SyntaxKind.Ampersand, BoundBinaryOperatorKind.BitwiseAND, typeof(bool));
            yield return new(SyntaxKind.DoubleAmpersand, BoundBinaryOperatorKind.LogicalAND, typeof(bool));
            yield return new(SyntaxKind.Pipe, BoundBinaryOperatorKind.BitwiseOR, typeof(bool));
            yield return new(SyntaxKind.DoublePipe, BoundBinaryOperatorKind.LogicalOR, typeof(bool));

            yield return new(SyntaxKind.Hat, BoundBinaryOperatorKind.BitwiseXOR, typeof(bool));

            yield return new(SyntaxKind.DoubleEquals, BoundBinaryOperatorKind.LogicalEquals, typeof(bool));
            yield return new(SyntaxKind.BangEquals, BoundBinaryOperatorKind.NotEquals, typeof(bool));
        }

        public static BoundBinaryOperator? Bind(SyntaxKind syntaxKind, Type leftType, Type rightType)
        { 
            for(int i = 0; i < _operators.Length; i++)
            {
                if(_operators[i].SyntaxKind == syntaxKind && _operators[i].LeftType == leftType && _operators[i].RightType == rightType)
                {
                    return _operators[i];
                }
            }
            return null;
        }
    }
}
