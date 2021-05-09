using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Uranium.CodeAnalysis.Symbols;
using Uranium.CodeAnalysis.Syntax;

namespace Uranium.CodeAnalysis.Binding.NodeKinds
{
    internal sealed class BoundBinaryOperator
    {
        public BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, TypeSymbol type, bool isCompound = false)
            : this(syntaxKind, kind, type, type, type, isCompound)
        {
        }

        public BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, TypeSymbol operandTypeSymbol, TypeSymbol resultTypeSymbol, bool isCompound = false)
            : this(syntaxKind, kind, operandTypeSymbol, operandTypeSymbol, resultTypeSymbol, isCompound)
        { }


        public BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, TypeSymbol leftTypeSymbol, TypeSymbol rightTypeSymbol, TypeSymbol resultTypeSymbol, bool isCompound = false)
        {
            SyntaxKind = syntaxKind;
            Kind = kind;
            LeftType = leftTypeSymbol;
            RightType = rightTypeSymbol;
            ResultType = resultTypeSymbol;
            IsCompound = isCompound;
        }

        static BoundBinaryOperator()
        {
            _operators = CreateOperators().ToArray();
        }

        public SyntaxKind SyntaxKind { get; }
        public BoundBinaryOperatorKind Kind { get; }
        public TypeSymbol LeftType { get; }
        public TypeSymbol RightType { get; }
        public TypeSymbol ResultType { get; }
        public bool IsCompound { get; }

        //Just an numberTypeSymbolsay of potential operators
        private readonly static BoundBinaryOperator[] _operators;

        private static IEnumerable<BoundBinaryOperator> CreateOperators()
        {
            //Using a for loop here for the sake of readability
            var numberTypeSymbols = new TypeSymbol[] { TypeSymbol.Bool, TypeSymbol.Int, TypeSymbol.Long, TypeSymbol.Float, TypeSymbol.Double };
            for(int i = 0; i < numberTypeSymbols.Length; i++)
            {
                for(int x = 0; x < numberTypeSymbols.Length; x++)
                {
                    if(SyntaxFacts.IsFloatingPoint(numberTypeSymbols[i]) && numberTypeSymbols[x] == TypeSymbol.Long)
                    {
                        continue;
                    }
                    else if (numberTypeSymbols[i] == TypeSymbol.Long && x > 2)
                    {
                        break;
                    }
                    TypeSymbol resultTypeSymbol;
                    if(SyntaxFacts.GetTypePriority(numberTypeSymbols[i]) > SyntaxFacts.GetTypePriority(numberTypeSymbols[x]))
                    {
                        resultTypeSymbol = numberTypeSymbols[i];
                    }
                    else
                    {
                        resultTypeSymbol = numberTypeSymbols[x];
                    }

                    yield return new(SyntaxKind.Plus, BoundBinaryOperatorKind.Addition, numberTypeSymbols[i], numberTypeSymbols[x], resultTypeSymbol);
                    yield return new(SyntaxKind.Minus, BoundBinaryOperatorKind.Subtraction, numberTypeSymbols[i], numberTypeSymbols[x], resultTypeSymbol);
                    yield return new(SyntaxKind.Multiply, BoundBinaryOperatorKind.Multiplication, numberTypeSymbols[i], numberTypeSymbols[x], resultTypeSymbol);
                    yield return new(SyntaxKind.Divide, BoundBinaryOperatorKind.Division, numberTypeSymbols[i], numberTypeSymbols[x], resultTypeSymbol);
                    yield return new
                        (
                            SyntaxKind.Pow, 
                            BoundBinaryOperatorKind.Pow, 
                            numberTypeSymbols[i], 
                            numberTypeSymbols[x] == TypeSymbol.Long ? TypeSymbol.Int : numberTypeSymbols[x], 
                            resultTypeSymbol
                        );
                    yield return new(SyntaxKind.DoubleEquals, BoundBinaryOperatorKind.LogicalEquals, numberTypeSymbols[i], numberTypeSymbols[x], TypeSymbol.Bool);
                    yield return new(SyntaxKind.BangEquals, BoundBinaryOperatorKind.NotEquals, numberTypeSymbols[i], numberTypeSymbols[x], TypeSymbol.Bool);

                    yield return new(SyntaxKind.DoubleEquals, BoundBinaryOperatorKind.LogicalEquals, numberTypeSymbols[i], TypeSymbol.Bool, TypeSymbol.Bool);
                    yield return new(SyntaxKind.BangEquals, BoundBinaryOperatorKind.NotEquals, numberTypeSymbols[i], TypeSymbol.Bool, TypeSymbol.Bool);
                    yield return new(SyntaxKind.DoubleEquals, BoundBinaryOperatorKind.LogicalEquals, TypeSymbol.Bool, numberTypeSymbols[i], TypeSymbol.Bool);
                    yield return new(SyntaxKind.BangEquals, BoundBinaryOperatorKind.NotEquals, TypeSymbol.Bool, numberTypeSymbols[i], TypeSymbol.Bool);

                    yield return new(SyntaxKind.PlusEquals, BoundBinaryOperatorKind.AdditionEquals, numberTypeSymbols[i], numberTypeSymbols[x], true);
                    yield return new(SyntaxKind.MinusEquals, BoundBinaryOperatorKind.SubtractionEquals, numberTypeSymbols[i], numberTypeSymbols[x], true);
                    yield return new(SyntaxKind.MultiplyEquals, BoundBinaryOperatorKind.MultiplicationEquals, numberTypeSymbols[i], numberTypeSymbols[x], true);
                    yield return new(SyntaxKind.DivideEquals, BoundBinaryOperatorKind.DivisionEquals, numberTypeSymbols[i], numberTypeSymbols[x], true);

                    yield return new(SyntaxKind.PlusPlus, BoundBinaryOperatorKind.AdditionAddition, numberTypeSymbols[i], true);
                    yield return new(SyntaxKind.MinusMinus, BoundBinaryOperatorKind.SubtractionSubtraction, numberTypeSymbols[i], true);

                    yield return new(SyntaxKind.LesserThan, BoundBinaryOperatorKind.LesserThan, numberTypeSymbols[i], numberTypeSymbols[x], TypeSymbol.Bool);
                    yield return new(SyntaxKind.LesserThanEquals, BoundBinaryOperatorKind.LesserThanEquals, numberTypeSymbols[i], numberTypeSymbols[x], TypeSymbol.Bool);

                    yield return new(SyntaxKind.GreaterThan, BoundBinaryOperatorKind.GreaterThan, numberTypeSymbols[i], numberTypeSymbols[x], TypeSymbol.Bool);
                    yield return new(SyntaxKind.GreaterThanEquals, BoundBinaryOperatorKind.GreaterThanEquals, numberTypeSymbols[i], numberTypeSymbols[x], TypeSymbol.Bool);                
                }
                yield return new(SyntaxKind.DoubleEquals, BoundBinaryOperatorKind.LogicalEquals, numberTypeSymbols[i], TypeSymbol.Bool, TypeSymbol.Bool);
                yield return new(SyntaxKind.BangEquals, BoundBinaryOperatorKind.NotEquals, numberTypeSymbols[i], TypeSymbol.Bool, TypeSymbol.Bool);

                yield return new(SyntaxKind.Ampersand, BoundBinaryOperatorKind.BitwiseAND, numberTypeSymbols[i], TypeSymbol.Bool);
                yield return new(SyntaxKind.Pipe, BoundBinaryOperatorKind.BitwiseOR, numberTypeSymbols[i], TypeSymbol.Bool);
                yield return new(SyntaxKind.Hat, BoundBinaryOperatorKind.BitwiseXOR, numberTypeSymbols[i]);
            }
            //These are down here as they do not need to be returned every iteration
            //They're always going to be the same things, so might as well
            yield return new(SyntaxKind.Ampersand, BoundBinaryOperatorKind.BitwiseAND, TypeSymbol.Bool);
            yield return new(SyntaxKind.DoubleAmpersand, BoundBinaryOperatorKind.LogicalAND, TypeSymbol.Bool);
            yield return new(SyntaxKind.Pipe, BoundBinaryOperatorKind.BitwiseOR, TypeSymbol.Bool);
            yield return new(SyntaxKind.DoublePipe, BoundBinaryOperatorKind.LogicalOR, TypeSymbol.Bool);

            yield return new(SyntaxKind.Hat, BoundBinaryOperatorKind.BitwiseXOR, TypeSymbol.Bool);

            yield return new(SyntaxKind.DoubleEquals, BoundBinaryOperatorKind.LogicalEquals, TypeSymbol.Bool);
            yield return new(SyntaxKind.BangEquals, BoundBinaryOperatorKind.NotEquals, TypeSymbol.Bool);
        }

        public static BoundBinaryOperator? Bind(SyntaxKind syntaxKind, TypeSymbol leftTypeSymbol, TypeSymbol rightTypeSymbol)
        { 
            for(int i = 0; i < _operators.Length; i++)
            {
                if(_operators[i].SyntaxKind == syntaxKind && _operators[i].LeftType == leftTypeSymbol && _operators[i].RightType == rightTypeSymbol)
                {
                    return _operators[i];
                }
            }
            return null;
        }
    }
}
