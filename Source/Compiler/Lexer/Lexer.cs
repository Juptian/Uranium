using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Lexing
{
    public class Lexer
    {

        public Lexer(string file)
        {
            FileContents = FileReader.OpenFile(file);
        }

        private string FileContents;


        public void ReadFile()
        {
            for(int i = 0; i < FileContents.Length; i++)
            {
                CheckChar(i + 1, FileContents[i]);
            }
        }

        private bool Match(char ch, int position)
        {
            return ch == FileContents[position];
        }

        private TokenType CheckChar(int CurrentIndex, char ch)
        {
            TokenType current = TokenType.Null;
            try
            {
                switch (ch)
                {
                    /*case '' or ' ':
                        break;*/
                    case '+':
                        current = Match('=', CurrentIndex + 1) ? TokenType.PlusEquals : TokenType.Plus;
                        Console.WriteLine(current);
                        break;
                    case '-':
                        current = Match('=', CurrentIndex + 1) ? TokenType.MinusEquals : TokenType.Minus;
                        Console.WriteLine(current);
                        break;
                    case '*':
                        if (Match('=', CurrentIndex + 1))
                            current = TokenType.MultiplyEquals;

                        else if (Match('*', CurrentIndex + 1))
                            current = TokenType.Pow;

                        else
                            current = TokenType.Multiply;
                        Console.WriteLine(current);
                        break;
                    case '/':
                        current = Match('=', CurrentIndex + 1) ? TokenType.DivideEquals : TokenType.Divide;
                        Console.WriteLine(current);
                        break;
                    case '>':
                        current = Match('=', CurrentIndex + 1) ? TokenType.GreaterThanEquals : TokenType.GreaterThan;
                        Console.WriteLine(current);
                        break;
                    case '<':
                        current = Match('=', CurrentIndex + 1) ? TokenType.LesserThanEquals : TokenType.LesserThan;
                        Console.WriteLine(current);
                        break;
                    case '=':
                        current = Match('=', CurrentIndex + 1) ? TokenType.Equals : TokenType.DoubleEquals;
                        Console.WriteLine(current);
                        break;

                    case ';':
                        current = TokenType.Semicolon;
                        Console.WriteLine(current);
                        break;
                    default:
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
