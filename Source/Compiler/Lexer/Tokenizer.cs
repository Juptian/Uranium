using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Lexer
{
    internal class Tokenizer
    {
        public Tokenizer(string File)
        {
            lexer = new Lexer(File);
        }

        private Lexer lexer;

        public void Read()
        {
            if (!lexer.IsFileOpen)
            {
                throw new Exception("File is not open!");
            }

            lexer.Next();
        }
    }
    
}
