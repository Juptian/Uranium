using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.Lexer;


namespace Compiler
{
    public static class Compiler
    {
        public static void Emit(string[] args)
        {
            Tokenizer tokenizer = new Tokenizer(args[0]);
            tokenizer.Read();
        }
    }
}
