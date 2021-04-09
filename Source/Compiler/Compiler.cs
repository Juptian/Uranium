using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.Lexing;


namespace Compiler
{
    public static class Compiler
    {
        public static void Emit(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("You must specify a file");
            }
            Lexer lexer = new Lexer(args[0]);
            lexer.ReadFile();
        }
    }
}
