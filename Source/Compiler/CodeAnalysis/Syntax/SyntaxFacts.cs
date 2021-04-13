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
        {
            switch(kind)
            {
                //Just operator precedence
                case SyntaxKind.Plus:
                case SyntaxKind.Minus:
                    return 1;

                case SyntaxKind.Multiply:
                case SyntaxKind.Divide:
                    return 2;

                case SyntaxKind.Pow:
                    return 3;

                default:
                    return 0;
            }
        }
        public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch(kind)
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

                case SyntaxKind.Plus:
                case SyntaxKind.Minus:
                    return 4;

                default:
                    return 0;
            }
        }

        internal static SyntaxKind GetKeywordKind(string text)
        {
            switch(text)
            {
                case "true":
                    return SyntaxKind.TrueKeyword;
                case "false":
                    return SyntaxKind.FalseKeyword;
                default:
                    return SyntaxKind.BadToken;
            }
        }
    }
}
