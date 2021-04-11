using System;
using System.Collections.Generic;
using System.Linq;
using Compiler.Logging;

namespace Compiler.Lexing
{
    public class Lexer
    {

        public Lexer(string contents)
        {
            m_FileContents = contents;
        }

        private readonly string m_FileContents;
        private readonly List<Token> m_TokenizedList = new();
        private int m_Index = 0;
#nullable enable
        public object? m_Current;
#nullable disable
        private char CurrentIndex => Peek(0);
        private char NextIndex => Peek(1);
        private TokenType current;

        

        public void LexFile()
        {
            for (; m_Index < m_FileContents.Length; m_Index++)
            {
                ReadSpecialChars(false);
                LexTokens(m_FileContents[m_Index]);
            }
            /*foreach (Token t in m_TokenizedList)
            {
                Console.WriteLine(t);
            }*/
        }

        private char Peek(int offset)
        {
            int ToPeekIndex = m_Index + offset;
            if (ToPeekIndex == m_FileContents.Length)
                return '\0';
            return m_FileContents[ToPeekIndex];
        }

        private bool Match(char ch, int offset)
        {
            if (ch.Equals(Peek(offset)))
            {
                m_Index++;
                return true;
            }
            return false;
        }

        private void ReadSpecialChars(bool KeepGoing)
        {
            var finished = false;
            var start = m_Index;
            current = TokenType.BadToken;
            while(!finished)
            {
                switch(CurrentIndex)
                {
                    //Special characters
                    case '\r':
                    case '\n':
                        current = TokenType.LineBreak;
                        ReadLineBreak();
                        AddTokenToList(current, CurrentIndex, null);
                        finished = !KeepGoing;
                        break;
                    case ' ':
                    case '\t':
                        ReadWhitespace();
                        current = TokenType.WhiteSpace;
                        AddTokenToList(current, CurrentIndex, null);
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
                var length = m_Index - start;
                if(length > 0)
                {
                    var text = m_FileContents.Substring(start, length);
                    Token t = new Token(current, m_Index, text, text);
                    m_TokenizedList.Add(t);
                }
            }
        }

        //Literally lexes a single character
        public TokenType LexTokens(char ch)
        {
            current = TokenType.BadToken;
            switch (ch)
            {
                #region Operators
                //Math operators
                case '+':
                    current = Match('=', 1) ? TokenType.PlusEquals : TokenType.Plus;
                    AddTokenToList(current, ch, null);
                    break;
                case '-':
                    current = Match('=', 1) ? TokenType.MinusEquals : TokenType.Minus;
                    AddTokenToList(current, ch, null);
                    break;
                case '*':
                    if (Match('=', 1))
                        current = TokenType.MultiplyEquals;
                    else if (Match('*', 1))
                        current = TokenType.Pow;
                    else
                        current = TokenType.Multiply;
                    AddTokenToList(current, ch, null);
                    break;
                case '/':
                    if (Match('=', 1))
                        current = TokenType.DivideEquals;
                    else if (Match('/', 1))
                    {
                        ReadSingleLineComment();
                        current = TokenType.SingleLineComment;
                    }
                    else if (Match('*', 1))
                    {
                        ReadMultiLineComment();
                        current = TokenType.MultiLineComment;
                    }
                    else
                        current = TokenType.Divide;
                    AddTokenToList(current, ch, null);
                    
                    break;
                case '>':
                    current = Match('=', 1) ? TokenType.GreaterThanEquals : TokenType.GreaterThan;
                    AddTokenToList(current, ch, null);
                    break;
                case '<':
                    current = Match('=', 1) ? TokenType.LesserThanEquals : TokenType.LesserThan;
                    AddTokenToList(current, ch, null);
                    break;
                case '=':
                    current = Match('=', 1) ? TokenType.DoubleEquals : TokenType.Equals;
                    AddTokenToList(current, ch, null);
                    break;
                case '^':
                    current = Match('=', 1) ? TokenType.HatEquals : TokenType.Hat;
                    AddTokenToList(current, ch, null);
                    break;

                //Also operators
                case '|':
                    current = Match('|', 1) ? TokenType.DoublePipe : TokenType.Pipe;
                    AddTokenToList(current, ch, null);
                    break;
                case '&':
                    current = Match('&', 1) ? TokenType.DoubleAmpersand : TokenType.Ampersand;
                    AddTokenToList(current, ch, null);
                    break;
                case '%':
                    current = Match('=', 1) ? TokenType.PercentEquals : TokenType.Percent;
                    AddTokenToList(current, ch, null);
                    break;
                case '~':
                    current = TokenType.Tilde;
                    AddTokenToList(current, ch, null);
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
                    current = TokenType.Number;
                    AddTokenToList(TokenType.Number, ch, m_Current);
                    break;
                #endregion

                #region Syntax
                //Pure syntax
                case ';':
                    current = TokenType.Semicolon;
                    AddTokenToList(current, ch, null); 
                    break;
                case '(':
                    current = TokenType.OpenParenthesis;
                    AddTokenToList(current, ch, null);                    
                    break;
                case ')':
                    current = TokenType.CloseParenthesis;
                    AddTokenToList(current, ch, null);
                    break;
                case '{':
                    current = TokenType.OpenCurlyBrace;
                    AddTokenToList(current, ch, null);
                    break;
                case '}':
                    current = TokenType.CloseCurlyBrace;
                    AddTokenToList(current, ch, null);
                    break;
                case '[':
                    current = TokenType.OpenBrackets;
                    AddTokenToList(current, ch, null);
                    break;
                case ']':
                    current = TokenType.CloseBrackets;
                    AddTokenToList(current, ch, null);
                    break;
                #endregion

                //Default
                default:
                    current = TokenType.Null;
                    AddTokenToList(current, ch, ch);
                    break;
            }

            return current;
        }

        private void AddTokenToList(TokenType tType, char ch, object? obj)
        {
            Token t = new(tType, m_Index, ch.ToString(), obj);
            m_TokenizedList.Add(t);
        }

        private void ReadLineBreak()
        {
            if(CurrentIndex == '\r' && NextIndex == '\n')
            {
                m_Index += 2;
            } 
            else
            {
                m_Index++;
            }
        }

        private void ReadWhitespace()
        {
            while(char.IsWhiteSpace(CurrentIndex))
            {
                m_Index++;
            }

            m_Current = TokenType.WhiteSpace;
        }

        private void ReadSingleLineComment()
        {
            m_Index++;
            var finished = false;
            var startIndex = m_Index;
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
                        m_Index++;
                        break;
                }
            }
            m_Current = TokenType.SingleLineComment;
            
            //Commented out, is here for debug purposes
            /*var length = m_Index - startIndex;
            Console.WriteLine(m_FileContents.Substring(startIndex, length));*/
        }

        private void ReadMultiLineComment()
        {
            m_Index++;
            var finished = false;
            var startIndex = m_Index;
            while(!finished)
            {
                switch(CurrentIndex)
                {

                    case '\0':
                        ErrorLogger.ReportUnfinishedMultiLineComment();
                        break;
                    case '*':
                        if(NextIndex.Equals('/'))
                        {
                            m_Index++;
                            finished = true;
                        } 
                        m_Index++;
                        break;
                    default:
                        m_Index++;
                        break;
                }
            }
            m_Current = TokenType.SingleLineComment;

            //Commented out, is here for debug purposes
            /*var length = m_Index - startIndex;
            Console.WriteLine(m_FileContents.Substring(startIndex, length));*/
        }

        private void ReadNum()
        {
            bool hasSeparator = false;
            bool isDecimal = false;
            bool hasMultiDecimals = false;
            int startIndex = m_Index;


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
                m_Index++;
            }
            //Getting the length of a number
            int length = m_Index - startIndex;

            //Get's the contents of the number, and replaces , with ., then makes it a CharArray so that I can join it in the text
            char[] cha_text = m_FileContents.Substring(startIndex, length).Replace(',', '.').ToCharArray();

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
                        m_Current = (float)value;
                    }
                    else
                    {
                        m_Current = value;
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
                        m_Current = (int)value;
                    }
                    else if (value <= uint.MaxValue)
                    {
                        m_Current = (uint)value;
                    }
                    else
                    {
                        m_Current = value;
                    }
                }
            }

            Console.WriteLine(m_Current);
        }
    }
}
