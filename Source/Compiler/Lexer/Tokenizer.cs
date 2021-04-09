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
            FReader = new FileReader(File);
        }

        private FileReader FReader;

        public void Read()
        {
            if (!FReader.FileOpen)
            {
                throw new Exception("File is not open!");
            }

            FReader.Next();
        }


    }
    
}
