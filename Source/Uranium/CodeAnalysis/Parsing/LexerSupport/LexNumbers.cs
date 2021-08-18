using System;
using System.Linq;
using Uranium.CodeAnalysis.Syntax;

namespace Uranium.CodeAnalysis.Lexing.LexerSupport
{
    internal static class LexNumbers
    {
        public static void ReadNum(Lexer lexer)
        {
            var hasSeparator = false;
            var isDecimal = false;
            var hasMultiDecimals = false;
            lexer.Start = lexer.Index;

            while ( char.IsDigit(lexer.CurrentIndex) ||
                  ((lexer.CurrentIndex == '_' || lexer.CurrentIndex == ' ') && char.IsDigit(lexer.NextIndex)) ||
                  ((lexer.CurrentIndex == '.' || lexer.CurrentIndex == ',') && char.IsDigit(lexer.NextIndex)))
            {
                if (!hasSeparator && (lexer.CurrentIndex == '_' || lexer.CurrentIndex == ' '))
                {
                    hasSeparator = true;
                }

                if (lexer.CurrentIndex == '.' || lexer.CurrentIndex == ',')
                {
                    hasMultiDecimals = isDecimal;
                    isDecimal = true;
                }
                lexer.Index++;
            }

            var length = lexer.Index - lexer.Start;

            //Replacing , with . here so that I can parse it into a number
            //This allows a user to chose between , and . as their decimal separator
            var charArray = lexer.Source.ToString(lexer.Start, length).Replace(',', '.').ToCharArray();

            var text = string.Join("", charArray.Where(e => !char.IsWhiteSpace(e) && !e.Equals('_')));

            lexer.Text = text;

            //Numbers cannot have multiple .s or ,s.
            if (hasMultiDecimals)
            {
                lexer.diagnostics.ReportInvalidNumber(new(lexer.Start, length), text, typeof(double));
            }

            if(lexer.PreviousIdentifier is not null && 
               lexer.PreviousIdentifier!.Kind != SyntaxKind.VarKeyword)
            {
                if(TypeChecker.IsFloatingPoint(lexer.PreviousIdentifier!.Kind))
                {
                    ParseDouble(text, length, lexer);
                }
                else
                {
                    if(isDecimal)
                    {
                        lexer.diagnostics.ReportInvalidDecimal(new(lexer.Start, length), text, lexer.PreviousIdentifier.Kind);
                    }
                    else
                    {
                        ParseLong(text, length, lexer);
                    }
                }
            }
            else if(isDecimal)
            {
                ParseDouble(text, length, lexer);
            } 
            else
            {
                ParseLong(text, length, lexer);
            }
            lexer.Index--;
        }
   
        private static void ParseDouble(string text, int length, Lexer lexer)
        {
            var digits = text.Length - 1;
            if (!double.TryParse(text, out var value))
            {
                lexer.diagnostics.ReportInvalidNumber(new(lexer.Start, length), text, typeof(double));
            }
            else if (lexer.PreviousIdentifier is not null && lexer.PreviousIdentifier!.Kind != SyntaxKind.VarKeyword)
            {
                if(lexer.PreviousIdentifier!.Kind is SyntaxKind.DoubleKeyword)
                {
                    lexer.CurrentValue = value;
                }
                else
                {
                    lexer.CurrentValue = (float)value;
                }
            }
            else
            {
                if (value >= float.MinValue && value <= float.MaxValue && digits < 8)
                {
                    lexer.CurrentValue = (float)value;
                }
                else
                {
                    lexer.CurrentValue = value;
                }
            }
        }

        private static void ParseLong(string text, int length, Lexer lexer)
        {
            if (!long.TryParse(text, out var value))
            {
                lexer.diagnostics.ReportInvalidNumber(new(lexer.Start, length), text, typeof(long));
            }
            else if(lexer.PreviousIdentifier is not null && lexer.PreviousIdentifier!.Kind != SyntaxKind.VarKeyword)
            {
                if(lexer.PreviousIdentifier!.Kind is SyntaxKind.IntKeyword)
                {
                    lexer.CurrentValue = (int)value;
                } 
                else
                {
                    lexer.CurrentValue = value;
                }
            }
            else
            {
                if(value <= int.MaxValue && value >= int.MinValue)
                {
                    lexer.CurrentValue = (int)value;
                }
                else
                {
                    lexer.CurrentValue = value;
                }
            }
        }
    }
}
