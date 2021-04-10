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
        private object? m_Current;
#nullable disable
        private char CurrentIndex => Peek(0);
        private char NextIndex => Peek(1);

        

        public void LexFile()
        {
            for (; m_Index < m_FileContents.Length; m_Index++)
            {
                Lex(m_FileContents[m_Index]);
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

        public TokenType Lex(char ch)
        {
            TokenType current = TokenType.BadToken;
            switch (ch)
            {
                //Special characters
                case '\r':
                case '\n':
                    current = TokenType.LineBreak;
                    m_TokenizedList.Add(new Token(current, m_Index, ch.ToString(), null));
                    break;

                //Math operators
                case '+':
                    current = Match('=', 1) ? TokenType.PlusEquals : TokenType.Plus;
                    m_TokenizedList.Add(new Token(current, m_Index, ch.ToString(), null));
                    break;
                case '-':
                    current = Match('=', 1) ? TokenType.MinusEquals : TokenType.Minus;
                    m_TokenizedList.Add(new Token(current, m_Index, ch.ToString(), null));
                    break;
                case '*':
                    if (Match('=', 1))
                        current = TokenType.MultiplyEquals;
                    else if (Match('*', m_Index))
                        current = TokenType.Pow;
                    else
                        current = TokenType.Multiply;
                    m_TokenizedList.Add(new Token(current, m_Index, ch.ToString(), null));
                    break;
                case '/':
                    if (Match('=', 1))
                        current = TokenType.DivideEquals;
                    else if (Match('/', 1))
                        current = TokenType.SingleLineComment;
                    else if (Match('*', 1))
                        current = TokenType.MultiLineComment;
                    else
                        current = TokenType.Divide;

                    m_TokenizedList.Add(new Token(current, m_Index, ch.ToString(), null));
                    break;
                case '>':
                    current = Match('=', 1) ? TokenType.GreaterThanEquals : TokenType.GreaterThan;
                    m_TokenizedList.Add(new Token(current, m_Index, ch.ToString(), null));
                    break;
                case '<':
                    current = Match('=', 1) ? TokenType.LesserThanEquals : TokenType.LesserThan;
                    m_TokenizedList.Add(new Token(current, m_Index, ch.ToString(), null));
                    break;
                case '=':
                    current = Match('=', 1) ? TokenType.DoubleEquals : TokenType.Equals;
                    m_TokenizedList.Add(new Token(current, m_Index, ch.ToString(), null));
                    break;

                //Also operators
                case '^':
                    current = Match('=', 1) ? TokenType.HatEquals : TokenType.Hat;
                    m_TokenizedList.Add(new Token(current, m_Index, ch.ToString(), null));
                    break;
                case '|':
                    current = Match('|', 1) ? TokenType.DoublePipe : TokenType.Pipe;
                    m_TokenizedList.Add(new Token(current, m_Index, ch.ToString(), null));
                    break;
                case '&':
                    current = Match('&', 1) ? TokenType.DoubleAmpersand : TokenType.Ampersand;
                    m_TokenizedList.Add(new Token(current, m_Index, ch.ToString(), null));
                    break;
                case '%':
                    current = Match('=', 1) ? TokenType.PercentEquals : TokenType.Percent;
                    m_TokenizedList.Add(new Token(current, m_Index, ch.ToString(), null));
                    break;
                case '~':
                    current = TokenType.Tilde;
                    m_TokenizedList.Add(new Token(current, m_Index, ch.ToString(), null));
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
                    break;

                //Pure syntax
                case ';':
                    current = TokenType.Semicolon;
                    m_TokenizedList.Add(new Token(current, m_Index, ch.ToString(), null));
                    break;
                case '(':
                    current = TokenType.OpenParenthesis;
                    m_TokenizedList.Add(new Token(current, m_Index, ch.ToString(), null));
                    break;
                case ')':
                    current = TokenType.CloseParenthesis;
                    m_TokenizedList.Add(new Token(current, m_Index, ch.ToString(), null));
                    break;
                case '{':
                    current = TokenType.OpenCurlyBrace;
                    m_TokenizedList.Add(new Token(current, m_Index, ch.ToString(), null));
                    break;
                case '}':
                    current = TokenType.CloseCurlyBrace;
                    m_TokenizedList.Add(new Token(current, m_Index, ch.ToString(), null));
                    break;
                case '[':
                    current = TokenType.OpenBrackets;
                    m_TokenizedList.Add(new Token(current, m_Index, ch.ToString(), null));
                    break;
                case ']':
                    current = TokenType.CloseBrackets;
                    m_TokenizedList.Add(new Token(current, m_Index, ch.ToString(), null));
                    break;

                //Default
                default:
                    m_TokenizedList.Add(new Token(TokenType.Null, m_Index, ch.ToString(), ch));
                    break;
            }
            return current;
        }

        private void ReadNum()
        {
            bool hasSeparator = false;
            bool isDecimal = false;
            bool hasMultiDecimals = false;
            int startIndex = m_Index;


            while (char.IsDigit(CurrentIndex) ||
                ((CurrentIndex == '_' || CurrentIndex == ' ') && char.IsDigit(NextIndex)) ||
                (CurrentIndex == '.' && char.IsDigit(NextIndex)))
            {
                if (!hasSeparator && (CurrentIndex == '_' || CurrentIndex == ' '))
                    hasSeparator = true;

                if (CurrentIndex == '.')
                {
                    hasMultiDecimals = isDecimal;
                        
                    isDecimal = true;
                }
                m_Index++;
            }
            int length = m_Index - startIndex;


            char[] cha_text = m_FileContents.Substring(startIndex, length).ToCharArray();

            string text = string.Join("", cha_text.Where<char>(e => !char.IsWhiteSpace(e) && !e.Equals('_')));
            //Numbers cannot start with _ or have multiple . s.
            if (text.StartsWith('_'))
            {
                //TODO: Make a logging system that throws errors :)
                Console.WriteLine("A number may not start with _");
            }

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
                    //Extra precautions to avoid someone parsing in a decimal...
                    //Commented out because they shouldn't be needed
                    else /*if (value >= double.MinValue && value <= double.MaxValue)*/
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
                    else if (value <= ulong.MaxValue)
                    {
                        m_Current = value;
                    }
                    else
                    {
                        throw new Exception("Could parse to unsigned long, but it is above the max value?");
                    }
                }
            }
            Console.WriteLine(m_Current);
        }
    }
}
