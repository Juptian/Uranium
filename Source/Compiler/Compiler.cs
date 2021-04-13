using System;
using Compiler.Lexing;
using Compiler.Parsing;
using Compiler.Syntax.Expression;
using System.Linq;
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
            string text = OpenFile(args[0]);
            var parser = new Parser(text);

            var syntaxTree = parser.Parse();

            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;

            Parser.PrettyPrint(syntaxTree.Root);

            Console.ForegroundColor = color;

            if(syntaxTree.Diagnostics.Any())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                foreach (var diag in syntaxTree.Diagnostics)
                    Console.WriteLine(diag);
                Console.ForegroundColor = color;
            } 
            else
            {
                var e = new Evaluator(syntaxTree.Root);
                var result = e.Evaluate();
                Console.WriteLine(result);
            }

            //Lexer lexer = new( OpenFile(args[0]) );
            //lexer.NextToken();
        }

        private static string OpenFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new Exception("File path must not be empty");
            }
            try
            {
                return File.ReadAllText(filePath) + "\0";
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not open file: " + e);
                return null;
            }
        }
    }
}
