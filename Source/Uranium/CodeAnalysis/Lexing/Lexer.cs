using System;
using System.Collections.Generic;
using System.Linq;
using Uranium.Logging;
using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Text;

namespace Uranium.CodeAnalysis.Lexing
{
    internal sealed class Lexer
    {
        private char PreviousIndex => Peek(-1);
        private char CurrentIndex => Peek(0);
        private char NextIndex => Peek(1);

        private int _start;
        private int _index;
        private SyntaxKind _current;
        private string? _text;
        private object? _currentValue;

        private readonly SourceText _source;
        private readonly DiagnosticBag _diagnostics = new();

        public Lexer(SourceText contents)
        {
            _source = contents;
            /*for (var i = 0; i < contents.Length; i++)
            {
                Console.WriteLine($"{contents[i]}, {i}");
            }*/
        }
        
        public DiagnosticBag Diagnostics => _diagnostics;

        public SyntaxToken Lex()
        {
            _currentValue = null;
            _current = SyntaxKind.BadToken;
            ReadSpecialChars(false);
            LexToken(CurrentIndex);

            //Console.WriteLine($"{_current}, {_text ?? CurrentIndex.ToString()}, {_index}, {_currentValue}");
            return new(_current, _index++, _text ?? PreviousIndex.ToString(), _currentValue);
        }

        private char Peek(int offset)
        {
            var peekIndex = _index + offset;

            return peekIndex >= _source.Length ? '\0' : _source[peekIndex];    
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
            var _start = _index;
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
                        _index++;
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
                            _index++;
                            ReadWhitespace();
                            _current = SyntaxKind.WhiteSpace;
                        }
                        else
                        {
                            finished = true;
                        }
                        break;
                }
                var length = _index - _start;
                if (length > 0)
                {
                    _text  = _source.ToString(_start, length);
                }
            }
        }

        //Literally lexes a single token
        //Yes this parses keywords
        public void LexToken(char ch)
        {
            _start = _index;
            //Using a bool to check conditions with more ease
            switch (ch)
            {
                //Math operators
                case '+':
                    if(Match('=', 1))
                    {
                        _current = SyntaxKind.PlusEquals;
                    } 
                    else if (Match('+', 1))
                    {
                        _current = SyntaxKind.PlusPlus;
                    } 
                    else
                    {
                        _current = SyntaxKind.Plus;
                    }
                    break;
                case '-':
                    if(Match('=', 1))
                    {
                        _current = SyntaxKind.MinusEquals;
                    } 
                    else if (Match('-', 1))
                    {
                        _current = SyntaxKind.MinusMinus;
                    } 
                    else
                    {
                        _current = SyntaxKind.Minus;
                    }
                    break;
                case '*':               
                    if (Match('=', 1))
                    {
                        _current = SyntaxKind.MultiplyEquals;
                    }
                    else if (Match('*', 1))
                    {
                        _current = SyntaxKind.Pow;
                    }
                    else
                    {
                        _current = SyntaxKind.Multiply;
                    }
                    break;
                case '/':
                    if (Match('=', 1))
                    {
                        _current = SyntaxKind.DivideEquals;
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
                    {
                        _current = SyntaxKind.Divide;
                    }
                        
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
                case '!':
                    _current = Match('=', 1) ? SyntaxKind.BangEquals : SyntaxKind.Bang;
                    break;

                //Also operators
                case '|':
                    _current = Match('|', 1) ? SyntaxKind.DoublePipe : SyntaxKind.Pipe;
                    break;
                case '&':
                    _current = Match('&', 1)  ? SyntaxKind.DoubleAmpersand : SyntaxKind.Ampersand;
                    break;
                case '%':
                    _current = Match('=', 1) ? SyntaxKind.PercentEquals : SyntaxKind.Percent;
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
                case ',':
                    _current = SyntaxKind.Comma;
                    break;
                case '.':
                    _current = SyntaxKind.Dot;
                    break;
                case ':':
                    _current = SyntaxKind.Colon;
                    break;
                case '\0':
                    _current = SyntaxKind.EndOfFile;
                    return;
                //If it matches none of the above, we want to check if it's a character.
                //This way our identifiers and keywords still work
                default:
                    if (char.IsLetter(ch))
                    {
                        ReadIdentifierOrKeyword();
                    }
                    break;
            }

            //Checking here for if it's an identifier token, this way we don't do anything with it.
            //This is because Identifier tokens should not be modified.
            switch(_current)
            {
                case SyntaxKind.IdentifierToken:
                case SyntaxKind.NumberToken:
                case SyntaxKind.BlockStatement:
                case SyntaxKind.ContinueStatement:
                case SyntaxKind.DoWhileStatement:
                case SyntaxKind.ExpressionStatement:
                case SyntaxKind.ForStatement:
                case SyntaxKind.IfStatement:
                case SyntaxKind.ElseKeyword:
                case SyntaxKind.MemberBlockStatement:
                case SyntaxKind.ReturnStatement:
                case SyntaxKind.WhileStatement:
                    break;
                default:
                    //Console.WriteLine(_current);
                    var length = _index - _start;
                    var text = SyntaxFacts.GetText(_current);

                    //If I remove the ` text is null `
                    //The tests fucking die
                    //So we leave it here.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    //That's why I'm checking if it's null now fuck off vs
                    if ((text is null && _text is null) || text.Equals("BadToken", StringComparison.OrdinalIgnoreCase))
                    {
                        text = _source.ToString(_start, length);
                    }
                    _text = text;
                    break;
#pragma warning restore CS8602 
            }
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
            _current = SyntaxKind.WhiteSpace;
            while(char.IsWhiteSpace(CurrentIndex))
            {
                switch(CurrentIndex)
                {
                    case '\0':
                        _current = SyntaxKind.EndOfFile;
                        return;
                    case '\r':
                    case '\n':
                        ReadLineBreak();
                        //_index++;
                        break;
                    default:
                        if(!char.IsWhiteSpace(CurrentIndex))
                        {
                            //How even the fuck?
                            //Not sure how or why but here we are!
                            return;    
                        }
                        _index++;
                        _current = SyntaxKind.WhiteSpace;
                        break;
                }
            }
        }

        private void ReadSingleLineComment()
        {
            _index++;
            var finished = false;
            // _start = _index;
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
            /*var length = _Index - _start;
            Console.WriteLine(_FileContents.ToString(_start, length));*/
        }

        private void ReadMultiLineComment()
        {
            _index++;
            var finished = false;
            _start = _index; 
            while (!finished)
            {
                switch (CurrentIndex)
                {

                    case '\0':
                        var length = _index - _start;
                        _diagnostics.ReportUnfinishedMultiLineComment(new(_start, length), length);
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
            /*var length = _Index - _startIndex;
            Console.WriteLine(_FileContents.ToString(_startIndex, length));*/
        }

        private void ReadNum()
        {
            var hasSeparator = false;
            var isDecimal = false;
            var hasMultiDecimals = false;
            _start = _index;

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

            var length = _index - _start;

            //Replacing , with . here so that I can parse it into a number
            //This allows a user to chose between , and . as their decimal separator
            var charArray = _source.ToString(_start, length).Replace(',', '.').ToCharArray();

            var text = string.Join("", charArray.Where(e => !char.IsWhiteSpace(e) && !e.Equals('_')));
            _text = text;

            //Numbers cannot _start with _ or have multiple . s.
            if (text.StartsWith('_'))
            {
                _diagnostics.ReportNumberStartWithUnderscore(new(_start, length), text, typeof(int));
            }

            //Numbers cannot have multiple .s or ,s.
            if (hasMultiDecimals)
            {
                _diagnostics.ReportInvalidNumber(new(_start, length), text, typeof(double));
            }

            if (isDecimal)
            {
                if (!double.TryParse(text, out var value))
                {
                    _diagnostics.ReportInvalidNumber(new(_start, length), text, typeof(double));
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
                    _diagnostics.ReportInvalidNumber(new(_start, length), text, typeof(long));
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

        }
   
        private void ReadIdentifierOrKeyword()
        {
            var _start = _index;
            while (char.IsLetter(CurrentIndex))
            {
                _index++;
            }
            var length = _index - _start;
            var text = _source.ToString(_start, length);
            var kind = SyntaxFacts.GetKeywordKind(text);
            
            _current = kind == SyntaxKind.BadToken ? SyntaxKind.IdentifierToken : kind;
            _text = text;
            //Console.WriteLine(_text);
            _index--;
        }
    }
}
