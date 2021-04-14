using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.CodeAnalysis.Syntax
{
    internal static class SyntaxFacts
    {
        private static readonly int _minusValue = 4;
        public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
            => kind switch
            {
                //Just operator precedence

                SyntaxKind.DoublePipe => _minusValue - 3,
                SyntaxKind.DoubleAmpersand => _minusValue - 2, 
                SyntaxKind.DoubleEquals or SyntaxKind.BangEquals => _minusValue - 1,

                SyntaxKind.Plus or SyntaxKind.Minus => _minusValue,
                SyntaxKind.Multiply or SyntaxKind.Divide => _minusValue + 1,
                SyntaxKind.Pow => _minusValue + 2,
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
                SyntaxKind.Plus or SyntaxKind.Minus or SyntaxKind.Bang => _minusValue + 3,
                _ => 0,
            };

        internal static SyntaxKind GetKeywordKind(string text)
            => text switch
            {
                "true" => SyntaxKind.TrueKeyword,
                "false" => SyntaxKind.FalseKeyword,
                "double" => SyntaxKind.DoubleKeyword,
                "string" => SyntaxKind.StringKeyword,
                "float" => SyntaxKind.FloatKeyword,
                "long" => SyntaxKind.LongKeyword,
                "bool" => SyntaxKind.BoolKeyword,
                "var" => SyntaxKind.VarKeyword,
                "struct" => SyntaxKind.StructKeyword,
                "class" => SyntaxKind.ClassKeyword,
                "namespace" => SyntaxKind.NamespaceKeyword,
                "enum" => SyntaxKind.EnumKeywrod,
                "typedef" => SyntaxKind.TypeDefKeyword,
                _ => SyntaxKind.BadToken,
            };
    }
}
