using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranium.CodeAnalysis.Syntax
{
    public static class SyntaxFacts
    {
        //Bare minimum should be the minus value, everything seems to revolve around it imo
        //So here it makes it easy to increase values or decrease them
        //I should realistically inline all of these values though
        private static readonly int _minusValue = 4;

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

        public static bool CheckForCompoundOperator(SyntaxToken token)
            => token.Kind switch
            {
                SyntaxKind.PlusEquals or
                SyntaxKind.MinusEquals or
                SyntaxKind.MultiplyEquals or
                SyntaxKind.DivideEquals or
                SyntaxKind.PowEquals => true,
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


        internal static SyntaxKind GetKeywordKind(string text)
            => text switch
            {
                "true" => SyntaxKind.TrueKeyword,
                "false" => SyntaxKind.FalseKeyword,
                "double" => SyntaxKind.DoubleKeyword,
                "char" => SyntaxKind.CharKeyword,
                "string" => SyntaxKind.StringKeyword,
                "float" => SyntaxKind.FloatKeyword,
                "long" => SyntaxKind.LongKeyword,
                "bool" => SyntaxKind.BoolKeyword,
                "var" => SyntaxKind.VarKeyword,
                "let" => SyntaxKind.LetConstKeyword,
                "const" => SyntaxKind.ConstKeyword,
                "struct" => SyntaxKind.StructKeyword,
                "class" => SyntaxKind.ClassKeyword,
                "namespace" => SyntaxKind.NamespaceKeyword,
                "enum" => SyntaxKind.EnumKeywrod,
                "typedef" => SyntaxKind.TypeDefKeyword,
                "null" => SyntaxKind.Null,
                "break" => SyntaxKind.BreakKeyword,
                "continue" => SyntaxKind.ContinueKeyword,
                "while" => SyntaxKind.WhileKeyword,
                "for" => SyntaxKind.ForKeyword,
                "if" => SyntaxKind.IfKeyword,
                "else" => SyntaxKind.ElseKeyword,
                "return" => SyntaxKind.ReturnKeyword,
                _ => SyntaxKind.IdentifierToken,
            };

        public static string GetText(SyntaxKind kind) => kind switch
        {
            //Keywords
            SyntaxKind.TrueKeyword => "true",
            SyntaxKind.FalseKeyword => "false",
            SyntaxKind.DoubleKeyword => "double",
            SyntaxKind.CharKeyword => "char",
            SyntaxKind.StringKeyword => "string",
            SyntaxKind.FloatKeyword => "float",
            SyntaxKind.LongKeyword => "long",
            SyntaxKind.BoolKeyword => "bool",
            SyntaxKind.VarKeyword => "var",
            SyntaxKind.LetConstKeyword => "let",
            SyntaxKind.ConstKeyword => "const",
            SyntaxKind.StructKeyword => "struct",
            SyntaxKind.ClassKeyword => "class",
            SyntaxKind.NamespaceKeyword => "namespace",
            SyntaxKind.EnumKeywrod => "enum",
            SyntaxKind.TypeDefKeyword => "typedef",
            SyntaxKind.Null => "null",
            SyntaxKind.BreakKeyword => "break",
            SyntaxKind.ContinueKeyword => "continue",
            SyntaxKind.WhileKeyword => "while",
            SyntaxKind.ForKeyword => "for",
            SyntaxKind.IfKeyword => "if",
            SyntaxKind.ElseKeyword => "else",
            SyntaxKind.ReturnKeyword => "return",

            //Symbols
            SyntaxKind.Equals => "=",
            SyntaxKind.Plus => "+",
            SyntaxKind.Minus => "-",
            SyntaxKind.Divide => "/",
            SyntaxKind.Multiply => "*",

            SyntaxKind.PlusPlus => "++",
            SyntaxKind.PlusEquals => "+=",
            SyntaxKind.MinusMinus => "--",
            SyntaxKind.MinusEquals => "-=",
            SyntaxKind.DivideEquals => "/=",
            SyntaxKind.MultiplyEquals => "*=",
            SyntaxKind.Pow => "**",
            SyntaxKind.PowEquals => "**=",
            
            SyntaxKind.Percent => "%",
            SyntaxKind.Ampersand => "&",
            SyntaxKind.Pipe => "|",
            SyntaxKind.Hat => "^",
            SyntaxKind.GreaterThan => ">", 
            SyntaxKind.LesserThan => "<",
            SyntaxKind.Bang => "!", 
            SyntaxKind.DoubleEquals => "==",
            
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

            _ => "BadToken"
        };

        public static Type? GetKeywordType(SyntaxKind kind) => kind switch
        {
            SyntaxKind.DoubleKeyword => typeof(double),
            SyntaxKind.CharKeyword => typeof(char),
            SyntaxKind.StringKeyword => typeof(string), 
            SyntaxKind.FloatKeyword => typeof(float),
            SyntaxKind.LongKeyword => typeof(long),
            SyntaxKind.BoolKeyword => typeof(bool),
            _ => null,
        };
        public static Type? GetKeywordType(string kind) => kind switch
        {
            "double" => typeof(double),
            "char" => typeof(char),
            "string" => typeof(string), 
            "float" => typeof(float),
            "lomg" => typeof(long),
            "bool" => typeof(bool),
            _ => null,
        };

    }
}
