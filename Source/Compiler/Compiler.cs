using System;
using Compiler.Lexing;
using System.IO;

namespace Compiler
{
    public static class Compiler
    {
        public static void Emit(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("You must specify a file");
                return;
            }
            Lexer lexer = new( OpenFile(args[0]) );
            lexer.LexFile();
        }

        private static string OpenFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new Exception("File path must not be empty");
            }
            try
            {
                return File.ReadAllText(filePath);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not open file: " + e);
                return null;
            }
        }
    }
}
