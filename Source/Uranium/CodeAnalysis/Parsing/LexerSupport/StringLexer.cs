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
                        if(IsEscapableCharacter(lexer.NextIndex))
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

        public static bool IsEscapableCharacter(char ch)
            => ch.Equals('n') || ch.Equals('r') || ch.Equals('"');
    }
}
