using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranium.CodeAnalysis.Syntax.SyntaxFactsSupport
{
    internal static class SyntaxFactsOperators
    {
        private const int _minusValue = 4;

        //Binary operators
        public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
            => kind switch
            {
                SyntaxKind.DoublePipe => _minusValue - 3,
                SyntaxKind.DoubleAmpersand => _minusValue - 2, 

                SyntaxKind.DoubleEquals or SyntaxKind.BangEquals or
                SyntaxKind.LesserThan or SyntaxKind.LesserThanEquals or
                SyntaxKind.GreaterThan or SyntaxKind.GreaterThanEquals => _minusValue - 1,

                SyntaxKind.Plus or SyntaxKind.Minus => _minusValue,
                SyntaxKind.Multiply or SyntaxKind.Divide => _minusValue + 1,
                SyntaxKind.Pow => _minusValue + 2,
                _ => 0,
            };

        public static IEnumerable<SyntaxKind> GetBinaryOperators()
        {
            var kinds = (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind));
            foreach(var kind in kinds)
            {
                if(GetBinaryOperatorPrecedence(kind) > 0)
                {
                    yield return kind;
                }
            }
        }

        //Unary operators
        public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
            => kind switch
            {
                SyntaxKind.Plus or 
                SyntaxKind.Minus or 
                SyntaxKind.Bang => _minusValue + 3,
                _ => 0,
            };
 
        public static IEnumerable<SyntaxKind> GetUnaryOperators()
        {
            var kinds = (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind));
            foreach(var kind in kinds)
            {
                if(GetUnaryOperatorPrecedence(kind) > 0)
                {
                    yield return kind;
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
                _ => token,
            };

    }
}
