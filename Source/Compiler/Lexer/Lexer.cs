using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Lexer
{
    class Lexer
    {

        public Lexer(string file)
        {
            FReader = new FileReader(file);
        }

        public FileReader FReader;

        public int CurrentIndex = 0;

        public bool IsFileOpen { get => FReader.FileOpen; private set => IsFileOpen = FReader.FileOpen; }

        public void Next()
        {

            for (int i = 0; i < FReader.CurrentFile[CurrentIndex].Length; i++)
            {
                char ch = FReader.CurrentFile[CurrentIndex][i];
                Console.WriteLine(ch);
                CheckChar(FReader.CurrentFile, CurrentIndex, i, ch);
            }
            CurrentIndex++;
            if (CurrentIndex < FReader.CurrentFile.Length)
            {
                Next();
            }
        }

        private void CheckChar(string[] Array, int CurrentIndex, int i, char ch)
        {
            TokenType current;
            try
            {
                switch (ch)
                {
                    /*case '' or ' ':
                        break;*/
                    case '+':
                        current = Array[CurrentIndex][i + 1] == '=' ? TokenType.PlusEquals : TokenType.Plus;
                        Console.WriteLine(current);
                        break;
                    case '-':
                        current = Array[CurrentIndex][i + 1] == '=' ? TokenType.MinusEquals : TokenType.Minus;
                        Console.WriteLine(current);
                        break;
                    case '*':
                        if (Array[CurrentIndex][i + 1] == '=')
                            current = TokenType.MultiplyEquals;

                        else if (Array[CurrentIndex][i + 1] == '*')
                            current = TokenType.Pow;

                        else
                            current = TokenType.Multiply;
                        Console.WriteLine(current);
                        break;
                    case '/':
                        current = Array[CurrentIndex][i + 1] == '=' ? TokenType.DivideEquals : TokenType.Divide;
                        Console.WriteLine(current);
                        break;
                    case '>':
                        current = Array[CurrentIndex][i + 1] == '=' ? TokenType.GreaterThanEquals : TokenType.GreaterThan;
                        Console.WriteLine(current);
                        break;
                    case '<':
                        current = Array[CurrentIndex][i + 1] == '=' ? TokenType.LesserThanEquals : TokenType.LesserThan;
                        Console.WriteLine(current);
                        break;
                    case '=':
                        current = Array[CurrentIndex][i + 1] == '=' ? TokenType.Equals : TokenType.DoubleEquals;
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

        }
    }
}
