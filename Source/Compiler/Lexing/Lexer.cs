using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.Logging;
using Compiler.Syntax;
#pragma warning disable CS8632
namespace Compiler.Lexing
{
    sealed class Lexer
    {

        public Lexer(string contents)
        {
            _FileContents = contents;
            for(int i = 0; i < contents.Length; i++)
            {
                Console.WriteLine(contents[i].ToString() + ", " + i);
            }
        }

        private readonly string _FileContents;
        private readonly List<SyntaxToken> _TokenizedList = new();
        private int _Index = 0;

        public object? _Current;

        private char CurrentIndex => Peek(0);
        private char NextIndex => Peek(1);
        private SyntaxKind current;

        private readonly List<string> _diagnostics = new();

        public IEnumerable<string> Diagnostics => _diagnostics;

        public SyntaxToken NextToken()
        {
            ReadSpecialChars(true);
            LexTokens(CurrentIndex);
            if (_Index == _FileContents.Length)
                return new SyntaxToken(SyntaxKind.EndOfFile, _Index, "\0", null);
            Console.Write(current + ", ");
            Console.WriteLine(_Index.ToString() + ", " +  CurrentIndex);
            _Index++;
            return new SyntaxToken(current, _Index, CurrentIndex.ToString(), _Current);
        }

        private char Peek(int offset)
        {
            int ToPeekIndex = _Index + offset;
            if (ToPeekIndex == _FileContents.Length)
                return '\0';
            return _FileContents[ToPeekIndex];
        }

        private bool Match(char ch, int offset)
        {
            if (ch.Equals(Peek(offset)))
                return true;
            return false;
        }

        private void ReadSpecialChars(bool KeepGoing)
        {
            var finished = false;
            var start = _Index;
            current = SyntaxKind.BadToken;
            while(!finished)
            {
                switch(CurrentIndex)
                {
                    //Special characters
                    case '\r':
                    case '\n':
                        current = SyntaxKind.LineBreak;
                        ReadLineBreak();
                        finished = !KeepGoing;
                        break;
                    case ' ':
                    case '\t':
                        ReadWhitespace();
                        current = SyntaxKind.WhiteSpace;
                        break;
                    case '\0':
                        current = SyntaxKind.EndOfFile;
                        finished = true;
                        break;
                    default:
                        if (char.IsWhiteSpace(CurrentIndex))
                        {
                            ReadWhitespace();
                        }
                        else
                        {
                            finished = true;
                        }
                        break;
                }
                var length = _Index - start;
                if(length > 0)
                {
                    var text = _FileContents.Substring(start, length);
                    SyntaxToken t = new(current, _Index, text, text);
                    _TokenizedList.Add(t);
                }
            }
        }

        //Literally lexes a single character
        public SyntaxKind LexTokens(char ch)
        {
            current = SyntaxKind.BadToken;
            switch (ch)
            {
                #region Operators
                //Math operators
                case '+':
                    current = Match('=', 1) ? SyntaxKind.PlusEquals : SyntaxKind.Plus;
                    break;
                case '-':
                    current = Match('=', 1) ? SyntaxKind.MinusEquals : SyntaxKind.Minus;
                    break;
                case '*':
                    if (Match('=', 1))
                        current = SyntaxKind.MultiplyEquals;
                    else if (Match('*', 1))
                        current = SyntaxKind.Pow;
                    else
                        current = SyntaxKind.Multiply;
                    break;
                case '/':
                    if (Match('=', 1))
                        current = SyntaxKind.DivideEquals;
                    else if (Match('/', 1))
                    {
                        ReadSingleLineComment();
                        current = SyntaxKind.SingleLineComment;
                    }
                    else if (Match('*', 1))
                    {
                        ReadMultiLineComment();
                        current = SyntaxKind.MultiLineComment;
                    }
                    else
                        current = SyntaxKind.Divide;
                    
                    break;
                case '>':
                    current = Match('=', 1) ? SyntaxKind.GreaterThanEquals : SyntaxKind.GreaterThan;
                    break;
                case '<':
                    current = Match('=', 1) ? SyntaxKind.LesserThanEquals : SyntaxKind.LesserThan;
                    break;
                case '=':
                    current = Match('=', 1) ? SyntaxKind.DoubleEquals : SyntaxKind.Equals;
                    break;
                case '^':
                    current = Match('=', 1) ? SyntaxKind.HatEquals : SyntaxKind.Hat;
                    break;

                //Also operators
                case '|':
                    current = Match('|', 1) ? SyntaxKind.DoublePipe : SyntaxKind.Pipe;
                    break;
                case '&':
                    current = Match('&', 1) ? SyntaxKind.DoubleAmpersand : SyntaxKind.Ampersand;
                    break;
                case '%':
                    current = Match('=', 1) ? SyntaxKind.PercentEquals : SyntaxKind.Percent;
                    break;
                case '~':
                    current = SyntaxKind.Tilde;
                    break;
                #endregion

                #region Numbers
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
                    current = SyntaxKind.NumberToken;
                    break;
                #endregion

                #region Syntax
                //Pure syntax
                case ';':
                    current = SyntaxKind.Semicolon;
                    break;
                case '(':
                    current = SyntaxKind.OpenParenthesis;
                    break;
                case ')':
                    current = SyntaxKind.CloseParenthesis;
                    break;
                case '{':
                    current = SyntaxKind.OpenCurlyBrace;
                    break;
                case '}':
                    current = SyntaxKind.CloseCurlyBrace;
                    break;
                case '[':
                    current = SyntaxKind.OpenBrackets;
                    break;
                case ']':
                    current = SyntaxKind.CloseBrackets;
                    break;
                #endregion
                case '\0':
                    current = SyntaxKind.EndOfFile;
                    return current;
                //Default
                default:
                    //current = SyntaxKind.Null;
                    break;
            }

            return current;
        }

        private void ReadLineBreak()
        {
            if(CurrentIndex == '\r' && NextIndex == '\n')
            {
                _Index += 2;
            } 
            else
            {
                _Index++;
            }
        }

        private void ReadWhitespace()
        {
            var done = false;
            while(!done)
            {
                switch(CurrentIndex)
                {
                    case '\0':
                    case '\r':
                    case '\n':
                        done = true;
                        break;
                    default:
                        if (!char.IsWhiteSpace(CurrentIndex))
                            done = true;
                        else
                           _Index++;
                        break;
                }
            }
        }

        private void ReadSingleLineComment()
        {
            _Index++;
            var finished = false;
            var startIndex = _Index;
            while(!finished)
            {
                switch(CurrentIndex)
                {
                    case '\0':
                    case '\r':
                    case '\n':
                        finished = true;
                        break;
                    default:
                        _Index++;
                        break;
                }
            }
            _Current = SyntaxKind.SingleLineComment;
            
            //Commented out, is here for debug purposes
            /*var length = _Index - startIndex;
            Console.WriteLine(_FileContents.Substring(startIndex, length));*/
        }

        private void ReadMultiLineComment()
        {
            _Index++;
            var finished = false;
            var startIndex = _Index;
            while(!finished)
            {
                switch(CurrentIndex)
                {

                    case '\0':
                        ErrorLogger.ReportUnfinishedMultiLineComment();
                        current = SyntaxKind.EndOfFile;
                        return;
                    case '*':
                        if(NextIndex.Equals('/'))
                        {
                            _Index++;
                            finished = true;
                        } 
                        _Index++;
                        break;
                    default:
                        _Index++;
                        break;
                }
            }
            _Current = SyntaxKind.SingleLineComment;

            //Commented out, is here for debug purposes
            /*var length = _Index - startIndex;
            Console.WriteLine(_FileContents.Substring(startIndex, length));*/
        }

        private void ReadNum()
        {
            bool hasSeparator = false;
            bool isDecimal = false;
            bool hasMultiDecimals = false;
            int startIndex = _Index;


            while (char.IsDigit(CurrentIndex) ||
                ((CurrentIndex == '_' || CurrentIndex == ' ') && char.IsDigit(NextIndex)) ||
                ((CurrentIndex == '.' || CurrentIndex == ',') && char.IsDigit(NextIndex)) )
            {
                if (!hasSeparator && (CurrentIndex == '_' || CurrentIndex == ' '))
                    hasSeparator = true;

                if (CurrentIndex == '.' || CurrentIndex == ',')
                {
                    hasMultiDecimals = isDecimal;
                        
                    isDecimal = true;
                }
                _Index++;
            }
            //Getting the length of a number
            int length = _Index - startIndex;

            //Get's the contents of the number, and replaces , with ., then makes it a CharArray so that I can join it in the text
            char[] cha_text = _FileContents.Substring(startIndex, length).Replace(',', '.').ToCharArray();

            //Makes a string out of cha_text
            string text = string.Join("", cha_text.Where<char>(e => !char.IsWhiteSpace(e) && !e.Equals('_')) );
            
            //Numbers cannot start with _ or have multiple . s.
            if (text.StartsWith('_'))
            {
                //TODO: Make a logging system that throws errors :)
                ErrorLogger.ReportNumberStartingWith_();
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
                        _Current = (float)value;
                    }
                    else
                    {
                        _Current = value;
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
                        _Current = (int)value;
                    }
                    else if (value <= uint.MaxValue)
                    {
                        _Current = (uint)value;
                    }
                    else
                    {
                        _Current = value;
                    }
                }
            }
            _Index--;
            //Console.WriteLine(_Current);
        }
    }
}
