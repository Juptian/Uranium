using System;
using System.Linq;
using System.Collections.Generic;

namespace Uranium.CodeAnalysis.Syntax
{
    public static class OperatorChecker
    {

        private const int _notProperOperator = 0;
        private const int _pipeValue = 1;
        private const int _ampersandValue = 2;
        private const int _comparisonValues = 3;
        private const int _minusValue = 4;
        private const int _multiplyValue = 5;
        private const int _powValue = 6;
        private const int _unaryValues = 7;

        //Binary operators
        public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
            => kind switch
            {
                SyntaxKind.Hat or 
                SyntaxKind.Pipe or
                SyntaxKind.DoublePipe => _pipeValue,
                
                SyntaxKind.Ampersand or
                SyntaxKind.DoubleAmpersand => _ampersandValue, 

                SyntaxKind.DoubleEquals or 
                SyntaxKind.BangEquals or
                SyntaxKind.LesserThan or 
                SyntaxKind.LesserThanEquals or
                SyntaxKind.GreaterThan or 
                SyntaxKind.GreaterThanEquals => _comparisonValues,

                SyntaxKind.Plus or 
                SyntaxKind.Minus => _minusValue,
                
                SyntaxKind.Multiply or 
                SyntaxKind.Divide => _multiplyValue,
                
                SyntaxKind.Pow => _powValue,
                _ => _notProperOperator,
            };

        public static IEnumerable<SyntaxKind> GetBinaryOperators()
        {
            var kinds = (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind));
            for(int i = 0; i < kinds.Length; i++)
            {
                if(GetBinaryOperatorPrecedence(kinds[i]) > _notProperOperator)
                {
                    yield return kinds[i];
                }
            }
        }

        //Unary operators
        public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
            => kind switch
            {
                SyntaxKind.Plus or 
                SyntaxKind.Minus or 
                SyntaxKind.Bang or
                SyntaxKind.Tilde => _unaryValues,
                _ => _notProperOperator,
            };
 
        public static IEnumerable<SyntaxKind> GetUnaryOperators()
        {
            var kinds = Enum.GetValues(typeof(SyntaxKind)) as SyntaxKind[];
            for(int i = 0; i < kinds!.Length; i++) 
            {
                if(GetUnaryOperatorPrecedence(kinds[i]) > _notProperOperator)
                {
                    yield return kinds[i];
                }    
            }
        }

        public static bool CheckForCompoundOperator(SyntaxToken token)
            => token.Kind switch
            {
                SyntaxKind.PlusEquals or
                SyntaxKind.MinusEquals or
                SyntaxKind.MultiplyEquals or
                SyntaxKind.DivideEquals or
                SyntaxKind.PowEquals or 
                SyntaxKind.PlusPlus or
                SyntaxKind.MinusMinus => true,
                _ => false,
            };

        public static SyntaxToken GetSoloOperator(SyntaxToken token)
            => token.Kind switch
            {
                SyntaxKind.PlusEquals => new(SyntaxKind.Plus, token.Position, token.Text, token.Value),
                SyntaxKind.MinusEquals => new(SyntaxKind.Minus, token.Position, token.Text, token.Value),
                SyntaxKind.MultiplyEquals => new(SyntaxKind.Multiply, token.Position, token.Text, token.Value),
                SyntaxKind.DivideEquals => new(SyntaxKind.Divide, token.Position, token.Text, token.Value),
                SyntaxKind.PowEquals => new(SyntaxKind.Pow, token.Position, token.Text, token.Value),
                SyntaxKind.PlusPlus => new(SyntaxKind.Plus, token.Position, token.Text, token.Value),
                SyntaxKind.MinusMinus => new(SyntaxKind.Minus, token.Position, token.Text, token.Value),

                _ => token,
            };

        public static SyntaxKind GetSoloOperator(SyntaxKind kind)
            => kind switch
            {
                SyntaxKind.PlusEquals => SyntaxKind.Plus,
                SyntaxKind.MinusEquals => SyntaxKind.Minus,
                SyntaxKind.MultiplyEquals => SyntaxKind.Multiply,
                SyntaxKind.DivideEquals => SyntaxKind.Divide,
                SyntaxKind.PowEquals => SyntaxKind.Pow, 
                SyntaxKind.PlusPlus => SyntaxKind.Plus,
                SyntaxKind.MinusMinus => SyntaxKind.Minus,
                _ => kind,
            };
    }
}
