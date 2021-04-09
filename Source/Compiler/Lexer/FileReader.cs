using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Compiler.Lexer
{
    internal sealed class FileReader
    {
        public FileReader(string File)
        {
            FileOpen = OpenFile(File);
        }

        public string[] CurrentFile;
        public bool FileOpen = false;

        private bool OpenFile(string FilePath)
        {
            if(FilePath == String.Empty)
            {
                throw new Exception("File path must not be empty");
            }
            CurrentFile = File.ReadAllLines(FilePath); // No try-catch here because I was gonna throw the exact same error kekw
            return true;
        }
    }
}
