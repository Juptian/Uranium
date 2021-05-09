using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranium.CodeAnalysis.Symbols;

namespace Uranium.CodeAnalysis.Syntax.SyntaxFactsSupport
{
    internal static class TypeChecker
    {
        public static bool IsFloatingPoint(SyntaxKind kind)
            => kind is SyntaxKind.DoubleKeyword ||
               kind is SyntaxKind.FloatKeyword;

        public static bool IsFloatingPoint(Type t)
            => t == typeof(float) || t == typeof(double);

        public static bool IsFloatingPoint(object obj)
            => obj is float or double;

        public static bool IsInteger(SyntaxKind kind)
            => kind is SyntaxKind.IntKeyword ||
               kind is SyntaxKind.LongKeyword;

        public static bool IsInteger(Type t)
            => t == typeof(int) || t == typeof(long);

        public static bool IsInteger(object obj)
            => obj is int or long;

        public static int GetTypePriority(object obj)
        {
            if(obj is int)
            {
                return 1;
            }
            if(obj is float)
            {
                return 2;
            }
            if(obj is long)
            {
                return 3;
            }
            if(obj is double)
            {
                return 4;
            }
            return 0;
        }

        public static int GetTypePriority(TypeSymbol symbol)
        {
            if(symbol == TypeSymbol.Int)
            {
                return 1;
            }
            
            if(symbol == TypeSymbol.Float)
            {
                return 2;
            }

            if(symbol == TypeSymbol.Long)
            {
                return 3;
            }

            if(symbol == TypeSymbol.Double)
            {
                return 4;
            }

            return 0;
        }
    }
}
