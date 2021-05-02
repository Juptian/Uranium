using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranium.CodeAnalysis.Syntax.SyntaxFactsSupport
{
    internal static class TypeChecker
    {
        public static bool IsFloatingPoint(SyntaxKind kind)
            => kind is SyntaxKind.DoubleKeyword ||
               kind is SyntaxKind.FloatKeyword;

        public static bool IsInteger(SyntaxKind kind)
            => kind is SyntaxKind.IntKeyword ||
               kind is SyntaxKind.LongKeyword;

    }
}
