using System;
using Uranium.CodeAnalysis.Syntax;

namespace Uranium.CodeAnalysis.Lexing.LexerSupport
{
    internal static class CommentLexer
    {
        public static void ReadSingleLineComment(Lexer lexer)
        {
            lexer.Index++;
            var finished = false;
#if DEBUG
            lexer.Start = lexer.Index;
#endif
            while (!finished)
            {
                switch (lexer.CurrentIndex)
                {
                    case '\0':
                    case '\r':
                    case '\n':
                        finished = true;
                        break;
                    default:
                        lexer.Index++;
                        break;
                }
            }
            lexer.CurrentValue = SyntaxKind.SingleLineComment;

#if DEBUG
            var length = lexer.Index - lexer.Start;
            Console.WriteLine(lexer.Source.ToString(lexer.Start, length));
#endif
        }

        public static void ReadMultiLineComment(Lexer lexer)
        {
            lexer.Index++;
            var finished = false;
            lexer.Start = lexer.Index; 
            while (!finished)
            {
                switch (lexer.CurrentIndex)
                {

                    case '\0':
                        var length = lexer.Index - lexer.Start;
                        lexer.diagnostics.ReportUnfinishedMultiLineComment(new(lexer.Start, length), length);
                        lexer.Current = SyntaxKind.EndOfFile;
                        return;
                    case '*':
                        if (lexer.NextIndex.Equals('/'))
                        {
                            lexer.Index++;
                            finished = true;
                        }
                        lexer.Index++;
                        break;
                    default:
                        lexer.Index++;
                        break;
                }
            }
            lexer.CurrentValue = SyntaxKind.SingleLineComment;
        }


    }
}
