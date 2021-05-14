using System;
using Uranium.CodeAnalysis.Symbols;

namespace Uranium.CodeAnalysis.Syntax
{
    public static class TextChecker
    {
        public static SyntaxKind GetSyntaxKind(string text)
            => text switch
            {
                "true" => SyntaxKind.TrueKeyword,
                "false" => SyntaxKind.FalseKeyword,
                "double" => SyntaxKind.DoubleKeyword,
                "char" => SyntaxKind.CharKeyword,
                "string" => SyntaxKind.StringKeyword,
                "float" => SyntaxKind.FloatKeyword,
                "long" => SyntaxKind.LongKeyword,
                "int" => SyntaxKind.IntKeyword,
                "bool" => SyntaxKind.BoolKeyword,
                "var" => SyntaxKind.VarKeyword,
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

                "=" => SyntaxKind.Equals,
                "+" => SyntaxKind.Plus,
                "-" => SyntaxKind.Minus,
                "/" => SyntaxKind.Divide,
                "*" => SyntaxKind.Multiply,

                "++" => SyntaxKind.PlusPlus,
                "+=" => SyntaxKind.PlusEquals,
                "--" => SyntaxKind.MinusMinus,
                "-=" => SyntaxKind.MinusEquals,
                "/=" => SyntaxKind.DivideEquals,
                "*=" => SyntaxKind.MultiplyEquals,
                "**" => SyntaxKind.Pow,
                "**=" => SyntaxKind.PowEquals,

                "%" => SyntaxKind.Percent,
                "&" => SyntaxKind.Ampersand,
                "|" => SyntaxKind.Pipe,
                "^" => SyntaxKind.Hat,
                ">" => SyntaxKind.GreaterThan,
                "<" => SyntaxKind.LesserThan,
                "!" => SyntaxKind.Bang,

                "==" => SyntaxKind.DoubleEquals,
                "%=" => SyntaxKind.PercentEquals,
                "^=" => SyntaxKind.HatEquals,
                ">=" => SyntaxKind.GreaterThanEquals,
                "<=" => SyntaxKind.LesserThanEquals,
                "!=" => SyntaxKind.BangEquals,
                "&&" => SyntaxKind.DoubleAmpersand,
                "||" => SyntaxKind.DoublePipe,

                ";" => SyntaxKind.Semicolon,
                ":" => SyntaxKind.Colon,
                "." => SyntaxKind.Dot,
                "," => SyntaxKind.Comma,
                "~" => SyntaxKind.Tilde,
                "(" => SyntaxKind.OpenParenthesis,
                ")" => SyntaxKind.CloseParenthesis,
                "{" => SyntaxKind.OpenCurlyBrace,
                "}" => SyntaxKind.CloseCurlyBrace,
                "[" => SyntaxKind.OpenBrackets,
                "]" => SyntaxKind.CloseBrackets,

                _ => SyntaxKind.BadToken,
            };

        public static string GetText(SyntaxKind kind)
            => kind switch
        {
            //Keywords
            SyntaxKind.TrueKeyword => "true",
            SyntaxKind.FalseKeyword => "false",
            SyntaxKind.DoubleKeyword => "double",
            SyntaxKind.CharKeyword => "char",
            SyntaxKind.StringKeyword => "string",
            SyntaxKind.FloatKeyword => "float",
            SyntaxKind.LongKeyword => "long",
            SyntaxKind.IntKeyword => "int",
            SyntaxKind.BoolKeyword => "bool",
            SyntaxKind.VarKeyword => "var",
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
            SyntaxKind.HatEquals => "^=", 
            SyntaxKind.GreaterThanEquals => ">=",
            SyntaxKind.LesserThanEquals => "<=",
            SyntaxKind.BangEquals => "!=",
            SyntaxKind.DoubleAmpersand => "&&",
            SyntaxKind.DoublePipe => "||",

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

        public static TypeSymbol? GetKeywordType(SyntaxKind kind) 
            => kind switch
        {
            SyntaxKind.DoubleKeyword => TypeSymbol.Double,
            SyntaxKind.CharKeyword => TypeSymbol.Char,
            SyntaxKind.StringKeyword => TypeSymbol.String, 
            SyntaxKind.FloatKeyword => TypeSymbol.Float,
            SyntaxKind.LongKeyword => TypeSymbol.Long,
            SyntaxKind.IntKeyword => TypeSymbol.Int,
            SyntaxKind.BoolKeyword => TypeSymbol.Bool,
            _ => null,
        };
        public static TypeSymbol? GetKeywordType(string kind) 
            => kind switch
        {
            "double" => TypeSymbol.Double,
            "char" => TypeSymbol.Char,
            "string" => TypeSymbol.String, 
            "float" => TypeSymbol.Float,
            "long" => TypeSymbol.Long,
            "int" => TypeSymbol.Int,
            "bool" => TypeSymbol.Bool,
            _ => null,
        };

        public static SyntaxKind GetKeyword(object obj)
            => obj switch
        {
            int => SyntaxKind.IntKeyword,
            long => SyntaxKind.LongKeyword,
            double => SyntaxKind.DoubleEquals,
            float => SyntaxKind.FloatKeyword,
            _ => SyntaxKind.BadToken,
        };
        
        public static bool IsVarKeyword(SyntaxKind kind)
            => kind is SyntaxKind.VarKeyword ||
               kind is SyntaxKind.ConstKeyword;
    }
}
