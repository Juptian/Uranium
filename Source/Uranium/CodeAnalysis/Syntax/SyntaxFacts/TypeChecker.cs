using System;
using System.Linq;
using Uranium.CodeAnalysis.Symbols;

namespace Uranium.CodeAnalysis.Syntax
{
    public static class TypeChecker
    {
        public static bool IsNumber(object obj)
            => obj is int or long or float or double;

        public static bool IsNumber(SyntaxKind kind)
            => IsFloatingPoint(kind) || IsInteger(kind);

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

        public static bool IsVarKeyword(this SyntaxKind kind)
            => kind == SyntaxKind.VarKeyword;

        public static int GetTypePriority(object obj)
        {
            return obj switch
            {
                int => 1,
                float => 2,
                long => 3,
                double => 4,
                string => 5,
                _ => 0
            };
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

            if (symbol == TypeSymbol.String) 
            {
                return 5;
            }
            return 0;
        }
    }
}
