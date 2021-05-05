using System;
using Uranium.CodeAnalysis.Syntax;

namespace Uranium.CodeAnalysis.Lexing.LexerSupport
{
    internal static class OperatorSigns
    {
        public static void PlusSign(Lexer lexer)
        {
            if(lexer.Match('=', 1))
            {
                lexer.Current = SyntaxKind.PlusEquals;
            } 
            else if (lexer.Match('+', 1))
            {
                lexer.Current = SyntaxKind.PlusPlus;
            } 
            else
            {
                lexer.Current = SyntaxKind.Plus;
             }
        }

        public static void MinusSign(Lexer lexer)
        {
            if(lexer.Match('=', 1))
            {
                lexer.Current = SyntaxKind.MinusEquals;
            } 
            else if (lexer.Match('-', 1))
            {
                lexer.Current = SyntaxKind.MinusMinus;
            } 
            else
            {
                lexer.Current = SyntaxKind.Minus;
            }
        }

        public static void MultiplySign(Lexer lexer)
        {
            if (lexer.Match('=', 1))
            {
                lexer.Current = SyntaxKind.MultiplyEquals;
            }
            else if (lexer.Match('*', 1))
            {
                lexer.Current = lexer.Match('=', 1) ? SyntaxKind.PowEquals : SyntaxKind.Pow;
            }
            else
            {
                lexer.Current = SyntaxKind.Multiply;
            }
        }

        public static void DivideSign(Lexer lexer)
        {
            if (lexer.Match('=', 1))
            {
                lexer.Current = SyntaxKind.DivideEquals;
            }
            else if (lexer.Match('/', 1))
            {
                CommentLexer.ReadSingleLineComment(lexer);
                lexer.Current = SyntaxKind.SingleLineComment;
            }
            else if (lexer.Match('*', 1))
            {
                CommentLexer.ReadMultiLineComment(lexer);
                lexer.Current = SyntaxKind.MultiLineComment;
            }
            else
            {
                lexer.Current = SyntaxKind.Divide;
            }
        }


    }
}
