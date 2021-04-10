using System;
using System.Collections.Generic;
using System.IO;

namespace Compiler.Lexing
{
    public class Lexer
    {

        public Lexer(string contents)
        {
            FileContents = contents;
        }

        private readonly string FileContents;
        private readonly List<Token> TokenizedList = new();


        public void LexFile()
        {
            for(int i = 0; i < FileContents.Length; i++)
            {
                TokenType t = CheckChar(i + 1, FileContents[i]);
                if(t != TokenType.BadToken)
                {
                    Console.Write(t);
                    Console.WriteLine(" " + i);
                }
            }
        }



        private bool Match(char ch, int position)
        {
            return ch.Equals(FileContents[position]);
        }

        private TokenType CheckChar(int currentIndex, char ch)
        {
            TokenType current = TokenType.BadToken;
            int NextIndex = currentIndex + 1;
            try
            {
                switch (ch)
                {
                    case '+':
                        current = Match('=', currentIndex + 1) ? TokenType.PlusEquals : TokenType.Plus;
                        TokenizedList.Add(new Token(current, currentIndex, ch.ToString(), null) );
                        break;
                    case '-':
                        current = Match('=', currentIndex + 1) ? TokenType.MinusEquals : TokenType.Minus;
                        TokenizedList.Add(new Token(current, currentIndex, ch.ToString(), null));
                        break;
                    case '*':
                        if (Match('=', NextIndex))
                            current = TokenType.MultiplyEquals;
                        else if (Match('*', NextIndex))
                            current = TokenType.Pow;
                        else
                            current = TokenType.Multiply;
                        TokenizedList.Add(new Token(current, currentIndex, ch.ToString(), null));
                        break;
                    case '/':
                        current = Match('=', currentIndex + 1) ? TokenType.DivideEquals : TokenType.Divide;
                        TokenizedList.Add(new Token(current, currentIndex, ch.ToString(), null));
                        break;
                    case '>':
                        current = Match('=', currentIndex + 1) ? TokenType.GreaterThanEquals : TokenType.GreaterThan;
                        TokenizedList.Add(new Token(current, currentIndex, ch.ToString(), null));
                        break;
                    case '<':
                        current = Match('=', currentIndex + 1) ? TokenType.LesserThanEquals : TokenType.LesserThan;
                        TokenizedList.Add(new Token(current, currentIndex, ch.ToString(), null));
                        break;
                    case '=':
                        current = Match('=', currentIndex + 1) ? TokenType.DoubleEquals : TokenType.Equals;
                        TokenizedList.Add(new Token(current, currentIndex, ch.ToString(), null));
                        break;

                    case ';':
                        current = TokenType.Semicolon;
                        TokenizedList.Add(new Token(current, currentIndex, ch.ToString(), null));
                        break;
                    default:
                        TokenizedList.Add(new Token(TokenType.Null, currentIndex, ch.ToString(), ch));
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
