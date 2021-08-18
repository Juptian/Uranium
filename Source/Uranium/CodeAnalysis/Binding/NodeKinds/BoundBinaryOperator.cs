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
        private static readonly TypeSymbol[] _NumberTypeSymbols = new TypeSymbol[] { TypeSymbol.Bool, TypeSymbol.Int, TypeSymbol.Long, TypeSymbol.Float, TypeSymbol.Double };


        //Just an _NumberTypeSymbolsay of potential operators
        private readonly static BoundBinaryOperator[] _operators;

        private static IEnumerable<BoundBinaryOperator> CreateOperators()
        {
                        for (int i = 0; i < _NumberTypeSymbols.Length; i++)
            {
                for (int x = 0; x < _NumberTypeSymbols.Length; x++)
                {
                    if (TypeChecker.IsFloatingPoint(_NumberTypeSymbols[i]) && _NumberTypeSymbols[x] == TypeSymbol.Long)
                    {
                        continue;
                    }
                    else if (_NumberTypeSymbols[i] == TypeSymbol.Long && x > 2)
                    {
                        break;
                    }
                    TypeSymbol resultTypeSymbol;
                    if (TypeChecker.GetTypePriority(_NumberTypeSymbols[i]) > TypeChecker.GetTypePriority(_NumberTypeSymbols[x]))
                    {
                        resultTypeSymbol = _NumberTypeSymbols[i];
                    }
                    else
                    {
                        resultTypeSymbol = _NumberTypeSymbols[x];
                    }

                    yield return new(SyntaxKind.Plus, BoundBinaryOperatorKind.Addition, _NumberTypeSymbols[i], _NumberTypeSymbols[x], resultTypeSymbol);
                    yield return new(SyntaxKind.Minus, BoundBinaryOperatorKind.Subtraction, _NumberTypeSymbols[i], _NumberTypeSymbols[x], resultTypeSymbol);
                    yield return new(SyntaxKind.Multiply, BoundBinaryOperatorKind.Multiplication, _NumberTypeSymbols[i], _NumberTypeSymbols[x], resultTypeSymbol);
                    yield return new(SyntaxKind.Divide, BoundBinaryOperatorKind.Division, _NumberTypeSymbols[i], _NumberTypeSymbols[x], resultTypeSymbol);
                    yield return new
                        (
                            SyntaxKind.Pow,
                            BoundBinaryOperatorKind.Pow,
                            _NumberTypeSymbols[i],
                            _NumberTypeSymbols[x] == TypeSymbol.Long ? TypeSymbol.Int : _NumberTypeSymbols[x],
                            resultTypeSymbol
                        );
                    yield return new(SyntaxKind.DoubleEquals, BoundBinaryOperatorKind.LogicalEquals, _NumberTypeSymbols[i], _NumberTypeSymbols[x], TypeSymbol.Bool);
                    yield return new(SyntaxKind.BangEquals, BoundBinaryOperatorKind.NotEquals, _NumberTypeSymbols[i], _NumberTypeSymbols[x], TypeSymbol.Bool);

                    yield return new(SyntaxKind.DoubleEquals, BoundBinaryOperatorKind.LogicalEquals, _NumberTypeSymbols[i], TypeSymbol.Bool, TypeSymbol.Bool);
                    yield return new(SyntaxKind.BangEquals, BoundBinaryOperatorKind.NotEquals, _NumberTypeSymbols[i], TypeSymbol.Bool, TypeSymbol.Bool);
                    yield return new(SyntaxKind.DoubleEquals, BoundBinaryOperatorKind.LogicalEquals, TypeSymbol.Bool, _NumberTypeSymbols[i], TypeSymbol.Bool);
                    yield return new(SyntaxKind.BangEquals, BoundBinaryOperatorKind.NotEquals, TypeSymbol.Bool, _NumberTypeSymbols[i], TypeSymbol.Bool);

                    yield return new(SyntaxKind.PlusEquals, BoundBinaryOperatorKind.AdditionEquals, _NumberTypeSymbols[i], _NumberTypeSymbols[x], true);
                    yield return new(SyntaxKind.MinusEquals, BoundBinaryOperatorKind.SubtractionEquals, _NumberTypeSymbols[i], _NumberTypeSymbols[x], true);
                    yield return new(SyntaxKind.MultiplyEquals, BoundBinaryOperatorKind.MultiplicationEquals, _NumberTypeSymbols[i], _NumberTypeSymbols[x], true);
                    yield return new(SyntaxKind.DivideEquals, BoundBinaryOperatorKind.DivisionEquals, _NumberTypeSymbols[i], _NumberTypeSymbols[x], true);

                    yield return new(SyntaxKind.PlusPlus, BoundBinaryOperatorKind.AdditionAddition, _NumberTypeSymbols[i], true);
                    yield return new(SyntaxKind.MinusMinus, BoundBinaryOperatorKind.SubtractionSubtraction, _NumberTypeSymbols[i], true);

                    yield return new(SyntaxKind.LesserThan, BoundBinaryOperatorKind.LesserThan, _NumberTypeSymbols[i], _NumberTypeSymbols[x], TypeSymbol.Bool);
                    yield return new(SyntaxKind.LesserThanEquals, BoundBinaryOperatorKind.LesserThanEquals, _NumberTypeSymbols[i], _NumberTypeSymbols[x], TypeSymbol.Bool);

                    yield return new(SyntaxKind.GreaterThan, BoundBinaryOperatorKind.GreaterThan, _NumberTypeSymbols[i], _NumberTypeSymbols[x], TypeSymbol.Bool);
                    yield return new(SyntaxKind.GreaterThanEquals, BoundBinaryOperatorKind.GreaterThanEquals, _NumberTypeSymbols[i], _NumberTypeSymbols[x], TypeSymbol.Bool);
                    yield return new(SyntaxKind.DoublePipe, BoundBinaryOperatorKind.LogicalOR, _NumberTypeSymbols[i], _NumberTypeSymbols[x], TypeSymbol.Bool);
                    yield return new(SyntaxKind.DoubleAmpersand, BoundBinaryOperatorKind.LogicalAND, _NumberTypeSymbols[i], _NumberTypeSymbols[x], TypeSymbol.Bool);
                }
                yield return new(SyntaxKind.DoubleEquals, BoundBinaryOperatorKind.LogicalEquals, _NumberTypeSymbols[i], TypeSymbol.Bool, TypeSymbol.Bool);
                yield return new(SyntaxKind.BangEquals, BoundBinaryOperatorKind.NotEquals, _NumberTypeSymbols[i], TypeSymbol.Bool, TypeSymbol.Bool);

                yield return new(SyntaxKind.Ampersand, BoundBinaryOperatorKind.BitwiseAND, _NumberTypeSymbols[i], TypeSymbol.Bool);
                yield return new(SyntaxKind.Pipe, BoundBinaryOperatorKind.BitwiseOR, _NumberTypeSymbols[i], TypeSymbol.Bool);
                yield return new(SyntaxKind.Hat, BoundBinaryOperatorKind.BitwiseXOR, _NumberTypeSymbols[i]);
            }
            //These are down here as they do not need to be returned every iteration
            //They're always going to be the same things, so might as well
            yield return new(SyntaxKind.Ampersand, BoundBinaryOperatorKind.BitwiseAND, TypeSymbol.Bool);
            yield return new(SyntaxKind.Pipe, BoundBinaryOperatorKind.BitwiseOR, TypeSymbol.Bool);

            yield return new(SyntaxKind.Hat, BoundBinaryOperatorKind.BitwiseXOR, TypeSymbol.Bool);

            yield return new(SyntaxKind.DoubleEquals, BoundBinaryOperatorKind.LogicalEquals, TypeSymbol.Bool);
            yield return new(SyntaxKind.BangEquals, BoundBinaryOperatorKind.NotEquals, TypeSymbol.Bool);
            foreach (var op in BindStringOperators()) 
            {
                yield return op;
            }
        }

        private static IEnumerable<BoundBinaryOperator> BindStringOperators() 
        {
            for (int i = 0; i < _NumberTypeSymbols.Length; i++) 
            {
                yield return new(SyntaxKind.Plus, BoundBinaryOperatorKind.Addition, TypeSymbol.String, _NumberTypeSymbols[i], TypeSymbol.String);
            }
            yield return new(SyntaxKind.Plus, BoundBinaryOperatorKind.Addition, TypeSymbol.String);
            yield return new(SyntaxKind.Plus, BoundBinaryOperatorKind.Addition, TypeSymbol.String, TypeSymbol.Char, TypeSymbol.String);
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
