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

        private char _currentIndex => Peek(0);
        private char _nextIndex => Peek(1);

        private int _index;
        private SyntaxKind _current;
        public object _currentValue;

        private readonly List<string> _diagnostics = new();

        public Lexer(string contents)
        {
            _source = contents;
            /*for (var i = 0; i < contents.Length; i++)
            {
                Console.WriteLine($"{contents[i]}, {i}");
            }*/
        }

        public IEnumerable<string> Diagnostics => _diagnostics;

        public SyntaxToken NextToken()
        {
            ReadSpecialChars(true);
            LexTokens(_currentIndex);
            if (_index == _source.Length)
            {
                return new(SyntaxKind.EndOfFile, _index, "\0", null);
            }

            Console.Write($"{_current}, ");
            Console.WriteLine($"{_index}, {_currentIndex}");
            _index++;
            return new(_current, _index, _currentIndex.ToString(), _currentValue);
        }

        private char Peek(int offset)
        {
            var peekIndex = _index + offset;

            if (peekIndex == _source.Length)
            {
                return '\0';
            }
            return _source[peekIndex];
        }

        private bool Match(char ch, int offset)
        {
            if (ch.Equals(Peek(offset)))
            {
                return true;
            }

            return false;
        }

        private void ReadSpecialChars(bool keepGoing)
        {
            var finished = false;
            var start = _index;
            _current = SyntaxKind.BadToken;
            while (!finished)
            {
                switch (_currentIndex)
                {
                    //Special characters
                    case '\r':
                    case '\n':
                        _current = SyntaxKind.LineBreak;
                        ReadLineBreak();
                        finished = !keepGoing;
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
                        if (char.IsWhiteSpace(_currentIndex))
                        {
                            ReadWhitespace();
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

        //Literally lexes a single character
        public SyntaxKind LexTokens(char ch)
        {
            _current = SyntaxKind.BadToken;
            switch (ch)
            {
                #region Operators
                //Math operators
                case '+':
                    _current = Match('=', 1) ? SyntaxKind.PlusEquals : SyntaxKind.Plus;
                    break;
                case '-':
                    _current = Match('=', 1) ? SyntaxKind.MinusEquals : SyntaxKind.Minus;
                    break;
                case '*':
                    if (Match('=', 1))
                        _current = SyntaxKind.MultiplyEquals;
                    else if (Match('*', 1))
                    {
                        _current = SyntaxKind.Pow;
                        _index++;
                    }
                    else
                        _current = SyntaxKind.Multiply;
                    break;
                case '/':
                    if (Match('=', 1))
                        _current = SyntaxKind.DivideEquals;
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
                    _current = Match('=', 1) ? SyntaxKind.GreaterThanEquals : SyntaxKind.GreaterThan;
                    break;
                case '<':
                    _current = Match('=', 1) ? SyntaxKind.LesserThanEquals : SyntaxKind.LesserThan;
                    break;
                case '=':
                    _current = Match('=', 1) ? SyntaxKind.DoubleEquals : SyntaxKind.Equals;
                    break;
                case '^':
                    _current = Match('=', 1) ? SyntaxKind.HatEquals : SyntaxKind.Hat;
                    break;

                //Also operators
                case '|':
                    _current = Match('|', 1) ? SyntaxKind.DoublePipe : SyntaxKind.Pipe;
                    break;
                case '&':
                    _current = Match('&', 1) ? SyntaxKind.DoubleAmpersand : SyntaxKind.Ampersand;
                    break;
                case '%':
                    _current = Match('=', 1) ? SyntaxKind.PercentEquals : SyntaxKind.Percent;
                    break;
                case '~':
                    _current = SyntaxKind.Tilde;
                    break;
                #endregion

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
                //Default
                default:
                    throw new($"Unexpected Syntax Kind: {_current}");
            }

            return _current;
        }

        private void ReadLineBreak()
        {
            if (_currentIndex == '\r' && _nextIndex == '\n')
            {
                _index += 2;
            }
            else
            {
                _index++;
            }
        }

        private void ReadWhitespace()
        {
            var done = false;
            while (!done)
            {
                switch (_currentIndex)
                {
                    case '\0':
                    case '\r':
                    case '\n':
                        done = true;
                        break;
                    default:
                        if (!char.IsWhiteSpace(_currentIndex))
                            done = true;
                        else
                            _index++;
                        break;
                }
            }
        }

        private void ReadSingleLineComment()
        {
            _index++;
            var finished = false;
            var startIndex = _index;
            while (!finished)
            {
                switch (_currentIndex)
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
                switch (_currentIndex)
                {

                    case '\0':
                        ErrorLogger.ReportUnfinishedMultiLineComment();
                        _current = SyntaxKind.EndOfFile;
                        return;
                    case '*':
                        if (_nextIndex.Equals('/'))
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

            while (char.IsDigit(_currentIndex) ||
                  (_currentIndex == '_' || _currentIndex == ' ') && char.IsDigit(_nextIndex) ||
                  (_currentIndex == '.' || _currentIndex == ',') && char.IsDigit(_nextIndex))
            {
                if (!hasSeparator && (_currentIndex == '_' || _currentIndex == ' '))
                {
                    hasSeparator = true;
                }

                if (_currentIndex == '.' || _currentIndex == ',')
                {
                    hasMultiDecimals = isDecimal;
                    isDecimal = true;
                }
                _index++;
            }

            //Getting the length of a number
            var length = _index - startIndex;

            //Get's the contents of the number, and replaces , with ., then makes it a CharArray so that I can join it in the text
            var charArray = _source.Substring(startIndex, length).Replace(',', '.').ToCharArray();

            //Makes a string out of cha_text
            var text = string.Join("", charArray.Where(e => !char.IsWhiteSpace(e) && !e.Equals('_')));

            //Numbers cannot start with _ or have multiple . s.
            if (text.StartsWith('_'))
            {
                //TODO: Make a logging system that throws errors :)
                ErrorLogger.ReportNumberStartingWithUnderscore();
            }

            //Numbers cannot have multiple .s or ,s.
            if (hasMultiDecimals)
            {
                ErrorLogger.ReportInvalidNumber();
            }

            if (isDecimal)
            {
                if (!double.TryParse(text, out var value))
                {
                    //Yeet();
                    Console.WriteLine("Couldn't parse to double! " + text);
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
                    Console.WriteLine("Number too big!" + text);
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
