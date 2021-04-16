using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranium.CodeAnalysis.Syntax
{
    public static class SyntaxFacts
    {
        private static readonly int _minusValue = 4;

        //Binary operators
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
                "null" => SyntaxKind.Null,
                _ => SyntaxKind.BadToken,
            };
        public static string GetText(SyntaxKind kind) => kind switch
        {
            //Keywords
            SyntaxKind.TrueKeyword => "true",
            SyntaxKind.FalseKeyword => "false",
            SyntaxKind.DoubleKeyword => "double",
            SyntaxKind.StringKeyword => "string",
            SyntaxKind.FloatKeyword => "float",
            SyntaxKind.LongKeyword => "long",
            SyntaxKind.BoolKeyword => "bool",
            SyntaxKind.VarKeyword => "var",
            SyntaxKind.StructKeyword => "struct",
            SyntaxKind.ClassKeyword => "class",
            SyntaxKind.NamespaceKeyword => "namespace",
            SyntaxKind.EnumKeywrod => "enum",
            SyntaxKind.TypeDefKeyword => "typedef",
            SyntaxKind.Null => "null",

            //Symbols
            SyntaxKind.Equals => "=",
            SyntaxKind.Plus => "+",
            SyntaxKind.Minus => "-",
            SyntaxKind.Divide => "/",
            SyntaxKind.Multiply => "*",
            SyntaxKind.Percent => "%",
            SyntaxKind.Ampersand => "&",
            SyntaxKind.Pipe => "|",
            SyntaxKind.Hat => "^",
            SyntaxKind.GreaterThan => ">", 
            SyntaxKind.LesserThan => "<",
            SyntaxKind.Bang => "!", 
            SyntaxKind.DoubleEquals => "==",
            SyntaxKind.PlusPlus => "++",
            SyntaxKind.PlusEquals => "+=",
            SyntaxKind.MinusMinus => "--",
            SyntaxKind.MinusEquals => "-=",
            SyntaxKind.DivideEquals => "/=",
            SyntaxKind.MultiplyEquals => "*=",
            SyntaxKind.Pow => "**",
            SyntaxKind.PercentEquals => "%=",
            SyntaxKind.DoubleAmpersand => "&&",
            SyntaxKind.DoublePipe => "||",
            SyntaxKind.HatEquals => "^=", 
            SyntaxKind.GreaterThanEquals => ">=",
            SyntaxKind.LesserThanEquals => "<=",
            SyntaxKind.BangEquals => "!=",
            SyntaxKind.Semicolon => ";",
            SyntaxKind.Colon => ":",
            SyntaxKind.Dot => ".",
            SyntaxKind.Comma => ",",
            SyntaxKind.Tilde => "~",
            SyntaxKind.OpenParenthesis => "(",
            SyntaxKind.CloseParenthesis => ")",
            SyntaxKind.OpenCurlyBrace => "{",
            SyntaxKind.CloseCurlyBrace => "}",
            SyntaxKind.OpenBrackets => "[",
            SyntaxKind.CloseBrackets => "]",
            
            _ => "BadToken",
        };
    }
}
