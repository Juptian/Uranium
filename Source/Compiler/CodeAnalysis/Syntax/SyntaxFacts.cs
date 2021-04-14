using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.CodeAnalysis.Syntax
{
    internal static class SyntaxFacts
    {
        public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
            => kind switch
            {
                //Just operator precedence
                SyntaxKind.Plus or SyntaxKind.Minus => 1,
                SyntaxKind.Multiply or SyntaxKind.Divide => 2,
                SyntaxKind.Pow => 3,
                _ => 0,
            };

        public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
            => kind switch
            {
                //Bound to 4 so that our tree looks correct, result is the same regardless
                //-1 * 3
                //
                //
                //    *
                //   / \
                //  -   3
                //  |
                //  1
                //Because this is how math works!
                SyntaxKind.Plus or SyntaxKind.Minus => 4,
                SyntaxKind.Bang => 4,
                _ => 0,
            };

        internal static SyntaxKind GetKeywordKind(string text)
            => text switch
            {
                "true" => SyntaxKind.TrueKeyword,
                "false" => SyntaxKind.FalseKeyword,
                _ => SyntaxKind.BadToken,
            };
    }
}
