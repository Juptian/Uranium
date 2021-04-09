using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Compiler.Lexer
{
    class FileReader
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
            try
            {
                CurrentFile = File.ReadAllLines(FilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine("Couldn't open file because: " + e);
                return false;
            }
            return true;
        }
    }
}
