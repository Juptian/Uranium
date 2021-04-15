using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.Logging;
using Compiler.CodeAnalysis.Syntax;

namespace Compiler.CodeAnalysis.Lexing
{
    internal sealed class Lexer
    {
        private readonly string _source;
        private readonly List<SyntaxToken> _tokens = new();

        private char CurrentIndex => Peek(0);
        private char NextIndex => Peek(1);

        private int _index;
        private SyntaxKind _current;
        private string? _text;
        private object? _currentValue;


        private readonly DiagnosticBag _diagnostics = new();

        public Lexer(string contents)
        {
            _source = contents;
            /*for (var i = 0; i < contents.Length; i++)
            {
                Console.Write($"{contents[i]}, {i}");
            }*/
        }

        public DiagnosticBag Diagnostics => _diagnostics;

        public SyntaxToken Lex()
        {
            _current = SyntaxKind.BadToken;
            ReadSpecialChars(false);
            LexToken(CurrentIndex);

            /*if (_index == _source.Length)
            {
                return new(SyntaxKind.EndOfFile, _index, "\0", null);
            }*/

            Console.Write($"{_current}, ");
            Console.WriteLine($"{_index}, {CurrentIndex}");
            return new(_current, _index++, _text ?? CurrentIndex.ToString(), _currentValue);
        }

        private char Peek(int offset)
        {
            var peekIndex = _index + offset;

            return peekIndex == _source.Length ? '\0' : _source[peekIndex];    
        }

        private bool Match(char ch, int offset)
        {
            if (ch.Equals(Peek(offset)))
            {
                _index++;
                return true;
            }

            return false;
        }

        private void ReadSpecialChars(bool keepGoing)
        {
            var finished = false;
            var start = _index;
            while (!finished)
            {
                switch (CurrentIndex)
                {
                    //Special characters
                    case '\r':
                    case '\n':
                        _current = SyntaxKind.LineBreak;
                        ReadLineBreak();
                        finished = !keepGoing;
                        _text = finished ? "\n" : _text;
                        break;
                    case ' ':
                    case '\t':
                        ReadWhitespace();
                        _current = SyntaxKind.WhiteSpace;
                        break;
                    case '\0':
                        _current = SyntaxKind.EndOfFile;
                        finished = true;
                        break;
                    default:
                        if (char.IsWhiteSpace(CurrentIndex))
                        {
                            ReadWhitespace();
                            _current = SyntaxKind.WhiteSpace;
                        }
                        else
                        {
                            finished = true;
                        }
                        break;
                }
                var length = _index - start;
                if (length > 0)
                {
                    var text = _source.Substring(start, length);
                    _tokens.Add(new(_current, _index, text, text));
                }
            }
        }

        //Literally lexes a single token
        //Yes this parses keywords
        public SyntaxKind LexToken(char ch)
        {
            if(char.IsLetter(ch))
            {
                var start = _index;
                while (char.IsLetter(CurrentIndex))
                {
                    _index++;
                }
                    
                var length = _index - start;
                var text = _source.Substring(start, length);
                var kind = SyntaxFacts.GetKeywordKind(text);
                _current = kind == SyntaxKind.BadToken ? SyntaxKind.IdentifierToken : kind;
                _text = text;
                _index--;
                return _current;
            }

            //Using a bool to check conditions with more ease
            bool matched;
            switch (ch)
            {
                //Math operators
                case '+':
                    if(Match('=', 1))
                    {
                        _current = SyntaxKind.PlusEquals;
                        _text = "+=";
                    } 
                    else if (Match('+', 1))
                    {
                        _current = SyntaxKind.PlusPlus;
                        _text = "++";
                    } 
                    else
                    {
                        _current = SyntaxKind.Plus;
                        _text = "+";
                    }
                    break;
                case '-':
                    if(Match('=', 1))
                    {
                        _current = SyntaxKind.MinusEquals;
                        _text = "-=";
                    } 
                    else if (Match('-', 1))
                    {
                        _current = SyntaxKind.MinusMinus;
                        _text = "--";
                    } 
                    else
                    {
                        _current = SyntaxKind.Minus;
                        _text = "-";
                    }
                    break;
                case '*':               
                    if (Match('=', 1))
                    {
                        _current = SyntaxKind.MultiplyEquals;
                        _text = "*=";
                    }
                    else if (Match('*', 1))
                    {
                        _current = SyntaxKind.Pow;
                        _text = "**";
                    }
                    else
                        _current = SyntaxKind.Multiply;
                    break;
                case '/':
                    if (Match('=', 1))
                    {
                        _current = SyntaxKind.DivideEquals;
                        _text = "/=";
                    }
                    else if (Match('/', 1))
                    {
                        ReadSingleLineComment();
                        _current = SyntaxKind.SingleLineComment;
                    }
                    else if (Match('*', 1))
                    {
                        ReadMultiLineComment();
                        _current = SyntaxKind.MultiLineComment;
                    }
                    else
                        _current = SyntaxKind.Divide;

                    break;
                case '>':
                    matched = Match('=', 1);
                    _text = matched ? ">=" : ">";
                    _current = matched ? SyntaxKind.GreaterThanEquals : SyntaxKind.GreaterThan;
                    break;
                case '<':
                    matched = Match('=', 1);
                    _text = matched ? "<=" : "<";
                    _current = matched ? SyntaxKind.LesserThanEquals : SyntaxKind.LesserThan;
                    break;
                case '=':
                    matched = Match('=', 1);
                    _text = matched ? "==" : "=";
                    _current = matched ? SyntaxKind.DoubleEquals : SyntaxKind.Equals;
                    break;
                case '^':
                    matched = Match('=', 1);
                    _text = matched ? "^=" : "^";
                    _current = matched ? SyntaxKind.HatEquals : SyntaxKind.Hat;
                    break;
                case '!':
                    matched = Match('=', 1);
                    _text = matched ? "!=" : "!";
                    _current = matched ? SyntaxKind.BangEquals : SyntaxKind.Bang;
                    break;

                //Also operators
                case '|':
                    matched = Match('|', 1);
                    _text = matched ? "||" : "|";
                    _current = matched ? SyntaxKind.DoublePipe : SyntaxKind.Pipe;
                    break;

                case '&':
                    matched = Match('&', 1);
                    _text = matched ? "&&" : "&";

                    _current = matched  ? SyntaxKind.DoubleAmpersand : SyntaxKind.Ampersand;
                    break;
                case '%':
                    matched = Match('=', 1);
                    _text = matched ? "%=" : "%";

                    _current = matched ? SyntaxKind.PercentEquals : SyntaxKind.Percent;
                    break;
                case '~':
                    _current = SyntaxKind.Tilde;
                    break;

                //Numbers
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    ReadNum();
                    _current = SyntaxKind.NumberToken;
                    break;

                //Pure syntax
                case ';':
                    _current = SyntaxKind.Semicolon;
                    break;
                case '(':
                    _current = SyntaxKind.OpenParenthesis;
                    break;
                case ')':
                    _current = SyntaxKind.CloseParenthesis;
                    break;
                case '{':
                    _current = SyntaxKind.OpenCurlyBrace;
                    break;
                case '}':
                    _current = SyntaxKind.CloseCurlyBrace;
                    break;
                case '[':
                    _current = SyntaxKind.OpenBrackets;
                    break;
                case ']':
                    _current = SyntaxKind.CloseBrackets;
                    break;
                case '\0':
                    _current = SyntaxKind.EndOfFile;
                    return _current;

                case ',':
                    _current = SyntaxKind.Comma;
                    break;
                case '.':
                    _current = SyntaxKind.Dot;
                    break;
                case ':':
                    _current = SyntaxKind.Colon;
                    break;
                //Default
                default:
                    //It could be a whitespace/linebreak/ect statement, so we just break
                    if(char.IsLetter(ch))
                    {
                        _current = SyntaxKind.Null;
                    }
                    _text = ch.ToString();
                    break;
            }
            return _current;
        }


        private void ReadLineBreak()
        {
            if (CurrentIndex == '\r' && NextIndex == '\n')
            {
                _index += 2;
            }
            else
            {
                _index++;
            }
            _current = SyntaxKind.LineBreak;
        }

        private void ReadWhitespace()
        {
            var done = false;

            while (!done)
            {
                switch (CurrentIndex)
                {
                    case '\0':
                    case '\r':
                    case '\n':
                        done = true;
                        break;
                    default:
                        if (!char.IsWhiteSpace(CurrentIndex))
                        {
                            done = true;
                        }
                        else
                        {
                            _index++;
                        }
                        break;
                }
            }
            _current = SyntaxKind.WhiteSpace;
        }

        private void ReadSingleLineComment()
        {
            _index++;
            var finished = false;
            //var startIndex = _index;
            while (!finished)
            {
                switch (CurrentIndex)
                {
                    case '\0':
                    case '\r':
                    case '\n':
                        finished = true;
                        break;
                    default:
                        _index++;
                        break;
                }
            }
            _currentValue = SyntaxKind.SingleLineComment;

            //Commented out, is here for debug purposes
            /*var length = _Index - startIndex;
            Console.WriteLine(_FileContents.Substring(startIndex, length));*/
        }

        private void ReadMultiLineComment()
        {
            _index++;
            var finished = false;
            var startIndex = _index;
            while (!finished)
            {
                switch (CurrentIndex)
                {

                    case '\0':
                        var length = _index - startIndex;
                        _diagnostics.ReportUnfinishedMultiLineComment(new(startIndex, length), length);
                        _current = SyntaxKind.EndOfFile;
                        return;
                    case '*':
                        if (NextIndex.Equals('/'))
                        {
                            _index++;
                            finished = true;
                        }
                        _index++;
                        break;
                    default:
                        _index++;
                        break;
                }
            }
            _currentValue = SyntaxKind.SingleLineComment;

            //Commented out, is here for debug purposes
            /*var length = _Index - startIndex;
            Console.WriteLine(_FileContents.Substring(startIndex, length));*/
        }

        private void ReadNum()
        {
            var hasSeparator = false;
            var isDecimal = false;
            var hasMultiDecimals = false;
            var startIndex = _index;

            while (char.IsDigit(CurrentIndex) ||
                  (CurrentIndex == '_' || CurrentIndex == ' ') && char.IsDigit(NextIndex) ||
                  (CurrentIndex == '.' || CurrentIndex == ',') && char.IsDigit(NextIndex))
            {
                if (!hasSeparator && (CurrentIndex == '_' || CurrentIndex == ' '))
                {
                    hasSeparator = true;
                }

                if (CurrentIndex == '.' || CurrentIndex == ',')
                {
                    hasMultiDecimals = isDecimal;
                    isDecimal = true;
                }
                _index++;
            }

            var length = _index - startIndex;

            //Replacing , with . here so that I can parse it into a number
            //This allows a user to chose between , and . as their decimal separator
            var charArray = _source.Substring(startIndex, length).Replace(',', '.').ToCharArray();

            _text = string.Join("", charArray);
            var text = string.Join("", charArray.Where(e => !char.IsWhiteSpace(e) && !e.Equals('_')));
            
            //Numbers cannot start with _ or have multiple . s.
            if (text.StartsWith('_'))
            {
                _diagnostics.ReportNumberStartWithUnderscore(new(startIndex, length), text, typeof(int));
            }

            //Numbers cannot have multiple .s or ,s.
            if (hasMultiDecimals)
            {
                _diagnostics.ReportInvalidNumber(new(startIndex, length), text, typeof(double));
            }

            if (isDecimal)
            {
                if (!double.TryParse(text, out var value))
                {
                    _diagnostics.ReportInvalidNumber(new(startIndex, length), text, typeof(double));
                }
                else
                {
                    if (value >= float.MinValue && value <= float.MaxValue)
                    {
                        _currentValue = (float)value;
                    }
                    else
                    {
                        _currentValue = value;
                    }
                }
            }
            else
            {
                if (!ulong.TryParse(text, out var value))
                {
                    _diagnostics.ReportInvalidNumber(new(startIndex, length), text, typeof(long));
                }
                else
                {
                    if (value <= int.MaxValue)
                    {
                        _currentValue = (int)value;
                    }
                    else if (value <= uint.MaxValue)
                    {
                        _currentValue = (uint)value;
                    }
                    else
                    {
                        _currentValue = value;
                    }
                }
            }
            _index--;
            //Console.WriteLine(_Current);
        }
    }
}
