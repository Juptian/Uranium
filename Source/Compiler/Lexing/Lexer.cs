using System;
using System.Collections.Generic;
using System.IO;

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


        public void LexFile()
        {
            for(; m_Index < m_FileContents.Length; m_Index++)
            {
                CheckChar(m_FileContents[m_Index]);
            }
            foreach(Token t in m_TokenizedList)
            {
                Console.WriteLine(t);
            }
        }



        private bool Match(char ch, int position)
        {
            if(ch.Equals(m_FileContents[position]))
            {
                m_Index++;
                return true;
            }
            return false;
        }

        private TokenType CheckChar(char ch)
        {
            TokenType current = TokenType.BadToken;
            try
            {
                switch (ch)
                {
                    case '+':
                        current = Match('=', m_Index + 1) ? TokenType.PlusEquals : TokenType.Plus;
                        m_TokenizedList.Add(new Token(current, m_Index, ch.ToString(), null) );
                        break;
                    case '-':
                        current = Match('=', m_Index + 1) ? TokenType.MinusEquals : TokenType.Minus;
                        m_TokenizedList.Add(new Token(current, m_Index, ch.ToString(), null));
                        break;
                    case '*':
                        if (Match('=', m_Index))
                            current = TokenType.MultiplyEquals;
                        else if (Match('*', m_Index))
                            current = TokenType.Pow;
                        else
                            current = TokenType.Multiply;
                        m_TokenizedList.Add(new Token(current, m_Index, ch.ToString(), null));
                        break;
                    case '/':
                        current = Match('=', m_Index + 1) ? TokenType.DivideEquals : TokenType.Divide;
                        m_TokenizedList.Add(new Token(current, m_Index, ch.ToString(), null));
                        break;
                    case '>':
                        current = Match('=', m_Index + 1) ? TokenType.GreaterThanEquals : TokenType.GreaterThan;
                        m_TokenizedList.Add(new Token(current, m_Index, ch.ToString(), null));
                        break;
                    case '<':
                        current = Match('=', m_Index + 1) ? TokenType.LesserThanEquals : TokenType.LesserThan;
                        m_TokenizedList.Add(new Token(current, m_Index, ch.ToString(), null));
                        break;
                    case '=':
                        current = Match('=', m_Index + 1) ? TokenType.DoubleEquals : TokenType.Equals;
                        m_TokenizedList.Add(new Token(current, m_Index, ch.ToString(), null));
                        break;

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
                    default:
                        m_TokenizedList.Add(new Token(TokenType.Null, m_Index, ch.ToString(), ch));
                        break;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Line may not end with an operator, are you missing a ;?\n" + e);
            }
            return current;
        }
    }
}
