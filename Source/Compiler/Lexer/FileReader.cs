using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Compiler.Lexing
{
    internal static class FileReader
    {

        public static string OpenFile(string FilePath)
        {
            if(string.IsNullOrWhiteSpace(FilePath))
            {
                throw new Exception("File path must not be empty");
            }
            try
            {
                return File.ReadAllText(FilePath);
            } 
            catch (Exception e)
            {
                Console.WriteLine("Could not open file: " + e);
                return null;
            }
        }
    }
}
