using System;

namespace Uranium.CodeAnalysis.Syntax.SyntaxFactsSupport
{
    internal static class TextChecker
    {
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
                "int" => SyntaxKind.IntKeyword,
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
            SyntaxKind.IntKeyword => "int",
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
            SyntaxKind.IntKeyword => typeof(int),
            SyntaxKind.BoolKeyword => typeof(bool),
            _ => null,
        };
        public static Type? GetKeywordType(string kind) => kind switch
        {
            "double" => typeof(double),
            "char" => typeof(char),
            "string" => typeof(string), 
            "float" => typeof(float),
            "long" => typeof(long),
            "int" => typeof(int),
            "bool" => typeof(bool),
            _ => null,
        };

    }
}
