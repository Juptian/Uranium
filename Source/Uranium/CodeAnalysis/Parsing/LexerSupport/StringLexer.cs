using System;
using System.Linq;
using System.Text;
using Uranium.CodeAnalysis.Syntax;

namespace Uranium.CodeAnalysis.Lexing.LexerSupport
{
    internal static class StringLexer
    {
        public static void ReadString(Lexer lexer, bool isMultiLine = false, bool isInterpolated = false)
        {
            lexer.Start = lexer.Index;
            lexer.Index++;
            StringBuilder sb = new();

            var completed = false;

            while(!completed)
            {
                switch(lexer.CurrentIndex)
                {
                    case '"':
                        completed = true;
                        break;
                    case '\r':
                    case '\n':
                        if(isMultiLine)
                        {
                            while(lexer.CurrentIndex == '\r' || lexer.CurrentIndex == '\n')
                            {
                                sb.Append(lexer.CurrentIndex);
                                lexer.Index++;
                            }
                        }
                        else
                        {
                            completed = true;
                            lexer.diagnostics.ReportUnfinishedSingleLineString(lexer.Source.ToString()[lexer.Start..lexer.Index], new(lexer.Start, lexer.Index - lexer.Start));
                        }
                        break;
                    case '\\':
                        if(IsEscapableCharacter(lexer.NextIndex, true))
                        {
                            var toAppend = $"\\{lexer.NextIndex}";
                            sb.Append(toAppend);
                            lexer.Index += 2;
                        }
                        else
                        {
                            sb.Append(lexer.Current);
                            lexer.Index++;
                        }
                        break;
                    case '\0':
                        completed = true;
                        lexer.diagnostics.ReportUnfinishedString(lexer.Source.ToString()[lexer.Start..lexer.Index], new(lexer.Start, lexer.Index - lexer.Start));
                        break;
                    default:
                        sb.Append(lexer.CurrentIndex);
                        lexer.Index++;
                        break;
                }
            }
            var length = lexer.Index - lexer.Start;
            lexer.CurrentValue = sb.ToString();
            lexer.Current = SyntaxKind.StringToken;
        }

        public static void ReadChar(Lexer lexer)
        {
            lexer.Index++;
            
            if (lexer.CurrentIndex.Equals('\\') && IsEscapableCharacter(lexer.NextIndex, false))
            {
                lexer.Index++;
                lexer.CurrentValue = string.Join("", '\\', lexer.CurrentIndex);
                lexer.Index++;
                if(lexer.CurrentIndex != '\'')
                {
                    lexer.diagnostics.ReportInvalidChar(new(lexer.Index - 2, 2));
                }
            }
            else if (lexer.CurrentIndex != '\'')
            {
                lexer.CurrentValue = lexer.CurrentIndex;
                lexer.Index++;
                if(lexer.CurrentIndex != '\'')
                {
                    lexer.diagnostics.ReportInvalidChar(new(lexer.Index - 2, 2));
                }
            }
            else
            {
                lexer.CurrentValue = string.Empty;
            }

            lexer.Current = SyntaxKind.CharToken;
        }

        public static bool IsEscapableCharacter(char ch, bool isString)
            => ch.Equals('n') || ch.Equals('r') || ch.Equals('t') || (ch.Equals('"') && isString) || (ch.Equals('\'') && !isString);
    }
}
