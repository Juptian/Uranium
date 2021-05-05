using System;
using Uranium.CodeAnalysis.Syntax;

namespace Uranium.CodeAnalysis.Lexing.LexerSupport
{
    internal static class WhitespaceReader
    {
        public static void ReadLineBreak(Lexer lexer)
        {
            if (lexer.CurrentIndex == '\r' && lexer.NextIndex == '\n')
            {
                lexer.Index += 2;
            }
            else
            {
                lexer.Index++;
            }
            lexer.Current = SyntaxKind.LineBreak;
        }

        public static void ReadWhitespace(Lexer lexer)
        {
            lexer.Current = SyntaxKind.WhiteSpace;
            while(char.IsWhiteSpace(lexer.CurrentIndex))
            {
                switch(lexer.CurrentIndex)
                {
                    case '\0':
                        lexer.Current = SyntaxKind.EndOfFile;
                        return;
                    case '\r':
                    case '\n':
                        ReadLineBreak(lexer);
                        //Index++;
                        break;
                    default:
                        if(!char.IsWhiteSpace(lexer.CurrentIndex))
                        {
                            return;    
                        }
                        lexer.Index++;
                        lexer.Current = SyntaxKind.WhiteSpace;
                        break;
                }
            }
        }

    }
}
